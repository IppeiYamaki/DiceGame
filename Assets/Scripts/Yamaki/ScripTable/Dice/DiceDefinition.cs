using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダイス本体を表す ScriptableObject
/// 複数の DiceFace を持ち、6 面に限らず n 面ダイスに対応します <br />
///
/// 作成方法: Project ビュー右クリック →
///   Create > DiceGame > DiceDefinition
/// </summary>
[CreateAssetMenu(fileName = "DiceDefinition_", menuName = "DiceGame/DiceDefinition")]
public class DiceDefinition : ScriptableObject
{
    // ─── ダイス名 ──────────────────────────────────────────

    [SerializeField]
    [Header("ダイス名")]
    [Tooltip("このダイスの識別名\n例: 「スターターダイス」")]
    private string m_diceName = "";

    // ─── 面リスト ──────────────────────────────────────────

    [SerializeField]
    [Header("面リスト")]
    [Tooltip("ダイスの各面に対応する DiceFace アセット\n" +
             "要素数がそのままダイスの面数になります（6 面に限りません）")]
    private List<DiceFace> m_faces = new List<DiceFace>();

    // ─── 読み取り専用プロパティ ───────────────────────────

    /// <summary>ダイスの識別名</summary>
    public string DiceName => m_diceName;

    /// <summary>ダイスが持つ面の一覧（読み取り専用）</summary>
    public IReadOnlyList<DiceFace> Faces => m_faces;

    /// <summary>ダイスの面数</summary>
    public int FaceCount => m_faces.Count;
}
