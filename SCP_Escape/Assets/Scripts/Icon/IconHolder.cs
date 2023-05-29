using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static GameManager;
using static Resource;
using System;

public class IconHolder : MonoBehaviour
{
    //public List<Icon> Icons { get; private set; } = new();
    public List<Icon> Icons;

    //public bool IsAnyIconReady { get; private set; }
    public bool IsAnyIconReady;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckIfIconsReady()
    {

    }

    //Grabs all the icons it needs from the gameManager and adds it to its list
    public void GrabIcons(params Resource[] resources)
    {
        
        for (int i = 0; i < resources.Length; i++)
        {
            Icon icon = Manager.GetFromIconPool(resources[i]);

            icon.gameObject.SetActive(true);


            icon.transform.SetParent(transform);

            icon.transform.localPosition = Vector3.zero;


            Icons.Add(icon);


        }

        FillIcons();

    }

    //Sets the radial fill for all icons
    void FillIcons()
    {
        List<float> amountToFill = new();

        for (float i = 0; i < Icons.Count; i++)
        {
            var icon = Icons[(int)i];

            amountToFill.Insert(0, (float)((i + 1f) / (float)Icons.Count));

            Debug.Log($"Fill amount = {icon.Background.fillAmount}");

            //Debug.Log($"Icon {icon.ResourceType} fill amount : {icon.Background.fillAmount}");
        }

        for (int i = 0; i < Icons.Count; i++)
        {
            var icon = Icons[i];

            icon.Background.fillAmount = amountToFill[i];
        }
    }

    public void SetReady(bool isReady)
    {
        IsAnyIconReady = isReady;

        foreach (Icon icon in Icons)
            icon.SetReady(isReady);
    }

    //Checks if Icons contains a cardType
    public bool ContainsType(ECardType cardType) => GetIconTypes().Contains(cardType);

    //Converts Icons to a list of cardTypes and returns it
    public List<ECardType> GetIconTypes() => Icons.Select(i => i.IconResource.CardType).ToList();
}
