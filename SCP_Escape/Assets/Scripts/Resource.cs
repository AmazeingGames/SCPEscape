using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Cards/Resources")]

public class Resource : ScriptableObject
{
    public enum CardType { Anomaly, Escapee, Ration, Insanity, Munition, Scientist}

    [SerializeField] CardType eCardType;

    [SerializeField] Sprite symbol;
    [SerializeField] Sprite texture;
    [SerializeField] Color cardColor;
    [SerializeField] char initial;

    public CardType _ECardType { get => eCardType; private set => eCardType = value; }
    public Sprite _Symbol { get => symbol; private set => symbol = value; }
    public Sprite _Texture { get => symbol; private set => texture = value; }
    public Color _CardColor { get => cardColor; private set => cardColor = value; }
    public char _Initial { get => initial; private set => initial = value; }

    public bool HasBeenPlayed { get; protected set; }
    public bool HasBeenConsumed { get; protected set; }
    public int HandIndex { get; protected set; }

}
