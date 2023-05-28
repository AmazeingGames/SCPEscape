using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static GameManager;
using static Resource;

public class IconHolder : MonoBehaviour
{
    public List<Icon> Icons { get; private set; } = new();

    public bool IsAnyIconReady { get; private set; }

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
            Debug.Log("ran 10");
            Icon icon = Manager.GetFromIconPool(resources[i]);

            Debug.Log("ran 11");

            icon.transform.SetParent(transform);

            Debug.Log("ran 12");

            Icons.Add(icon);

            Debug.Log("ran 13");

        }

        FillIcons();
        Debug.Log("ran 00");

    }

    //Sets the radial fill for all icons
    void FillIcons()
    {
        for (int i = 0; i < Icons.Count; i++)
        {
            var icon = Icons[i];

            icon.Background.fillAmount = (i / Icons.Count);
        }
    }

    public void SetReady(bool isReady) => IsAnyIconReady = isReady;

    //Checks if Icons contains a cardType
    public bool ContainsType(ECardType cardType) => GetIconTypes().Contains(cardType);

    //Converts Icons to a list of cardTypes and returns it
    public List<ECardType> GetIconTypes() => Icons.Select(i => i.IconResource.CardType).ToList();
}
