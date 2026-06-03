# CommandGame
2Dのターン性コマンドバトルをUnityで制作しました。<br> このゲームではプレイヤーが戦略を練りつつ、敵を倒していくゲームです。

### 【目次】
1.ファイル構成<br>
2.基本情報<br>
3.ゲームコンセプト、技術コンセプト<br>
4.ゲーム概要<br>
5.実装詳細<br>
5.技術と工夫と苦労<br>


## 【プロジェクト構成】
```
- CommandGame_UnityProject:Unityで動作するファイル<br>Unityhubでこのファイルを開いてもらうと、Unity内のデータを確認することができます。

- Scriptファイル:ソースコードがすべて入ったファイル
    ├── Battle：バトルシステムのソースコードが入ってます
|
|       BGMScript:BGMの処理が書かれたソースコードが入っています。
|
|       ButtonScript:ボタン操作、UIのソースコードが入っています。
|
|       EnemyScript:敵のステータスなどが書かれたソースコードが入っています。
|
|       Json：JsonでUI表示処理などが書かれたソースコードが入っています。
|
|       PlayerScript：プレイヤーのステータスなどが入っています。
|
|       SaveSystem:セーブ＆ロード処理が書かれたソースコードが入っています。
|
|       SEScript：効果音の処理が書かれたソースコードが入っています。
|
|       Stage：ステージセレクト画面のソースコードが入っています。
|
|       UIScript:UI関係がまとめたソースコードが入っています。
|  
- README
```


## 【ゲームコンセプト】<br>
**～プレイヤーが戦略を練りつつ、敵を撃破するリソース型コマンドバトル～**

## 【技術コンセプト】
- ** UniTaskとCoroutineによる非同期制御でターン性コマンドバトルを実現。**
- **継承＋SO（スクリプタブルオブジェクト）、JSONを用いて、ステータス管理を汎用化**
---------------------------------------------------------------------------------------------------------------------------------------

# 【ゲームについて】
  
## 【バトルの流れ】
バトルはターン性でプレイヤーと敵のターンが交互に進行していきます。<br>

*プレイヤーターン*
- 左から順番に、各キャラクターが行動を選択
- スキル・必殺技・回復・バフを使い分けて敵を攻略
- 攻撃時は対象を自分で選択可能

*エネミーターン*
- 敵はランダムに行動し、協力な全体攻撃や状態異常を使う場合もある。

## 【ゲームの操作方法】
プレイヤーは通常攻撃と、スキルと必殺を使って戦います。<br>

*キー割り当て*
- Aキー:敵に通常攻撃でダメージを与えます。
- Sキー:スキルを発動できます。ステージによっては使用制限があることも。
- Fキー：必殺技を使用時に押すことができます。しかし、制限があるため、使用は計画的に。
<img width="600" height="290" alt="image" src="https://github.com/user-attachments/assets/e3bf887d-328c-4837-a836-86fe3aeb9b9f" />

---------------------------------------------------------------------------------------------------------------------------------------

*状態確認ボタン*<br>
キャラクターの状態を確認することができます。<br>
<br>
<img width="267" height="92" alt="image" src="https://github.com/user-attachments/assets/b5f76519-8966-429d-95f8-a3f736d624a0" /><br>
****************************************************************************************************************************************
## 【プレイヤーキャラ紹介：スキル・必殺技紹介】

<img width="1600" height="725" alt="image" src="https://github.com/user-attachments/assets/035f12b6-3509-47d6-8079-54d9eb784de3" /><br>

## 【敵キャラ紹介】
<img width="1600" height="814" alt="image" src="https://github.com/user-attachments/assets/1b81031a-2cd1-4bc1-934a-db14ce8abc85" /><br>

<img width="1600" height="725" alt="image" src="https://github.com/user-attachments/assets/eba7fe03-d7c6-49e1-8476-a80e6c925817" /> <br>

-----------------------------------------------------------------------------------------------------------------------------------------
# 【プログラムについて】

