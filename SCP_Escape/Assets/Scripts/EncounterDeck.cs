using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

//The architecture of this should be like a black box the GameManager can use to perform actions related to the encounter deck. The gamemanager will manage the actual deck and the encounterDeck will act as an assistant-manager (assistant to the manager)
public class EncounterDeck : MonoBehaviour
{
    //So I can either create an event for the card to be sent to the discard, and the event is raised whenever the card finishes lerping, or I can subscribe to the onChoiceSelection event and wait for the animation to finish before discarding the card.

    public static EncounterDeck Deck { get; private set; }

    GameObject EncounterPool => Manager.EncounterPool;
    List<Encounter> DrawPile => Manager.EncounterDeck;

    void Awake()
    {
        if (Deck == null)
            Deck = this;
        else
            Destroy(Deck);
    }


    //The purpose of this is to grab from the encounter pool and set it to an encounter
    EncounterCard DrawNextEncounter()
    {
        if (DrawPile.Count <= 0)
            return null;

        EncounterCard encounter = GetFromEncounterPool();

        if (encounter != null)
            encounter.SetAndMatchEncounter(DrawPile[0]);

        return encounter;
    }

    EncounterCard GetFromEncounterPool() => GetTypeFromPool<EncounterCard>(EncounterPool);

    //Purpose is to have a function that can be called by choiceCards that will add a card to the Encounter deck discard pile, to be shuffled in the next time we need more encounters
    //This will be done with events
    void AddCardToDiscard(EncounterCard cardToDiscard)
    {

    }

    //Purpose is to have a function that can be called by the gameManager that will add cards directly to the draw pile that we can draw in the immediate future. Examples are: 'greed', 'famine', and 'raids' in the original game when you have too many total resources, too little of a specific resource, and too much of a specific resource.
    void AddCardToDraw()
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

    //Because cards are going to be moved in and out of potential play (i.e. neither in draw nor discard) we're going to likely need an 'EncounterPool' to store cards like 'Greed' or 'Questlines' when they're somewhere other than potential play, meaning the purpose of this is to send an encounterCard to the encounterPool
    //TO DO: Create GameManager function to move to pool 
    void MoveCardToPool(EncounterCard encounterToMove)
    {
        
    }

    

}
