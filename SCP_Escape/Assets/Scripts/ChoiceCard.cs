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

    public bool isReady;
    public Choice _Choice { get => choice; private set => choice = value; }

    void Start()
    {
        DataMatchChoice();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log($"Is Ready? : {IsReady()}");
        }
    }

    //Works
    bool IsReady()
    {
        var consumerTypes = GameManager.ConvertResourceCardListToResourceType(GameManager.Instance.consumer);
        var requirementTypes = GameManager.ConvertResourceCardListToResourceType(choice.ResourceRequirements.ToList());

        return requirementTypes.Intersect(consumerTypes).Count() == requirementTypes.Count();
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

            if (reward != null)
                actualValues++;
        }

        if (actualValues > 0)
        {
            rewardText.gameObject.SetActive(false);
        }
        else
        {
            rewardsHolder.gameObject.SetActive(false);
        }


    }

    void SetAllIcons()
    {
        Debug.Log($"Is choice null? : {choice == null}. Is requirementsHolder null? : {requirementsHolder == null}. Is choice.ResourceRequirements array null? : {choice.ResourceRequirements == null}");
        SetIcons(choice.ResourceRequirements, requirementsHolder.transform);
        SetIcons(choice.ResourceRewards, rewardsHolder.transform);
    }

    void SetIcons(Resource[] listRefernce, Transform parent)
    {
        for (int i = 0; i < listRefernce.Length; i++)
        { 
            var currentResource = listRefernce[i];
            if (currentResource == null)
            {
                Debug.Log($"Left loop at i = {i}");
                break;
            }
            var setIcon = GameManager.Instance.GetFromIconPool(currentResource);

            setIcon.gameObject.SetActive(true);
            setIcon.transform.SetParent(parent);
        }
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