### *挑戦(チャレンジ)*　<br>
- UniTask＋Corutineを導入し、バトル中の処理を非同期化<br>

-　継承＋ScriptableOnject＋JSONを使い、ステータス管理を汎用化

### *成果*

- 非同期処理を導入をすることで、ターン性を実現し、シームレスなゲーム体験に成功しました。
  ※プレイヤーの操作時のみ、処理が止まりますが、その他は中断なくゲームを進行され、よりプレイヤーターン時に戦略性のゲーム体験になっています。<br>

- 開発効率があがり、新キャラクターの追加時に継承とＳＯでパラメータの編集が簡単になりました。

## 【バトルについて】

- ターン性コマンドバトルには没入感と戦略性が必要と考えています。<br>
  そこで中断のないシームレスなゲーム体験を実現するために、UniTaskとCoroutineを用いて、実装しました。<br>

- UI表示とエフェクトの発生時間をフレームや秒数で管理しています。<br>

```C#
//UIマネージャーからプレイヤーターン表示
UIManager.Instance.PlayerTurnUI.SetActive(true);

//1フレーム待つ（キャンセルトークンが呼ばれたらキャンセル）
await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

//UIマネージャーからプレイヤーターン非表示
UIManager.Instance.PlayerTurnUI.SetActive(false);

//UIマネージャーからプレイヤーターン時に表示するUIを表示
UIManager.Instance.StartPlayerTurnUI();
```


- プレイヤーターンUIの表示をUniTaskを用いて数秒後にUIが非表示になるよう実装しています<br>

<img width="600" height="325" alt="Video Project" src="https://github.com/user-attachments/assets/051efb73-ca2b-4c61-a91a-b30923d456c0" /><br>

## 【キャラクターステータス:再利用×拡張性】<br>

### 継承

- 再利用性向上：共通ロジックを親クラスで実装

-　拡張も簡単：新たな機能を子クラスで追加

-細かなキャラの差別化：例えば支援A/強化Bなど、特定の役所向けのクラスを複数定義可能にしました。

<img width="549" height="400" alt="image" src="https://github.com/user-attachments/assets/4df0cc5c-ca01-4b4d-b959-c98d3e725845" />

```C#
public abstract class BasePlayerStatus : MonoBehaviour
{
/// <summary>
    /// プレイヤーのダメージメソッド
    /// </summary>
    /// <param name="damage">敵の攻撃力をダメージにする</param>
    public abstract void PlayerOnDamage(int damage);

    /// <summary>
    /// プレイヤーの通常攻撃メソッド
    /// </summary>
    public abstract void NormalAttack();

    /// <summary>
    /// プレイヤーのスキルメソッド
    /// </summary>
    public abstract void PlayerSkill();

    /// <summary>
    /// プレイヤーの必殺技メソッド
    /// </summary>
    public abstract void SpecialSkill();

    /// <summary>
    /// スクリプタブルオブジェクトからパラメータを設定するメソッド
    /// </summary>
    protected abstract void SetPlayerParameters();
}
```

↑共通の処理を親クラスで実装

```C#
public class Buffer : BasePlayerStatus
{
/// <summary>
    /// バフ実行メソッド
    /// </summary>
    /// <param name="target">プレイヤーを取得する</param>
    public void OnBuff(BasePlayerStatus target)
    {
        //バフ対象を選択するウィンドウを非表示
        bufferTargetWindow.HideBuffTargetWindow();

        Debug.LogWarning(target.PlayerID + "の攻撃力" + target.AttackPower);

        //バフスキル効果音再生
        PlayerSE.Instance.Play_BufferSkillSE();

        //バッファーのスキルエフェクトを生成
        GameObject effectInstance = Instantiate(bufferSkillEffect, target.transform.position,Quaternion.identity);

        //バフ力をターゲットの攻撃力を加算
        target.AttackPower += BuffPower;

        Debug.LogWarning($"{target.PlayerID} バフ後の攻撃力: {target.AttackPower} (InstanceID: {target.GetInstanceID()})");

        //エフェクトを消去
        Destroy(effectInstance, 3f);

        //バッファーが行動したのでtrueに
        IsBufferAction = true;

        IsPlayerAction = true;

        //バッファーのターンが終了したらスキル制限と必殺制限カウントのUIを非表示
        if (IsBufferAction)
        {
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);
        }

        //ターゲットのバフしたかのフラグをtrueにする
        target.HasBuff = true;
    }
}
```
↑子クラス側で、役職向けの処理を実装と、共通処理継承して再利用するだけで実装を効率化しました。<br>

