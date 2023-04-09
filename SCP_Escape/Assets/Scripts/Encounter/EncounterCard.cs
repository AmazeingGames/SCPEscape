using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class EncounterCard : MonoBehaviour
{
    [SerializeField] float lerpSpeed = .5f;
    [SerializeField] Encounter encounter;
    [SerializeField] LayerMask encounterCardLayer;
    [SerializeField] Vector2 hiddenPosition;
    [SerializeField] Vector2 revealedPosition;
    [SerializeField] AnimationCurve curve;

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
    bool canBeClicked = true;

    bool isHiding;
    bool shouldLerp;
    float current;
    float target;
    Vector2 lerpTo;
    Vector3 startPosition;

    void Start()
    {
        
    }

    void Update()
    {
        IsMouseOver();
        IsClicked();

        MoveChoices();

        LerpTo();

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
        if (canBeClicked && isMouseOver && Input.GetMouseButtonDown(0))
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
        Debug.Log("Revealed Choices, hid encounter");

        HideEncounter();
    }

    //Purpose is to start the lerp and move the encounter out of the way to make room for the choices when they're revealed
    void HideEncounter()
    {
        target = 1;
        current = 0;

        transform.SetParent(GameManager.Instance.GameCanvas.transform);

        isHiding = true;
        startPosition = transform.position;
        lerpTo = hiddenPosition;

        shouldLerp = true;
    }

    //Purpose is to hide the choices and remove them from play, not allowing them to be selected.
    //This would also move the encounter to the forefront.
    void HideChoices()
    {
        Debug.Log("Hid Choices, revealed encounter");

        RevealEncounter();
    }

    //Purpose is to start the lerp and move the encounter front and center
    void RevealEncounter()
    {
        target = 1;
        current = 0;

        isHiding = false;
        startPosition = transform.position;
        lerpTo = revealedPosition;
        
        shouldLerp = true;
    }

    //Purpose is to animate moving the card up and down when clicking the encounter, and set the proper parent once lerp is finished
    void LerpTo()
    {
        if (shouldLerp)
        {
            Debug.Log($"Current : {current}. Target : {target}. ");
            canBeClicked = false;

            //Not sure why current doesn't have an effect on the speed of lerp.
            //Might have something to do with world positions? Nope, has nothing to do with world positions.
            current = Mathf.MoveTowards(current, target, lerpSpeed * Time.deltaTime);

            transform.position = Vector3.Lerp(startPosition, lerpTo, curve.Evaluate(current));

            if (current == target)
            {
                Debug.Log("Finished Lerp");
                canBeClicked = true;
                shouldLerp = false;

                if (isHiding)
                    transform.parent = GameManager.Instance.GameCanvas.transform;
                else
                    transform.parent = GameManager.Instance.Choices.transform;
            }
        }
    }

    //Purpose of this is to hide the choices on selection
    void OnChoiceSelection(Resource.ECardType[] nothing)
    {
        isChoiceSelected = true;

        HideChoices();
    }

    //Purpose is to set all of this card's details to that of an assigned scriptable object encounter
    void SetEncounter(Encounter encounter)
    {
        this.encounter = encounter;
    }
}
