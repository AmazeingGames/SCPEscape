using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.UIElements;
using UnityEngine;
using static Resource;
using static UnityEditor.Progress;

//TO DO: Refactor variables 
public class GameManager : MonoBehaviour
{
    public class CardSelectionEventArgs : EventArgs
    {
        public ECardType[] ResourceRewards { get; }
        public List<EncounterCard> EncounterRewards { get; }

        public CardSelectionEventArgs(ECardType[] resourceRewards, List<EncounterCard> encounterRewards)
        {
            ResourceRewards = resourceRewards;
            EncounterRewards = encounterRewards;
        }
    }

    public static GameManager Manager;

    GameObject resourcePool;
    GameObject handHolder;
    GameObject resourceConsumer;
    GameObject iconPool;
    GameObject iconHolderPool;

    public GameObject Nodes { get; private set; }
    public GameObject ChoicePool { get; private set; }
    public GameObject GameCanvas { get; private set; }
    public GameObject EncounterPool { get; private set; }
    public GameObject Choices { get; private set; }

    [SerializeField] int holdingCardLayer;
    [SerializeField] int newestCardLayer;
    [SerializeField] int defaultLayer;

    [SerializeField] ChoiceCard choiceCard;

    [SerializeField] int encounterPoolSize;
    [SerializeField] EncounterCard encounterCard;

    [SerializeField] int choicePoolSize;
    [SerializeField] int iconPoolSize;
    [SerializeField] int iconHolderPoolSize;
    [SerializeField] Icon icon;

    [SerializeField] float holdingCardScaleMultiplier;
    [SerializeField] Vector3 resourceConsumerCardScale;
    [SerializeField] Vector3 handHolderCardScale;
    [SerializeField] LayerMask handHolderLayer;
    [SerializeField] LayerMask resourceConsumerLayer;
    [SerializeField] LayerMask cardLayer;
    [SerializeField] int resourcePoolSize;
    [SerializeField] ResourceCard resourceCard;
    [SerializeField] IconHolder iconHolder;

    [SerializeField] Resource ration;
    [SerializeField] Resource escapee;
    [SerializeField] Resource scientist;
    [SerializeField] Resource insanity;
    [SerializeField] Resource munition;
    [SerializeField] Resource anomaly;

    [SerializeField] Resource ration1;
    [SerializeField] Resource escapee1;
    [SerializeField] Resource scientist1;
    [SerializeField] Resource insanity1;
    [SerializeField] Resource munition1;
    [SerializeField] Resource anomaly1;

    public Dictionary<ECardType, Resource> TypeToResource { get; private set; } = new();

    public bool hasAddedResources = false;


    //proably use an event instead
    public event Action<ECardType, bool> OnCardChangeInConsumer;

    public List<ResourceCard> Hand { get; private set; } = new();
    public List<ResourceCard> Consumer { get; private set; } = new List<ResourceCard>();

    public List<Resource> resources;
    public List<Resource> resources1;
    
    readonly List<ResourceCard> allCards = new();
    readonly List<Icon> allIcons = new();

    public List<Sprite> indicators;

    ResourceCard holdingResourceCard = null;
    Vector3 grabbedPosition;
    Vector3 regularScale = Vector3.one;
    int regularSortingOrder = 0;

