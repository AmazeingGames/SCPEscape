using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Icon : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image symbol;
    [SerializeField] TextMeshProUGUI initial;

    public bool IsReady { get; private set; }

    public Resource IconResource { get; private set; } = null;
    public Resource.ECardType ResourceType => IconResource.CardType;

    void Start()
    {
        GameManager.Instance.AddIcon(this);   
    }

    void Update()
    {
        //This is called every frame in order make sure that if parts of the resource ever change, say its color, it would sync up to that change instantly
        SetResource(IconResource);
    }

    //Given a resourceRef as a parameter, sets all the data of the icon to the referenced resource
    //Serializes the resource, text, and color based on the input paramater
    public void SetResource(Resource resourceRefernce)
    {
        Color backgroundColor;

        if (IsReady)
            backgroundColor = resourceRefernce.CardColor;
        else
            backgroundColor = resourceRefernce.SymbolColor;

        IconResource = resourceRefernce;
        initial.text = $"{resourceRefernce.Initial}";
        background.color = backgroundColor;

        //symbol.sprite = resourceRefernce.Symbol;
        //symbol.color = resourceRefernce.SymbolColor;
        //background.color = resourceRefernce.CardColor;
        //initial.color = resourceRefernce.CardColor;
    }


    public void SetReady(bool isReady)
    {
        //Debug.Log($"SetReady : {isReady}");
        IsReady = isReady;
    }
}
