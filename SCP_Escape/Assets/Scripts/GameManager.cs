using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    }

    void Start()
    {
        for(int i = 0; i < resourcePoolSize; i++)
        {
            var card = Instantiate(resourceCard, resourcePool.transform);
            card.gameObject.SetActive(false);
        }

        /*
        AddResourceToHand(anomaly);
        AddResourceToHand(escapee); 
        AddResourceToHand(scientist);
        AddResourceToHand(insanity);
        AddResourceToHand(munition);
        AddResourceToHand(ration);
        */
    }

    void Update()
    {
        
        if (holdingResource != null)
        {
            holdingResource.GameObject().transform.position = GetMousePosition();
        }

        if (!hasAddedResources)
        {
            hasAddedResources = true;

            AddResourceToHand(anomaly);
            AddResourceToHand(escapee);
            AddResourceToHand(scientist);
            AddResourceToHand(insanity);
            AddResourceToHand(munition);
            AddResourceToHand(ration);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)
        {
            Debug.Log("Pressed Down");
        }

        /*
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            AddResourceToHand(anomaly);
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddResourceToHand(escapee);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddResourceToHand(insanity);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddResourceToHand(munition);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AddResourceToHand(ration);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AddResourceToHand(scientist);
        }
        */
    }

    void AddResourceToHand(Resource resource)
    {
        if (resourcePool.transform.childCount > 0)
        {
            ResourceCard cardToAdd = null;

            for (int i = 0; i < resourcePool.transform.childCount; i++)
            {
                if (resourcePool.transform.GetChild(i).gameObject.activeSelf == false)
                {
                    cardToAdd = resourcePool.transform.GetChild(i).GetComponent<ResourceCard>();
                    break;
                }
            }

            if (cardToAdd == null)
            {
                Debug.LogWarning("Card to Add is null; no more cards in resource pool to add.");
                return;
            }
            else
                Debug.Log($"Added {resource}, {cardToAdd} to hand");


            cardToAdd.SetResource(resource);

            cardToAdd.gameObject.SetActive(true);

            cardToAdd.transform.position = GetMousePosition();

            if (hand.Count > 1)
                for (int i = 0; i < hand.Count; i++)
                {
                    if (hand[i]._Resource._ECardType == resource._ECardType)
                    {
                        hand.Insert(i, cardToAdd);
                    }
                }
            else
                hand.Add(cardToAdd);

            DebugHand();
        }
    }

    void DebugHand()
    {
        Debug.Log($"Cards in hand are: {hand}");
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
