using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using static GameManager;
using static EncounterDeck;
using static EncounterAnimator;
using Animation = EncounterAnimator.Animation;

public class EncounterCard : MonoBehaviour
{
    public event EventHandler<CardAnimationEventArgs> CardAnimation = null;

    [SerializeField] Encounter encounter;

    [Header("Encounter Details")]
    [SerializeField] TextMeshProUGUI encounterName;
    [SerializeField] TextMeshProUGUI flavorText;
    [SerializeField] LayerMask encounterCardLayer;


    public List<ChoiceCard> ChoiceCards { get; } = new();
    public Encounter Encounter { get => encounter; private set => encounter = value; }

    bool areChoicesRevealed;
    bool isMouseOver = false;
    bool isClicked = false;
    bool isChoiceSelected = false;
    bool canBeClicked = true;

    public Action LerpStart;
    public Action LerpEnd;

    void Start()
    {
        LerpStart += OnLerpStart;
        LerpEnd += OnLerpEnd;

        //This used to be in Update, not sure why, but consider moving it back in case of bugs
        GetChoices();
    }

    void Update()
    {
        SetAndMatchEncounter(encounter);

        CheckMouseOver();
        IsClicked();

        MoveCards();
    }

    private void OnEnable()
    {
        CardAnimation += AnimationManager.OnCardAnimation;
    }

    private void OnDisable()
    {
        CardAnimation += AnimationManager.OnCardAnimation;
    }

    //Checks if the mouse is over this card
    void CheckMouseOver() => isMouseOver = IsOver(encounterCardLayer, transform);

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
                CardAnimation?.Invoke(this, new CardAnimationEventArgs(cardToAnimate: this, animationsToPlay: new List<Animation>() 
                { Animation.HideChoices, Animation.RevealEncounter }));
            else
                CardAnimation?.Invoke(this, new CardAnimationEventArgs(cardToAnimate: this, animationsToPlay: new List<Animation>() 
                { Animation.RevealChoices, Animation.HideEncounter }));

            areChoicesRevealed = !areChoicesRevealed;
        }
    }

    //Grabs the empty choices from the GameManager's pool and supplies them with choice data and puts them into a list.
    void GetChoices()
    {
        if (ChoiceCards.Count <= 0)
        {
            for (int i = 0; i < encounter.Choices.Count; i++)
            {
                ChoiceCard currentChoiceCard = Manager.GetFromChoicePool();

                Choice currentChoice = encounter.Choices[i];

                ChoiceCards.Add(currentChoiceCard);

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

        CardAnimation?.Invoke(this, new CardAnimationEventArgs(cardToAnimate: this, animationsToPlay: new List<Animation>() { Animation.DiscardEncounter }));

        foreach (ChoiceCard choiceCard in ChoiceCards)
            choiceCard.DiscardChoice();
    }

    void OnLerpStart()
    {
        canBeClicked = false;
    }

    void OnLerpEnd()
    {
        canBeClicked = true;
    }

    void DiscardCleanup()
    {
        encounter = null;
        isChoiceSelected = false;
    }

    //Purpose is to set all of this card's details to that of an assigned scriptable object encounter
    public void SetAndMatchEncounter(Encounter encounter)
    {
        this.encounter = encounter;

        if (encounter == null)
            return;

        flavorText.text = encounter.FlavorText;
        encounterName.text = encounter.EncounterName;
    }
}
