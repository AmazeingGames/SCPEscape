using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Cards/Encounters")]
public class Encounter : ScriptableObject
{
    [field: FormerlySerializedAs("encounterName")]          [field: SerializeField] public string EncounterName;
    [field: FormerlySerializedAs("flavorText")]             [field: SerializeField] public string FlavorText;
    [field: FormerlySerializedAs("choices")]                [field: SerializeField] public List<Choice> Choices = new();
    [field: FormerlySerializedAs("cardArt")]                [field: SerializeField] public Image CardArt;
    [field: FormerlySerializedAs("isConstantEncounter")]    [field: SerializeField] public bool IsConstantEncounter;
}
