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

    public List<GameObject> deck = new List<GameObject>();

    public List<ResourceCard> hand = new List<ResourceCard>();
    public List<Transform> handSlots = new List<Transform>();
    //public List<bool> availableHandSlots = new List<bool>();

    ResourceCard holdingResource = null;
    Vector3 grabbedPosition;

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
            Instantiate(resourceCard, resourcePool.transform);
        }
    }

    void Update()
    {
        if (holdingResource != null)
        {
            holdingResource.GameObject().transform.position = GetMousePosition();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
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
    }

    void AddResourceToHand(Resource resource)
    {
        Debug.Log($"Added {resource} to hand");
        if (resourcePool.transform.childCount > 0)
        {
            ResourceCard cardToAdd = resourcePool.transform.GetChild(0).GetComponent<ResourceCard>();

            cardToAdd.SetResource(resource);

            cardToAdd.gameObject.SetActive(true);

            cardToAdd.transform.position = GetMousePosition();

            if (hand.Count > 1)
                for (int i = 0; i < hand.Count; i++)
                {
                    Debug.Log($"Is hand null? : {hand == null}. Is Resouce null? : {resource == null}");

                    if (hand[i]._Resource._ECardType == resource._ECardType)
                    {
                        hand.Insert(i, cardToAdd);
                    }
                }
            else
                hand.Add(cardToAdd);
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
