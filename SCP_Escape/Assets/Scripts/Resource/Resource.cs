using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Cards/Resources")]

public class Resource : ScriptableObject
{
    public enum ECardType { Anomaly, Escapee, Insanity, Munition, Ration, Scientist }

    [field: FormerlySerializedAs("cardType")]                   [field: SerializeField] public ECardType CardType { get; private set; }
    [field: FormerlySerializedAs("indicatorBackground")]        [field: SerializeField] public Sprite IndicatorBackground { get; private set; }
    [field: FormerlySerializedAs("indicatorBorder")]            [field: SerializeField] public Sprite IndicatorBorder { get; private set; }
    [field: FormerlySerializedAs("symbol")]                     [field: SerializeField] public Sprite Symbol { get; private set; }
    [field: FormerlySerializedAs("texture")]                    [field: SerializeField] public Sprite Texture { get; private set; }
    [field: FormerlySerializedAs("indicatorBackgroundColor")]   [field: SerializeField] public Color IndicatorBackgroundColor { get; private set; }
    [field: FormerlySerializedAs("indicatorBorderColor")]       [field: SerializeField] public Color IndicatorBorderColor { get; private set; }
    [field: FormerlySerializedAs("symbolColor")]                [field: SerializeField] public Color SymbolColor { get; private set; }
    [field: FormerlySerializedAs("textureColor")]               [field: SerializeField] public Color TextureColor { get; private set; }
    [field: FormerlySerializedAs("initial")]                    [field: SerializeField] public char Initial { get; private set; }
    [field: FormerlySerializedAs("initialColor")]               [field: SerializeField] public Color InitialColor { get; private set; }
    [field: FormerlySerializedAs("textureOffeset")]             [field: SerializeField] public Vector2 TextureOffeset { get; private set; }
    [field: FormerlySerializedAs("cardColor")]                  [field: SerializeField] public Color CardColor { get; private set; }

    public bool HasBeenPlayed { get; protected set; }
    public bool HasBeenConsumed { get; protected set; }
    public int HandIndex { get; protected set; }

}
