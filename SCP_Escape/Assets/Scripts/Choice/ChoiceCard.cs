using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;
using static Resource;
using static GameManager;
using static System.Type;

//TO DO: Fix issue where pressing the encounter card automatically selects the choice card underneath
public class ChoiceCard : MonoBehaviour
{
    /* Things a Choice Card GameObject Needs:
     * 1. A boxCollider Component - Detects when pressed, mouse is over, when is pressed
     * 2. Icon Displays (loss) - 5 Icon displays that can change their sprite in order to match the resource cost
     * 3. Icon Displays (gain) - 5 Icon displays that can change their sprite in order to match the resource reward
     * 4. Layout Group (gain) - Holds the reward icons and organizes them
     * 5. Layour Group (loss) - Holds the cost icons and organizes them
     * 6. Text MPro (flavor) - Holds the flavor text for the choice
     * 7. Text MPro (gain) - Holds the reward text if the reward is to add cards
     * 8. Choice Object - Holds the card data to data match
     *  a. Highlight - Highlights the card when the player hovers over it 
     */
    
    public enum EChoiceState { Ready, Unready, Unavailable }


    [SerializeField] Choice choice;
    [Header("Card Properties")]
    [SerializeField] LayerMask choiceCardLayer;
    [SerializeField] TextMeshProUGUI flavorText;
    [SerializeField] TextMeshProUGUI rewardText;
    [SerializeField] GameObject requirementsHolder;
    [SerializeField] GameObject rewardsHolder;


    [Header("Card Color")]
    [SerializeField] Color regularBorderColor;
    [SerializeField] Color highlightedBorderColor;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color unavailableColor;
    [SerializeField] Color unreadyColor;
    [SerializeField] Color readyColor; 
    [SerializeField] Image cardColorOverlay;
    [SerializeField] Image borderColorOverlay;

    List<ECardType> CardTypesInConsumer => ConvertResourceCardListToResourceType(Manager.Consumer);

    List<ECardType> overlappingConsumerTypes = new();

    readonly List<IconHolder> iconResourceRequirements = new();

    public EChoiceState ChoiceState { get; private set; } = EChoiceState.Unready;

    public Choice Choice { get => choice; private set => choice = value; }

    public bool IsReady { get; private set; } = false;
    bool isMouseOver = false;

    bool isMouseHolding = false;

    void Start()
    {
        DataMatchChoice();
    }

    void Update()
    {
        //Sets important variables
        IsMouseOver();
        IsMouseHolding();
        SetIsReady();

        //Updates based on those variables
        SetState();
        SetChoiceColor();
        UpdateIcon();
        
        //
        SelectChoice();
    }

    //Calls the ReadyIcon func whenever a card is changed in the GameManager
    void UpdateIcon()
    {
        Manager.onCardChangeInConsumer -= ReadyIcon;
        Manager.onCardChangeInConsumer += ReadyIcon;
    }

    //Tells 'gameManager' to : remove all cards in the Consumer and add rewards to the player's hand/deck
    void SelectChoice()
    {
        if (isMouseOver && Input.GetMouseButtonUp(0) && IsReady)
        {
            Debug.Log($"Invoked Choice {flavorText.text}");
            Manager.onChoiceSelection?.Invoke(choice.ResourceRewards);
        }
    }

    //Sets the color of the card based on cards in the consumer and whether it's being clicked
    void SetChoiceColor()
    {
        cardColorOverlay.gameObject.SetActive(true);

        if (isMouseHolding)
        {
            SetColor(highlightedColor, highlightedBorderColor);
            return;
        }
        
        switch (ChoiceState)
        {
            case EChoiceState.Unready:
                SetColor(unreadyColor, regularBorderColor);
                break;
            case EChoiceState.Ready:
                SetColor(readyColor, regularBorderColor);
                break;
            case EChoiceState.Unavailable:
                SetColor(unavailableColor, regularBorderColor);
                break;
        }
    }

