# DiceGame-ToBeDetermined-
ケーススタディで作成するDiceGame（仮）のリポジトリ

---


## 開発環境

- Unity  
  - Version: **Unity 6.3 LTS (6000.3.13f1)**
 
---

## ブランチ運用ルール

### ブランチ構成

#### stable ブランチ
- **本番環境で動作している、またはリリース可能なコード**を管理するブランチ
- 常に動作保証されている状態を保つ
- **直接コミットは禁止**

#### develop（または main）ブランチ
- 日常開発の成果を集約するブランチ
- 動作確認が完了した機能がマージされる
- `stable` に反映する前の確認用ブランチ

#### feature ブランチ
- 機能追加・修正など、各自の作業用ブランチ
- `develop`（または `main`）から作成する
- 命名例：
  - `feature/dice-animation`

 ### 開発の流れ

1. `develop`（または `main`）から **feature ブランチを作成**
2. feature ブランチで作業
3. 作業完了後、**Pull Request を作成**
4. レビュー・動作確認後、`develop` にマージ
5. リリース可能な状態になったら `stable` にマージ

### Pull Request / コミットに関するルール

- **stable / develop / main への直接コミットは禁止**
- 必ず **Pull Request を作成**すること
- Pull Request には以下を記載すること
  - 変更内容の概要
  - 動作確認内容
- レビューを受けてからマージする

---


## コード編集ルール

### 命名規則

- **ファイル名/クラス/構造体/enum**: `UpperCamelCase` 　（例: `GraphicsDevice`, `SceneBase`）
- **ローカル変数/引数**: `lowerCamelCase` 　（例: `deltaTime`, `assetPath`）
- **メンバ変数**: `m_lowerCamelCase` （例: `m_device`, `m_speed`）
- **グローバル変数**: `g_lowerCamelCase` （例: `g_device`, `g_speed`）

---

## DiceFace ScriptableObject システム（Yamaki）

### 概要

ダイスの出目ごとの効果を ScriptableObject で編集できる仕組みです。
プランナーが Project ビューからアセットを作成し、Inspector で効果を自由に設定できます。

### 関連ファイル

```
Assets/Scripts/Yamaki/ScripTable/Dice/
  DiceFaceTarget.cs     … 対象を表す enum (Self / Enemy)
  ActionEntry.cs        … 1 つの効果エントリ（アクション・対象・基本値）
  DiceFace.cs           … 1 面分の効果を持つ ScriptableObject
  DiceDefinition.cs     … ダイス本体（複数の DiceFace を持つ）ScriptableObject
  DiceFaceExecutor.cs   … DiceFace の効果リストを順番に実行するユーティリティ
```

### アセットの作り方

#### 1. DiceFace アセットを作成する

Project ビュー右クリック → **Create > DiceGame > DiceFace**

Inspector で以下を設定します。

| フィールド | 説明 |
|---|---|
| Face Name | 面の識別名（例: `攻撃×3`）。省略可。 |
| Actions | 効果エントリの一覧。＋ボタンで追加。 |

#### 2. ActionEntry を追加する

Actions リストの要素ごとに以下を設定します。

| フィールド | 説明 |
|---|---|
| Action | 実行する DiceAction アセット（例: AttackAction / BlockAction） |
| Target | `Self`（使用者）または `Enemy`（敵対対象） |
| Value | DiceAction に渡す基本値（整数）。バフ等の最終計算は別途行う。 |

#### 3. DiceDefinition アセットを作成する

Project ビュー右クリック → **Create > DiceGame > DiceDefinition**

Inspector の **Faces** リストに作成した DiceFace アセットを登録します。
要素数がそのままダイスの面数になります（6 面に限りません）。

### 設定例

**「攻撃×3 面」を作る場合**

1. DiceFace アセット（`DiceFace_Attack3` など）を作成
2. Actions に以下を 3 つ追加:
   - Action = `AttackAction`  /  Target = `Enemy`  /  Value = `1`
3. この面が出たとき、`AttackAction.Execute` が 3 回呼ばれ 合計 3 ダメージ

**「攻撃×3（2 ダメ版）面」の場合**

Value を `2` にすると 1 エントリあたり 2 ダメージ → 3 エントリで 合計 6 ダメージ

### コードからの実行方法

```csharp
// DiceFace の全効果を順番に実行する
DiceFaceExecutor.Execute(diceFace, user, enemy);
```

- `diceFace` … 実行したい DiceFace アセット
- `user` … 使用者エンティティ（ICombatEntity）
- `enemy` … 敵対対象エンティティ（ICombatEntity）

各 ActionEntry の `Value` がそのまま `DiceAction.Execute(user, target, diceValue)` の
`diceValue` 引数（実態: 基本値）に渡されます。

---

## ディレクトリ構成

```

Assets/
  Scenes/
    個人名（人数分）
  Prefabs/
    個人名（人数分）
  Scripts/
    個人名（人数分）

