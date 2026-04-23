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

## ディレクトリ構成

```

Assets/
  Scenes/
    個人名（人数分）
  Prefabs/
    個人名（人数分）
  Scripts/
    個人名（人数分）

