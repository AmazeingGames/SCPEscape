using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class ResourceCard : MonoBehaviour
{
    [SerializeField] Resource resource;

    [SerializeField] Image symbolComponent;
    [SerializeField] Image textureComponent;
    [SerializeField] TextMeshProUGUI initialComponent;
    [SerializeField] Image colorOverlayComponent;
    [SerializeField] Canvas canvasComponent;

    public Image numberSymbol;
    public Image numberColor;

    public Resource _Resource { get => resource; private set => resource = value; }
    public Canvas _CanvasComponent { get => canvasComponent; private set => canvasComponent = value; }

    void Awake()
    {

    }

    void Start()
    {
        //Debug.Log($"Is GameManager.Instance null? : {GameManager.Instance == null}");

        if (resource == null)
        {
            Debug.LogWarning("Resource should not be null");
        }
        else
        {
            //PrintCard();
            DataMatchResource();
        }
        
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
        //colorOverlayComponent.color = resource._CardColor;

        symbolComponent.preserveAspect = true;
        textureComponent.preserveAspect = true;
    }

    public void PrintCard()
    {
        Debug.Log($"Card Type is: {resource._ECardType}, Symbol is {resource._Symbol}, Texture is {resource._Texture}, Color is {resource._CardColor}, Initial is {resource._Initial}");
    }

}
