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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    GameObject resourcePool;
    GameObject handHolder;
    GameObject resourceConsumer;

    [SerializeField] int holdingCardLayer;
    [SerializeField] int newestCardLayer;
    [SerializeField] int defaultLayer;

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


    public bool hasAddedResources = false;

    public List<GameObject> deckDiscard = new List<GameObject>();
    public List<GameObject> deck = new List<GameObject>();
    public List<ResourceCard> hand = new List<ResourceCard>();
    public List<ResourceCard> consumer = new List<ResourceCard>();
    public List<Resource> resources;
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

        resources = new List<Resource> { ration, escapee, scientist, insanity, munition, anomaly };
        //indicators = new List<Sprite> { oneIndicator, twoIndicator, threeIndicator, fourIndicator, fiveIndicator };

        CreateResourcePool();
    }

    void Start()
    {
        for (int i = 0; i < 1; i++)
        {
            AddPoolResourcesToHand(1, 1, 1, 1, 1, 1);
        }

        for(int i = 0; i < resources.Count; i++)
        {
            Debug.Log(resources[i].name);
            AddCardToConsumer(GetFromResourcePool(resources[i]));
        }
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
            AddPoolResourceToHand(anomaly);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddPoolResourceToHand(escapee);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddPoolResourceToHand(insanity);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddPoolResourceToHand(munition);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            AddPoolResourceToHand(ration);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AddPoolResourceToHand(scientist);
        }

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

    void AddPoolResourceToHand(Resource resource)
    {
        ResourceCard cardToAdd = GetFromResourcePool(resource);

        if (cardToAdd == null)
        {
            Debug.LogWarning("Card to Add is null; no more cards in resource pool to add.");
            return;
        }
        AddCardToHand(cardToAdd);
    }

    void DataMatchResources()
    {
        Debug.Log($"Pre | hand size: {hand.Count}. Consumer size: {consumer.Count}");

        var handAndConsumerResources = hand.Concat(consumer);

        Debug.Log($"Post | hand size: {hand.Count}. Consumer size: {consumer.Count}");


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

    ResourceCard GetFromResourcePool(Resource resource)
    {
        if (!(resourcePool.transform.childCount > 0))
            return null;

        for (int i = 0; i < resourcePool.transform.childCount; i++)
        {
            var cardToReturn = resourcePool.transform.GetChild(i).GetComponent<ResourceCard>();

            if (cardToReturn.gameObject.activeSelf == false)
            {
                if (cardToReturn._Resource == resource)
                {
                    return cardToReturn;
                }
            }
        }
        return null;
    }

    void AddCardToHand(ResourceCard resourceCard)
    {
        AddTo(resourceCard, handHolder.transform, false, handHolderCardScale);

        resourceCard.IndicatorBackground.gameObject.SetActive(false);

        hand.Add(resourceCard);

        consumer.Remove(resourceCard);
    }

    void ResetHandCard(ResourceCard resourceCard)
    {
        resourceCard.gameObject.transform.SetParent(null, false);

        AddTo(resourceCard, handHolder.transform, false, handHolderCardScale);

        resourceCard.IndicatorBackground.gameObject.SetActive(false);
    }

    void ResetConsumerCard(ResourceCard resourceCard)
    {
        var parent = resourceConsumer.transform.Find(resourceCard._Resource.CardType.ToString());

        resourceCard.IndicatorBackground.gameObject.SetActive(true);

        UpdateConsumerIndicators(resourceCard._Resource.CardType, -1);

        AddTo(resourceCard, parent, false, resourceConsumerCardScale);


    }

    void AddCardToConsumer(ResourceCard resourceCard)
    {
        if (resourceCard._Resource == null)
            Debug.LogWarning("Resource ResouceCard is null");
        if (resourceConsumer == null)
            Debug.LogWarning("ResourceConsumer is null");
        if (resourceConsumer.transform.Find(resourceCard._Resource.CardType.ToString()) == null)
            Debug.Log("Warning, cannot find resource consumer card holder");

        var parent = resourceConsumer.transform.Find(resourceCard._Resource.CardType.ToString());

        resourceCard.IndicatorBackground.gameObject.SetActive(true);

        Debug.Log($"Resource Card Type: {resourceCard._Resource.CardType}");

        AddTo(resourceCard, parent, false, resourceConsumerCardScale);

        var indicatorNum = UpdateConsumerIndicators(resourceCard._Resource.CardType, 0);
        
        consumer.Add(resourceCard);
        resourceCard.IndicatorNumber.sprite = indicators[indicatorNum];

        hand.Remove(resourceCard);

        Debug.Log($"Is numberSymbol null? : {(resourceCard.ResourceSymbol == null)}");
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
        Debug.Log("Updated Indicators");

        List<ResourceCard> resourceCards = new List<ResourceCard>();

        for (int i = 0; i < consumer.Count; i++)
        {
            var card = consumer[i];

            if (card._Resource.CardType == resourceType)
            {
                indicatorNum++;
                resourceCards.Add(card);
            }
        }

        if (indicatorNum >= indicators.Count)
            indicatorNum = indicators.Count - 1;

        for(int i = 0; i < resourceCards.Count; i++)
        {
            var card = resourceCards[i];
            card.IndicatorNumber.sprite = indicators[indicatorNum];
        }
        return indicatorNum;
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
            holdingResourceCard = isOverCard.transform.gameObject.GetComponent<ResourceCard>();

            regularScale = holdingResourceCard.transform.localScale;
            regularSortingOrder = holdingResourceCard._CanvasComponent.sortingOrder;

            holdingResourceCard.transform.localScale *= holdingCardScaleMultiplier;
            
            holdingResourceCard._CanvasComponent.sortingLayerID = holdingCardLayer;

            if(consumer.Contains(holdingResourceCard))
            {
                UpdateConsumerIndicators(holdingResourceCard._Resource.CardType, -2);
            }

           
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

            if (IsOverHandHolder() && !hand.Contains(holdingResourceCard))
            {
                Debug.Log("Dropped card in hand");
                AddCardToHand(holdingResourceCard);
            }
            else if (IsOverResourceConsumer() && !consumer.Contains(holdingResourceCard))
            {
                Debug.Log("Dropped card in consumer");
                AddCardToConsumer(holdingResourceCard);
            }
            else
            {
                Debug.Log("Reset Card");

                if (hand.Contains(holdingResourceCard))
                {
                    ResetHandCard(holdingResourceCard);
                }
                else if (consumer.Contains(holdingResourceCard))
                {
                    ResetConsumerCard(holdingResourceCard);
                }
                else
                    Debug.Log("No list contains this card");
            }

            holdingResourceCard = null;
        }
        
    }

    public void DragCard()
    {
        if (holdingResourceCard != null)
        {
            holdingResourceCard.transform.position = GetMousePosition();

            //Debug.Log($"Is HoldingCard over hand? : {IsOverHandHolder()}");
            //Debug.Log($"Is HoldingCard over consumer? : {IsOverResourceConsumer()}");
        }
    }


}