    void Awake()
    {
        if (Manager == null)
            Manager = this;
        else
            Destroy(Manager);

        TypeToResource = new Dictionary<ECardType, Resource>();

        handHolder = GameObject.Find("HandHolder");
        resourcePool = GameObject.Find("ResourcePool");
        resourceConsumer = GameObject.Find("ResourceConsumer");
        ChoicePool = GameObject.Find("ChoicePool");
        iconPool = GameObject.Find("IconPool");
        iconHolderPool = GameObject.Find("IconHolderPool");

        EncounterPool = GameObject.Find("EncounterPool");
        Choices = GameObject.Find("Choices");
        GameCanvas = GameObject.Find("GameCanvas");
        Nodes = GameObject.Find("Nodes");

        resources = new List<Resource> { ration, escapee, scientist, insanity, munition, anomaly };
        resources1 = new List<Resource> { ration1, escapee1, scientist1, insanity1, munition1, anomaly1 };

        CreateEncounterPool();
        CreateResourcePool();
        CreateIconPool();
        CreateChoicePool();
        CreateIconHolderPool();

        TypeToResource = new Dictionary<ECardType, Resource>()
        {
            { ECardType.Anomaly  ,  anomaly1    },
            { ECardType.Escapee  ,  escapee1    },
            { ECardType.Insanity ,  insanity1   },
            { ECardType.Munition ,  munition1   },
            { ECardType.Ration   ,  ration1     },
            { ECardType.Scientist,  scientist1  },
        };
    }

    void Start()
    {
        AddStartingResources();

        SwapResources(resources1);
        SwapIcons(resources1);
    }

    void Update()
    {
        GrabCard();
        DragCard();
        DropCard();

        CheckCardSizes();
        DeveloperCommands();
    }

    void AddStartingResources()
    {
        AddPoolResourcesToHand(1, 1, 1, 1, 1, 1);
    }

