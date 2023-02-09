using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCard : MonoBehaviour
{
    [SerializeField] Resource resource;
    // Start is called before the first frame update

    void Start()
    {
        PrintCard();
    }

    public void OnMouseDown()
    {
        GameManager.Instance.GrabCard(this);
    }

    public void OnMouseUp()
    {
        GameManager.Instance.DropCard();
    }

    public void PrintCard()
    {
        Debug.Log($"Card Type is: {resource._ECardType}, Symbol is {resource._Symbol}, Texture is {resource._Texture}, Color is {resource._CardColor}, Initial is {resource._Initial}");
    }

}