## 【Scriptable Objectによるパラメータ管理の汎用化】

**～アセットの汎用化×外部連携でメンテナンス作業の負荷を削減～** 

パラメータなどのデータをAsset化、IDを呼び出すことで、ロードできるよう実装しました。

```C#
/// <summary>
/// プレイヤーのデータベースで利用するパラメータ
/// </summary>
[Serializable]
public class PlayerParameters
{
    [Tooltip("プレイヤーの名前データ")]
    public string PlayerNameData;

    [Tooltip("プレイヤーの名前の最大体力のデータ")]
    public int PlayerMaxHPData;

    [Tooltip("プレイヤー攻撃力データ")]
    public int PlayerAttackPowerData;

    [Tooltip("プレイヤーのバフパワーデータ")]
    public int BuffPowerData;

    [Tooltip("プレイヤーの回復力データ")]
    public int HealPowerData;
}
``` 
```C#
/// <summary>
/// プレイヤーのスクリタブルオブジェクトを作成
/// </summary>
[CreateAssetMenu(fileName ="PlayerDataBase",menuName = "ScriptableObject/PlayerDataBase")]
public class PlayerDataBase : ScriptableObject
{
    //プレイヤーのパラメータのリスト
    public List<PlayerParameters> PlayerParameters = new List<PlayerParameters>();
}
```
#### *↑プレイヤーのパラメータをリスト化*

<img width="786" height="474" alt="image" src="https://github.com/user-attachments/assets/31dae8f0-5747-4499-9d7d-4549703e1cde" /><br>
#### *↑Unity内で編集可能に*<br>

#### *実行時にSOからIDを呼び出してパラメータをロードできる形で実装*

```C#
protected override void SetPlayerParameters()
{
    // LINQを使い、プレイヤーデータベースからPlayerIDと一致するプレイヤー情報を取得
    var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);

    // 一致するプレイヤー情報が見つかった場合、パラメータを設定
    if (playerData != null)
    {

        //最大体力のデータを読み込み
        PlayerMaxHP = playerData.PlayerMaxHPData;

        //アタッカーの現在のHPを最大に設定してHPバーも最大に設定
        PlayerCurrentHP = PlayerMaxHP;

        Debug.Log($"PlayerCuurentHP:{PlayerCurrentHP},PlayerMaxHP:{PlayerMaxHP}");
        
        PlayerHPBar.maxValue = PlayerCurrentHP;
        PlayerHPBar.value = PlayerCurrentHP;
        PlayerHPBar.minValue = 0;

        //攻撃力をアタッカーのデータの攻撃力を読み込む
        AttackPower = playerData.PlayerAttackPowerData;

        //初期攻撃力もアタッカーの攻撃力に設定
        PlayerResetAttackPower = AttackPower;

        //生存状態にする
        IsAlive = true;
    }
}
```

JSONによるテキスト外部化
<img width="637" height="361" alt="image" src="https://github.com/user-attachments/assets/c938395b-83a6-4c8d-aeb9-feae98493ae3" />

バトル状況などのテキスト情報をJSONで一括管理し、IDで呼び出し、C#に変換し表示できるよう実装しました。

<img width="698" height="199" alt="image" src="https://github.com/user-attachments/assets/d850b8bd-b70d-4819-b60f-19d325d157eb" />

```JSON
{
    "BattleTexts":[
        {
            "id": "AttackerOnDebuff",
            "text": "アタッカーにデバフが付与された！"
        },
        {
            "id": "BufferOnDebuff",
            "text": "バッファーにデバフが付与された！"
        }
    ]
}
```
↑JSONで書かれたテキストデータをゲームシナリオのスクリプトのように作成