    //Adds resources to hand correlated to the nums 1 - 9
    void DeveloperCommands()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DataMatchResources();
        }
        DataMatchResources();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddPoolResourceToHand(ECardType.Anomaly);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddPoolResourceToHand(ECardType.Escapee);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddPoolResourceToHand(ECardType.Insanity);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddPoolResourceToHand(ECardType.Munition);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AddPoolResourceToHand(ECardType.Ration);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AddPoolResourceToHand(ECardType.Scientist);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SwapResources(resources);
            SwapIcons(resources);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SwapResources(resources1);
            SwapIcons(resources1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
        }
    }

    //Adds an icon instance to the icons list
    public void AddIcon(Icon iconToAdd) => allIcons.Add(iconToAdd);

    //Given a list of Resource Cards, returns a list of their resource types 
    public static List<ECardType> ConvertResourceCardListToResourceType(List<ResourceCard> resourceCards) => ListToResourceTypeList(resourceCards, t => t._Resource.CardType);

    //Given a list of Resource Objects, returns a list of their resource type
    public static List<ECardType> ConvertResourceCardListToResourceType(List<Resource> resourceCards) => ListToResourceTypeList(resourceCards, t => t.CardType);

    //Given a list of T, returns a list of resource types
    static List<ECardType> ListToResourceTypeList<T>(List<T> listToConvert, Func<T, ECardType> conversionMethod)
    {
        List<ECardType> returnList = new();

        for (int i = 0; i < listToConvert.Count; i++)
        {
            var current = listToConvert[i];

            ECardType cardType = conversionMethod(current);

            returnList.Add(cardType);
        }
        return returnList;
    }

    //Sets the resource object of all resources cards
    void SwapResources(List<Resource> resources) => SetResourceObject(allCards, c => c.SetResource(ReturnResourceMatch(resources, c._Resource.CardType)));

    //Sets the resource object of all resource icons
    void SwapIcons(List<Resource> resources) => SetResourceObject(allIcons, i => i.SetResource(ReturnResourceMatch(resources, i.IconResource.CardType)));

    //Given a list of values, performs a method to that value*
    void SetResourceObject<T>(List<T> valuesToSet, Action<T> setSomething)
    {
        for (int i = 0; i < valuesToSet.Count; i++)
        {
            var current = valuesToSet[i];

            setSomething(current);
        }
    }

    //Given a list of Resources, returns the first resource in that list that matches a given resourceType 
    Resource ReturnResourceMatch(List<Resource> resources, ECardType resourceTypeToMatch)
    {
        for(int i = 0; i < resources.Count; i++)
        {
            Resource card = resources[i];

            if (card.CardType == resourceTypeToMatch)
                return card;
        }
        return null;
    }

    #region Mouse Check

    //Checks if the player's mouse is over a resourceCard
    RaycastHit2D IsOverCard() => IsOver(cardLayer);

    //Checks if the player's mouse is over the hand
    RaycastHit2D IsOverHandHolder() => IsOver(handHolderLayer);

    //Checks if the player's mouse is over the consumer
    RaycastHit2D IsOverResourceConsumer() => IsOver(resourceConsumerLayer);

    //Checks if the player's mouse is over a certain layer
    public static RaycastHit2D IsOver(LayerMask layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, int.MaxValue, layer);

        return hit;
    }

    public static bool IsOver(LayerMask layer, Transform transformToCheck)
    {
        var hit = IsOver(layer);

        if (hit.transform == transformToCheck)
            return true;

        return false;
    }

    //Returns a vector3 of the player's mouse position
    static public Vector3 GetMousePosition()
    {
        Vector3 mousePos;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z += Camera.main.nearClipPlane;

        return mousePos;
    }

    #endregion

    #region Create Objects & Pools
    //Hopefully I don't need to unsubscribe... 
    void CreateChoicePool() => CreateObjectPool(obj: choiceCard, parent: ChoicePool, size: choicePoolSize, setReady: c => c.ChoiceSelection += OnChoiceSelection);

    void CreateEncounterPool() => CreateObjectPool(obj: encounterCard, parent: EncounterPool, size: encounterPoolSize);

    void CreateResourcePool()
    {
        for (int i = 0; i < resources.Count; i++)
            for (int n = 0; n < resourcePoolSize; n++)
                CreateObject(obj: resourceCard, parent: resourcePool, setReady: card => card.SetResource(resources[i]), list: allCards);
    }

    void CreateIconHolderPool() => CreateObjectPool(obj: iconHolder, parent: iconHolderPool, size: iconHolderPoolSize);

    void CreateIconPool()
    {
        for (int i = 0; i < resources.Count; i++)
            for (int n = 0; n < iconPoolSize; n++)
                CreateObject(obj: icon, parent: iconPool, setReady: icon => icon.SetResource(resources[i]), list: allIcons);
    }


    //Instantiates a given number of inactive objects, as a child of a given transform
    public static List<T> CreateObjectPool<T>(T obj, Transform parent, int size, Action<T> setReady = null, List<T> addToList = null) where T : MonoBehaviour
    {
        List<T> objectList = new();

        for (int i = 0; i < size; i++)
        {
            T createdObject = CreateObject(obj, parent, list: objectList);

            setReady?.Invoke(createdObject);
        }
            

        addToList?.AddRange(objectList);

        return objectList;
    }

    //Instantiates a given number of inactive objects, as a child of a given gameObject
    public static List<T> CreateObjectPool<T>(T obj, GameObject parent, int size, Action<T> setReady = null, List<T> addToList = null) where T : MonoBehaviour => CreateObjectPool(obj, parent.transform, size, setReady, addToList);

    //Instantiates and returns an inactive object, as a child of a given [gameObject], stored in a given list, and readied with a given method
    public static T CreateObject<T>(T obj, GameObject parent, Action<T> setReady, List<T> list) where T : MonoBehaviour => CreateObject(obj, parent.transform, setReady, list);

    //Instantiates and returns an inactive object, as a child of a given [transform], stored in a given list, and readied with a given method
    public static T CreateObject<T>(T obj, Transform parent, Action<T> setReady = null, List<T> list = null) where T : MonoBehaviour
    {
        var createdObj = Instantiate(obj, parent);
        createdObj.gameObject.SetActive(false);

        setReady?.Invoke(createdObj);
        list?.Add(createdObj);

        return createdObj;
    }
    #endregion

    //Retrieves a number of resource cards from the resource pool and adds them to the player's hand
    void AddPoolResourcesToHand(int anomalies = 0, int escapees = 0, int insanities = 0, int munitions = 0, int rations = 0, int scientists = 0)
    {
        List<ResourceCard> resourcesToAdd = new();

        for (int i = 0; i < resourcePool.transform.childCount; i++)
        {
            ResourceCard card = resourcePool.transform.GetChild(i).GetComponent<ResourceCard>();

            void AddResourceIfAvailable(ref int resource)
            {
                if (resource > 0)
                {
                    resource--;
                    resourcesToAdd.Add(card);
                }
            }

            if (card.gameObject.activeSelf == false)
                switch (card._Resource.CardType)
                {
                    case ECardType.Anomaly:
                        AddResourceIfAvailable(ref anomalies);
                        break;
                    case ECardType.Escapee:
                        AddResourceIfAvailable(ref escapees);
                        break;
                    case ECardType.Insanity:
                        AddResourceIfAvailable(ref insanities);
                        break;
                    case ECardType.Munition:
                        AddResourceIfAvailable(ref munitions);
                        break;
                    case ECardType.Ration:
                        AddResourceIfAvailable(ref rations);
                        break;
                    case ECardType.Scientist:
                        AddResourceIfAvailable(ref scientists);
                        break;
                }
        }

        foreach (ResourceCard resourceCard in resourcesToAdd)
            MoveCardToHand(resourceCard);
    }

    //Retrieves a resource card, matching a given type, from the resource pool, to the player's hand
    void AddPoolResourceToHand(ECardType resourceType)
    {
        ResourceCard cardTypeToAdd = GetFromResourcePool(resourceType);

        if (cardTypeToAdd == null)
        {
            Debug.LogWarning("Card to Add is null; no more cards in resource pool to add.");
            return;
        }
        MoveCardToHand(cardTypeToAdd);
    }

    //Ensures every active resource card's data matches their scriptable object
    void DataMatchResources()
    {
        var handAndConsumerResources = Hand.Concat(Consumer);

        foreach(ResourceCard resourceCard in handAndConsumerResources)
            resourceCard.DataMatchResource();
    }

    //Ensures resource cards in the consumer and hand are size appropriate for their current zone
    void CheckCardSizes()
    {
        CheckCardSize(Hand, handHolderCardScale);
        CheckCardSize(Consumer, resourceConsumerCardScale);

        static void CheckCardSize(List<ResourceCard> listToCheck, Vector3 mandatorySize)
        {
            for (int i = 0; i < listToCheck.Count; i++)
            {
                var currentCard = listToCheck[i];

                if (currentCard.transform.localScale != mandatorySize)
                    currentCard.transform.localScale = mandatorySize;
            }
        }
    }

    //We should instead set blanks rather than grab specific value types
    #region Get From Pool
    //Returns the first inactive choice from the choice pool
    public ChoiceCard GetFromChoicePool() => GetTypeFromPool<ChoiceCard>(ChoicePool);

    //Returns the first inactive resource matching a given resource type, from the resource pool
    ResourceCard GetFromResourcePool(ECardType resourceType) => GetTypeFromPool<ResourceCard>(resourcePool, c => c._Resource.CardType == resourceType);

    //Grabs the first inactive Icon Holder, it grabs all the icons it needs, and then we return it
    public IconHolder GetFromIconHolderPool(params Resource[] resources)
    {
        IconHolder iconHolder = GetTypeFromPool<IconHolder>(iconHolderPool);

        iconHolder.GrabIcons(resources);

        return iconHolder;
    }

    //Returns the first inactive icon matching a given resource type, from the icon pool
    public Icon GetFromIconPool(ECardType resourceType) => GetTypeFromPool<Icon>(iconPool, i => i.IconResource.CardType == resourceType);

    //Returns the first inactive icon matching a given resource object, from the icon pool
    public Icon GetFromIconPool(Resource resource) => GetTypeFromPool<Icon>(iconPool, i => i.IconResource == resource);

    //Returns the first inactive object from a given object pool that matches a given condition, if a condition is given
    public static T GetTypeFromPool<T>(GameObject parent, Predicate<T> extraCondition = null) where T : MonoBehaviour
    {
        if (!(parent.transform.childCount > 0))
            return null;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            T returnIcon = parent.transform.GetChild(i).GetComponent<T>();

            if (returnIcon.gameObject.activeSelf == false)
                if (extraCondition == null || extraCondition(returnIcon))
                    return returnIcon;
        }
        return null;
    }
    #endregion

    //Removes all cards in the consumer and moves them to the resource pool.
    //To do : Make a method to move cards into a given pool
    void ConsumeAllCards()
    {
        while (Consumer.Count > 0)
        {
            var currentCard = Consumer[0];

            MoveToPool(currentCard, newParent: resourcePool, cleanup: RemoveFromConsumer);
        }
    }

    //Responsibility is to perform cleanup after a choice option is selected
    //Adds rewards and consumes all cards
    //TO DO : Adds 'rewards' to the player's deck
    void OnChoiceSelection(object sender, CardSelectionEventArgs args)
    {
        ConsumeAllCards();

        for (int i = 0; i < args.ResourceRewards.Length; i++)
        {
            var currentResourceType = args.ResourceRewards[i];

            AddPoolResourceToHand(currentResourceType);
        }
    }

    #region Move To
    void MoveToPool<T>(T objectToMove, GameObject newParent, Action<T> cleanup = null) where T : MonoBehaviour
    {
        cleanup?.Invoke(objectToMove);

        objectToMove.gameObject.SetActive(false);

        objectToMove.transform.SetParent(newParent.transform);
    }

    void MoveCardToHolding(ResourceCard resourceCard)
    {
        holdingResourceCard = resourceCard;

        MoveTo(resourceCard, newParent: null, keepWorldPosition: true, Hand, Consumer);
    }

    void MoveCardToHand(ResourceCard resourceCard)
    {
        MoveTo(resourceCard, newParent: handHolder, keepWorldPosition: false, newLocalScale: handHolderCardScale, displayIndicator: false, addToList: Hand, removeFromLists: Consumer);
    }

    void MoveCardToConsumer(ResourceCard resourceCard)
    {
        var parent = resourceConsumer.transform.Find(resourceCard._Resource.CardType.ToString());

        MoveTo(resourceCard, parent, keepWorldPosition: false, newLocalScale: resourceConsumerCardScale, addToList: Consumer, displayIndicator: true, removeFromLists: Hand);

        UpdateConsumerIndicators(resourceCard._Resource.CardType, -1);

        OnCardChangeInConsumer?.Invoke(resourceCard._Resource.CardType, true);
    }


    //Prepares a card to be added into a new zone
    //Gives the card a new parent and removes it from a set of given lists
    void MoveTo(ResourceCard resourceCardToMove, Transform newParent, bool keepWorldPosition, params List<ResourceCard>[] removeFromLists)
    {
        resourceCardToMove.transform.SetParent(newParent, keepWorldPosition);

        foreach (List<ResourceCard> list in removeFromLists)
        {
            if (list == Consumer)
                RemoveFromConsumer(resourceCardToMove);

            else if (list.Contains(resourceCardToMove))
                list.Remove(resourceCardToMove);
        }
    }

    //Prepares a card to be added into a new zone
    //Given a resource card, sets it active, sets a new parent, sets its local position and scale, removes it from a given set of lists
    void MoveTo(ResourceCard resourceCardToMove, GameObject newParent, bool keepWorldPosition, Vector3 newLocalScale, List<ResourceCard> addToList, bool displayIndicator, params List<ResourceCard>[] removeFromLists)
    {
        if (!resourceCardToMove.isActiveAndEnabled)
            resourceCardToMove.gameObject.SetActive(true);

        resourceCardToMove.IndicatorBackground.gameObject.SetActive(displayIndicator);

        Transform transform = newParent == null ? null : newParent.transform;

        MoveTo(resourceCardToMove, transform, keepWorldPosition, removeFromLists);

        resourceCardToMove.transform.localPosition = Vector3.zero;

        resourceCardToMove.transform.localScale = newLocalScale;

        addToList?.Add(resourceCardToMove);
    }

    //Same as the above, except this accepts a Transform instead of a gameObject
    void MoveTo(ResourceCard resourceCardToMove, Transform newParent, bool keepWorldPosition, Vector3 newLocalScale, List<ResourceCard> addToList, bool displayIndicator, params List<ResourceCard>[] removeFromLists)
    {
        GameObject gameObject = newParent == null ? null : newParent.gameObject;

        MoveTo(resourceCardToMove, gameObject, keepWorldPosition, newLocalScale, addToList, displayIndicator, removeFromLists);
    }

    void RemoveFromConsumer(ResourceCard resourceCard)
    {
        if (Consumer.Remove(resourceCard))
            OnCardChangeInConsumer?.Invoke(resourceCard._Resource.CardType, false);
    }
    #endregion

    //Updates the consumer indicator of a given type, by a given amount
    void UpdateConsumerIndicators(ECardType resourceType, int amountToAdd)
    {
        List<ResourceCard> typeCardsInConsumer = GetResourcesFromConsumer(resourceType);

        amountToAdd += typeCardsInConsumer.Count;

        if (amountToAdd >= indicators.Count)
            amountToAdd = indicators.Count - 1;

        foreach (ResourceCard card in typeCardsInConsumer)
            card.IndicatorNumber.sprite = indicators[amountToAdd];
    }

    //Returns a list of all resource cards that match a given type in the consumer
    public List<ResourceCard> GetResourcesFromConsumer(ECardType resourceType)
    {
        List<ResourceCard> resourceCards = new();

        foreach (ResourceCard card in Consumer)
            if (card._Resource.CardType == resourceType)
                resourceCards.Add(card);

        return resourceCards;
    }

    #region Card Pickup
    //Saves the data of the grabbed card to be used later, updates the consumer, and sets the card as the holding card
    public void GrabCard()
    {
        var isOverCard = IsOverCard();

        if (isOverCard && Input.GetMouseButtonDown(0))
        {
            var resourceCard = isOverCard.transform.gameObject.GetComponent<ResourceCard>();

            MoveCardToHolding(resourceCard);

            regularScale = holdingResourceCard.transform.localScale;

            regularSortingOrder = holdingResourceCard._CanvasComponent.sortingOrder;

            holdingResourceCard.transform.localScale *= holdingCardScaleMultiplier;
            
            holdingResourceCard._CanvasComponent.sortingLayerID = holdingCardLayer;

            UpdateConsumerIndicators(holdingResourceCard._Resource.CardType, -1);
        }
    }

    //Rewrites the data of the card to be the same as it was when grabbed, and sets it to the proper zone
    public void DropCard()
    {
        if (holdingResourceCard != null && Input.GetMouseButtonUp(0))
        {
            holdingResourceCard.transform.localScale = regularScale;
            holdingResourceCard._CanvasComponent.sortingOrder = regularSortingOrder;

            if (IsOverHandHolder() && IsOverResourceConsumer())
                Debug.LogWarning("Mouse is over two colliders. Bug.");

            var resources = GetResourcesFromConsumer(holdingResourceCard._Resource.CardType);
            
            if (IsOverResourceConsumer() && resources.Count < 5)
                MoveCardToConsumer(holdingResourceCard);
            else
                MoveCardToHand(holdingResourceCard);

            holdingResourceCard = null;
        }
        
    }

    public void DragCard()
    {
        if (holdingResourceCard != null)
            holdingResourceCard.transform.position = GetMousePosition();
    }
    #endregion
}