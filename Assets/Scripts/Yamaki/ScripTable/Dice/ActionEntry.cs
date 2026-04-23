using System;
using UnityEngine;

/// <summary>
/// 1 つの "効果エントリ" を表す直列化可能なデータクラス。
/// DiceFace が持つ List&lt;ActionEntry&gt; の要素として使用します。
/// </summary>
[Serializable]
public class ActionEntry
{
    // ─── アクション ───────────────────────────────────────

    [SerializeField]
    [Header("アクション")]
    [Tooltip("実行する DiceAction アセットを設定してください（例: AttackAction / BlockAction）")]
    private DiceAction m_action;

    // ─── 対象 ─────────────────────────────────────────────

    [SerializeField]
    [Header("対象")]
    [Tooltip("効果を適用する対象\nSelf=使用者, Enemy=敵")]
    private DiceFaceTarget m_target = DiceFaceTarget.Enemy;

    // ─── 基本値 ───────────────────────────────────────────

    [SerializeField]
    [Header("基本値")]
    [Tooltip("DiceAction.Execute へ渡す基本値（バフ等の最終計算は別の場所で行う）")]
    private int m_value = 1;

    // ─── 読み取り専用プロパティ ───────────────────────────

    /// <summary>実行する DiceAction</summary>
    public DiceAction Action => m_action;

    /// <summary>効果を適用する対象</summary>
    public DiceFaceTarget Target => m_target;

    /// <summary>
    /// DiceAction.Execute に渡す基本値<br />
    /// ※ Execute の引数名は diceValue ですが、ここでは "出目の値" ではなく
    ///   "プランナーが設定した基本値" として扱います <br />
    ///   バフ等による最終値の計算は呼び出し側で行ってください
    /// </summary>
    public int Value => m_value;
}
