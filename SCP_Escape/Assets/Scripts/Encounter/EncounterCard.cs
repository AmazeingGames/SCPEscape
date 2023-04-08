using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterCard : MonoBehaviour
{
    [SerializeField] Encounter encounter;
    [SerializeField] LayerMask encounterCardLayer;

    /* Functionality of the Encounter Card:
     * Clicking on the card will slide the encounter to the side and reveal the associated choice cards
     * Clicking on the card again will slide the encounter to the front and hide the associated choice cards
     * Selecting a choice will give the rewards to the player. The rewards are given by the choice card and not the EncounterCard
     * Selecting a choice will fold up all the cards. This is read via the GameManager; an action delegate is invoked by the selected choice that the encounter will read
     */

    bool areChoicesRevealed;
    bool isMouseOver = false;
    bool isClicked = false;
    bool isChoiceSelected = false;

    void Start()
    {
        
    }

    void Update()
    {
        IsMouseOver();
        IsClicked();

        MoveChoices();

        GameManager.Instance.onChoiceSelection -= OnChoiceSelection;
        GameManager.Instance.onChoiceSelection += OnChoiceSelection;
    }

    //Sets true if mouse is over this choice
    void IsMouseOver()
    {
        var hit = GameManager.IsOver(encounterCardLayer);


        if (hit.transform == transform)
        {
            isMouseOver = true;
            return;
        }
        isMouseOver = false;
    }

    //Sets true if mouse is pressed while hovering
    //Sets false if mouse leaves or lets up
    void IsClicked()
    {
        if (isMouseOver && Input.GetMouseButtonDown(0))
        {
            isClicked = true;
        }
        else
        {
            isClicked = false;
        }
    }

    //Purpose is to reveal/hide choices when the encounter is clicked.
    //Choices will be initially hidden from the player, clicking the encounter reveals them and hides the encounter. Clicking the encounter again, hides the choices, and repeat.
    void MoveChoices()
    {
        if (isChoiceSelected)
            return;

        if (isClicked)
        {
            if (areChoicesRevealed)
            {
                HideChoices();
            }
            else
            {
                RevealChoices();
            }

            areChoicesRevealed = !areChoicesRevealed;
        }
    }

    //Ideally the animations would play out like this: A card is drawn from the deck, placed face down in the center of the table and flips over revealing the encounter. Clicking causes it to slide to the side, and choice cards to slide out from under it, revealing the player's options

    //Purpose is to reveal the choices and put them into play, allowing them to be selected.
    //This would also move the encounter to the side to obscure it.
    void RevealChoices()
    {
        Debug.Log("Revealed Choices");
    }

    //Purpose is to hide the choices and remove them from play, not allowing them to be selected.
    //This would also move the encounter to the forefront.
    void HideChoices()
    {
        Debug.Log("Hid Choices");
    }

    //Purpose of this is to hide the choices on selection
    void OnChoiceSelection(Resource.ECardType[] nothing)
    {
        isChoiceSelected = true;

        HideChoices();
    }
}
