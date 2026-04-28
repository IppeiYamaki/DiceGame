# RuntimeDice システム 使い方メモ

ローグライクのラン中にダイスを部分改造でき、かつセーブ/ロード可能な RuntimeDice システムです。
マスタとしての SO（`DiceDefinition` / `DiceFace` / `DiceAction`）は変更せず、ラン中はそれらを Deep Copy した
`RuntimeDice` を改造して使います。セーブデータは SO 参照を持たず、すべて **永続 ID（GUID 文字列）** で
直列化されます。

## 構成ファイル

`Assets/Scripts/Yamaki/ScripTable/Dice/Runtime/`

| ファイル | 役割 |
| --- | --- |
| `PersistentScriptableObject.cs` | 永続 ID（GUID）を持つ SO 基底クラス。`DiceDefinition` / `DiceFace` / `DiceAction` がこれを継承します |
| `DiceMasterRegistry.cs` | ID → SO 実体への解決テーブル（ScriptableObject） |
| `RuntimeActionEntry.cs` | ランタイムの 1 効果エントリ。`DiceAction` を ID で参照 |
| `RuntimeDiceFace.cs` | ランタイムの 1 面（`RuntimeActionEntry` のリスト） |
| `RuntimeDice.cs` | ランタイムの 1 ダイス + 改造 API |
| `RuntimeDiceFactory.cs` | マスタ `DiceDefinition` から `RuntimeDice` を Deep Copy 生成 |
| `RuntimeDiceFaceExecutor.cs` | `RuntimeDiceFace` の効果リストを実行（SO 版 `DiceFaceExecutor` の Runtime 版） |
| `RunManager.cs` | 1 ラン中の `RuntimeDice` 保持 + セーブ/ロード |

## 1. マスタ SO の作り方

通常通り、Project ビューで右クリックして作成します。

- `Create > DiceGame > DiceDefinition`
- `Create > DiceGame > DiceFace`
- `Create > Scriptable Objects/DiceActions/Attack` など

これらはすべて `PersistentScriptableObject` を継承しており、Editor 上で開いたタイミングで
`PersistentId` が自動採番されます。Inspector の「永続 ID（自動付与）」欄に GUID 文字列が
入っていることを確認してください（手で書き換えないでください）。

### マスタレジストリ

`Create > DiceGame > DiceMasterRegistry` で `DiceMasterRegistry` を 1 つ作り、
セーブ/ロードで使う可能性のある `DiceDefinition` / `DiceFace` / `DiceAction` をすべて登録します。

> 面の差し替え (`ReplaceFace`) や `AddEntry` で渡す可能性のある SO は、必ず登録しておいてください。

シーン起動時に `RunManager` の `m_registry` フィールドへ参照をセットしておけば、
`Awake()` で `DiceMasterRegistry.SetActive(...)` が呼ばれます。

## 2. ラン開始時に Runtime 生成

```csharp
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private RunManager m_runManager;
    [SerializeField] private List<DiceDefinition> m_starterDice;

    private void Start()
    {
        m_runManager.StartRun(m_starterDice);
    }
}
```

`StartRun` 内では、各 `DiceDefinition` について
`RuntimeDiceFactory.CreateFrom(master)` が呼ばれ、`RuntimeDice` が Deep Copy で生成されます。
マスタ SO は一切変更されません。

## 3. 改造 API 例

```csharp
RuntimeDice dice = m_runManager.GetDice(0);

// ある面を別のマスタ面で差し替える（イベント "面を入れ替え"）
dice.ReplaceFace(faceIndex: 2, newMasterFace: replacementFace);

// あるエントリの基本値を +1（イベント "攻撃力アップ"）
dice.IncreaseEntryValue(faceIndex: 0, entryIndex: 0, delta: +1);

// 既存の面に新しい効果を追加（イベント "面に追加効果"）
dice.AddEntry(faceIndex: 1, action: blockAction, target: DiceFaceTarget.Self, value: 3);

// 効果を削除（イベント "余計な効果を削る"）
dice.RemoveEntry(faceIndex: 1, entryIndex: 0);
```

## 4. 実行

SO 版 `DiceFaceExecutor.Execute(face, user, enemy)` と同じ感覚で、
ランタイム版を呼び出します。

```csharp
RuntimeDice dice = m_runManager.GetDice(0);
RuntimeDiceFace rolled = dice.GetFace(rolledIndex);

RuntimeDiceFaceExecutor.Execute(rolled, player, enemy);
```

`RuntimeActionEntry` は `DiceAction` を ID で持っているので、内部で
`DiceMasterRegistry.Active.ResolveAction(id)` を使って実体に変換してから
`DiceAction.Execute(user, target, entry.Value)` を呼びます。

> バフ等込みの最終ダメージ計算は別レイヤーの責務で、ここでは "基本値" をそのまま渡します。

## 5. セーブ / ロードの流れ

`RunManager` がそのまま窓口です。

```csharp
// セーブ（Application.persistentDataPath/dicegame_run.json）
m_runManager.SaveRun();

// ロード（成功すると現在のランは上書きされ、IsRunActive = true になる）
m_runManager.LoadRun();

// クリア / ゲームオーバー時
m_runManager.EndRun();      // RuntimeDice を破棄して初期化
m_runManager.DeleteSave();  // 必要に応じてセーブファイルも削除
```

セーブデータは `JsonUtility` で `RunSaveData`（`List<RuntimeDice>`）を直列化したものです。
SO 参照は **一切持たず**、`DiceDefinition` / `DiceFace` / `DiceAction` はすべて GUID 文字列で参照します。
ロード時には `DiceMasterRegistry` 経由で実体 SO に解決されるため、

- マスタ SO の中身（数値・効果リスト）は変えても OK（ID が同じなら参照は維持されます）
- マスタ SO のアセットを **削除 / ID 変更** すると解決できなくなります

ことに注意してください。

## 補足: ID の付与 / 後付けについて

- 既存 SO は基底クラスを `ScriptableObject` → `PersistentScriptableObject` に差し替えるだけで対応済みです
  （フィールド構成は不変なので Inspector 設定値は壊れません）
- ID は Editor の `OnValidate()` で自動採番されます（`#if UNITY_EDITOR` で分離済み）
- 既存アセットは Editor で一度開く / 触ると採番されます。Project ビューですべて選択 → Inspector で
  値を変更（戻す）すると一括採番できます
