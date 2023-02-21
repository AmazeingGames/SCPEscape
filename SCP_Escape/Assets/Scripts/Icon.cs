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
    public Resource IconResource { get; private set; } = null;

    void Start()
    {
        GameManager.Instance.AddIcon(this);   
    }

    public void SetResource(Resource resourceRefernce)
    {
        /*
        if (IconResource != null)
        {
            Debug.Log("IconType is already set");
            return;
        }
        */
        IconResource = resourceRefernce;
        //symbol.sprite = resourceRefernce.Symbol;
        //symbol.color = resourceRefernce.SymbolColor;
        initial.text = $"{resourceRefernce.Initial}";
        background.color = resourceRefernce.CardColor;
        //initial.color = resourceRefernce.InitialColor;
    }

    void Update()
    {

    }

    
}
