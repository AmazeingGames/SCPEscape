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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    GameObject resourcePool;
    GameObject handHolder;
    GameObject resourceConsumer;
    GameObject iconPool;

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
    [SerializeField] Icon icon;

    [SerializeField] float holdingCardScaleMultiplier;
    [SerializeField] Vector3 resourceConsumerCardScale;
    [SerializeField] Vector3 handHolderCardScale;
    [SerializeField] LayerMask handHolderLayer;
    [SerializeField] LayerMask resourceConsumerLayer;
    [SerializeField] LayerMask cardLayer;
    [SerializeField] int resourcePoolSize;
    [SerializeField] ResourceCard resourceCard;

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

    public bool hasAddedResources = false;

    public Action<Resource.ECardType, bool> onCardChangeInConsumer;
    public Action<Resource.ECardType[]> onChoiceSelection;


    public List<ResourceCard> hand = new List<ResourceCard>();
    public List<ResourceCard> consumer { get; private set; } = new List<ResourceCard>();

    public List<Resource> resources;
    public List<Resource> resources1;

    List<ResourceCard> allCards = new List<ResourceCard>();
    List<Icon> allIcons = new List<Icon>();

    public List<Sprite> indicators;

    ResourceCard holdingResourceCard = null;
    Vector3 grabbedPosition;
    Vector3 regularScale = Vector3.one;
    int regularSortingOrder = 0;

    EncounterDeck encounterDeck;
    EncounterCard currentEncounter;

    public List<Encounter> EncounterDeck { get; private set; } = new();
    public List<Encounter> DiscardPile { get; private set; } = new();

    public EncounterCard ActiveEncounter { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        handHolder = GameObject.Find("HandHolder");
        resourcePool = GameObject.Find("ResourcePool");
        resourceConsumer = GameObject.Find("ResourceConsumer");
        ChoicePool = GameObject.Find("ChoicePool");
        iconPool = GameObject.Find("IconPool");
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
    }

    void Start()
    {
        AddStartingResources();

        //Not sure what this code is for
        for (int i = 0; i < resources.Count; i++)
        {
            //Debug.Log(resources[i].name);
            //AddCardToConsumer(GetFromResourcePool(resources[i].CardType));
        }

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

        SelectChoice();
    }

    void AddStartingResources()
    {
        //For loop can be removed
        for (int i = 0; i < 1; i++)
        {
            AddPoolResourcesToHand(1, 1, 1, 1, 1, 1);
        }
    }

    //Adds resources to hand correlated to the nums 0 - 9
    void DeveloperCommands()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DataMatchResources();
        }
        DataMatchResources();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddPoolResourceToHand(Resource.ECardType.Anomaly);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddPoolResourceToHand(Resource.ECardType.Escapee);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddPoolResourceToHand(Resource.ECardType.Insanity);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddPoolResourceToHand(Resource.ECardType.Munition);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AddPoolResourceToHand(Resource.ECardType.Ration);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AddPoolResourceToHand(Resource.ECardType.Scientist);
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

    public void AddIcon(Icon iconToAdd)
    {
        allIcons.Add(iconToAdd);
    }


    //Given a list of Resource Cards, returns a list of their resource types 
    public static List<Resource.ECardType> ConvertResourceCardListToResourceType(List<ResourceCard> resourceCards)
    {
        return ListToResourceTypeList(resourceCards, t => t._Resource.CardType);
    }

    //Given a list of Resource Objects, returns a list of their resource type
    public static List<Resource.ECardType> ConvertResourceCardListToResourceType(List<Resource> resourceCards)
    {      
        return ListToResourceTypeList(resourceCards, t => t.CardType);
    }

    //Given a list of T, returns a list of resource types | Brush up on : 
    //Generics              (pg 222)
    //Delegates             (pg 282)
    //Lambda Expressions    (pg 294)
    static List<Resource.ECardType> ListToResourceTypeList<T>(List<T> listToConvert, Func<T, Resource.ECardType> conversionMethod)
    {
        List<Resource.ECardType> returnList = new();

        for (int i = 0; i < listToConvert.Count; i++)
        {
            var current = listToConvert[i];

            Resource.ECardType cardType = conversionMethod(current);

            returnList.Add(cardType);
        }
        return returnList;
    }

    //Sets the resource object of all resources cards
    void SwapResources(List<Resource> resources)
    {
        SetResourceObject(allCards, c => c.SetResource(ReturnResourceMatch(resources, c._Resource.CardType)));
    }

    //Sets the resource object of all resource icons
    void SwapIcons(List<Resource> resources)
    {
        SetResourceObject(allIcons, i => i.SetResource(ReturnResourceMatch(resources, i.IconResource.CardType)));
    }

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

    //Checks if the player's mouse is over a resourceCard
    RaycastHit2D IsOverCard()
    {
        return IsOver(cardLayer);
    }

    //Checks if the player's mouse is over the hand
    RaycastHit2D IsOverHandHolder()
    {
        return IsOver(handHolderLayer);
    }

    //Checks if the player's mouse is over the consumer
    RaycastHit2D IsOverResourceConsumer()
    {
        return IsOver(resourceConsumerLayer);
    }

    //Checks if the player's mouse is over a certain layer
    public static RaycastHit2D IsOver(LayerMask layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, int.MaxValue, layer);

        return hit;
    }

    //Returns a vector3 of the player's mouse position
    static public Vector3 GetMousePosition()
    {
        Vector3 mousePos;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z += Camera.main.nearClipPlane;

        return mousePos;
    }
    
    //Creates the object pool for choice cards
    void CreateChoicePool()
    {
        CreateObjectPool(obj: choiceCard, parent: ChoicePool, size: choicePoolSize);
    }

    //Creates the object pool for encounter cards
    void CreateEncounterPool()
    {
        CreateObjectPool(obj: encounterCard, parent: EncounterPool, size: encounterPoolSize);
    }

    //Creates the object pool for resource cards
    void CreateResourcePool()
    {
        for (int i = 0; i < resources.Count; i++)
        {
            for (int n = 0; n < resourcePoolSize; n++)
            {
                CreateObject(obj: resourceCard, parent: resourcePool, setReady: card => card.SetResource(resources[i]), list: allCards);
            }
        }
    }

    //Creates the object pool for resource icons
    void CreateIconPool()
    {
        for (int i = 0; i < resources.Count; i++)
        {
            for (int n = 0; n < iconPoolSize; n++)
            {
                CreateObject(obj: icon, parent: iconPool, setReady: icon => icon.SetResource(resources[i]), list: allIcons);
            }
        }
    }

    //Instantiates an inactive pool of objects, of a given size, under a given parent
    void CreateObjectPool<T>(T obj, Transform parent, int size) where T : MonoBehaviour
    {
        for (int i = 0; i < size; i++)
        {
            CreateObject(obj, parent);
        }
    }

    //Instantiates an inactive pool of objects, of a given size, under a given parent
    void CreateObjectPool<T>(T obj, GameObject parent, int size) where T : MonoBehaviour
    {
        CreateObjectPool(obj, parent.transform, size);
    }

    //Instantiates and returns an inactive object, under a given parent, stored in a given list, and readied with a given method
    T CreateObject<T>(T obj, GameObject parent, Action<T> setReady, List<T> list) where T : MonoBehaviour
    {
        return CreateObject(obj, parent.transform, setReady, list);
    }

    //Instantiates and returns an inactive object, under a given parent, stored in a given list, and readied with a given method
    T CreateObject<T>(T obj, Transform parent, Action<T> setReady = null, List<T> list = null) where T : MonoBehaviour
    {
        var createdObj = Instantiate(obj, parent);
        createdObj.gameObject.SetActive(false);

        setReady?.Invoke(createdObj);
        list?.Add(createdObj);

        return createdObj;
    }

    //Retrieves a number of resource cards from the resource pool and adds them to the player's hand
    void AddPoolResourcesToHand(int anomalies, int escapees, int insanities, int munitions, int rations, int scientists)
    {
        List<ResourceCard> resourcesToAdd = new List<ResourceCard>();

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
            {
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
        }

        foreach (ResourceCard resourceCard in resourcesToAdd)
            AddCardToHand(resourceCard);
    }

    //Retrieves a resource card from the resource pool and adds it to the player's hand
    void AddPoolResourceToHand(ECardType resourceType)
    {
        ResourceCard cardTypeToAdd = GetFromResourcePool(resourceType);

        if (cardTypeToAdd == null)
        {
            Debug.LogWarning("Card to Add is null; no more cards in resource pool to add.");
            return;
        }
        AddCardToHand(cardTypeToAdd);
    }

    //Makes sure every resource card in the games has their data is matched to the scriptable object
    void DataMatchResources()
    {
        var handAndConsumerResources = hand.Concat(consumer);

        foreach(ResourceCard resourceCard in handAndConsumerResources)
            resourceCard.DataMatchResource();
    }

    //Makes sure resource cards in the consumer and hand are size appropriate for their zone
    void CheckCardSizes()
    {
        CheckCardSize(hand, handHolderCardScale);
        CheckCardSize(consumer, resourceConsumerCardScale);
    }

    //Makes sure all the cards in a given list are of a given size
    void CheckCardSize(List<ResourceCard> listToCheck, Vector3 mandatorySize)
    {
        for(int i = 0; i < listToCheck.Count; i++)
        {
            var currentCard = listToCheck[i];

            if (currentCard.transform.localScale != mandatorySize)
            {
                currentCard.transform.localScale = mandatorySize;
            }
        }
    }

    //Returns the first inactive choice from the choice pool
    public ChoiceCard GetFromChoicePool() => GetTypeFromPool<ChoiceCard>(ChoicePool);

    //Returns the first inactive resource, matching a given resource type, from the resource pool
    ResourceCard GetFromResourcePool(ECardType resourceType) => GetTypeFromPool<ResourceCard>(resourcePool, c => c._Resource.CardType == resourceType);

    //Returns the first inactive icon, matching a given resource type, from the icon pool
    public Icon GetFromIconPool(ECardType resourceType) => GetTypeFromPool<Icon>(iconPool, i => i.IconResource.CardType == resourceType);

    //Returns the first inactive icon, matching a given resource object, from the icon pool
    public Icon GetFromIconPool(Resource resource) => GetTypeFromPool<Icon>(iconPool, i => i.IconResource == resource);

    //Returns the first inactive object from an object pool that matches a given condition
    private T GetTypeFromPool<T>(GameObject parent, Func<T, bool> extraCondition = null) where T : MonoBehaviour
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

    void AddToHolding(ResourceCard resourceCard)
    {
        holdingResourceCard = resourceCard;

        resourceCard.transform.SetParent(null);

        hand.Remove(resourceCard);

        RemoveFromConsumer(resourceCard);
    }

    void AddCardToHand(ResourceCard resourceCard)
    {
        AddTo(resourceCard, handHolder.transform, false, handHolderCardScale);

        resourceCard.IndicatorBackground.gameObject.SetActive(false);

        hand.Add(resourceCard);

        RemoveFromConsumer(resourceCard);
    }

    //Purpose is to remove all cards in the consumer; moves them to resource pool.
    void ConsumeAllCards()
    {
        while (consumer.Count > 0)
        {
            var currentCard = consumer[0];

            currentCard.gameObject.SetActive(false);

            currentCard.transform.SetParent(resourcePool.transform);

            RemoveFromConsumer(currentCard);
        }
    }

    //Invoked when a choice is selected:
    //Consumes all cards in the consumer
    //Adds 'rewards' to the player's hand
    //TO DO : Adds 'rewards' to the player's deck
    void ChoiceSelection(Resource.ECardType[] choiceRewards)
    {
        ConsumeAllCards();

        for (int i = 0; i < choiceRewards.Length; i++)
        {
            var currentResourceType = choiceRewards[i];

            AddPoolResourceToHand(currentResourceType);
        }
    }

    //Responsible for subscribing to subscribing and calling the 'ChoiceSelection' function
    void SelectChoice()
    {
        onChoiceSelection -= ChoiceSelection;
        onChoiceSelection += ChoiceSelection;
    }

    void RemoveFromConsumer(ResourceCard resourceCard)
    {
        if (consumer.Remove(resourceCard))
        {
            onCardChangeInConsumer?.Invoke(resourceCard._Resource.CardType, false);
        }
    }

    void AddCardToConsumer(ResourceCard resourceCard)
    {
        resourceCard.IndicatorBackground.gameObject.SetActive(true);

        var parent = resourceConsumer.transform.Find(resourceCard._Resource.CardType.ToString());

        AddTo(resourceCard, parent, false, resourceConsumerCardScale);

        consumer.Add(resourceCard);
        var indicatorNum = UpdateConsumerIndicators(resourceCard._Resource.CardType, -1);

        onCardChangeInConsumer?.Invoke(resourceCard._Resource.CardType, true);

        hand.Remove(resourceCard);
    }

    void AddTo(ResourceCard resourceCardToAdd, Transform transformParent, bool keepWorldPosition, Vector3 newLocalScale)
    {
        if (!resourceCardToAdd.isActiveAndEnabled)
            resourceCardToAdd.gameObject.SetActive(true);

        resourceCardToAdd.transform.SetParent(transformParent, keepWorldPosition);

        resourceCardToAdd.transform.localPosition = Vector3.zero;

        resourceCardToAdd.transform.localScale = newLocalScale;
    }

    int UpdateConsumerIndicators(Resource.ECardType resourceType, int indicatorNum)
    {
        List<ResourceCard> resourceCards = GetResourcesFromConsumer(resourceType);

        indicatorNum += resourceCards.Count;

        if (indicatorNum >= indicators.Count)
            indicatorNum = indicators.Count - 1;

        for (int i = 0; i < resourceCards.Count; i++)
        {
            var card = resourceCards[i];
            card.IndicatorNumber.sprite = indicators[indicatorNum];
        }

        return indicatorNum;
    }

    public List<ResourceCard> GetResourcesFromConsumer(Resource.ECardType resourceType)
    {
        List<ResourceCard> resourceCards = new();

        for (int i = 0; i < consumer.Count; i++)
        {
            var card = consumer[i];

            if (card._Resource.CardType == resourceType)
            {
                resourceCards.Add(card);
            }
        }

        return resourceCards;
    }

    void PrintHand()
    {
        string result = "List contents: ";
        for(int i = 0; i < hand.Count; i++)
        {
            ResourceCard item = hand[i];

            result += item._Resource.ToString() + ", ";
        }
        Debug.Log(result);
    }

    public void GrabCard()
    {
        var isOverCard = IsOverCard();

        if (isOverCard && Input.GetMouseButtonDown(0))
        {
            var resourceCard = isOverCard.transform.gameObject.GetComponent<ResourceCard>();

            AddToHolding(resourceCard);

            regularScale = holdingResourceCard.transform.localScale;
            regularSortingOrder = holdingResourceCard._CanvasComponent.sortingOrder;

            holdingResourceCard.transform.localScale *= holdingCardScaleMultiplier;
            
            holdingResourceCard._CanvasComponent.sortingLayerID = holdingCardLayer;

            UpdateConsumerIndicators(holdingResourceCard._Resource.CardType, -1);
        }
    }

    public void DropCard()
    {
        if (holdingResourceCard != null && Input.GetMouseButtonUp(0))
        {
            holdingResourceCard.transform.localScale = regularScale;
            holdingResourceCard._CanvasComponent.sortingOrder = regularSortingOrder;

            if (IsOverHandHolder() && IsOverResourceConsumer())
            {
                Debug.LogWarning("Mouse is over two colliders. Bug.");
            }

            var resources = GetResourcesFromConsumer(holdingResourceCard._Resource.CardType);

            
            if (IsOverResourceConsumer() && resources.Count < 5)
            {
                AddCardToConsumer(holdingResourceCard);
            }
            else
            {
                AddCardToHand(holdingResourceCard);
            }

            holdingResourceCard = null;
        }
        
    }

    public void DragCard()
    {
        if (holdingResourceCard != null)
        {
            holdingResourceCard.transform.position = GetMousePosition();
        }
    }


}
