using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using static GameManager;
using static EncounterDeck;

public class EncounterCard : MonoBehaviour
{
    //On second thought this event is sort of pointless.
    //It's an event meant to tell the encounter deck that an animation is finished so we can play a new animation from the encounter deck
    //Why not play the animation here instead?

    //public event Action<EncounterCard> FinishedAnimation;

    [SerializeField] Encounter encounter;

    [Header("Encounter Details")]
    [SerializeField] TextMeshProUGUI encounterName;
    [SerializeField] TextMeshProUGUI flavorText;
    [SerializeField] LayerMask encounterCardLayer;

    [Header("Encounter Lerping")]
    [SerializeField] float lerpSpeed = .5f;
    [SerializeField] AnimationCurve curve;

    Transform hiddenPosition;
    Transform revealedPosition;

    GameObject Nodes => Manager.Nodes;

    readonly List<ChoiceCard> choiceCards = new();

    public bool IsActiveChoice;

    bool areChoicesRevealed;
    bool isMouseOver = false;
    bool isClicked = false;
    bool isChoiceSelected = false;
    bool canBeClicked = true;

    bool isHiding;
    bool shouldLerp;
    float current;
    Vector2 lerpTo;
    Vector3 startPosition;

    void Start()
    {
        hiddenPosition = Nodes.transform.Find("EncounterHidden");
        revealedPosition = Nodes.transform.Find("EncounterRevealed");
    }

    void Update()
    {
        SetAndMatchEncounter(encounter);

        CheckMouseOver();
        IsClicked();

        GetChoices();

        MoveCards();
        LerpTo();
    }

    //Checks if the mouse is over this card
    void CheckMouseOver() => isMouseOver = GameManager.IsOver(encounterCardLayer, transform);

    //Checks if the player is clicking on the card
    void IsClicked() => isClicked = (canBeClicked && isMouseOver && Input.GetMouseButtonDown(0));

    //Purpose is to reveal/hide choices when the encounter is clicked.
    //Choices will be initially hidden from the player, clicking the encounter reveals them and hides the encounter. Clicking the encounter again, hides the choices, and repeat.
    void MoveCards()
    {
        if (isChoiceSelected)
            return;

        if (isClicked)
        {
            if (areChoicesRevealed)
                ObscureChoices();
            else
                RevealChoices();

            areChoicesRevealed = !areChoicesRevealed;
        }
    }

    //Ideally the animations would play out like this: A card is drawn from the deck, placed face down in the center of the table and flips over revealing the encounter. Clicking causes it to slide to the side, and choice cards to slide out from under it, revealing the player's options
    //Purpose is to reveal the choices and put them into play, allowing them to be selected.
    //This would also move the encounter to the side to obscure it.
    void RevealChoices()
    {
        HideEncounter();
        ActivateChoices(setActive : true);
    }

    //Purpose is to hide the choices and remove them from play, not allowing them to be selected.
    //This would also move the encounter to the forefront.
    void ObscureChoices()
    {
        RevealEncounter();
        ActivateChoices(setActive : false);
    }

    //Purpose is to start the lerp and move the encounter out of the way to make room for the choices when they're revealed
    void HideEncounter() => StartLerp(isHiding: true, lerpTo: hiddenPosition.position, newParent: Manager.GameCanvas.transform);

    //Purpose is to start the lerp and move the encounter front and center
    void RevealEncounter() => StartLerp(isHiding : false, lerpTo : revealedPosition.position, newParent : null);

    //Responsible for setting the variables needed to actually lerp
    void StartLerp(bool isHiding, Vector2 lerpTo, Transform newParent)
    {
        if (newParent != null)
            transform.SetParent(newParent);

        current = 0;
        startPosition = transform.position;

        this.isHiding = isHiding;
        this.lerpTo = lerpTo;

        shouldLerp = true;
    }

    //Purpose is to animate moving the card up and down when clicking the encounter, and set the proper parent once lerp is finished
    void LerpTo()
    {
        if (shouldLerp)
        {
            canBeClicked = false;

            current = Mathf.MoveTowards(current, 1, lerpSpeed * Time.deltaTime);

            transform.position = Vector3.Lerp(startPosition, lerpTo, curve.Evaluate(current));

            //Lerp is finished; cleanup
            if (current == 1)
            {
                canBeClicked = true;
                shouldLerp = false;

                Transform newParent;

                if (isHiding)
                    newParent = Manager.GameCanvas.transform;
                else
                    newParent = Manager.Choices.transform;
                
                transform.SetParent(newParent, false);

                if (isChoiceSelected)
                {
                    Debug.Log("Finished animation, ready to discard");

                    //FinishedAnimation.Invoke(this);
                }

            }
        }
    }

    //The animation in my head for this is the choices come down in a single pile, then slide over to both sides from the center until all cards are in place.
    //This could be done by placing node locations.
    //This is a lot easier to place manually, granted the cards don't move or change too much, than it is to place them procedurally via nodes

    //Purpose is to show or hide the choices by setting them active
    void ActivateChoices(bool setActive)
    {
        for (int i = 0; i < choiceCards.Count; i++)
        {
            ChoiceCard currentChoiceCard = choiceCards[i];

            currentChoiceCard.gameObject.SetActive(setActive);
            currentChoiceCard.transform.SetParent(Manager.Choices.transform);
        }
    }

    //Grabs the empty choices from the GameManager's pool and supplies them with choice data and puts them into a list.
    void GetChoices()
    {
        if (choiceCards.Count <= 0)
        {
            for (int i = 0; i < encounter.Choices.Count; i++)
            {
                ChoiceCard currentChoiceCard = Manager.GetFromChoicePool();

                Choice currentChoice = encounter.Choices[i];

                choiceCards.Add(currentChoiceCard);

                currentChoiceCard.SetChoice(currentChoice);
                currentChoiceCard.transform.SetParent(transform);

                currentChoiceCard.ChoiceSelection += OnChoiceSelection;
            
            }
        }

    }

    //Hides the choices on selection and makes sure the card can't be selected again
    void OnChoiceSelection(object sender, CardSelectionEventArgs args)
    {
        isChoiceSelected = true;

        ObscureChoices();
    }

    //Purpose is to set all of this card's details to that of an assigned scriptable object encounter
    public void SetAndMatchEncounter(Encounter encounter)
    {
        this.encounter = encounter;

        flavorText.text = encounter.FlavorText;
        encounterName.text = encounter.EncounterName;
    }
}
