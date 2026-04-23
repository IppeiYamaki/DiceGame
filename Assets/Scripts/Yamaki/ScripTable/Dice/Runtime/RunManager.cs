using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 1 ラン中のセーブデータの直列化用コンテナ
/// JsonUtility は Dictionary や Guid を直接扱えないため、List + 文字列で表現します
/// </summary>
[Serializable]
public class RunSaveData
{
    [SerializeField]
    [Tooltip("セーブデータのフォーマットバージョン（互換性チェック用）")]
    private int m_version = 1;

    [SerializeField]
    [Tooltip("ラン中保持されている全 RuntimeDice")]
    private List<RuntimeDice> m_runtimeDice = new List<RuntimeDice>();

    /// <summary>セーブデータのフォーマットバージョン</summary>
    public int Version => m_version;

    /// <summary>セーブされた RuntimeDice 一覧（読み取り専用）</summary>
    public IReadOnlyList<RuntimeDice> RuntimeDice => m_runtimeDice;

    public RunSaveData() { }

    /// <summary>
    /// 既存の RuntimeDice 一覧からセーブデータを構築します
    /// </summary>
    public RunSaveData(IList<RuntimeDice> dice)
    {
        if (dice == null) return;
        foreach (RuntimeDice d in dice)
        {
            if (d == null) continue;
            m_runtimeDice.Add(d);
        }
    }
}

/// <summary>
/// 1 ラン分の <see cref="RuntimeDice"/> を保持・セーブ/ロードするマネージャ
///
/// 想定フロー:
///   1. シーン起動時に <see cref="DiceMasterRegistry.SetActive(DiceMasterRegistry)"/> を呼ぶ
///   2. ラン開始時に <see cref="StartRun(IList{DiceDefinition})"/> でマスタから RuntimeDice を生成
///   3. ラン中にイベントから RuntimeDice の改造 API を呼ぶ
///   4. 任意のタイミングで <see cref="SaveRun"/> / <see cref="LoadRun"/>
///   5. クリア / ゲームオーバー時に <see cref="EndRun"/> で破棄
/// </summary>
public class RunManager : MonoBehaviour
{
    // ─── マスタ参照 ──────────────────────────────────────

    [SerializeField]
    [Header("マスタレジストリ")]
    [Tooltip("ID → SO の解決に使う DiceMasterRegistry。\n" +
             "Awake 時に DiceMasterRegistry.SetActive にも登録されます。")]
    private DiceMasterRegistry m_registry;

    [SerializeField]
    [Header("セーブファイル名")]
    [Tooltip("Application.persistentDataPath 配下に作成するセーブファイル名")]
    private string m_saveFileName = "dicegame_run.json";

    // ─── ラン中データ ────────────────────────────────────

    private readonly List<RuntimeDice> m_runtimeDice = new List<RuntimeDice>();

    private bool m_isRunActive;

    // ─── 公開プロパティ ──────────────────────────────────

    /// <summary>ラン進行中かどうか</summary>
    public bool IsRunActive => m_isRunActive;

    /// <summary>現在のラン中 RuntimeDice 一覧（読み取り専用）</summary>
    public IReadOnlyList<RuntimeDice> RuntimeDice => m_runtimeDice;

    /// <summary>使用中のレジストリ</summary>
    public DiceMasterRegistry Registry => m_registry;

    // ─── Unity ライフサイクル ────────────────────────────

    private void Awake()
    {
        if (m_registry != null)
        {
            DiceMasterRegistry.SetActive(m_registry);
        }
    }

    // ─── ラン制御 ────────────────────────────────────────

    /// <summary>
    /// マスタ <see cref="DiceDefinition"/> 群から RuntimeDice を生成してランを開始します
    /// 既にラン中の場合はいったん <see cref="EndRun"/> してから開始します
    /// </summary>
    public void StartRun(IList<DiceDefinition> masterDice)
    {
        if (m_isRunActive)
        {
            Debug.LogWarning("[RunManager] 既にラン中です。一度 EndRun してから開始します。");
            EndRun();
        }

        m_runtimeDice.Clear();

        if (masterDice != null)
        {
            foreach (DiceDefinition master in masterDice)
            {
                RuntimeDice dice = RuntimeDiceFactory.CreateFrom(master);
                if (dice != null)
                {
                    m_runtimeDice.Add(dice);
                }
            }
        }

        m_isRunActive = true;
    }

    /// <summary>
    /// ラン中の RuntimeDice をすべて破棄して初期状態に戻します
    /// クリア / ゲームオーバー時に呼んでください
    /// </summary>
    public void EndRun()
    {
        m_runtimeDice.Clear();
        m_isRunActive = false;
    }

    /// <summary>
    /// 指定インデックスの RuntimeDice を取得します（範囲外なら null）
    /// </summary>
    public RuntimeDice GetDice(int index)
    {
        if (index < 0 || index >= m_runtimeDice.Count) return null;
        return m_runtimeDice[index];
    }

    // ─── セーブ / ロード ────────────────────────────────

    /// <summary>
    /// 現在のラン状態を JSON にシリアライズしてファイルに保存します
    /// </summary>
    public bool SaveRun()
    {
        try
        {
            RunSaveData data = new RunSaveData(m_runtimeDice);
            string json = JsonUtility.ToJson(data, prettyPrint: true);

            string path = GetSavePath();
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[RunManager] SaveRun に失敗しました: {e}");
            return false;
        }
    }

    /// <summary>
    /// セーブファイルから RuntimeDice を復元します
    /// 成功すると現在のラン状態は上書きされ、ラン中状態になります
    /// </summary>
    public bool LoadRun()
    {
        try
        {
            string path = GetSavePath();
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[RunManager] セーブファイルが見つかりません: {path}");
                return false;
            }

            string json = File.ReadAllText(path);
            RunSaveData data = JsonUtility.FromJson<RunSaveData>(json);
            if (data == null)
            {
                Debug.LogWarning("[RunManager] セーブデータのパースに失敗しました。");
                return false;
            }

            m_runtimeDice.Clear();
            foreach (RuntimeDice dice in data.RuntimeDice)
            {
                if (dice == null) continue;
                m_runtimeDice.Add(dice);
            }

            m_isRunActive = true;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[RunManager] LoadRun に失敗しました: {e}");
            return false;
        }
    }

    /// <summary>
    /// セーブファイルが存在すれば削除します
    /// </summary>
    public bool DeleteSave()
    {
        try
        {
            string path = GetSavePath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[RunManager] DeleteSave に失敗しました: {e}");
            return false;
        }
    }

    /// <summary>
    /// セーブファイルの絶対パスを返します
    /// </summary>
    public string GetSavePath()
    {
        string fileName = string.IsNullOrEmpty(m_saveFileName) ? "dicegame_run.json" : m_saveFileName;
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}
