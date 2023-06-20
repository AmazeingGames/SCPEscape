using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;
using static EncounterAnimator;
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

        ShuffleIntoDeck(StartingEncounters);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            DrawNextEncounter();
    }


    //The purpose of this is to grab from the encounter pool and set it to an encounter
    void DrawNextEncounter()
    {
        EncounterCard encounter = GetFromEncounterPool();

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
            {
                ShuffleInDiscard();
                debugString += "No cards left to draw in draw pile. Shuffle deck. ";
            }

            Debug.Log(debugString);

            return;
        }

        Debug.Log("set encounter");

        ActiveEncounter = encounter;

        Debug.Log($"Set active encounter card {ActiveEncounter}");

        ActiveEncounter.SetAndMatchEncounter(DrawPile[0]);

        ActiveEncounter.gameObject.SetActive(true);

        ActiveEncounter.transform.SetParent(Manager.Choices.transform);

        StartCoroutine(SetActiveEncounter());
    }

    //Purpose of this is to prepare the given card as the next encounter card
    IEnumerator SetActiveEncounter()
    {
        if (ActiveEncounter.ChoiceCards.Count == 0)
            yield return null;

        AnimationManager.DiscardEncounter += AddCardToDiscard;

        foreach (ChoiceCard card in ActiveEncounter.ChoiceCards)
        {
            card.gameObject.SetActive(true);
            card.ReadyStartingIcons();
        }

        yield break;
    }

    EncounterCard GetFromEncounterPool() => GetTypeFromPool<EncounterCard>(EncounterPool);

    //Purpose is to add a card to the Encounter deck discard pile, to be shuffled in the next time we need more encounters
    void AddCardToDiscard(EncounterCard cardToDiscard)
    {
        Debug.Log("Adding card to discard");

        DrawPile.Remove(cardToDiscard.Encounter);
        DiscardPile.Add(cardToDiscard.Encounter);
        
        MoveToPool(cardToDiscard, EncounterPool);

        if (cardToDiscard == ActiveEncounter)
        {
            Debug.Log("Set Active Encounter null");
            ActiveEncounter = null;
        }
    }


    //Purpose is to have a way to randomize the draw pile when shuffling cards directly into draw
    void ShuffleInDiscard()
    {
        ShuffleIntoDeck(DiscardPile);
        
        DiscardPile.Clear();
    }

    //Purpose is a way into introduce cards into the encounter deck draw pile
    void ShuffleIntoDeck(List<Encounter> encountersToAdd) => ShuffleIntoDeck(encountersToAdd.ToArray());

    void ShuffleIntoDeck(params Encounter[] encountersToAdd)
    {
        DrawPile.AddRange(encountersToAdd);

        DrawPile.Shuffle();
    }

    //Because cards are going to be moved in and out of potential play (i.e. neither in draw nor discard) we're going to likely need an 'EncounterPool' to store cards like 'Greed' or 'Questlines' when they're somewhere other than potential play, meaning the purpose of this is to send an encounterCard to the encounterPool
    //TO DO: Create GameManager function to move to pool 
    void MoveCardToPool(EncounterCard encounterToMove)
    {
        
    }

    

}
