
# Ludo Project
## 🎲 Digital Board Game Implementation (Technical Detail)

伝統的なボードゲーム「Ludo」をUnityで再現したプロジェクトです。
Rigidbodyを用いた物理演算ダイス、Cinemachineによる動的なカメラスイッチング、そしてNodeベースの経路管理システムを統合し、実物に近いプレイ感覚のデジタル化を目指しています。

<img width="1919" height="1079" alt="スクリーンショット 2025-09-19 104944" src="https://github.com/user-attachments/assets/5caa841c-4b1f-4b62-bf21-a0e416dcbd96" />



## 🧠 Application Concept

| Feature | Description |
| :--- | :--- |
| **Logic-Based Pathfinding** | 盤面の各マスをNodeとして定義。全プレイヤー共通のルートと個別のゴールルートを動的に結合し、駒の進行を管理 |
| **Physics Dice System** | 物理演算によるダイスロールを実装。上面判定（DiceSide）により、プログラム的な乱数以上のリアリティを確保 |
| **Adaptive Camera** | プレイヤーのターンや状況に応じて、Cinemachineカメラを自動で切り替えるセレクターを搭載 |

## 🛠 Tech Stack Details

| Component | Technology |
| :--- | :--- |
| **Engine** | Unity |
| **Camera Control** | Unity Cinemachine (Virtual Camera System) |
| **Physics** | Rigidbody / BoxCollider (ダイスの物理挙動と衝突判定) |
| **UI Framework** | Unity UI (UGUI) / Legacy Text による情報表示 |

### Key Mechanics

| Mechanism | Description |
| :--- | :--- |
| **State Machine** | GameManagerによる「待機・ダイスロール・プレイヤー交代」の厳密なゲーム進行管理 |
| **Route Navigation** | Routeクラスがマスの座標リストを保持。Gizmosによるエディタ上での経路可視化も実装 |
| **Piece Interaction** | 駒（Stone）が重なった際のキック判定（ベースへの帰還）や、ゴール到達判定を自動化 |

## 🚀 Measurement Protocols (Game Sequence)

ゲームの実行サイクルと主要な制御フローは以下の通りです。

| Phase | Description | Key Script |
| :--- | :--- | :--- |
| **1. Initialize** | 設定に基づきプレイヤー（HUMAN/CPU）と駒を生成し、順番を決定 | StartGame.cs |
| **2. Dice Roll** | 物理ダイスを振り、静止後の上面判定により進むマス数を確定 | Dice.cs / DiceSide.cs |
| **3. Evaluation** | 駒がベースから出撃可能か（6の目）、または移動可能かを判定 | GameManager.cs |
| **4. Movement** | 経路リストに従い、弧を描くようなステップアニメーションを実行 | Stone.cs / Route.cs |
| **5. Outcome** | 敵駒の排除、および全員がゴールに入ったかどうかの勝利判定 | Stone.cs / GameOver.cs |

## 📊 Data Schema (Internal Logic)

| Class | Type | Description |
| :--- | :--- | :--- |
| **Entity** | Serializable Class | プレイヤー名、所持する駒、プレイヤータイプ（HUMAN/CPU）の保持 |
| **Node** | Component | 各マスの専有状態（isTaken）と、そこに存在する駒の参照を管理 |
| **Route List** | List<Transform> | 盤面上の座標データをインデックス順に保持する経路データ |

## ⚡ 使用方法

| Step | Action |
| :--- | :--- |
| **1** | `Application` フォルダ内の `Ludo.exe` を起動 |
| **2** | 各プレイヤーのタイプ（HUMANまたはCPU）を設定してスタート |
| **3** | 自分のターンで「Roll」ボタンを押し、ダイスを振る |
| **4** | 移動可能な駒が光る（selector）ので、クリックして移動を開始 |
| **5** | 全ての駒をゴールさせ、リザルト画面で順位を確認 |

---
「伝統をコードで再定義する」。物理的なダイスの挙動と、厳密なルールロジックを組み合わせることで、ボードゲームの面白さをエンジニアリングの視点から表現しています。

