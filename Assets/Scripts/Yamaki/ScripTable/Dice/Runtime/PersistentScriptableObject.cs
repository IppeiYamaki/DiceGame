using System;
using UnityEngine;

/// <summary>
/// 永続 ID（GUID 文字列）を保持する ScriptableObject 基底クラス
/// セーブ/ロード時に "SO 参照" を直接シリアライズせず、ID 経由で復元するために使用します
///
/// 既存 SO（DiceDefinition / DiceFace / DiceAction 等）の基底クラスをこの型に差し替えるだけで
/// 永続 ID を後付けできます（フィールド構成は変えないため Inspector 設定値は壊れません）
///
/// Editor 上では新規アセット作成 / 重複アセット時に自動で GUID を採番します
/// 実行時には何もしません（ID は事前にシリアライズ済みの値を読み出すだけ）
/// </summary>
public abstract class PersistentScriptableObject : ScriptableObject
{
    // ─── 永続 ID（GUID 文字列） ────────────────────────────

    [SerializeField]
    [Header("永続 ID（自動付与）")]
    [Tooltip("セーブ/ロード時にこの SO アセットを一意に識別するための GUID 文字列\n" +
             "Editor で空のときに自動採番されます。手で書き換えないでください。")]
    private string m_persistentId = "";

    // ─── 読み取り専用プロパティ ───────────────────────────

    /// <summary>このアセットの永続 ID（GUID 文字列）</summary>
    public string PersistentId => m_persistentId;

    /// <summary>
    /// 永続 ID が未設定なら新しく採番します
    /// 通常は Editor 経由で自動的に呼ばれるため、ランタイムから直接呼ぶ必要はありません
    /// </summary>
    public void EnsurePersistentId()
    {
        if (string.IsNullOrEmpty(m_persistentId))
        {
            m_persistentId = Guid.NewGuid().ToString("N");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

#if UNITY_EDITOR
    // Editor 上でアセットが読み込まれた / 値が編集されたタイミングで ID を確保します
    // ランタイムでは呼ばれません
    protected virtual void OnValidate()
    {
        EnsurePersistentId();
    }
#endif
}
