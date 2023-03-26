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
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    GameObject resourcePool;
    GameObject handHolder;
    GameObject resourceConsumer;
    GameObject iconPool;

    [SerializeField] int holdingCardLayer;
    [SerializeField] int newestCardLayer;
    [SerializeField] int defaultLayer;

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

    public List<GameObject> deckDiscard = new List<GameObject>();
    public List<GameObject> deck = new List<GameObject>();
    public List<ResourceCard> hand = new List<ResourceCard>();
    public List<ResourceCard> consumer { get; private set; } = new List<ResourceCard>();
    public List<Resource> resources;
    public List<Resource> resources1;

    List<ResourceCard> allCards = new List<ResourceCard>();
    List<Icon> allIcons = new List<Icon>();

    public List<Sprite> indicators;
    //public List<bool> availableHandSlots = new List<bool>();

    ResourceCard holdingResourceCard = null;
    Vector3 grabbedPosition;
    Vector3 regularScale = Vector3.one;
    int regularSortingOrder = 0;

    //NOTE TO SELF: In order to create cool card overlap effect, create slots that hold the cards and make sure to have worldPosition stay true when setting the parent

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        handHolder = GameObject.Find("HandHolder");
        resourcePool = GameObject.Find("ResourcePool");
        resourceConsumer = GameObject.Find("ResourceConsumer");
        iconPool = GameObject.Find("IconPool");

        resources = new List<Resource> { ration, escapee, scientist, insanity, munition, anomaly };
        resources1 = new List<Resource> { ration1, escapee1, scientist1, insanity1, munition1, anomaly1 };

        //indicators = new List<Sprite> { oneIndicator, twoIndicator, threeIndicator, fourIndicator, fiveIndicator };

        CreateResourcePool();
        CreateIconPool();
    }

    void Start()
    {
        for (int i = 0; i < 1; i++)
        {
            AddPoolResourcesToHand(1, 1, 1, 1, 1, 1);
        }

        for(int i = 0; i < resources.Count; i++)
        {
            //Debug.Log(resources[i].name);
            //AddCardToConsumer(GetFromResourcePool(resources[i].CardType));
        }

        //Debug.Log($"Getting anomaly from Icon Pool: {GetFromIconPool(anomaly)}");
    }

    public void AddIcon(Icon iconToAdd)
    {
        allIcons.Add(iconToAdd);
    }

    void Update()
    {
        //Debug.Log($"Is over card? : {IsOverCard()}");

        GrabCard();
        DragCard();
        DropCard();

        CheckCardSizes();

        //Debug.Log($"Is over card? : {(IsOverCard() == true)}");
        //Debug.Log($"Is over hand? : {(IsOverHandHolder() == true)}");
        //Debug.Log($"Is over consumer? : {(IsOverResourceConsumer() == true)}");

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
    }

    void ReadyChoices()
    {

    }

    public static List<Resource.ECardType> ConvertResourceCardListToResourceType(List<ResourceCard> resourceCards)
    {
        List<Resource.ECardType> returnList = new List<Resource.ECardType>();

        for (int i = 0; i < resourceCards.Count; i++)
        {
            var card = resourceCards[i];

            var cardType = card._Resource.CardType;

            returnList.Add(cardType);
        }
        return returnList;
    }

    public static List<Resource.ECardType> ConvertResourceCardListToResourceType(List<Resource> resourceCards)
    {
        List<Resource.ECardType> returnList = new List<Resource.ECardType>();

        for (int i = 0; i < resourceCards.Count; i++)
        {
            var card = resourceCards[i];

            var cardType = card.CardType;

            returnList.Add(cardType);
        }
        return returnList;
    }

    

    void SwapResources(List<Resource> resources)
    {
        for(int i = 0; i < allCards.Count; i++)
        {
            var card = allCards[i];

            card.SetResource(ReturnResourceMatch(resources, card._Resource.CardType));
        }
    }

    void SwapIcons(List<Resource> resources)
    {
        for (int i = 0; i < allIcons.Count; i++)
        {
            var icon = allIcons[i];

            icon.SetResource(ReturnResourceMatch(resources, icon.IconResource.CardType));
        }
    }

    Resource ReturnResourceMatch(List<Resource> resources, Resource.ECardType resourceTypeToMatch)
    {
        for(int i = 0; i < resources.Count; i++)
        {
            var card = resources[i];

            if (card.CardType == resourceTypeToMatch)
                return card;
        }
        return null;
    }

    RaycastHit2D IsOverCard()
    {
        return IsOver(cardLayer);
    }

    RaycastHit2D IsOverHandHolder()
    {
        return IsOver(handHolderLayer);
    }

    RaycastHit2D IsOverResourceConsumer()
    {
        return IsOver(resourceConsumerLayer);
    }

    RaycastHit2D IsOver(LayerMask layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, int.MaxValue, layer);

        return hit;
    }

    static public Vector3 GetMousePosition()
    {
        Vector3 mousePos;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z += Camera.main.nearClipPlane;

        return mousePos;
    }

    void CreateResourcePool()
    {
        for (int i = 0; i < resources.Count; i++)
        {
            for (int n = 0; n < resourcePoolSize; n++)
            {
                var card = Instantiate(resourceCard, resourcePool.transform);
                card.SetResource(resources[i]);
                card.gameObject.SetActive(false);

                allCards.Add(card);
            }
        }
    }

    void CreateIconPool()
    {
        for (int i = 0; i < resources.Count; i++)
        {
            for (int n = 0; n < iconPoolSize; n++)
            {
                var icon = Instantiate(this.icon, iconPool.transform);
                icon.SetResource(resources[i]);
                icon.gameObject.SetActive(false);

                allIcons.Add(icon);
            }
        }
    }

    void AddPoolResourcesToHand(int anomalies, int escapees, int insanities, int munitions, int rations, int scientists)
    {
        List<ResourceCard> resourcesToAdd = new List<ResourceCard>();

        for(int i = 0; i < resourcePool.transform.childCount; i++)
        {
            ResourceCard card = resourcePool.transform.GetChild(i).GetComponent<ResourceCard>();

            if (card.gameObject.activeSelf == false)
            {
                switch (card._Resource.CardType)
                {
                    case Resource.ECardType.Anomaly:
                        if(anomalies > 0)
                        {
                            anomalies--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.ECardType.Escapee:
                        if(escapees > 0)
                        {
                            escapees--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.ECardType.Insanity:
                        if(insanities > 0)
                        {
                            insanities--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.ECardType.Munition:
                        if(munitions > 0)
                        {
                            munitions--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.ECardType.Ration: 
                        if(rations > 0)
                        {
                            rations--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.ECardType.Scientist:
                        if(scientists > 0)
                        {
                            scientists--;
                            resourcesToAdd.Add(card);
                        }
                        break;  
                }
            }
        }

        for(int i = 0; i < resourcesToAdd.Count; i++)
        {
            var currentCard = resourcesToAdd[i];

            AddCardToHand(currentCard);
        }
    }

    void AddPoolResourceToHand(Resource.ECardType resourceType)
    {
        ResourceCard cardTypeToAdd = GetFromResourcePool(resourceType);

        if (cardTypeToAdd == null)
        {
            Debug.LogWarning("Card to Add is null; no more cards in resource pool to add.");
            return;
        }
        AddCardToHand(cardTypeToAdd);
    }

    void DataMatchResources()
    {
        var handAndConsumerResources = hand.Concat(consumer);

        foreach(ResourceCard resource in handAndConsumerResources)
        {
            resource.DataMatchResource();
        }
    }

    void CheckCardSizes()
    {
        CheckCardSize(hand, handHolderCardScale);
        CheckCardSize(consumer, resourceConsumerCardScale);
    }

    void CheckCardSize(List<ResourceCard> listToCheck, Vector3 mandatoryHandSize)
    {
        for(int i = 0; i < listToCheck.Count; i++)
        {
            var currentCard = listToCheck[i];

            if (currentCard.transform.localScale != mandatoryHandSize)
            {
                currentCard.transform.localScale = mandatoryHandSize;
            }
        }
    }

    ResourceCard GetFromResourcePool(Resource.ECardType resourceType)
    {
        if (!(resourcePool.transform.childCount > 0))
            return null;

        for (int i = 0; i < resourcePool.transform.childCount; i++)
        {
            var cardToReturn = resourcePool.transform.GetChild(i).GetComponent<ResourceCard>();

            if (cardToReturn.gameObject.activeSelf == false)
            {
                if (cardToReturn._Resource.CardType == resourceType)
                {
                    return cardToReturn;
                }
            }
        }
        return null;
    }

    public Icon GetFromIconPool(Resource.ECardType resourceType)
    {
        return GetIconFromPool(icon => icon.IconResource.CardType == resourceType);
    }

    public Icon GetFromIconPool(Resource resource)
    {
        return GetIconFromPool(icon => icon.IconResource == resource);
    }

    private Icon GetIconFromPool(Func<Icon, bool> condition)
    {
        if (!(iconPool.transform.childCount > 0))
            return null;

        for (int i = 0; i < iconPool.transform.childCount; i++)
        {
            Icon returnIcon = iconPool.transform.GetChild(i).GetComponent<Icon>();

            if (returnIcon.gameObject.activeSelf == false)
            {
                if (condition(returnIcon))
                    return returnIcon;
            }
        }

        return null;
    }

    void AddToHolding(ResourceCard resourceCard)
    {
        holdingResourceCard = resourceCard;

        resourceCard.transform.SetParent(null);
        hand.Remove(resourceCard);
        consumer.Remove(resourceCard);
    }

    void AddCardToHand(ResourceCard resourceCard)
    {
        AddTo(resourceCard, handHolder.transform, false, handHolderCardScale);

        resourceCard.IndicatorBackground.gameObject.SetActive(false);

        hand.Add(resourceCard);

        consumer.Remove(resourceCard);
    }

    void AddCardToConsumer(ResourceCard resourceCard)
    {
        /*
        if (resourceCard._Resource == null)
            Debug.LogWarning("Resource ResouceCard is null");
        if (resourceConsumer == null)
            Debug.LogWarning("ResourceConsumer is null");
        if (resourceConsumer.transform.Find(resourceCard._Resource.CardType.ToString()) == null)
            Debug.Log("Warning, cannot find resource consumer card holder");

        Debug.Log($"Resource Card Type: {resourceCard._Resource.CardType}");
        */
        resourceCard.IndicatorBackground.gameObject.SetActive(true);

        var parent = resourceConsumer.transform.Find(resourceCard._Resource.CardType.ToString());

        AddTo(resourceCard, parent, false, resourceConsumerCardScale);

        consumer.Add(resourceCard);
        var indicatorNum = UpdateConsumerIndicators(resourceCard._Resource.CardType, -1);

        hand.Remove(resourceCard);

        //Debug.Log($"Is numberSymbol null? : {(resourceCard.ResourceSymbol == null)}");
    }

    void AddTo(ResourceCard resourceCard, Transform transformParent, bool keepWorldPosition, Vector3 newLocalScale)
    {
        if (!resourceCard.isActiveAndEnabled)
            resourceCard.gameObject.SetActive(true);

        resourceCard.transform.SetParent(transformParent, keepWorldPosition);

        resourceCard.transform.localPosition = Vector3.zero;

        resourceCard.transform.localScale = newLocalScale;
    }

    int UpdateConsumerIndicators(Resource.ECardType resourceType, int indicatorNum)
    {
        //Debug.Log("Updated Indicators");
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

    List<ResourceCard> GetResourcesFromConsumer(Resource.ECardType resourceType)
    {
        List<ResourceCard> resourceCards = new List<ResourceCard>();
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
            //Debug.Log("Grabbing card");

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
            //Debug.Log("Dropping card");

            holdingResourceCard.transform.localScale = regularScale;
            holdingResourceCard._CanvasComponent.sortingOrder = regularSortingOrder;

            if (IsOverHandHolder() && IsOverResourceConsumer())
            {
                Debug.LogWarning("Mouse is over two colliders. Bug.");
            }

            var resources = GetResourcesFromConsumer(holdingResourceCard._Resource.CardType);

            
            if (IsOverResourceConsumer() && resources.Count < 5)
            {
               //Debug.Log("Dropped card in consumer");
                AddCardToConsumer(holdingResourceCard);
            }
            else
            {
                //Debug.Log("Dropped card in hand");
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