    //Sets the color of the 'ChoiceCard' to the arguements
    //First sets the cardWhite then the cardBorder
    void SetColor(Color cardColor, Color borderColor)
    {
        SetColor(cardColor);

        borderColorOverlay.color = borderColor;
    }

    void SetColor(Color cardColor) => cardColorOverlay.color = cardColor;

    
    //Sets true if mouse is over this choice
    void IsMouseOver() => isMouseOver = GameManager.IsOver(choiceCardLayer, transform);

    //Sets true if mouse is pressed while hovering
    //Sets false if mouse leaves or lets up
    void IsMouseHolding()
    {
        if (!isMouseOver || Input.GetMouseButtonUp(0))
        {
            isMouseHolding = false;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isMouseHolding = true;
        }
    }

    //Sets the enum 'ChoiceState', based on whether the requirements have been fulfilled, and determines if it is ready to be selected
    void SetState()
    {
        if (IsReady)
            ChoiceState = EChoiceState.Ready;
        else
            ChoiceState = EChoiceState.Unready;
    }


    //Checks if the consumer contains the correct resources based on the choice's required resources
    //Returns a bool true if all the resources are met and the exact amount of resources are contained
    void SetIsReady()
    {
        List<ECardType> consumerTypes = CardTypesInConsumer;
        List<List<ECardType>> requirementTypes = choice.ResourceRequirements.ToList();

        if (consumerTypes.Count() == 0 || requirementTypes.Count() == 0)
        {
            IsReady = false;
            return;
        }

        bool areAllIconsReadied = false;
        int readyIcons = 0;

        foreach (IconHolder iconHolder in iconResourceRequirements)
        {
            if (iconHolder.IsAnyIconReady)
            {
                readyIcons++;
                continue;
            }
        }

        if (readyIcons == iconResourceRequirements.Count)
            areAllIconsReadied = true;

        IsReady = (consumerTypes.Count() == iconResourceRequirements.Count() && areAllIconsReadied);

    }

    //Sets the value of 'isReady' for one of the icon requirements based on the given paramates
    //Called whenever a card is added or removed from/to the consumer
    void ReadyIcon(ECardType resourceType, bool isReadyingIcon)
    {
        //Makes sure if there are more cards than requirements, removing one won't change any of the icons' status
        if (!isReadyingIcon)
        {
            int resourceTypeCountInRequirements = GetIconsOfTypeFromRequirements(resourceType).Count;
            int resourceTypeCountInConsumer = Manager.GetResourcesFromConsumer(resourceType).Count;

            if (resourceTypeCountInConsumer >= resourceTypeCountInRequirements)
                return;
        }

        //Responsible for deciding whether or not the icon holder is ready
        //TO DO: If possible, decrease code nesting
        for (int i = 0; i < iconResourceRequirements.Count; i++)
        {
            IconHolder currentIconHolder = iconResourceRequirements[i];

            if (currentIconHolder.ContainsType(resourceType))
            {
                bool shouldSet = true;

                if (currentIconHolder.IsAnyIconReady != isReadyingIcon)
                {
                    if (i + 1 < iconResourceRequirements.Count)
                    {
                        var nextIconHolder = iconResourceRequirements[i + 1];

                        if (!isReadyingIcon && nextIconHolder.ContainsType(resourceType) && nextIconHolder.IsAnyIconReady != isReadyingIcon)
                            shouldSet = false;
                    }

                    if (shouldSet)
                    {
                        currentIconHolder.SetReady(isReadyingIcon);
                        break;
                    }
                }
            }
        }
    }

    //Gets a list of all icons from requirements that match the given type
    public List<IconHolder> GetIconsOfTypeFromRequirements(ECardType resourceType)
    {
        List<IconHolder> IconResources = new();

        for (int i = 0; i < iconResourceRequirements.Count; i++)
        {
            IconHolder iconHolder = iconResourceRequirements[i];

            if (iconHolder.ContainsType(resourceType))
                IconResources.Add(iconHolder);
        }

        return IconResources;
    }

