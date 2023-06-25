using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.Serialization;

public class ResourceCard : MonoBehaviour
{
    [field: FormerlySerializedAs("resource")]               [field: SerializeField] public Resource Resource { get; private set; }

    [field: FormerlySerializedAs("textureComponent")]       [field: SerializeField] public Image TextureComponent { get; private set; }
    [field: FormerlySerializedAs("initialComponent")]       [field: SerializeField] public TextMeshProUGUI InitialComponent { get; private set; }
    [field: FormerlySerializedAs("canvasComponent")]        [field: SerializeField] public Canvas CanvasComponent { get; private set; }
    [field: FormerlySerializedAs("resourceSymbol")]         [field: SerializeField] public Image ResourceSymbol { get; private set; }

    [field: FormerlySerializedAs("indicatorBorder")]        [field: SerializeField] public Image IndicatorBorder { get; private set; }
    [field: FormerlySerializedAs("indicatorNumber")]        [field: SerializeField] public Image IndicatorNumber { get; private set; }
    [field: FormerlySerializedAs("indicatorBackground")]    [field: SerializeField] public Image IndicatorBackground { get; private set; }

    [field: FormerlySerializedAs("CardWhite")]              [field: SerializeField] public Image CardWhite { get; private set; }
    [field: FormerlySerializedAs("CardSafeArea")]           [field: SerializeField] public Image CardSafeArea { get; private set; }
    [field: FormerlySerializedAs("icon")]                   [field: SerializeField] public Icon Icon { get; private set; }

    void Start()
    {
        if (Resource == null)
            Debug.LogWarning("Resource should not be null");
        else
            DataMatchResource();
        
        Icon.SetResource(Resource);
    }

    public void SetResource(Resource resource)
    {
        Resource = resource;

        DataMatchResource();
    }

    public void DataMatchResource()
    {
        ResourceSymbol.sprite = Resource.Symbol;
        TextureComponent.sprite = Resource.Texture;
        InitialComponent.text = Resource.Initial.ToString();
        InitialComponent.color = Resource.InitialColor;
        ResourceSymbol.color = Resource.SymbolColor;
        TextureComponent.color = Resource.TextureColor;
        TextureComponent.rectTransform.position = Resource.TextureOffeset;

        IndicatorBackground.sprite = Resource.IndicatorBackground;
        IndicatorBackground.color = Resource.IndicatorBackgroundColor;
        IndicatorBorder.sprite = Resource.IndicatorBorder;
        IndicatorBorder.color = Resource.IndicatorBorderColor;

        CardSafeArea.color = Resource.CardColor;
        CardWhite.color = Resource.CardColor;

        ResourceSymbol.preserveAspect = true;
        TextureComponent.preserveAspect = true;
    }

    public void PrintCard()
    {
        Debug.Log($"Card Type is: {Resource.CardType}, Symbol is {Resource.Symbol}, Texture is {Resource.Texture}, Color is , Initial is {Resource.Initial}");
    }

}
