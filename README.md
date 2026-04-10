## 節約！軍師ちゃん

https://github.com/user-attachments/assets/b77cc9dd-4892-4e4d-a447-dc5c99e2f47b

<img width="1238" height="697" alt="image" src="https://github.com/user-attachments/assets/8e2e5116-ecdf-49f1-b07c-3c60ea294c40" />

<img width="1238" height="694" alt="image" src="https://github.com/user-attachments/assets/a346bf17-b00b-4e6d-8088-4b26552e1cc0" />


### ゲームについて
#### URL
https://unityroom.com/games/260303_td_gunshi

#### 操作方法
本作は編集モード、戦闘モードを切り替えて遊ぶゲームです。

```
【編集モード中】
[E]キー : 配置切り替え
[D]キー : 削除切り替え
[ESC]キー : 戦闘モードへ

・配置時
- 左クリック: 選択, マップ配置
- 右クリック: 回転
- マウスホイール: ズーム

・削除時
- 左クリック: ユニットの削除

【戦闘モード中】
[ESC]キー : 編集モードへ

```
ユニットと敵それぞれに特徴があるので、特徴を活かした配置取りを考えてみましょう。
また、お金を使うほど強力なユニットを配置できますが、軍師ちゃんはなるべく黒字を目指したいようです。

余裕があれば、所持金をなるべく多く残してクリアを目指してみましょう！

#### ゲームの説明
本作はタワーディフェンスゲームになります。

敵を通すとライフが1つずつ減り、なくなるとゲームオーバーです。

ユニットを配置していき、敵の進行を食い止めましょう。

### 開発環境等
* 製作時間：150時間ほど
* Unity Ver：6000.0.65f1
* ジャンル：3D タワーディフェンス
* 作業範囲：ゲームデザイン / プログラム / UIなど

### 作成の目的
以下の理解を目的としました。

* Unityにおける3Dゲーム開発の基礎
* 3Dモデルに使われる用語
* グリッドベースで作られているゲームシステムの設計
* ゲームで使われる数学範囲の把握
* ライトユーザーがクリアできる難易度を保ちつつ、上級者も楽しめるようなゲーム設計の考え方

### 考慮した部分など
#### GridSystem
DictionaryとHashSetを用いたグリッドシステムを実装して、ユニットの配置やマスの占有状態の管理がどのように行われているかを確認した
* https://github.com/skonishi1125/unity_3d_tower_defense/blob/main/Assets/Script/GridSystem.cs
* https://github.com/skonishi1125/unity_3d_tower_defense/blob/main/Assets/Script/GridCellOccupier.cs

#### ユニットの機能拡張
オブジェクト指向をベースとしたクラス設計を意識して、オーバーライドで機能拡張ができるようにした
* https://github.com/skonishi1125/unity_3d_tower_defense/blob/main/Assets/Script/Unit/Unit.cs
  * https://github.com/skonishi1125/unity_3d_tower_defense/blob/main/Assets/Script/Unit/UnitCombat.cs
  * https://github.com/skonishi1125/unity_3d_tower_defense/blob/main/Assets/Script/Unit/Weaker/WeakerCombat.cs

### お借りした素材など

```
【3Dモデル】鉄アレイ　ダンベル
https://booth.pm/en/items/4929143
敵

A downloadable SFX Pack
https://jdsherbert.itch.io/pixel-ui-sfx-pack
SFX関連

KayKit : Mini-Game Variety Pack (1.2)
https://kaylousberg.itch.io/kay-kit-mini-game-variety-pack
ユニット、背景

Low Poly Castle Pack
https://itch.io/queue/c/3439372/low-poly-building?game_id=2100416
城モデル

OtoLogic
https://otologic.jp/

Parallax Skybox Shader 2D for Unity
https://dev-anim3.itch.io/parallax-skybox-shader-2d-for-unity
Skybox

simple sci-fi tower defense asset pack
https://trockk.itch.io/simple-sci-fi-tower-defense-asset-pack

Snail Model
https://rkuhlf-assets.itch.io/snail
ユニット：つむりん

Tower Defense Kit
https://kenney-assets.itch.io/tower-defense-kit
ユニット、背景

くらげ工匠
http://www.kurage-kosho.info/

【フリーBGM】我が家の日常
https://www.youtube.com/watch?v=BBi2_Ky_11s

効果音ラボ
https://soundeffect-lab.info/

効果音工房
https://umipla.com/
```
