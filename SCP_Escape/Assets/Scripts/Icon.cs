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

    public bool isReady;

    public Resource IconResource { get; private set; } = null;

    void Start()
    {
        GameManager.Instance.AddIcon(this);   
    }

    void Update()
    {
        SetResource(IconResource);
    }

    public void SetResource(Resource resourceRefernce)
    {
        Color backgroundColor;

        if (isReady)
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

    void BecomeSet()
    {
        
    }

    
}
