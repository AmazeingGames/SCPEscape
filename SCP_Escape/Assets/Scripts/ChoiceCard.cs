using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Linq;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine.UI;

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
    [SerializeField] TextMeshProUGUI flavorText;
    [SerializeField] TextMeshProUGUI rewardText;
    [SerializeField] GameObject requirementsHolder;
    [SerializeField] GameObject rewardsHolder;

    [SerializeField] Color unavailableColor; //Even less colored (Dark-Greyed Out) 
    [SerializeField] Color unreadyColor; //Uncolored (Greyed Out)
    [SerializeField] Color readyColor; //Colored (Yellow)
    //Colored (Yellow) transparent filter that appears when over the card
    [SerializeField] Image cardColorOverlay;
    [SerializeField] Image borderColorOverlay;
    List<Resource.ECardType> CardTypesInConsumer => GameManager.ConvertResourceCardListToResourceType(GameManager.Instance.consumer);

    List<Resource.ECardType> overlappingConsumerTypes;

    readonly List<Icon> iconResourceRequirements = new();

    public EChoiceState ChoiceState { get; private set; } = EChoiceState.Unready;

    public Choice _Choice { get => choice; private set => choice = value; }

    void Start()
    {
        DataMatchChoice();
    }

    void Update()
    {
        SetState();
        SetChoiceColor();
        GameManager.Instance.onCardChangeInConsumer -= ReadyIcon;
        GameManager.Instance.onCardChangeInConsumer += ReadyIcon;
    }

    private void OnMouseOver()
    {
        
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseExit()
    {
        
    }

    void SetChoiceColor()
    {
        cardColorOverlay.gameObject.SetActive(true);

        switch (ChoiceState)
        {
            case EChoiceState.Unready:
                cardColorOverlay.color = unreadyColor;
                break;
            case EChoiceState.Ready:
                cardColorOverlay.color = readyColor;
                break;
            case EChoiceState.Unavailable:
                cardColorOverlay.color = unavailableColor;
                break;
        }
    }

    //Sets the enum 'ChoiceState', based on whether the requirements have been fulfilled, and determines if it is ready to be selected
    void SetState()
    {
        bool isReady = IsReady();

        if (isReady)
        {
            ChoiceState = EChoiceState.Ready;
        }
        else
        {
            ChoiceState = EChoiceState.Unready;
        }
    }


    //Checks if the consumer contains the correct resources based on the choice's required resources
    //Returns a bool true if all the resources are met and the exact amount of resources are contained
    bool IsReady()
    {
        var consumerTypes = CardTypesInConsumer;
        var requirementTypes = choice.ResourceRequirements.ToList();

        if (consumerTypes.Count() == 0 || requirementTypes.Count() == 0)
        {
            //Debug.Log("Lists are empty");
            return false;
        }

        overlappingConsumerTypes = consumerTypes.Where(requirementTypes.Contains).ToList();

        int overlappingElementsCount = overlappingConsumerTypes.Count();
        
        return (overlappingElementsCount == requirementTypes.Count() && consumerTypes.Count() == requirementTypes.Count());
    }

    //Sets the value of 'isReady' for one of the icon requirements based on the given paramates
    //Called whenever a state change occurs in the consumer via an event
    void ReadyIcon(Resource.ECardType resourceType, bool setReady)
    {
        //Makes sure if there are more cards than requirements in the consumer, removing one won't change any of the icons
        if (!setReady)
        {
            int resourceTypeCountInRequirements = GetIconsOfTypeFromRequirements(resourceType).Count;
            int resourceTypeCountInConsumer = GameManager.Instance.GetResourcesFromConsumer(resourceType).Count;

            //Debug.Log($"ResourceTypeCountInConsumer : {resourceTypeCountInConsumer}  ResourceTypeCountInRequirements : {resourceTypeCountInRequirements}");

            if (resourceTypeCountInConsumer >= resourceTypeCountInRequirements)
            {
                return;
            }
        }

        Debug.Log($"Set readied icon.");
        for (int i = 0; i < iconResourceRequirements.Count; i++)
        {
            Icon currentIcon = iconResourceRequirements[i];

            if (currentIcon.ResourceType == resourceType)
            {
                bool shouldSet = true;
                if (currentIcon.IsReady != setReady)
                {
                    if (i + 1 < iconResourceRequirements.Count)
                    {
                        var nextIcon = iconResourceRequirements[i + 1];
                        if (!setReady && nextIcon.ResourceType == resourceType && nextIcon.IsReady != setReady)
                        {
                            shouldSet = false;
                        }
                    }

                    if (shouldSet)
                    {
                        currentIcon.SetReady(setReady);
                        break;
                    }
                }
            }
        }
    }


    public List<Icon> GetIconsOfTypeFromRequirements(Resource.ECardType resourceType)
    {
        List<Icon> IconResources = new();

        for (int i = 0; i < iconResourceRequirements.Count; i++)
        {
            Icon icon = iconResourceRequirements[i];

            if (icon.ResourceType == resourceType)
            {
                IconResources.Add(icon);
            }
        }

        return IconResources;
    }

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

    void ActivateAndDeactivateRewards()
    {
        int actualValues = 0;

        for (int i = 0; i < choice.ResourceRewards.Length; i++)
        {
            var reward = choice.ResourceRewards[i];

            if (IsEnumValueValid(reward))
                actualValues++;
        }

        if (actualValues > 0)
        {
            rewardText.gameObject.SetActive(false);
        }
        else
        {
            rewardsHolder.SetActive(false);
        }


    }

    void SetAllIcons()
    {
        //Debug.Log($"Is choice null? : {choice == null}. Is requirementsHolder null? : {requirementsHolder == null}. Is choice.ResourceRequirements array null? : {choice.ResourceRequirements == null}");

        SetIcons(choice.ResourceRequirements, requirementsHolder.transform, iconResourceRequirements);
        SetIcons(choice.ResourceRewards, rewardsHolder.transform, null);
    }

    void SetIcons(Resource.ECardType[] listRefernce, Transform parent, List<Icon> listToAdd)
    {
        for (int i = 0; i < listRefernce.Length; i++)
        {
            Resource.ECardType currentResourceType = listRefernce[i];

            if (!IsEnumValueValid(currentResourceType))
            {
                Debug.Log($"Left loop at i = {i}");
                break;
            }

            var setIcon = GameManager.Instance.GetFromIconPool(currentResourceType);

            setIcon.gameObject.SetActive(true);
            setIcon.transform.SetParent(parent);

            listToAdd?.Add(setIcon);
        }
    }

    static bool IsEnumValueValid(Enum enumeration)
    {
        bool returnValue = Enum.IsDefined(enumeration.GetType(), enumeration);

        //Debug.Log($"IsEnumValueValid? : {returnValue}");

        return returnValue;
    }

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
