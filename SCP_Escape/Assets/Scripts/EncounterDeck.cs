using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

//The architecture of this should be like a black box the GameManager can use to perform actions related to the encounter deck. The gamemanager will manage the actual deck and the encounterDeck will act as an assistant-manager (assistant to the manager)
//Now I'm thinking I can just put all of the functionality of the encounter deck here instead since the GameManager is already pretty full
public class EncounterDeck : MonoBehaviour
{
    //So I can either create an event for the card to be sent to the discard, and the event is raised whenever the card finishes lerping, or I can subscribe to the onChoiceSelection event and wait for the animation to finish before discarding the card.

    public static EncounterDeck Deck { get; private set; }

    public EncounterCard ActiveEncounter { get; private set; } = null;
    GameObject EncounterPool => Manager.EncounterPool;
    List<Encounter> DrawPile => Manager.EncounterDeck;

    void Awake()
    {
        if (Deck == null)
            Deck = this;
        else
            Destroy(Deck);

        Debug.Log("Encounter Deck");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            DrawNextEncounter();
    }


    //The purpose of this is to grab from the encounter pool and set it to an encounter
    void DrawNextEncounter()
    {
        EncounterCard encounter = GetFromEncounterPool();
        
        if (encounter == null || ActiveEncounter != null || DrawPile.Count <= 0)
            return;

        SetActiveEncounter(encounter);
    }

    //Purpose of this is to prepare the given card as the next encounter card
    void SetActiveEncounter(EncounterCard activeEncounterCard)
    {
        ActiveEncounter = activeEncounterCard;

        ActiveEncounter.SetAndMatchEncounter(DrawPile[0]);

        ActiveEncounter.transform.SetParent(Manager.Choices.transform);
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
