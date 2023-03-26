using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Linq;

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

    [SerializeField] Choice choice;
    [SerializeField] TextMeshProUGUI flavorText;
    [SerializeField] TextMeshProUGUI rewardText;
    [SerializeField] GameObject requirementsHolder;
    [SerializeField] GameObject rewardsHolder;
    
    [SerializeField] Color regularColor;
    [SerializeField] Color readyColor;
    [SerializeField] Color hoveringColor;

    List<Resource.ECardType> CardTypesInConsumer => GameManager.ConvertResourceCardListToResourceType(GameManager.Instance.consumer);

    List<Resource.ECardType> overlappingConsumerTypes;

    readonly List<Icon> iconResourceRequirements = new();

    public Choice _Choice { get => choice; private set => choice = value; }

    void Start()
    {
        DataMatchChoice();
    }

    void Update()
    {
        IsReady();
        ReadyIcons();
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

        //Debug.Log($"There is {overlappingElementsCount} items in overlappingElementsCount");
        
        return (overlappingElementsCount == requirementTypes.Count() && consumerTypes.Count() == requirementTypes.Count());
    }

    //Sets the value of 'isReady' for each of the icon requirements based on the cards in the consumer
    //This essentially changes just changes the color of the icons and gives visual feedback for placing cards in the consumer
    //Hinges on the idea that repeated items will appear next to one another
    void ReadyIcons()
    {
        int amountOfMatchingResourceInConsumer = 0;
        int amountOfMatchingIconsAhead;
        int typeLoopStartNum = 0;

        for (int i = 0; i < iconResourceRequirements.Count; i++)
        {
            Icon currentIcon = iconResourceRequirements[i];

            var currentIconResourceType = currentIcon.ResourceType;

            int nextNum = 1 + i;
            if (nextNum < iconResourceRequirements.Count && i != 0)
            {
                var nextIconResourceType = iconResourceRequirements[nextNum].ResourceType;

                if (currentIconResourceType != nextIconResourceType)
                    typeLoopStartNum = nextNum;
            }

            if (overlappingConsumerTypes == null || CardTypesInConsumer.Count() == 0)
            {
                Debug.Log("Set false here 1");

                iconResourceRequirements[i].SetReady(false);
            }
            else
                foreach (Resource.ECardType currentConsumerResourceType in overlappingConsumerTypes)
                {
                    if (currentIconResourceType == currentConsumerResourceType)
                    {
                        amountOfMatchingResourceInConsumer++;
                    }
                }

            if (amountOfMatchingResourceInConsumer == 0 && i < iconResourceRequirements.Count)
            {
                Debug.Log("Set false here 2");
                iconResourceRequirements[i].SetReady(false);
            }

            if (typeLoopStartNum > amountOfMatchingResourceInConsumer)
            {
                //Debug.Log("Set false here 3");
                //iconResourceRequirements[i].SetReady(false);
            }

            amountOfMatchingIconsAhead = 0;
            for (int a = i + 1; a < iconResourceRequirements.Count; a++)
            {
                var next = iconResourceRequirements[a].ResourceType;

                if (currentIconResourceType == next)
                    amountOfMatchingIconsAhead++;
            }
            while (amountOfMatchingResourceInConsumer > 0)
            {
                

                Debug.Log($"Set true here. i is {i}");
                iconResourceRequirements[i].SetReady(true);

                amountOfMatchingResourceInConsumer--;

                if (amountOfMatchingIconsAhead > 0)
                {
                    i++;
                    Debug.Log("bumped i");
                }
                else
                    Debug.Log("Didn't bump i");
            }

            
        }
    }

    void DataMatchChoice()
    {
        if(choice == null)
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
