using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceCard : MonoBehaviour
{
    [SerializeField] Resource resource;

    [SerializeField] Image symbolComponent;
    [SerializeField] Image textureComponent;
    [SerializeField] TextMeshProUGUI initialComponent;

    public Resource _Resource { get => resource; private set => resource = value; }

    void Awake()
    {

    }

    void Start()
    {
        //Debug.Log($"Is GameManager.Instance null? : {GameManager.Instance == null}");
        
        if (resource == null)
        {
            Debug.LogWarning("RESOURCE SHOULD NOT BE NULL");
        }
        else
        {
            //PrintCard();

            DataMatchResource();
        }
        
    }

    public void OnMouseDown()
    {
        GameManager.Instance.GrabCard(this);
    }

    public void OnMouseUp()
    {
        GameManager.Instance.DropCard();
    }

    public void SetResource(Resource resource)
    {
        this.resource = resource;

        DataMatchResource();
    }

    void DataMatchResource()
    {
        symbolComponent.sprite = resource._Symbol;
        textureComponent.sprite = resource._Texture;
        initialComponent.text = resource._Initial.ToString();

        symbolComponent.preserveAspect = true;
        textureComponent.preserveAspect = true;
    }

    public void PrintCard()
    {
        Debug.Log($"Card Type is: {resource._ECardType}, Symbol is {resource._Symbol}, Texture is {resource._Texture}, Color is {resource._CardColor}, Initial is {resource._Initial}");
    }

}