```C#
/// <summary>
    /// バトル中に状況をテキストで表示するメソッド
    /// </summary>
    /// <param name="id">表示したいテキストのJSONID</param>
    public void ShowBattleActionText(string id)
    {
        //JSONから読み込んだデータをいれるリストからidを取得
        BattleText battleText = battleTextConvertedList.Find(text => text.id == id);

        if (battleText != null)
        {
            //IDをもとにテキストを表示しテキストウィンドウも表示
            characterBattleActionText.text = battleText.text;
            textWindow.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"指定された ID ({id}) に対応するテキストが見つかりません");
        }
    }
```
```C#
//アタッカーのターン開始通知表示
BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");
```
#### *↑状況に応じてBattlesystem側でIDで呼び出すことで、表示できるよう実装しています。* <br>

JSONの変換のスクリプトも作成
```C#
public class JsonLoader : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ロードしたいJsonファイルのアドレス")]
    private string jsonLoadAddress;

    public IEnumerator LoadJsonText(Action<string> onSuccess)
    {
        // 指定されたアドレスキーからTextAssetを非同期でロードする
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(jsonLoadAddress);

        //読み込みが完了するまでまつ
        yield return handle;

        //読み込みが成功したかチェック
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //読み込んだTextAssetの中身を取得する
            string json = handle.Result.text;

            //取得したTextAssetの内容をコンソールで確認
            Debug.Log(json);

            //少し待つ
            yield return null;

            //成功を表示
            Debug.Log("成功!");

            //成功時のコールバック（JsonUtilityでのパースや表示処理を呼び出す。
            onSuccess?.Invoke(json);
        }
        else
        {
            Debug.Log($"Jsonのロード失敗:{jsonLoadAddress}");
        }

        //読み込んだリソースを解放する
        Addressables.Release(handle);
    }
}
```

## この結果、JSONを書き換えることで即反映できるため、編集作業を削減しました。<br>


### 【セーブ＆ロード機能の実装】<br>

プレイヤーのパラメータは初回時はSOでロードしていますが、レベルアップ以降は、JSONで保存され、JSONからロードされるようにしています。<br>

- レベルアップ時にJSONへの書き出し処理を実行 <br>

```C#
/// <summary>
/// Playerのパラメータ、ステージの状況を保存するスクリプト
/// </summary>
public static class SaveManager
{

    /// <summary>
    /// Jsonにレベルアップしたランタイムのパラメータデータを保存するメソッド
    /// </summary>
    /// <param name="players">レベルアップするプレイヤー</param>
    public static void SavePlayers(List<BasePlayerStatus> players)
    {
        //レベルアップするプレイヤーのリストに入っているキャラのランタイムデータを保存
        foreach (var player in players)
        {
            var data = new PlayerSaveData
            {
                playerID_SaveData = player.PlayerID,
                level_SaveData = player.Level,
                attackPower_SaveData = player.AttackPower,
                playerMaxHP_SaveData = player.PlayerMaxHP,

                bufferBuffPower_saveData = player.BuffPower,
                healerHealPower_saveData = player.HealPower
            };

            //Jsonの文字列に変換して保存（出力Jsonをインデントする）
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            //保存パス、ファイル名指定
            string path = Application.persistentDataPath + $"/{player.PlayerID}_save.json";

            //指定した保存パスにJsonを書き込み
            File.WriteAllText(path, json);
        }
    }
}
```
#### *↑プレイヤーデータを保存するコード* <br>

<img width="851" height="442" alt="image" src="https://github.com/user-attachments/assets/39259269-5402-4a7b-a1ea-b343609c943b" />

#### ↑自身のパソコンにJSONデータとして保存されます。 <br>

[初回ロード時]

