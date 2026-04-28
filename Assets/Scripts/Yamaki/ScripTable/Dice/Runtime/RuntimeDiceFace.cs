using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ランタイム上の "1 面分" のデータ
/// マスタ <see cref="DiceFace"/> から Deep Copy して生成し、ラン中に自由に改造できます
/// セーブ時には JsonUtility でそのまま直列化できます
/// </summary>
[Serializable]
public class RuntimeDiceFace
{
    // ─── 由来となったマスタ面の ID（参照用、表示用） ────

    [SerializeField]
    [Tooltip("コピー元となった DiceFace の PersistentId（GUID 文字列）\n" +
             "改造後も 元はどの面か を表示・トレースするために保持します")]
    private string m_sourceFaceId;

    // ─── 面の名前 ────────────────────────────────────────

    [SerializeField]
    [Tooltip("デバッグ用の識別名")]
    private string m_faceName;

    // ─── 効果リスト ──────────────────────────────────────

    [SerializeField]
    [Tooltip("この面が出たときに上から順に実行されるアクション一覧")]
    private List<RuntimeActionEntry> m_actions = new List<RuntimeActionEntry>();

    // ─── 公開プロパティ ──────────────────────────────────

    /// <summary>由来となったマスタ <see cref="DiceFace"/> の PersistentId</summary>
    public string SourceFaceId => m_sourceFaceId;

    /// <summary>面の識別名</summary>
    public string FaceName => m_faceName;

    /// <summary>効果エントリ一覧（読み取り専用）</summary>
    public IReadOnlyList<RuntimeActionEntry> Actions => m_actions;

    // ─── コンストラクタ ──────────────────────────────────

    // JsonUtility 用の引数なしコンストラクタ
    public RuntimeDiceFace() { }

    /// <summary>
    /// マスタ <see cref="DiceFace"/> から Deep Copy で生成します
    /// </summary>
    public static RuntimeDiceFace CreateFrom(DiceFace masterFace)
    {
        if (masterFace == null)
        {
            Debug.LogWarning("[RuntimeDiceFace] masterFace が null です。空の面を返します。");
            return new RuntimeDiceFace();
        }

        RuntimeDiceFace face = new RuntimeDiceFace
        {
            m_sourceFaceId = masterFace.PersistentId,
            m_faceName = masterFace.FaceName,
        };

        foreach (ActionEntry entry in masterFace.Actions)
        {
            string actionId = entry.Action != null ? entry.Action.PersistentId : "";
            face.m_actions.Add(new RuntimeActionEntry(actionId, entry.Target, entry.Value));
        }

        return face;
    }

    // ─── 改造 API（RuntimeDice からのみ使われる想定） ───

    /// <summary>エントリを末尾に追加します</summary>
    internal void AddEntry(RuntimeActionEntry entry)
    {
        if (entry == null) return;
        m_actions.Add(entry);
    }

    /// <summary>指定インデックスのエントリを削除します</summary>
    internal bool RemoveEntryAt(int index)
    {
        if (index < 0 || index >= m_actions.Count) return false;
        m_actions.RemoveAt(index);
        return true;
    }

    /// <summary>指定インデックスのエントリを返します（範囲外なら null）</summary>
    internal RuntimeActionEntry GetEntryAt(int index)
    {
        if (index < 0 || index >= m_actions.Count) return null;
        return m_actions[index];
    }

    /// <summary>面の名前を上書きします（ReplaceFace 用）</summary>
    internal void SetFaceName(string name)
    {
        m_faceName = name;
    }

    /// <summary>由来 ID を上書きします（ReplaceFace 用）</summary>
    internal void SetSourceFaceId(string id)
    {
        m_sourceFaceId = id;
    }

    /// <summary>エントリリストをすべて入れ替えます（ReplaceFace 用）</summary>
    internal void ReplaceAllEntries(List<RuntimeActionEntry> newEntries)
    {
        m_actions = newEntries ?? new List<RuntimeActionEntry>();
    }
}
