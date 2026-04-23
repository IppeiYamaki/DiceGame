using UnityEngine;

[CreateAssetMenu(fileName = "Debuff_", menuName = "Scriptable Objects/Status/Debuff")]
public class DebuffId : ScriptableObject {
    [Header("UI")]
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Resist")]
    // 例：耐性カテゴリをここに持たせる（毒耐性、火傷耐性、呪い耐性…）
    public DebuffResistType resistType;
}

public enum DebuffResistType {
    Poison,
    Burn,
    Curse,
    Slow,
}