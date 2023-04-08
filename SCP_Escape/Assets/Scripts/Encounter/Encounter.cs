using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Cards/Encounters")]
public class Encounter : ScriptableObject
{
    /* The data that is required for each Encounter Card:
     * List of choices to resolve the encounter
     * Whether or not the encounter is a quest-line
     * Is the encounter shuffled back into the deck/discard by default? I.e, is it shuffled regardless of the choice you make or is it dependent on the choice?
     * The card art of the encounter
     * 
     * Functionality of the encounter:
     * Choices need to be able to add/remove encounter cards to the deck
     */

    [SerializeField] List<ChoiceCard> choices = new();
    [SerializeField] bool isQuestline;
    [SerializeField] bool isConstantEncounter;

    [SerializeField] Image cardArt;
}
