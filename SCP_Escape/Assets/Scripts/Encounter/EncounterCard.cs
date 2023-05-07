using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class EncounterCard : MonoBehaviour
{
    [SerializeField] Encounter encounter;

    [Header("Encounter Details")]
    [SerializeField] TextMeshProUGUI encounterName;
    [SerializeField] TextMeshProUGUI flavorText;
    [SerializeField] LayerMask encounterCardLayer;

    [Header("Encounter Lerping")]
    [SerializeField] float lerpSpeed = .5f;
    //Replace these Vector2's with nodes and use their transform.position instead.
    [SerializeField] AnimationCurve curve;

    Transform hiddenPosition;
    Transform revealedPosition;

    GameObject Nodes => GameManager.Instance.Nodes;

    /* Functionality of the Encounter Card:
     * Clicking on the card will slide the encounter to the side and reveal the associated choice cards
     * Clicking on the card again will slide the encounter to the front and hide the associated choice cards
     * Selecting a choice will give the rewards to the player. The rewards are given by the choice card and not the EncounterCard
     * Selecting a choice will fold up all the cards. This is read via the GameManager; an action delegate is invoked by the selected choice that the encounter will read
     */

    List<ChoiceCard> choiceCards = new();

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

        IsMouseOver();
        IsClicked();

        GetChoices();

        MoveCards();
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
    void MoveCards()
    {
        if (isChoiceSelected)
            return;

        if (isClicked)
        {
            if (areChoicesRevealed)
            {
                ObscureChoices();
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
        MoveChoices(setActive : true);
    }

    //Purpose is to hide the choices and remove them from play, not allowing them to be selected.
    //This would also move the encounter to the forefront.
    void ObscureChoices()
    {
        Debug.Log("Hid Choices, revealed encounter");

        RevealEncounter();
        MoveChoices(setActive : false);
    }

    //Purpose is to start the lerp and move the encounter out of the way to make room for the choices when they're revealed
    void HideEncounter()
    {
        StartLerp(isHiding : true, lerpTo : hiddenPosition.position, newParent : GameManager.Instance.GameCanvas.transform);
    }

    //Purpose is to start the lerp and move the encounter front and center
    void RevealEncounter()
    {
        StartLerp(isHiding : false, lerpTo : revealedPosition.position, newParent : null);
    }

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

            if (current == 1)
            {
                Debug.Log("Finished Lerp");

                canBeClicked = true;
                shouldLerp = false;

                if (isHiding)
                {
                    //MoveChoices(setActive: true);
                    transform.SetParent(GameManager.Instance.GameCanvas.transform, false);
                }
                else
                {
                    transform.SetParent(GameManager.Instance.Choices.transform, false);
                }
            }
        }
    }

    //The animation in my head for this is the choices come down in a single pile, then slide over to both sides from the center until all cards are in place.
    //This could be done by placing node locations, which, come to think of it, I should have done with the 'hidden' and 'revealed' location, instead of figuring it out manually.
    //System of nodes : Create a second horizontal layout group and organize the nodes that way, turning them on and off and get a perfect adaptable and scalable system for figuring out positions. The only problem I can think of is how to tell which node is which, but that shouldn't be a hard fix.

    //Purpose is to hide/reveal the children choice cards
    void MoveChoices(bool setActive)
    {
        for (int i = 0; i < choiceCards.Count; i++)
        {
            ChoiceCard currentChoiceCard = choiceCards[i];

            currentChoiceCard.gameObject.SetActive(setActive);
            currentChoiceCard.transform.SetParent(GameManager.Instance.Choices.transform);
        }
    }

    //Grabs the empty choices from the GameManager'a pool and supplies them with choice data and puts them into a list.
    void GetChoices()
    {
        //if (IsActiveChoice && choiceCards.Count <= 0)
        if (choiceCards.Count <= 0)
        {
            for (int i = 0; i < encounter.Choices.Count; i++)
            {
                ChoiceCard currentChoiceCard = GameManager.Instance.GetFromChoicePool();

                if (currentChoiceCard == null)
                {
                    Debug.Log("null");
                    break;
                }

                Choice currentChoice = encounter.Choices[i];

                if (currentChoice == null)
                {
                    Debug.Log("null1");
                    break;
                }

                choiceCards.Add(currentChoiceCard);
                currentChoiceCard.SetChoice(currentChoice);
                currentChoiceCard.transform.SetParent(transform);

                //Debug.Log($"Grabbed {i} choices");
                Debug.Log($"ChoiceCards.Count : {choiceCards.Count}");
            }
        }

    }

    //Purpose of this is to hide the choices on selection
    void OnChoiceSelection(Resource.ECardType[] nothing)
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
