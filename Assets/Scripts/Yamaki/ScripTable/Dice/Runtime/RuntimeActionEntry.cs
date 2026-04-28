using System;
using UnityEngine;

/// <summary>
/// ランタイム上の "1 つの効果エントリ"
/// SO 参照を直接持たず、<see cref="DiceAction"/> を永続 ID（GUID 文字列）で参照します
/// セーブ時には JsonUtility 等でそのまま直列化できます
/// </summary>
[Serializable]
public class RuntimeActionEntry
{
    // ─── DiceAction 参照（ID） ────────────────────────────

    [SerializeField]
    [Tooltip("実行する DiceAction の PersistentId（GUID 文字列）")]
    private string m_actionId;

    // ─── 対象 ─────────────────────────────────────────────

    [SerializeField]
    [Tooltip("効果の対象（Self=使用者, Enemy=敵）")]
    private DiceFaceTarget m_target;

    // ─── 基本値 ───────────────────────────────────────────

    [SerializeField]
    [Tooltip("DiceAction.Execute へ渡す基本値")]
    private int m_value;

    // ─── 公開プロパティ ───────────────────────────────────

    /// <summary>DiceAction の永続 ID（GUID 文字列）</summary>
    public string ActionId => m_actionId;

    /// <summary>効果を適用する対象</summary>
    public DiceFaceTarget Target => m_target;

    /// <summary>DiceAction.Execute に渡す基本値</summary>
    public int Value => m_value;

    // ─── コンストラクタ ───────────────────────────────────

    // JsonUtility 用の引数なしコンストラクタ
    public RuntimeActionEntry() { }

    /// <summary>
    /// ID と対象、基本値を指定してエントリを生成します
    /// </summary>
    public RuntimeActionEntry(string actionId, DiceFaceTarget target, int value)
    {
        m_actionId = actionId;
        m_target = target;
        m_value = value;
    }

    // ─── 改造用 setter（RuntimeDice からのみ呼ばれる想定） ─

    /// <summary>基本値に delta を加算します</summary>
    internal void AddToValue(int delta)
    {
        m_value += delta;
    }

    /// <summary>基本値を上書きします</summary>
    internal void SetValue(int newValue)
    {
        m_value = newValue;
    }
}
