using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class ResourceCard : MonoBehaviour
{
    [SerializeField] Resource resource;

    [SerializeField] Image textureComponent;
    [SerializeField] TextMeshProUGUI initialComponent;
    [SerializeField] Canvas canvasComponent;
    [SerializeField] Image resourceSymbol;

    [SerializeField] Image indicatorBorder;
    [SerializeField] Image indicatorNumber;
    [SerializeField] Image indicatorBackground;

    [SerializeField] Image CardWhite;
    [SerializeField] Image CardSafeArea;
    [SerializeField] Icon icon;

    public Image IndicatorBackground { get => indicatorBackground; private set => indicatorBackground = value; }
    public Image IndicatorBorder { get => indicatorBorder; private set => indicatorBorder = value; }
    public Image IndicatorNumber { get => indicatorNumber; private set => indicatorNumber = value; }
    public Image ResourceSymbol { get => resourceSymbol; private set => resourceSymbol = value; }
    public Resource _Resource { get => resource; private set => resource = value; }
    public Canvas _CanvasComponent { get => canvasComponent; private set => canvasComponent = value; }

    public void SetIcon()
    {

    }

    void Awake()
    {

    }

    void Start()
    {
        //Debug.Log($"Is GameManager.Deck null? : {GameManager.Deck == null}");

        if (resource == null)
        {
            Debug.LogWarning("Resource should not be null");
        }
        else
        {
            DataMatchResource();
        }
        
        icon.SetResource(resource);
    }

    public void SetResource(Resource resource)
    {
        this.resource = resource;

        DataMatchResource();
    }

    public void DataMatchResource()
    {
        resourceSymbol.sprite = resource.Symbol;
        textureComponent.sprite = resource.Texture;
        initialComponent.text = resource.Initial.ToString();
        initialComponent.color = resource.InitialColor;
        resourceSymbol.color = resource.SymbolColor;
        textureComponent.color = resource.TextureColor;
        textureComponent.rectTransform.position = resource.TextureOffeset;

        indicatorBackground.sprite = resource.IndicatorBackground;
        indicatorBackground.color = resource.IndicatorBackgroundColor;
        indicatorBorder.sprite = resource.IndicatorBorder;
        indicatorBorder.color = resource.IndicatorBorderColor;

        CardSafeArea.color = resource.CardColor;
        CardWhite.color = resource.CardColor;

        resourceSymbol.preserveAspect = true;
        textureComponent.preserveAspect = true;
    }

    public void PrintCard()
    {
        Debug.Log($"Card Type is: {resource.CardType}, Symbol is {resource.Symbol}, Texture is {resource.Texture}, Color is , Initial is {resource.Initial}");
    }

}
