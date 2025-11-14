using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// バトルマネージャー
/// </summary>
public abstract class BaseBattleManager : MonoBehaviour
{
    //ベースのバトルマネージャーインスタンス化用
    private static BaseBattleManager instance;

    /// <summary>
    /// インスタンスのゲッター
    /// </summary>
    public static BaseBattleManager Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("アタッカー")]
    protected Attacker attacker;

    [SerializeField]
    [Tooltip("バッファー")]
    protected Buffer buffer;

    [SerializeField]
    [Tooltip("ヒーラー")]
    protected Healer healer;

    [SerializeField]
    [Tooltip("プレイヤーの生存リスト")]
    protected List<BasePlayerStatus> alivePlayers = new List<BasePlayerStatus>();

    /// <summary>
    /// プレイヤーの生存リストのゲッター
    /// </summary>
    public List<BasePlayerStatus> AlivePlayers
    {
        get => alivePlayers;
    }

    [SerializeField]
    [Tooltip("レベルアップを行うリスト")]
    protected List<BasePlayerStatus> LevelUPPlayerList = new List<BasePlayerStatus>();

    [SerializeField]
    [Tooltip("敵の生存リスト")]
    public List<BaseEnemyStatus> aliveEnemies = new List<BaseEnemyStatus>();

    [SerializeField]
    [Tooltip("ターン開始エフェクト")]
    private GameObject startTurnEffect;

    /// <summary>
    /// ターン開始エフェクトのゲッター
    /// </summary>
    public GameObject StartTurnEffect
    {
        get => startTurnEffect;
    }

    [SerializeField]
    [Tooltip("Firstターン開始エフェクト生成位置")]
    private Transform firstTurnEffect_SpawnPoint;

    /// <summary>
    /// Firstターン開始エフェクト生成位置のゲッター
    /// </summary>
    public Transform FirstTurnEffect_SpawnPoint
    {
        get => firstTurnEffect_SpawnPoint;
    }

    [SerializeField]
    [Tooltip("Secondターン開始エフェクト生成位置")]
    private Transform secondTurnEffect_SpawnPoint;

    /// <summary>
    /// Secondターン開始エフェクト生成位置のゲッター
    /// </summary>
    public Transform SecondTurnEffect_SpawnPoint
    {
        get => secondTurnEffect_SpawnPoint;
    }

    [SerializeField]
    [Tooltip("Thirdターン開始エフェクト生成位置")]
    private Transform thirdTurnEffect_SpawnPoint;

    /// <summary>
    /// Thirdターン開始エフェクト生成位置のゲッター
    /// </summary>
    public Transform ThirdTurnEffect_SpawnPoint
    {
        get => thirdTurnEffect_SpawnPoint;
    }

    [SerializeField]
    [Tooltip("終了ボタンスクリプト")]
    protected PushExitButton pushExitButton;

    //プレイヤーターンか
    protected bool IsPlayerTurn;

    //ゲームクリアか
    protected bool isGameClear;

    //ゲームオーバーか
    protected bool isGameOver;
    
    //ポーズモードにできるか
    protected bool canPoseMode;

    //ステージ2が解放されたか
    private bool isUnlockStage2;

    /// <summary>
    /// ステージ2が解放フラグのゲッターセッター
    /// </summary>
    public bool IsUnlockStage2
    {
        get => isUnlockStage2;
        set => isUnlockStage2 = value;
    }

    //ステージ3が解放されたか
    private bool isUnlockStage3;

    /// <summary>
    /// ステージ3が解放フラグのゲッターセッター
    /// </summary>
    public bool IsUnlockStage3
    {
        get => isUnlockStage3;
        set => isUnlockStage3 = value;
    }

    //ターンの待ち時間
    protected const float TurnDelay = 1f;

    /// <summary>
    /// 非同期処理のキャンセルに使用するトークンソース
    /// バトル終了時やゲーム終了時にキャンセルシグナルを送るために使用
    /// </summary>
    protected CancellationTokenSource cts;

    /// <summary>
    /// ベースのバトルマネージャーをインスタンス化
    /// シングルトンパターンを使用して、シーン内に1つのインスタンスのみ存在するようにします
    /// </summary>
    private void Awake()
    {
        // インスタンスが未設定の場合は自身を設定
        if (instance == null)
        {
            instance = this;
        }
        // 既にインスタンスが存在する場合は重複を防ぐため削除
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// バトル開始時の初期化処理
    /// UIの初期状態設定、プレイヤーとターゲットの初期化を行います
    /// 派生クラスでオーバーライドして拡張できます
    /// </summary>
    protected virtual void Start()
    {
        // UIマネージャーを使用してバトル開始時のUI状態を設定（全て非表示から開始）
        UIManager.Instance.StartUI();

        // バトルは常にプレイヤーターンから開始
        IsPlayerTurn = true;

        // 全プレイヤーキャラクターを生存状態に設定
        attacker.IsAlive = true;
        buffer.IsAlive = true;
        healer.IsAlive = true;

        // ゲームの勝敗判定フラグを初期化
        isGameClear = false;
        isGameOver = false;

        // バトル開始時のデフォルトターゲットを設定（最初の敵をターゲットに）
        PlayerTargetSelect.Instance.SetStartBattleTarget();

        // ステータスウィンドウボタンを半透明にして開けないようにする（バトル開始直後は使用不可）
        PushOpenStatusWindow.Instance.TransparentStatusButton();
    }

    /// <summary>
    /// 毎フレーム実行される更新処理
    /// ポーズメニューの表示/非表示の切り替えとゲーム終了処理を行います
    /// </summary>
    protected virtual void Update()
    {
        // ポーズモードが利用可能な場合の入力処理
        if (canPoseMode)
        {
            // Escキーでポーズメニューを表示
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.ShowPauseMode();
            }
            // Tabキーでポーズメニューを閉じる
            else if (Input.GetKeyDown(KeyCode.Tab) && canPoseMode)
            {
                UIManager.Instance.HidePauseMode();
            }
        }

        // ゲーム終了ボタンが押された場合の処理
        if (pushExitButton.IsQuitGame)
        {
            // 実行中の非同期処理をキャンセル
            cts.Cancel();
            // キャンセルトークンソースのリソースを解放
            cts.Dispose();
        }
    }

    /// <summary>
    /// プレイヤーターン時の共通処理（抽象メソッド）
    /// 各ステージのバトルシステムで具体的な実装を行います
    /// プレイヤーの入力を受け付け、攻撃、スキル、必殺技の実行を制御します
    /// </summary>
    /// <param name="player">行動を実行するプレイヤーキャラクター</param>
    /// <param name="normalKey">通常攻撃を実行するキー（通常はAキー）</param>
    /// <param name="skillKey">スキルを実行するキー（通常はSキー）</param>
    /// <param name="specialKey">必殺技を実行するキー（通常はFキー）</param>
    /// <param name="token">非同期処理のキャンセル用トークン</param>
    /// <returns>プレイヤーの行動が完了するまで待機するUniTask</returns>
    protected abstract UniTask PlayerTurnAction(BasePlayerStatus player, KeyCode normalKey, 
        KeyCode skillKey, KeyCode specialKey, CancellationToken token);


    /// <summary>
    /// ゲームクリア時にUI表示とデータ保存を行うメソッド
    /// レベルアップ処理を開始し、その後クリアUIを表示します
    /// </summary>
    protected virtual void DelayGameClearUI()
    {
        // プレイヤーのレベルアップとセーブ処理を実行
        StartCoroutine(PlayerLevelUP());
    }

    /// <summary>
    /// ゲームクリア条件を確認するメソッド（抽象メソッド）
    /// 各ステージで具体的なクリア条件を実装します
    /// </summary>
    /// <returns>ゲームクリアの場合true、それ以外false</returns>
    protected abstract bool GameClearCheck();


    /// <summary>
    /// プレイヤーのレベルアップとパラメータを保存するコールチン
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator PlayerLevelUP()
    {
        Debug.Log($"アタッカー{attacker.AttackPower},{attacker.PlayerMaxHP}");
        Debug.Log($"バッファー{buffer.AttackPower},{buffer.PlayerMaxHP},{buffer.BuffPower}");
        Debug.Log($"ヒーラー{healer.AttackPower},{healer.PlayerMaxHP},{healer.HealPower}");

        //レベルアップするプレイヤーのリストのキャラをレベルアップ
        foreach (var player in LevelUPPlayerList)
        {
            player.LevelUP();
        }

        Debug.Log($"アタッカー{attacker.AttackPower},{attacker.PlayerMaxHP}");
        Debug.Log($"バッファー{buffer.AttackPower},{buffer.PlayerMaxHP},{buffer.BuffPower}");
        Debug.Log($"ヒーラー{healer.AttackPower},{healer.PlayerMaxHP},{healer.HealPower}");

        //レベルアップしたキャラのパラメータを保存
        SaveManager.SavePlayers(LevelUPPlayerList);

        //保存パスを表示
        Debug.Log("保存パス：" + Application.persistentDataPath);

        //レベルアップしたことをウィンドウ表示
        BattleActionTextManager.Instance.ShowBattleActionText("LevelUPText");

        //2フレーム待つ
        yield return new WaitForSeconds(2);

        //レベルアップしたことを通知するウィンドウを非表示
        StartCoroutine(HidePlayerActionText());

        // UIマネージャーからゲームクリアUIを表示
        UIManager.Instance.GameClearUI();
    }

    /// <summary>
    /// ゲームオーバーしたかの確認するメソッド
    /// </summary>
    /// <returns></returns>
    protected virtual bool GameOverCheck()
    {
        //もし味方が全滅したらゲームオーバー
        if(alivePlayers.Count == 0)
        {
            //ゲームオーバーフラグをtrue
            isGameOver = true;

            //UIマネージャーからゲームオーバーを表示
            UIManager.Instance.GameOverUI();

            //ゲームオーバーなのでtrueを返す
            return true;
        }
        //味方が生きているのでfalseを返す
        return false;
    }

    /// <summary>
    /// 指定した位置にターン開始エフェクトを生成するコールチン
    /// </summary>
    /// <param name="spawnPoint">エフェクトを生成する位置</param>
    /// <returns>エフェクト表示後の待機時間</returns>
    protected IEnumerator ShowStartTurnEffect(Transform spawnPoint)
    {
        //指定された位置にターン開始エフェクトを生成
        GameObject startTurnEffectInstance = Instantiate(StartTurnEffect, spawnPoint.position, Quaternion.identity);

        //ターン開始エフェクトを2秒後消去
        Destroy(startTurnEffectInstance, TurnDelay);

        //0.5秒待機
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// JSONファイルのプレイヤー状況通知テキストを非表示にするコールチン
    /// </summary>
    /// <returns></returns>
    protected IEnumerator HidePlayerActionText()
    {
        //1秒待つ
        yield return new WaitForSeconds(TurnDelay);

        //JSONファイルの状況通知テキストを非表示
        BattleActionTextManager.Instance.TextDelayHide();

        //1フレーム待つ
        yield return null;
    }

    /// <summary>
    /// ステージデータをロードするメソッド
    /// </summary>
    /// <param name="data">Jsonに保存されているステージデータ</param>
    protected void LoadStageData(StageSaveData data)
    {
        isUnlockStage2 = data.Stage2UnLock_SaveData;

        //ステージ3解放のフラグデータをステージセーブデータからロード
        isUnlockStage3 = data.Stage3UnLock_SaveData;
    }
}
