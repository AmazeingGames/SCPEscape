using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] Resource.ECardType[] resourceRequirements = new Resource.ECardType[6];
    [SerializeField] Resource.ECardType[] resourceRewards = new Resource.ECardType[6];
    [SerializeField] List<EncounterCard> cardsToAdd = new List<EncounterCard>();
    [SerializeField] bool shouldWinGame;
    [SerializeField] bool shouldLoseGame;
    [SerializeField] string flavorText;

    public Resource.ECardType[] ResourceRequirements { get => resourceRequirements; private set => resourceRequirements = value; }
    public Resource.ECardType[] ResourceRewards { get => resourceRewards; private set => resourceRewards = value; }
    public List<EncounterCard> CardsToAdd { get => cardsToAdd; private set => cardsToAdd = value; }
    public bool ShouldWinGame { get => shouldWinGame; private set => shouldWinGame = value; }
    public bool ShouldLoseGame { get => shouldLoseGame; private set => shouldLoseGame = value; }
    public string FlavorText { get => flavorText; private set => flavorText = value; }
}
