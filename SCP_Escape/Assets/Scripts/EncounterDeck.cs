using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The architecture of this should be like a black box the GameManager can use to perform actions related to the encounter deck. The gamemanager will manage the actual deck and the encounterDeck will act as an assistant-manager (assistant to the manager)
public class EncounterDeck : MonoBehaviour
{
    public static EncounterDeck Instance { get; private set; }

    GameObject EncounterPool => GameManager.Instance.EncounterPool;

    List<Encounter> _EncounterDeck => GameManager.Instance.EncounterDeck;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }


    //The purpose of this is to grab the first inactive encounter from the resourcePool and return it
    //TO DO: I can use GameManager's [T GetTypeFromPool<T>] function instead
    EncounterCard DrawNextEncounter()
    {
        if (EncounterPool.transform.childCount <= 0)
            return null;

        if (GameManager.Instance.EncounterDeck.Count <= 0)
            return null;

        for (int i = 0; i < EncounterPool.transform.childCount; i++)
        {
            EncounterCard nextEncounter = EncounterPool.transform.GetChild(i).GetComponent<EncounterCard>();

            if (nextEncounter.gameObject.activeSelf == false)
            {
                nextEncounter.SetAndMatchEncounter(_EncounterDeck[0]);

                return nextEncounter;
            }
        }
        return null;
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

    //Because cards are going to be moved in and out of potential play (i.e. neither in draw nor discard) we're going to likely need an 'EncounterPool' to store cards like 'Greed' or 'Questlines' when they're somewhere other than potential play, meaning the purpose of this is to send an encounterCard to the encounterPool
    //TO DO: Create GameManager function to move to pool 
    void MoveCardToPool(EncounterCard encounterToMove)
    {
        
    }

    //Because cards are going to be moved in and out of potential play (i.e. neither in draw nor discard) we're likely to need an 'EncounterPool' to store encounters out of play, meaning the purpose of this is to get a reference to a specific card in the encounterPool. When we pull cards from the encounterPool we can just assign them a scriptableObject that will determine what kind of card it is.
    //Doesn't [DrawNextEncounter] implement this already?
    //Use GameManager's function instead
    EncounterCard GetCardFromPool(EncounterCard encounterToGet)
    {
        return null;
    }

}
