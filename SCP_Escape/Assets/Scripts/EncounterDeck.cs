using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterDeck : MonoBehaviour
{
    List<EncounterCard> drawPile = new();
    List<EncounterCard> discardPile = new();
    List<EncounterCard> outOfPlay = new();

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    //Purpose it to have a function that can be called by choiceCards that will add a card to the Encounter deck discard pile, to be shuffled in the next time we need more encounters
    //Creates the illusion we're discarding and drawing the same cards or altered cards
    public void AddCardToDiscard()
    {

    }

    //Purpose is to have a function that can be called by the gameManager that will add cards directly to the draw pile that we can draw in the immediate future. Examples are: 'greed', 'famine', and 'raids' in the original game when you have too many total resources, too little of a specific resource, and too much of a specific resource.
    public void AddCardToDraw()
    {

    }

    //Purpose is to have a way to randomize the draw pile when shuffling cards directly into draw
    void ShuffleDrawPile()
    {

    }

    //Purpose is to have a way to combine the draw and discard when we run out of cards to draw.
    void ShuffleInDeck()
    {

    }

    //Because cards are going to be moved in and out of potential play (i.e. neither in draw nor discard) we're going to likely need an 'EncounterPool' to store cards like 'Greed' or 'Questlines' when they're somewhere other than potential play
    void MoveCardToPool(EncounterCard encounterToMove)
    {
        
    }

    EncounterCard GetCardFromPool(EncounterCard encounterToGet)
    {
        return null;
    }
}
