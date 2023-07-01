using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Resource;

[CreateAssetMenu(menuName = "Cards/Choices")]
public class Choice : ScriptableObject
{
    [SerializeField] List<ECardType> resourceRequirement1 = new();
    [SerializeField] List<ECardType> resourceRequirement2 = new();
    [SerializeField] List<ECardType> resourceRequirement3 = new();
    [SerializeField] List<ECardType> resourceRequirement4 = new();
    [SerializeField] List<ECardType> resourceRequirement5 = new();
    [SerializeField] List<ECardType> resourceRequirement6 = new();

    [field: FormerlySerializedAs("resourceRewards")]    [field: SerializeField] public ECardType[] ResourceRewards = new ECardType[6];
    [field: FormerlySerializedAs("encounterRewards")]   [field: SerializeField] public List<EncounterCard> EncounterRewards = new();
    [field: FormerlySerializedAs("shouldWinGame")]      [field: SerializeField] public bool ShouldWinGame;
    [field: FormerlySerializedAs("shouldLoseGame")]     [field: SerializeField] public bool ShouldLoseGame;
    [field: FormerlySerializedAs("flavorText")]         [field: SerializeField] public string FlavorText;

    public List<ECardType>[] ResourceRequirements { get => new List<ECardType>[6] { resourceRequirement1, resourceRequirement2, resourceRequirement3, resourceRequirement4, resourceRequirement5, resourceRequirement6 }; }
}
