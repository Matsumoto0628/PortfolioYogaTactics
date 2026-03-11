# YogaTactics

ヨガを用いたゲーム依存症対策ゲーム

https://github.com/user-attachments/assets/211a3d63-69be-40ca-b7c1-639823bb431a

- **開発期間** : 9ヶ月
- **制作人数** : 6人
- **リリース先** : [itch.io](https://iput-alpha.itch.io/yoga-tactics)

### 動作環境

- **OS** : Windows 11、Mac OS
- **モーショントラッキング用デバイス** : Webカメラ

<br>
<br>

## 背景

世界保健機関（WHO）が国際疾病分類 ICD-11 にて「ゲーム障害」を正式に定義したように、ゲームのやりすぎは社会問題となっている。しかし「やめなさい」という言葉は子どもの反発を招きやすく、親子間のトラブルにもなりかねない。

この課題に対し、運動がゲーム依存症に伴う心理機能の改善に効果があるという研究に着目した。体力や筋力を問わず誰でも取り組めるヨガをゲームに組み合わせることで、遊びながら自然とゲーム時間をコントロールできる仕組みを作りたいと考え、本作を制作した。

<br>
<br>

## チーム内の役割・提案

ヨガのポーズをモチーフにしたキャラクターを登場させることを提案し、採用された。

敵キャラクターの実装を担当。多様なキャラクターに対応できるよう、ジェネリックFSMで状態管理の仕組みを共通化し、ScriptableObjectでパラメータを外部化することでプランナーが調整しやすい設計にした。また、デザイナー・プランナーと仕様・実現性について定期的に話し合い、実装に反映した。

<br>
<br>

## 使用技術

- **言語** : C#
- **IDE** : Visual Studio Code
- **ゲームエンジン** : Unity
- **ライブラリ** : MediaPipe, MediaPipeUnityPlugin

<br>

### 選定理由

#### MediaPipe

| ライブラリ | 特徴 |
|------------|------|
| **MediaPipe** | Googleが開発したクロスプラットフォームのMLパイプラインライブラリ。ポーズ推定をリアルタイムで処理できる |
| OpenPose | 精度は高いがモデルが重く、セットアップが複雑 |
| MoveNet | 非常に軽量だが、精度がやや低く細かい姿勢の検出に不向き |

ヨガのポーズをリアルタイムで認識するにあたり、精度・速度・導入のしやすさのバランスが必要だった。
MediaPipeはWebカメラの映像から33点の骨格ランドマークを高精度かつ低遅延で取得でき、MediaPipe Unity Pluginを通じてC#から利用できるため選定した。

<br>
<br>

## 技術的な工夫

### ジェネリックFSMによる敵AIの状態管理

`FSM<EState, TPawn>` というジェネリッククラスを定義し、状態の型とPawnの型を任意に指定できる設計にした。KnightやButterflyなど異なるキャラクターがそれぞれのFSMを独立して実装できる。

https://github.com/Matsumoto0628/PortfolioYogaTactics/blob/2ab6a09f18c1dca863480223adabb0558d60e350/Scripts/Pawn/FSM.cs#L4-L10

### ScriptableObjectによるパラメータの外部化

体力・攻撃力・移動速度などの基本パラメータをScriptableObjectで管理し、コードを変更せずにエディタ上でバランス調整できるようにした。`CannonSO`が`PawnSO`を継承する形で、キャラクター固有の属性を拡張している。

https://github.com/Matsumoto0628/PortfolioYogaTactics/blob/0a7204e0031bfe9ab63ff72ef75787faa0351ac1/Scripts/Pawn/PawnSO.cs#L4-L10

https://github.com/Matsumoto0628/PortfolioYogaTactics/blob/a13b481b4137ba0b137d6004070073e0748793b3/Scripts/Pawn/CannonSO.cs#L4-L8

### Modifierパターンによるステータス変更

`IStatusModifierOnce`インターフェース経由でステータスの変更を適用する設計にした。変更時に新しい`PawnStatus`インスタンスを生成するアプローチをとることで、副作用を最小化している。

https://github.com/Matsumoto0628/PortfolioYogaTactics/blob/7890ed10b36f19b3aa12415c6b19235a579706f8/Scripts/Pawn/Modifier/IStatusModifierOnce.cs#L1-L4

https://github.com/Matsumoto0628/PortfolioYogaTactics/blob/0fe66025f83859c3ae44b8bde0513ddb17283df1/Scripts/Pawn/Modifier/BonusBoost.cs#L1-L16

### オブジェクトプールによるGC負荷の抑制

エフェクト生成時のGCアロケーションを抑えるため、Stackベースのオブジェクトプールを実装した。`Poolable`抽象クラスで`OnGet()`・`OnReturn()`のライフサイクルを統一し、アニメーション終了時に自動でプールへ返却される仕組みにした。事前に一定数のインスタンスを生成するため初期化コストが増加するトレードオフがあるが、エフェクトが頻繁に発生する戦闘中のフレームレート安定性を優先した。

https://github.com/Matsumoto0628/PortfolioYogaTactics/blob/492a872f7543b793f26cd48ae21770940266ba25/Scripts/ObjectPool/Poolable.cs#L3-L8

<br>
<br>

## インストール方法
1. [itch.io](https://iput-alpha.itch.io/yoga-tactics)のページから `YogaTactics_Win.zip` をダウンロード
2. ZIPを展開する
3. `YogaTactics.exe` を起動する
