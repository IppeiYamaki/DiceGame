using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダイスの "1 面分の効果" を表す ScriptableObject。
/// Inspector から複数の ActionEntry（アクション・対象・基本値）を設定できます。
///
/// 作成方法: Project ビュー右クリック →
///   Create > DiceGame > DiceFace
/// </summary>
[CreateAssetMenu(fileName = "DiceFace_", menuName = "DiceGame/DiceFace")]
public class DiceFace : ScriptableObject
{
    // ─── 面の名前（省略可） ───────────────────────────────

    [SerializeField]
    [Header("面の名前（省略可）")]
    [Tooltip("Inspector や デバッグ用の識別名。例: 「攻撃×3」など")]
    private string m_faceName = "";

    // ─── 効果リスト ────────────────────────────────────────

    [SerializeField]
    [Header("効果リスト")]
    [Tooltip("この面が出たときに上から順に実行されるアクション一覧。\n" +
             "同じ ActionEntry を複数追加すると、その回数だけ効果が発動します。")]
    private List<ActionEntry> m_actions = new List<ActionEntry>();

    // ─── 読み取り専用プロパティ ───────────────────────────

    /// <summary>Inspector で設定した面の識別名</summary>
    public string FaceName => m_faceName;

    /// <summary>この面が持つ効果エントリの一覧（読み取り専用）</summary>
    public IReadOnlyList<ActionEntry> Actions => m_actions;
}