    public void SetChoice(Choice choice) => this.choice = choice;

    //Reads the data of the 'choice' scriptable object and sets the 'choiceCard' variables to match
    void DataMatchChoice()
    {
        if (choice == null)
        {
            Debug.Log("Choice is null");
            return;
        }

        flavorText.text = choice.FlavorText;
        rewardText.text = GetRewardText();

        ActivateAndDeactivateRewards();  

        SetAllIcons();
    }

    //
    void ActivateAndDeactivateRewards()
    {
        int actualValues = 0;

        foreach (ECardType reward in choice.ResourceRewards)
            if (IsEnumValueValid(reward))
                actualValues++;

        if (actualValues > 0)
            rewardText.gameObject.SetActive(false);
        else
            rewardsHolder.SetActive(false);
    }

    //
    void SetAllIcons()
    {
        SetIcons(choice.ResourceRequirements, requirementsHolder.transform, iconResourceRequirements);

        SetIcons(choice.ResourceRewards, rewardsHolder.transform, null, false);
    }

    //The problem with these two functions is that they still treat all the given icons as individual icons, creating a new icon holder for each icon and supplying every icon holder exactly 1 icon, instead of supplying every icon holder the amount given in the list
    void SetIcons(List<ECardType>[] listRefernce, Transform parent, List<IconHolder> listToAdd)
    {
        foreach (List<ECardType> list in listRefernce)
        {
            if (list == null)
                continue;

            SetIcons(list.ToArray(), parent, listToAdd, true);
        }
    }

    void SetIcons(ECardType[] listRefernce, Transform parent, List<IconHolder> listToAdd, bool shouldGroupIcons)
    {
        for (int i = 0; i < listRefernce.Length; i++)
        {
            ECardType currentResourceType = listRefernce[i];

            if (!IsEnumValueValid(currentResourceType))
            {
                Debug.Log($"Left loop at i = {i}");
                break;
            }

            IconHolder setIconHolder = null;

            if (shouldGroupIcons)
            {
                var resources = (from t in listRefernce select Manager.TypeToResource[t]);

                resources.ToList();

                setIconHolder = Manager.GetFromIconHolderPool(resources.ToArray());
            }
            else
            {
                Resource resource = Manager.TypeToResource[currentResourceType];

                setIconHolder = Manager.GetFromIconHolderPool(resource);
            }
           

            setIconHolder.gameObject.SetActive(true);
            setIconHolder.transform.SetParent(parent);

            listToAdd?.Add(setIconHolder);

            if (shouldGroupIcons)
                break;
        }
    }

    //Enum.IsDefined checks if a value is contatined in an enumeration; but that still doesn't explain what this code is for
    //Why would/wouldn't converting an Enum to a type make it no longer contained in the enumeration?
    //What is the usefulness from this check??
    //I'm still confused about this
    static bool IsEnumValueValid(Enum enumeration)
    {
        bool isDefined = Enum.IsDefined(enumeration.GetType(), enumeration);

        //Debug.Log($"Enumeration: {enumeration}. Enumeration.GetType() : {enumeration.GetType()}. IsDefined : {isDefined}");

        return isDefined;
    }
        

    //
    //Consider using string builder class instead
    string GetRewardText()
    {
        if (choice.ResourceRewards.Count() > 0)
        {
            string text = "Add";

            for (int i = 0; i < choice.CardsToAdd.Count; i++)
            {
                var current = choice.CardsToAdd[i];
                text += $" \"{current.name}\"";

                if (i + 1 < choice.CardsToAdd.Count)
                {
                    if (i + 2 == choice.CardsToAdd.Count)
                        text += " and";
                    else
                        text += ",";
                }

            }
            text += " to the encounter deck.";
            return text;
        }
        return null;
    }
}
