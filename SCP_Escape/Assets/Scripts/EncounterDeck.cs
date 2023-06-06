using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

//The architecture of this should be like a black box the GameManager can use to perform actions related to the encounter deck. The gamemanager will manage the actual deck and the encounterDeck will act as an assistant-manager (assistant to the manager)
//Now I'm thinking I can just put all of the functionality of the encounter deck here instead since the GameManager is already pretty full
public class EncounterDeck : MonoBehaviour
{
    //So I can either create an event for the card to be sent to the discard, and the event is raised whenever the card finishes lerping, or I can subscribe to the onChoiceSelection event and wait for the animation to finish before discarding the card.

    public List<Encounter> StartingEncounters;

    public static EncounterDeck DeckManager { get; private set; }

    public EncounterCard ActiveEncounter { get; private set; } = null;
    GameObject EncounterPool => Manager.EncounterPool;

    public List<Encounter> DrawPile { get; private set; } = new();
    public List<Encounter> DiscardPile { get; private set; } = new();

    void Awake()
    {
        if (DeckManager == null)
            DeckManager = this;
        else
            Destroy(DeckManager);

        Debug.Log("Encounter Deck");

        FillDrawPile();
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

        Debug.Log($"Drew encounter card {encounter} | is null : {encounter == null}");

        bool noEncountersLeft           = encounter == null;
        bool activeEncounterAlreadySet  = ActiveEncounter != null;
        bool noCardsInDraw              = DrawPile.Count <= 0;

        if (noEncountersLeft || activeEncounterAlreadySet || noCardsInDraw)
        {
            string debugString = "Early return : ";

            if (noEncountersLeft)
               debugString += "No encounters left in pool. ";
            if (activeEncounterAlreadySet)
                debugString += "Trying to set encounter when encounter already exists. ";
            if (noCardsInDraw)
                debugString += "No cards left to draw in draw pile. Shuffle deck. ";
            Debug.Log(debugString);

            return;
        }

        SetActiveEncounter(encounter);
    }

    //Purpose of this is to prepare the given card as the next encounter card
    void SetActiveEncounter(EncounterCard activeEncounterCard)
    {
        ActiveEncounter = activeEncounterCard;

        Debug.Log($"Set active encounter card {ActiveEncounter}");

        ActiveEncounter.SetAndMatchEncounter(DrawPile[0]);

        ActiveEncounter.gameObject.SetActive(true);

        ActiveEncounter.transform.SetParent(Manager.Choices.transform);

        //This doesn't seem to work
        foreach (ChoiceCard choiceCard in activeEncounterCard.ChoiceCards)
        {
            choiceCard.ChoiceSelection += AddCardToDiscard;
        }
    }

    EncounterCard GetFromEncounterPool() => GetTypeFromPool<EncounterCard>(EncounterPool);

    //Purpose is to have a function that can be called by ChoiceCards that will add a card to the Encounter deck discard pile, to be shuffled in the next time we need more encounters
   
    //For some reason this function isn't being ran
    void AddCardToDiscard(object sender, CardSelectionEventArgs args)
    {
        Debug.Log("Discard dis card");
    }

    //Purpose is to have a function that can be called by the gameManager that will add cards directly to the draw pile that we can draw in the immediate future. Examples are: 'greed', 'famine', and 'raids' in the original game when you have too many total resources, too little of a specific resource, and too much of a specific resource.
    void AddCardToDraw()
    {

    }

    //Purpose is to have a way to randomize the draw pile when shuffling cards directly into draw
    void ShuffleDrawPile()
    {

    }

    //Fills the initial encounter deck to contain cards
    void FillDrawPile()
    {
        DrawPile.AddRange(StartingEncounters);
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