```C#
/// <summary>
/// パラメータを設定するメソッド
/// プレイヤーのデータベースから読み込み
/// </summary>
protected override void SetPlayerParameters()
{
    // LINQを使い、プレイヤーデータベースからPlayerIDと一致するプレイヤー情報を取得
    var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);

    // 一致するプレイヤー情報が見つかった場合、パラメータを設定
    if (playerData != null)
    {

        //最大体力のデータを読み込み
        PlayerMaxHP = playerData.PlayerMaxHPData;

        //アタッカーの現在のHPを最大に設定してHPバーも最大に設定
        PlayerCurrentHP = PlayerMaxHP;

        Debug.Log($"PlayerCuurentHP:{PlayerCurrentHP},PlayerMaxHP:{PlayerMaxHP}");
        
        PlayerHPBar.maxValue = PlayerCurrentHP;
        PlayerHPBar.value = PlayerCurrentHP;
        PlayerHPBar.minValue = 0;

        //攻撃力をアタッカーのデータの攻撃力を読み込む
        AttackPower = playerData.PlayerAttackPowerData;

        //初期攻撃力もアタッカーの攻撃力に設定
        PlayerResetAttackPower = AttackPower;

        //生存状態にする
        IsAlive = true;
    }
}
```
#### *↑初回時はSOからロード* <br>

[セーブデータ存在の時]
```C#
/// <summary>
/// セーブデータがあればセーブしたプレイヤーデータからステータスを読み込む
/// </summary>
protected virtual void Start()
{
    //プレイヤーIDのセーブデータを取得
    string path = Application.persistentDataPath + $"/{PlayerID}_save.Json";

    //セーブデータがあれば読み、C#に変換した後、ステータスロード処理開始
    if(File.Exists(path))
    {
        string json = File.ReadAllText(path);
        PlayerSaveData saveData = JsonConvert.DeserializeObject<PlayerSaveData>(json);
        
        //セーブデータからステータスを読み込む
        ApplySaveData(saveData);
    }

    //セーブデータがなければスクリプタブルオブジェクトからロード
    else
    {
        SetPlayerParameters();
    }
}
```
↑セーブデータが存在すればゲーム開始時にJSONからロードして復元 <br>

- これにより、拡張性とメンテナンス性を両立しています。
  └ SO→JSONという遷移をフロー化し、
　└ SOで初期値管理 → JSONでランタイム永続化」の仕組みを確立<br>

 ## 【アピールポイントまとめ：工夫した点】 <br>
 
### - *これまで触れたことのない新技術に積極的にチャレンジ* <br>

ScriptableObject（SO）／JSONによるデータ管理を導入し、パラメータ追加やテキスト編集をプログラミング不要で行えるように設計。<br>

初回起動時はSO、以降はJSONからロードする「SO→JSON移行フロー」を確立し、将来的な仕様変更やコンテンツ拡張を容易に。<br>

### - *非同期処理で「止まらない没入感」を実現* <br>

UniTask＋Coroutineを組み合わせ、ターン中のステージ演出や全体攻撃の演出をフレーム単位で途切れさせず同期。 <br>

複数演出の連携やキャンセル処理を安定化させ、プレイヤー視点で“止まらない操作感”を達成。<br>

## 【アピールポイントまとめ：苦労して得た成長】 <br>
1.*UniTaskのキャンセル処理の実装*<br>
課題：どのタイミング・どの場所で処理をキャンセルするべきか判断が難しかった<br>
対応：サンプルコードやドキュメントを参照しつつ、デバックと試行錯誤の末、正常動作を実現<br>

2.*敵選択UIの不具合修正*<br>
課題：クリックした敵にUIが正しく追従せず、原因特定に時間を要した<br>
対応：原因を精査し、クリック時にUIを移動する方式に変更して解消<br>

### 成長
1.問題が発生したときにも、落ち着いて、現在起きている原因を分析し、考え、修正する力が付いた。<br>
2.完成するために、最後まで諦めずに、制作に取り組んだ<br>
3.読みやすいコードになるよう修正を繰り返したことで、システム面だけでなく、自分のスキルも上がったと考えている。<br>