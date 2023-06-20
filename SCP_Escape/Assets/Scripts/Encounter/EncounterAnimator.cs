using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;

public class EncounterAnimator : MonoBehaviour
{

    public enum Animation { Unknown, DrawEncounter, DiscardEncounter, HideEncounter, RevealEncounter, HideChoices, RevealChoices }

    public event Action<EncounterCard> DiscardEncounter = null;

    public class CardAnimationEventArgs : EventArgs
    {
        public EncounterCard CardToAnimate { get; }
        public List<Animation>[] AnimationsToPlay { get; }

        public CardAnimationEventArgs(EncounterCard cardToAnimate, params List<Animation>[] animationsToPlay)
        {
            CardToAnimate = cardToAnimate;
            AnimationsToPlay = animationsToPlay;
        }
    }

    public static EncounterAnimator AnimationManager { get; private set; }

    [Header("Encounter Lerping")]
    [SerializeField] float lerpSpeed = .5f;
    [SerializeField] AnimationCurve curve;

    Transform hiddenPosition;
    Transform revealedPosition;
    Transform fullyHiddenPosition;

    GameObject Nodes => Manager.Nodes;

    Vector3 startPosition;
  
    float current;

    void Awake()
    {
        if (AnimationManager == null)
            AnimationManager = this;
        else
            Destroy(AnimationManager);

        Debug.Log($"Animation Manager null : {AnimationManager == null}");
    }

    // Start is called before the first frame update
    void Start()
    {
        hiddenPosition = Nodes.transform.Find("EncounterHidden");
        revealedPosition = Nodes.transform.Find("EncounterRevealed");
        fullyHiddenPosition = Nodes.transform.Find("FullyHiddenPosition");
    }

    private void Update()
    {
        
    }

    public void OnCardAnimation(object sender, CardAnimationEventArgs args)
    {
        foreach (List<Animation> animations in args.AnimationsToPlay)
        {
            foreach (Animation animation in animations)
            {
                Debug.Log($"Playing animation : \"{animation}\"");
                switch (animation)
                {
                    case Animation.DiscardEncounter:
                        StartCoroutine(LerpEncounterCard(args.CardToAnimate, lerpTo: revealedPosition.position, newParent: Manager.Choices.transform, animationEvent: Animation.DiscardEncounter));

                        ActivateChoices(args.CardToAnimate.ChoiceCards, setActive: false);
                        break;

                    case Animation.DrawEncounter:
                        break;

                    case Animation.HideEncounter:
                        StartCoroutine(LerpEncounterCard(args.CardToAnimate, lerpTo: hiddenPosition.position, tempParent: Manager.GameCanvas.transform, newParent: Manager.GameCanvas.transform));
                        break;

                    case Animation.RevealEncounter:
                        StartCoroutine(LerpEncounterCard(args.CardToAnimate, lerpTo: revealedPosition.position, newParent: Manager.Choices.transform));
                        break;

                    case Animation.HideChoices:
                        ActivateChoices(args.CardToAnimate.ChoiceCards, setActive: false);
                        break;

                    case Animation.RevealChoices:
                        ActivateChoices(args.CardToAnimate.ChoiceCards, setActive: true);
                        break;

                    default:
                        Debug.LogWarning("Animation not known");
                        break;
                }
            }
        }
    }

    //Purpose is to animate moving the card up and down when clicking the encounter, and set the proper parent once lerp is finished
    public IEnumerator LerpEncounterCard(EncounterCard encounterCard, Vector2 lerpTo, bool shouldSetTempParent = true, Transform tempParent = null, bool shouldSetNewParent = true, Transform newParent = null, Animation animationEvent = Animation.Unknown)
    {
        encounterCard.LerpStart();

        var lerpStartPosition = encounterCard.transform.position;

        if (shouldSetTempParent )
            encounterCard.transform.SetParent(tempParent);

        encounterCard.transform.position = lerpStartPosition;

        current = 0;
        startPosition = encounterCard.transform.position;

        while (true)
        {

            current = Mathf.MoveTowards(current, 1, lerpSpeed * Time.deltaTime);

            encounterCard.transform.position = Vector3.Lerp(startPosition, lerpTo, curve.Evaluate(current));

            //Lerp is finished; cleanup
            if (current == 1)
            {
                Debug.Log("Coroutine finished");

                if (shouldSetNewParent)
                    encounterCard.transform.SetParent(newParent, false);

                switch (animationEvent)
                {
                    case Animation.Unknown:
                        break;
                    case Animation.DrawEncounter:
                        break;
                    case Animation.DiscardEncounter:
                        Debug.Log("Invoked discard encounter");
                        DiscardEncounter.Invoke(encounterCard);
                        break;
                    case Animation.HideEncounter:
                        break;
                    case Animation.RevealEncounter:
                        break;
                    case Animation.HideChoices:
                        break;
                    case Animation.RevealChoices:
                        break;
                }

                encounterCard.LerpEnd();

                yield break;
            }
            yield return null;
        }
    }

    //Purpose is to show or hide the choices by setting them active
    public void ActivateChoices(List<ChoiceCard> choiceCards, bool setActive)
    {
        for (int i = 0; i < choiceCards.Count; i++)
        {
            ChoiceCard currentChoiceCard = choiceCards[i];

            currentChoiceCard.gameObject.SetActive(setActive);
            currentChoiceCard.transform.SetParent(Manager.Choices.transform);
        }
    }
}