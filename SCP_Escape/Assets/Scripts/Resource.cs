using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : ScriptableObject
{
    public enum CardType { Anomaly, Escapee, Prisoner, Food, Key, Misfortune }

    [SerializeField] CardType ECardType;
    [SerializeField] Sprite sprite;

    public bool HasBeenPlayed { get; protected set; }
    public bool HasBeenConsumed { get; protected set; }

    public int HandIndex { get; protected set; }

    public void OnMouseDown()
    {
        GameManager.Instance.GrabCard(this);
    }

    public void OnMouseUp()
    {
        GameManager.Instance.DropCard();
    }
}
