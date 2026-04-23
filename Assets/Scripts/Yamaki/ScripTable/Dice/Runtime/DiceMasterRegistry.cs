using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マスタ SO（DiceDefinition / DiceFace / DiceAction）を永続 ID から実体参照へ解決するためのレジストリ
///
/// セーブデータには SO 参照ではなく PersistentId（GUID 文字列）を保存し、
/// ロード時にこのレジストリを介して実体 SO を引き当てます
///
/// 作成方法: Project ビュー右クリック →
///   Create > DiceGame > DiceMasterRegistry
///
/// 使い方: シーン起動時などに <see cref="SetActive(DiceMasterRegistry)"/> で
/// アクティブインスタンスを設定しておくと、RuntimeDiceFaceExecutor などから
/// <see cref="Active"/> 経由で参照できます
/// </summary>
[CreateAssetMenu(fileName = "DiceMasterRegistry", menuName = "DiceGame/DiceMasterRegistry")]
public class DiceMasterRegistry : ScriptableObject
{
    // ─── 登録用リスト（Inspector から設定） ───────────────

    [SerializeField]
    [Header("ダイス定義（マスタ）")]
    [Tooltip("セーブ/ロードで参照させたい DiceDefinition アセットを登録します")]
    private List<DiceDefinition> m_diceDefinitions = new List<DiceDefinition>();

    [SerializeField]
    [Header("ダイス面（マスタ）")]
    [Tooltip("セーブ/ロードで参照させたい DiceFace アセットを登録します\n" +
             "面の差し替え（ReplaceFace）で使う面はここに登録しておく必要があります")]
    private List<DiceFace> m_diceFaces = new List<DiceFace>();

    [SerializeField]
    [Header("アクション（マスタ）")]
    [Tooltip("セーブ/ロードで参照させたい DiceAction アセットを登録します\n" +
             "ランタイムから ID 経由で実行される全 DiceAction を登録してください")]
    private List<DiceAction> m_diceActions = new List<DiceAction>();

    // ─── ID → 実体 のキャッシュ（最初のアクセス時に構築） ─

    private Dictionary<string, DiceDefinition> m_definitionById;
    private Dictionary<string, DiceFace> m_faceById;
    private Dictionary<string, DiceAction> m_actionById;

    // ─── アクティブインスタンス（プロセス全体でひとつ） ─

    private static DiceMasterRegistry s_active;

    /// <summary>
    /// 現在アクティブなレジストリ
    /// シーン起動時などに <see cref="SetActive(DiceMasterRegistry)"/> で設定してください
    /// </summary>
    public static DiceMasterRegistry Active => s_active;

    /// <summary>
    /// アクティブなレジストリを設定します（null で解除）
    /// </summary>
    public static void SetActive(DiceMasterRegistry registry)
    {
        s_active = registry;
    }

    // ─── 解決 API ─────────────────────────────────────────

    /// <summary>永続 ID から DiceDefinition を取得します（見つからなければ null）</summary>
    public DiceDefinition ResolveDefinition(string persistentId)
    {
        EnsureCache();
        if (string.IsNullOrEmpty(persistentId)) return null;
        return m_definitionById.TryGetValue(persistentId, out DiceDefinition def) ? def : null;
    }

    /// <summary>永続 ID から DiceFace を取得します（見つからなければ null）</summary>
    public DiceFace ResolveFace(string persistentId)
    {
        EnsureCache();
        if (string.IsNullOrEmpty(persistentId)) return null;
        return m_faceById.TryGetValue(persistentId, out DiceFace face) ? face : null;
    }

    /// <summary>永続 ID から DiceAction を取得します（見つからなければ null）</summary>
    public DiceAction ResolveAction(string persistentId)
    {
        EnsureCache();
        if (string.IsNullOrEmpty(persistentId)) return null;
        return m_actionById.TryGetValue(persistentId, out DiceAction action) ? action : null;
    }

    /// <summary>
    /// キャッシュを強制再構築します
    /// 実行中に Inspector で登録内容を変更したときに呼んでください
    /// </summary>
    public void RebuildCache()
    {
        m_definitionById = null;
        m_faceById = null;
        m_actionById = null;
        EnsureCache();
    }

    // ─── 内部実装 ─────────────────────────────────────────

    // ID → 実体 の辞書をまだ作っていなければ構築する
    private void EnsureCache()
    {
        if (m_definitionById != null && m_faceById != null && m_actionById != null) return;

        m_definitionById = new Dictionary<string, DiceDefinition>();
        foreach (DiceDefinition def in m_diceDefinitions)
        {
            if (def == null) continue;
            string id = def.PersistentId;
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[DiceMasterRegistry] DiceDefinition '{def.name}' に PersistentId がありません。スキップします。");
                continue;
            }
            if (m_definitionById.TryGetValue(id, out DiceDefinition existing) && existing != def)
            {
                Debug.LogError($"[DiceMasterRegistry] DiceDefinition の PersistentId が重複しています: " +
                               $"'{existing.name}' と '{def.name}' (id={id})。後者を採用します。");
            }
            m_definitionById[id] = def;
        }

        m_faceById = new Dictionary<string, DiceFace>();
        foreach (DiceFace face in m_diceFaces)
        {
            if (face == null) continue;
            string id = face.PersistentId;
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[DiceMasterRegistry] DiceFace '{face.name}' に PersistentId がありません。スキップします。");
                continue;
            }
            if (m_faceById.TryGetValue(id, out DiceFace existingFace) && existingFace != face)
            {
                Debug.LogError($"[DiceMasterRegistry] DiceFace の PersistentId が重複しています: " +
                               $"'{existingFace.name}' と '{face.name}' (id={id})。後者を採用します。");
            }
            m_faceById[id] = face;
        }

        m_actionById = new Dictionary<string, DiceAction>();
        foreach (DiceAction action in m_diceActions)
        {
            if (action == null) continue;
            string id = action.PersistentId;
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[DiceMasterRegistry] DiceAction '{action.name}' に PersistentId がありません。スキップします。");
                continue;
            }
            if (m_actionById.TryGetValue(id, out DiceAction existingAction) && existingAction != action)
            {
                Debug.LogError($"[DiceMasterRegistry] DiceAction の PersistentId が重複しています: " +
                               $"'{existingAction.name}' と '{action.name}' (id={id})。後者を採用します。");
            }
            m_actionById[id] = action;
        }
    }
}
