using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ランタイム上の "1 個分のダイス" データ
/// マスタ <see cref="DiceDefinition"/> から Deep Copy で生成し、ラン中に部分改造できます
/// セーブ時には JsonUtility でそのまま直列化できます
///
/// SO 参照は一切持たず、すべて永続 ID（GUID 文字列）で参照しています
/// </summary>
[Serializable]
public class RuntimeDice
{
    // ─── 由来となったマスタ定義の ID ────────────────────

    [SerializeField]
    [Tooltip("コピー元となった DiceDefinition の PersistentId（GUID 文字列）")]
    private string m_sourceDefinitionId;

    // ─── ダイス名 ────────────────────────────────────────

    [SerializeField]
    [Tooltip("デバッグ・UI 用の識別名")]
    private string m_diceName;

    // ─── 面リスト ────────────────────────────────────────

    [SerializeField]
    [Tooltip("ダイスの各面（要素数 = 面数）")]
    private List<RuntimeDiceFace> m_faces = new List<RuntimeDiceFace>();

    // ─── 公開プロパティ ──────────────────────────────────

    /// <summary>由来となったマスタ <see cref="DiceDefinition"/> の PersistentId</summary>
    public string SourceDefinitionId => m_sourceDefinitionId;

    /// <summary>ダイスの識別名</summary>
    public string DiceName => m_diceName;

    /// <summary>面の一覧（読み取り専用）</summary>
    public IReadOnlyList<RuntimeDiceFace> Faces => m_faces;

    /// <summary>面数</summary>
    public int FaceCount => m_faces.Count;

    // ─── コンストラクタ ──────────────────────────────────

    // JsonUtility 用の引数なしコンストラクタ
    public RuntimeDice() { }

    /// <summary>
    /// マスタ <see cref="DiceDefinition"/> から RuntimeDice を内部生成します
    /// 通常は <see cref="RuntimeDiceFactory.CreateFrom(DiceDefinition)"/> 経由で使ってください
    /// </summary>
    internal static RuntimeDice CreateInternal(DiceDefinition master)
    {
        RuntimeDice dice = new RuntimeDice
        {
            m_sourceDefinitionId = master.PersistentId,
            m_diceName = master.DiceName,
        };

        foreach (DiceFace face in master.Faces)
        {
            dice.m_faces.Add(RuntimeDiceFace.CreateFrom(face));
        }

        return dice;
    }

    // ─── 改造 API ────────────────────────────────────────

    /// <summary>
    /// 指定インデックスの面を、新しいマスタ面の Deep Copy で置き換えます
    /// </summary>
    /// <param name="faceIndex">置き換える面のインデックス（0 始まり）</param>
    /// <param name="newMasterFace">新しい面のマスタ SO</param>
    public bool ReplaceFace(int faceIndex, DiceFace newMasterFace)
    {
        if (faceIndex < 0 || faceIndex >= m_faces.Count)
        {
            Debug.LogWarning($"[RuntimeDice] ReplaceFace: faceIndex {faceIndex} が範囲外です。");
            return false;
        }
        if (newMasterFace == null)
        {
            Debug.LogWarning("[RuntimeDice] ReplaceFace: newMasterFace が null です。");
            return false;
        }

        // 完全に新しい RuntimeDiceFace を作って差し替える
        m_faces[faceIndex] = RuntimeDiceFace.CreateFrom(newMasterFace);
        return true;
    }

    /// <summary>
    /// 指定エントリの基本値に delta を加算します
    /// </summary>
    public bool IncreaseEntryValue(int faceIndex, int entryIndex, int delta)
    {
        RuntimeActionEntry entry = GetEntry(faceIndex, entryIndex);
        if (entry == null) return false;
        entry.AddToValue(delta);
        return true;
    }

    /// <summary>
    /// 指定面に新しいエントリを追加します
    /// </summary>
    /// <param name="faceIndex">追加先の面のインデックス</param>
    /// <param name="action">追加するアクション（PersistentId が必要）</param>
    /// <param name="target">対象</param>
    /// <param name="value">基本値</param>
    public bool AddEntry(int faceIndex, DiceAction action, DiceFaceTarget target, int value)
    {
        if (faceIndex < 0 || faceIndex >= m_faces.Count)
        {
            Debug.LogWarning($"[RuntimeDice] AddEntry: faceIndex {faceIndex} が範囲外です。");
            return false;
        }
        if (action == null)
        {
            Debug.LogWarning("[RuntimeDice] AddEntry: action が null です。");
            return false;
        }
        if (string.IsNullOrEmpty(action.PersistentId))
        {
            Debug.LogWarning($"[RuntimeDice] AddEntry: action '{action.name}' に PersistentId がありません。" +
                             "セーブ後にロードできなくなる可能性があります。");
        }

        m_faces[faceIndex].AddEntry(new RuntimeActionEntry(action.PersistentId, target, value));
        return true;
    }

    /// <summary>
    /// 指定面の指定エントリを削除します
    /// </summary>
    public bool RemoveEntry(int faceIndex, int entryIndex)
    {
        if (faceIndex < 0 || faceIndex >= m_faces.Count)
        {
            Debug.LogWarning($"[RuntimeDice] RemoveEntry: faceIndex {faceIndex} が範囲外です。");
            return false;
        }

        return m_faces[faceIndex].RemoveEntryAt(entryIndex);
    }

    /// <summary>
    /// 指定面（読み取り専用、範囲外なら null）
    /// </summary>
    public RuntimeDiceFace GetFace(int faceIndex)
    {
        if (faceIndex < 0 || faceIndex >= m_faces.Count) return null;
        return m_faces[faceIndex];
    }

    // ─── 内部ヘルパー ────────────────────────────────────

    // 面・エントリの境界チェック付き取得
    private RuntimeActionEntry GetEntry(int faceIndex, int entryIndex)
    {
        if (faceIndex < 0 || faceIndex >= m_faces.Count)
        {
            Debug.LogWarning($"[RuntimeDice] faceIndex {faceIndex} が範囲外です。");
            return null;
        }

        RuntimeActionEntry entry = m_faces[faceIndex].GetEntryAt(entryIndex);
        if (entry == null)
        {
            Debug.LogWarning($"[RuntimeDice] entryIndex {entryIndex} が範囲外です（face {faceIndex}）。");
        }
        return entry;
    }
}
