using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceCard : MonoBehaviour
{
    [SerializeField] Resource resource;

    [SerializeField] Image symbol;
    [SerializeField] Image texture;
    [SerializeField] TextMeshProUGUI initial;

    void Awake()
    {

    }

    void Start()
    {
        Debug.Log($"Is GameManager.Instance null? : {GameManager.Instance == null}");
        PrintCard();

        symbol.sprite = resource._Symbol;
        texture.sprite = resource._Texture;
        initial.text = resource._Initial.ToString();

        symbol.preserveAspect = true;
        texture.preserveAspect = true;
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
        //Debug.Log($"Card Type is: {resource._ECardType}, Symbol is {resource._Symbol}, Texture is {resource._Texture}, Color is {resource._CardColor}, Initial is {resource._Initial}");
    }

}
