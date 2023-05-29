using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;

public class Icon : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image symbol;
    [SerializeField] TextMeshProUGUI initial;

    public Image Background { get => background; private set => background = value; }
    public bool IsReady { get; private set; }

    //public Resource IconResource { get; private set; } = null;
    public Resource IconResource;
    public Resource.ECardType ResourceType => IconResource.CardType;

    void Start()
    {
        Manager.AddIcon(this);   
    }

    void Update()
    {
        SetResource(IconResource);
    }

    //Given a resourceRef as a parameter, sets all the data of the icon to the referenced resource
    //Serializes the resource, text, and color based on the input paramater
    public bool SetResource(Resource resourceRefernce)
    {
        if (resourceRefernce == null)
            return false;

        Color backgroundColor;

        if (IsReady)
            backgroundColor = resourceRefernce.CardColor;
        else
            backgroundColor = resourceRefernce.SymbolColor;

        IconResource = resourceRefernce;
        initial.text = $"{resourceRefernce.Initial}";
        background.color = backgroundColor;

        return true;

        //symbol.sprite = resourceRefernce.Symbol;
        //symbol.color = resourceRefernce.SymbolColor;
        //background.color = resourceRefernce.CardColor;
        //initial.color = resourceRefernce.CardColor;
    }


    public void SetReady(bool isReady)
    {
        IsReady = isReady;
    }
}
