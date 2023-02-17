using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Cards/Resources")]

public class Resource : ScriptableObject
{
    public enum ECardType { Anomaly, Escapee, Insanity, Munition, Ration, Scientist }

    [SerializeField] ECardType cardType;

    [SerializeField] Sprite indicatorBackground;
    [SerializeField] Sprite indicatorBorder;
    [SerializeField] Sprite symbol;
    [SerializeField] Sprite texture;
    [SerializeField] Color indicatorBackgroundColor;
    [SerializeField] Color indicatorBorderColor;
    [SerializeField] Color symbolColor;
    [SerializeField] Color textureColor;
    [SerializeField] char initial;
    [SerializeField] Color initialColor;
    [SerializeField] Vector2 textureOffeset;

    public ECardType CardType { get => cardType; private set => cardType = value; }
    public Sprite Symbol { get => symbol; private set => symbol = value; }
    public Sprite Texture { get => texture; private set => texture = value; }
    public Sprite IndicatorBackground { get => indicatorBackground; private set => indicatorBackground = value; }
    public Vector2 TextureOffeset { get => textureOffeset; private set => textureOffeset = value; }
    public Color IndicatorBackgroundColor { get => indicatorBackgroundColor; private set => indicatorBackgroundColor = value; }
    public Color IndicatorBorderColor { get => indicatorBorderColor; private set => indicatorBorderColor = value; }
    public Sprite IndicatorBorder { get => indicatorBorder; private set => indicatorBorder = value; }
    public Color InitialColor { get => initialColor; private set => initialColor = value; }
    public char Initial { get => initial; private set => initial = value; }
    public Color SymbolColor { get => symbolColor; private set => symbolColor = value; }
    public Color TextureColor { get => textureColor; private set => textureColor = value; }

    public bool HasBeenPlayed { get; protected set; }
    public bool HasBeenConsumed { get; protected set; }
    public int HandIndex { get; protected set; }

}
