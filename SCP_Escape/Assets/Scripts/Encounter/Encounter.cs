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
    [SerializeField] string encounterName;
    [SerializeField] string flavorText;
    [SerializeField] List<Choice> choices = new();
    [SerializeField] Image cardArt;
    [SerializeField] bool isConstantEncounter;


    public List<Choice> Choices { get => choices; private set => choices = value; }
    public string EncounterName { get => encounterName; private set => encounterName = value; }
    public string FlavorText { get => flavorText; private set => flavorText = value; }
    public Image CardArt { get => cardArt; private set => cardArt = value; }
    public bool IsConstantEncounter { get => isConstantEncounter; private set => isConstantEncounter = value; }

}
