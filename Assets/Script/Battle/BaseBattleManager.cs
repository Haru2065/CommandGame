using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("敵の生存リスト")]
    public List<BaseEnemyStatus> aliveEnemies = new List<BaseEnemyStatus>();


    [SerializeField]
    [Tooltip("レベルアップを行うリスト")]
    protected List<BasePlayerStatus> LevelUPPlayerList = new List<BasePlayerStatus>();

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

    //プレイヤーターンか
    public bool IsPlayerTurn;

    //ゲームクリアか
    protected bool isGameClear;

    //ゲームオーバーか
    protected bool isGameOver;
    
    //ポーズモードにできるか
    protected bool canPoseMode;

    //ステージ3が解放されたか
    public bool IsUnlockStage3;

    /// <summary>
    /// ベースのバトルマネージャーをインスタンス化
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //共通の処理はベースでのバトルマネージャーで初期化
    // Start is called before the first frame update
    protected virtual void Start()
    {
        //UIマネージャーから一度UIを全て非表示
        UIManager.Instance.StartUI();

        //最初はプレイヤーターンから開始
        IsPlayerTurn = true;

        //各プレイヤーキャラを生存状態をtrueに
        attacker.IsAlive = true;
        buffer.IsAlive = true;
        healer.IsAlive = true;

        //ゲームクリア、ゲームオーバーの判定フラグはfalseに
        isGameClear = false;
        isGameOver = false;

        //バトル開始時にターゲットを設定
        PlayerTargetSelect.Instance.SetStartBattleTarget();
    }

    /// <summary>
    /// ポーズモードにする
    /// </summary>
    // Update is called once per frame
    protected virtual void Update()
    {
        //エスケープキーが押されたら終了ボタンとタイトルボタンを表示
        if (canPoseMode)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.ShowPauseMode();
            }
            //タブキーで閉じる
            else if (Input.GetKeyDown(KeyCode.Tab) && canPoseMode)
            {
                UIManager.Instance.HidePauseMode();
            }
        }
    }
    /// <summary>
    /// 遅れてクリアUIを表示するメソッド
    /// </summary>
    protected virtual void DelayGameClearUI()
    {
        //UIマネージャーからゲームクリアUIを表示
        UIManager.Instance.GameClearUI();

        //プレイヤーのレベルアップ処理を行う
        StartCoroutine(PlayerLevelUP());
    }

    /// <summary>
    /// ゲームクリアしたかの確認するメソッド
    /// </summary>
    /// <returns>バトル終了フラグ</returns>
    protected abstract bool GameClearCheck();


    /// <summary>
    /// プレイヤーのレベルアップとパラメータを保存するコールチン
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator PlayerLevelUP()
    {
        //レベルアップしたことをウィンドウ表示
        BattleActionTextManager.Instance.ShowBattleActionText("LevelUPText");
        

        //2フレーム待つ
        yield return new WaitForSeconds(2f);

        //レベルアップしたことを通知するウィンドウを非表示
        StartCoroutine(HidePlayerActionText());

        Debug.Log($"アタッカー{attacker.AttackPower},{attacker.PlayerMaxHP}");
        Debug.Log($"バッファー{buffer.AttackPower},{buffer.PlayerMaxHP},{buffer.buffPower}");
        Debug.Log($"ヒーラー{healer.AttackPower},{healer.PlayerMaxHP},{healer.healPower}");

        //レベルアップするプレイヤーのリストのキャラをレベルアップ
        foreach (var player in LevelUPPlayerList)
        {
            player.LevelUP();
        }

        Debug.Log($"アタッカー{attacker.AttackPower},{attacker.PlayerMaxHP}");
        Debug.Log($"バッファー{buffer.AttackPower},{buffer.PlayerMaxHP},{buffer.buffPower}");
        Debug.Log($"ヒーラー{healer.AttackPower},{healer.PlayerMaxHP},{healer.healPower}");

        //レベルアップしたキャラのパラメータを保存
        SaveManager.SavePlayers(LevelUPPlayerList);

        //保存パスを表示
        Debug.Log("保存パス：" + Application.persistentDataPath);
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
        Destroy(startTurnEffectInstance, 2f);

        //0.5秒待機
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// JSONファイルの状況通知テキストを非表示にするコールチン
    /// </summary>
    /// <returns></returns>
    protected IEnumerator HidePlayerActionText()
    {
        //2フレーム待つ
        yield return new WaitForSeconds(2f);

        //JSONファイルの状況通知テキストを非表示
        BattleActionTextManager.Instance.TextDelayHide();

        //1フレーム待つ
        yield return null;
    }
}
