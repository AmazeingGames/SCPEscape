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
    HandHolder handHolder;
    ResourceConsumer resourceConsumer;

    public float holdingCardScaleMultiplier;
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
    public List<Resource> resources;
    //public List<bool> availableHandSlots = new List<bool>();

    ResourceCard holdingResourceCard = null;
    Vector3 grabbedPosition;
    Vector3 regularScale = Vector3.one;

    //NOTE TO SELF: In order to create cool card overlap effect, create slots that hold the cards and make sure to have worldPosition stay true when setting the parent

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        handHolder = GameObject.Find("HandHolder").GetComponent<HandHolder>();
        resourcePool = GameObject.Find("ResourcePool");
        resourceConsumer = GameObject.Find("ResourceConsumer").GetComponent<ResourceConsumer>();

        resources = new List<Resource> { ration, escapee, scientist, insanity, munition, anomaly };

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
            AddCardToConsumer(GetFromResourcePool(resources[i]));
        }
    }

    void Update()
    {
        //Debug.Log($"Is over card? : {IsOverCard()}");

        GrabCard();
        DragCard();
        DropCard();
        
    }

    RaycastHit2D IsOverCard()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, int.MaxValue, cardLayer);

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
                switch (card._Resource._ECardType)
                {
                    case Resource.CardType.Anomaly:
                        if(anomalies > 0)
                        {
                            anomalies--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.CardType.Escapee:
                        if(escapees > 0)
                        {
                            escapees--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.CardType.Insanity:
                        if(insanities > 0)
                        {
                            insanities--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.CardType.Munition:
                        if(munitions > 0)
                        {
                            munitions--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.CardType.Ration: 
                        if(rations > 0)
                        {
                            rations--;
                            resourcesToAdd.Add(card);
                        }
                        break;
                    case Resource.CardType.Scientist:
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
        AddTo(resourceCard, handHolder.transform, true);

        hand.Add(resourceCard);
    }

    void AddCardToConsumer(ResourceCard resourceCard)
    {
        if (resourceCard._Resource == null)
            Debug.LogWarning("Resource ResouceCard is null");
        if (resourceConsumer == null)
            Debug.LogWarning("ResourceConsumer is null");
        if (resourceConsumer.transform.Find(resourceCard._Resource._ECardType.ToString()) == null)
            Debug.Log("Warning, cannot find resource consumer card holder");

        AddTo(resourceCard, resourceConsumer.transform.Find(resourceCard._Resource._ECardType.ToString()), false);
    }

    void AddTo(ResourceCard resourceCard, Transform transformParent, bool keepWorldPosition)
    {
        if (!resourceCard.isActiveAndEnabled)
            resourceCard.gameObject.SetActive(true);

        resourceCard.transform.SetParent(transformParent, keepWorldPosition);

        resourceCard.transform.localPosition = Vector3.zero;
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
            Debug.Log("Grabbing card");

            holdingResourceCard = isOverCard.transform.gameObject.GetComponent<ResourceCard>();

            regularScale = holdingResourceCard.transform.localScale;

            holdingResourceCard.transform.localScale *= holdingCardScaleMultiplier;
        }
    }

    public void DropCard()
    {
        if (holdingResourceCard != null && Input.GetMouseButtonUp(0))
        {
            Debug.Log("Dropping card");

            holdingResourceCard.transform.localScale = regularScale;


            if (resourceConsumer.IsMouseOver && handHolder.IsMouseOver)
            {
                Debug.LogWarning("Mouse is over two colliders. Bug.");
            }

            if (resourceConsumer.IsMouseOver)
            {
                AddCardToConsumer(holdingResourceCard);
            }
            else if (handHolder.IsMouseOver)
            {
                AddCardToHand(holdingResourceCard);
            }
            else
            {
                Debug.Log("Do something. Mouse is over nothing.");
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
