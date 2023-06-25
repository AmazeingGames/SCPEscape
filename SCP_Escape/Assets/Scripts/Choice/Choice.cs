using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Resource;

[CreateAssetMenu(menuName = "Cards/Choices")]
public class Choice : ScriptableObject
{
    /* The data that is required for each choice card:
       1. Resource Cost - The amount, and kind, of resources that must be sacrificed in order to fulfill the encounter choice.This could range from 0 - 6
     * 2. Resource Gain - The amount, and kind, of resources that will be gained as a result of fulfilling the encounter choice.This could range from 0 - 6
     * 3. Encounter Deck - The cards that will be added to the encounter deck as a result of this choice
     * 4. Cards or Resources - Is the reward to add cards to the deck or to gain resources
     * 4. Game State - The state of the game after the card is fulfilled (win/lose)
     *  a. Flavor Text - The reason this choice does a certain thing
     *  b. Background Art - The display art that is shown on the back of the choice card

     */

    [SerializeField] List<ECardType> resourceRequirement1 = new();
    [SerializeField] List<ECardType> resourceRequirement2 = new();
    [SerializeField] List<ECardType> resourceRequirement3 = new();
    [SerializeField] List<ECardType> resourceRequirement4 = new();
    [SerializeField] List<ECardType> resourceRequirement5 = new();
    [SerializeField] List<ECardType> resourceRequirement6 = new();

    [field: FormerlySerializedAs("resourceRewards")]    public ECardType[] ResourceRewards = new ECardType[6];
    [field: FormerlySerializedAs("encounterRewards")]   public List<EncounterCard> EncounterRewards = new();
    [field: FormerlySerializedAs("shouldWinGame")]      public bool ShouldWinGame;
    [field: FormerlySerializedAs("shouldLoseGame")]     public bool ShouldLoseGame;
    [field: FormerlySerializedAs("flavorText")]         public string FlavorText;

    public List<ECardType>[] ResourceRequirements { get => new List<ECardType>[6] { resourceRequirement1, resourceRequirement2, resourceRequirement3, resourceRequirement4, resourceRequirement5, resourceRequirement6 }; }
}
