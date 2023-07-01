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
using UnityEngine.Serialization;

public class EncounterCard : MonoBehaviour
{
    public event EventHandler<CardAnimationEventArgs> StartCardAnimation = null;

    [field: FormerlySerializedAs("encounter")] [field: SerializeField] public Encounter Encounter { get; private set; }

    [Header("Encounter Details")]
    [SerializeField] TextMeshProUGUI encounterName;
    [SerializeField] TextMeshProUGUI flavorText;
    [SerializeField] LayerMask encounterCardLayer;


    public List<ChoiceCard> ChoiceCards { get; } = new();

    bool areChoicesRevealed;
    bool isMouseOver = false;
    bool isClicked = false;
    bool isChoiceSelected = false;
    bool canBeClicked = true;

    [SerializeField] float CoyoteClickTimerLength;
    float CoyoteClickTime = 0f;

    public Action LerpStarted;
    public Action LerpFinished;

    void Start()
    {
    }

    void Update()
    {
        GetChoices();

        SetAndMatchEncounter(Encounter);

        CheckMouseOver();
        IsClicked();

        MoveCards();

        CoyoteClickTime -= Time.deltaTime;
    }

    private void OnEnable()
    {
        StartCardAnimation += AnimationManager.OnCardAnimation;
        AnimationManager.DiscardEncounter += DiscardCleanup;

        LerpStarted += OnLerpStart;
        LerpFinished += OnLerpEnd;
    }

    private void OnDisable()
    {
        StartCardAnimation -= AnimationManager.OnCardAnimation;
        AnimationManager.DiscardEncounter -= DiscardCleanup;

        LerpStarted -= OnLerpStart;
        LerpFinished -= OnLerpEnd;
    }

    //Checks if the mouse is over this card
    void CheckMouseOver() => isMouseOver = IsOver(encounterCardLayer, transform);

    //Checks if the player is clicking on the card
    void IsClicked()
    {
        isClicked = (canBeClicked && isMouseOver && Input.GetMouseButtonDown(0));

        if (Input.GetMouseButtonDown(0) && isMouseOver)
        {
            if (!canBeClicked)
                CoyoteClickTime = CoyoteClickTimerLength;
        }

        if (canBeClicked && CoyoteClickTime > 0)
        {
            isClicked = true;
            CoyoteClickTime = 0;
        }
    }

    //Purpose is to reveal/hide choices when the encounter is clicked.
    //Choices will be initially hidden from the player, clicking the encounter reveals them and hides the encounter. Clicking the encounter again, hides the choices, and repeat.
    void MoveCards()
    {
        if (isChoiceSelected)
            return;

        if (isClicked)
        {
            if (areChoicesRevealed)
            {
                StartCardAnimation?.Invoke(this, new CardAnimationEventArgs(cardToAnimate: this, animationsToPlay: new List<Animation>()
                { Animation.HideChoices, Animation.RevealEncounter }));
            }
            else
            {
                StartCardAnimation?.Invoke(this, new CardAnimationEventArgs(cardToAnimate: this, animationsToPlay: new List<Animation>()
                { Animation.RevealChoices, Animation.HideEncounter }));
            }
                

            areChoicesRevealed = !areChoicesRevealed;
        }
    }

    //Grabs the empty choices from the GameManager's pool and supplies them with choice data and puts them into a list.
    void GetChoices()
    {
        if (ChoiceCards.Count <= 0)
        {
            for (int i = 0; i < Encounter.Choices.Count; i++)
            {
                ChoiceCard currentChoiceCard = Manager.GetFromChoicePool();

                Choice currentChoice = Encounter.Choices[i];

                ChoiceCards.Add(currentChoiceCard);

                currentChoiceCard.SetChoice(currentChoice);
                currentChoiceCard.transform.SetParent(transform);
                currentChoiceCard.transform.position = new Vector3(100, 100, currentChoiceCard.transform.position.z);

                currentChoiceCard.ChoiceSelection += OnChoiceSelection;
            }
        }

    }

    //Hides the choices on selection and makes sure the card can't be selected again
    void OnChoiceSelection(object sender, CardSelectionEventArgs args)
    {
        isChoiceSelected = true;

        StartCardAnimation?.Invoke(this, new CardAnimationEventArgs(cardToAnimate: this, animationsToPlay: new List<Animation>() { Animation.DiscardEncounter }));

        foreach (ChoiceCard choiceCard in ChoiceCards)
        {
            choiceCard.DiscardChoice();
            choiceCard.ChoiceSelection -= OnChoiceSelection;
        }
    }

    void OnLerpStart()
    {
        canBeClicked = false;
    }

    void OnLerpEnd()
    {
        canBeClicked = true;
    }

    void DiscardCleanup(EncounterCard encounterCard)
    {
        if (encounterCard == this)
        {
            Encounter = null;
            isChoiceSelected = false;
            areChoicesRevealed = false;
            ChoiceCards.Clear();
        }
        
    }

    //Purpose is to set all of this card's details to that of an assigned scriptable object encounter
    public void SetAndMatchEncounter(Encounter encounter)
    {
        if (encounter == null)
            Debug.LogWarning("Setting encounter to null");
        Encounter = encounter;

        if (encounter == null)
            return;

        flavorText.text = encounter.FlavorText;
        encounterName.text = encounter.EncounterName;
    }
}
