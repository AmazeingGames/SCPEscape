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

    [SerializeField] GameObject resourcePool;

    [SerializeField] int resourcePoolSize;
    [SerializeField] ResourceCard resourceCard;
    [SerializeField] Resource ration;
    [SerializeField] Resource escapee;
    [SerializeField] Resource scientist;
    [SerializeField] Resource insanity;
    [SerializeField] Resource munition;
    [SerializeField] Resource anomaly;

    public bool hasAddedResources = false;

    public List<GameObject> deck = new List<GameObject>();

    public List<ResourceCard> hand = new List<ResourceCard>();
    public List<Transform> handSlots = new List<Transform>();
    public List<Resource> resources;
    //public List<bool> availableHandSlots = new List<bool>();

    ResourceCard holdingResource = null;
    Vector3 grabbedPosition;

    //NOTE TO SELF: Remove instantiated card from resource pool into the hand OR only grab resource cards that are setActice to false. Unity crashes otherwise.

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        resources = new List<Resource> { ration, escapee, scientist, insanity, munition, anomaly };

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

    void Start()
    {
        for (int i = 0; i < 1; i++)
        {
            AddResourcesToHand(1, 1, 1, 1, 1, 1);
        }
    }

    void Update()
    {
        
        if (holdingResource != null)
        {
            holdingResource.GameObject().transform.position = GetMousePosition();
        }

    }

    void AddResourcesToHand(int anomalies, int escapees, int insanities, int munitions, int rations, int scientists)
    {
        if (resourcePool.transform.childCount <= 0)
            return;

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

            AddToHand(currentCard);
        }
    }

    void AddResourceToHand(Resource resource)
    {
        ResourceCard cardToAdd = GetFromResourcePool(resource);

        if (cardToAdd == null)
        {
            Debug.LogWarning("Card to Add is null; no more cards in resource pool to add.");
            return;
        }
        AddToHand(cardToAdd);
            
        //PrintHand();
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

    void AddToHand(ResourceCard resourceInPool)
    {
        Debug.Log($"Added {resourceInPool} to hand");

        resourceInPool.gameObject.SetActive(true);

        resourceInPool.transform.position = GetMousePosition();

        hand.Add(resourceInPool);
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

    static public Vector3 GetMousePosition()
    {
        Vector3 mousePos;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z += Camera.main.nearClipPlane;

        return mousePos;
    }

    public void GrabCard(ResourceCard cardToGrab)
    {
        holdingResource = cardToGrab;
    }

    public void DropCard()
    {
        holdingResource.transform.position = GetMousePosition();

        holdingResource = null;
    }

    
    

}
