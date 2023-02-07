using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Resource> deck = new List<Resource>();
    public List<Transform> handSlots = new List<Transform>();
    public List<bool> availableHandSlots = new List<bool>();

    Resource holdingResource = null;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    void Start()
    {

    }

    void Update()
    {
        if (holdingResource != null)
        {
            holdingResource.GameObject().transform.position = GetMousePosition();
        }
    }

    public void AddResource(Resource.CardType cardType)
    {
        
    }

    static public Vector3 GetMousePosition()
    {
        Vector3 mousePos;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z += Camera.main.nearClipPlane;

        return mousePos;
    }

    public void GrabCard(Resource cardHolding)
    {
        holdingResource = cardHolding;
    }

    public void DropCard()
    {

    }

    

}
