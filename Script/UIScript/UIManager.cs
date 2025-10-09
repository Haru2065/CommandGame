using UnityEngine;

/// <summary>
/// UI関係を一括管理
/// </summary>
public class UIManager : MonoBehaviour
{
    //UIマネージャーのインスタンス化用
    private static UIManager instance;

    /// <summary>
    /// インスタンスのゲッター
    /// </summary>
    public static UIManager Instance
    {
        get => instance;
    }

    /// <summary>
    /// UIマネージャーをインスタンス化
    /// </summary>
    void Awake()
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

    [SerializeField]
    [Tooltip("プレイヤーのターンUI")]
    private GameObject playerTurnUI;
    /// <summary>
    /// プレイヤーターンのUIのゲッター
    /// </summary>
    public GameObject PlayerTurnUI
    {
        get => playerTurnUI;
    }








    [SerializeField]
    [Tooltip("敵のターンUI")]
    private GameObject enemyTurnUI;

    /// <summary>
    /// 敵のターンのUIのゲッター
    /// </summary>
    public GameObject EnemyTurnUI
    {
        get => enemyTurnUI;
    }

    [SerializeField]
    [Tooltip("キーボードA")]
    private GameObject aKey;

    [SerializeField]
    [Tooltip("キーボードS")]
    private GameObject sKey;

    [SerializeField]
    [Tooltip("キーボードF")]
    private GameObject fKey;

    [SerializeField]
    [Tooltip("通常攻撃UI")]
    private GameObject attackUI;

    [SerializeField]
    [Tooltip("スキルUI")]
    private GameObject skillUI;

    [SerializeField]
    [Tooltip("必殺UI")]
    private GameObject specialUI;

    [SerializeField]
    [Tooltip("ゲームクリアUI")]
    private GameObject gameClearUI;

    [SerializeField]
    [Tooltip("ゲームオーバーUI")]
    private GameObject gameOverUI;

    [SerializeField]
    [Tooltip("終了ボタン")]
    private GameObject exitButton;

    [SerializeField]
    [Tooltip("再挑戦ボタン")]
    private GameObject reTryButton;

    [SerializeField]
    [Tooltip("タイトルボタン")]
    private GameObject titleButton;

    [SerializeField]
    [Tooltip("ステータスウィンドウ")]
    private GameObject statusWindow;

    /// <summary>
    /// ステータスウィンドウのゲッター
    /// </summary>
    public GameObject StatusWindow
    {
        get => statusWindow;
    }

    [SerializeField]
    [Tooltip("ステータスウィンドウを開くボタン")]
    private GameObject openStatusWindowButton;

    /// <summary>
    /// ステータスウィンドウを開くボタンのゲッター
    /// </summary>
    public GameObject OpenStatusWindowButton
    {
        get => openStatusWindowButton;
    }


    [SerializeField]
    [Tooltip("ステータスウィンドウを閉じるボタン")]
    private GameObject closeStatusWindowButton;

    /// <summary>
    /// ステータスウィンドウを閉じるボタンのゲッター
    /// </summary>
    public GameObject CloseStatusWindowButton
    {
        get => closeStatusWindowButton;
    }

    [SerializeField]
    [Tooltip("ポーズ中のパネル")]
    private GameObject pausePanel;

    [SerializeField]
    [Tooltip("ポーズ中の終了ボタン")]
    private GameObject pauseExitButton;

    [SerializeField]
    [Tooltip("ポーズ中のタイトルボタン")]
    private GameObject pauseTitleButton;

    [SerializeField]
    [Tooltip("ステージセレクト画面ボタン")]
    private GameObject stageSelectButton;

    [SerializeField]
    [Tooltip("スキル制限カウントのテキスト")]
    private GameObject skillLimitCountText;

    /// <summary>
    /// アタッカーのスキル制限カウントのテキストのゲッターセッター
    /// </summary>
    public GameObject SkillLimitCountText
    {
        get => skillLimitCountText;
        set => skillLimitCountText = value;
    }

    [SerializeField]
    [Tooltip("必殺制限カウントのテキスト")]
    private GameObject specialLimitCountText;

    /// <summary>
    /// 必殺制限カウントのテキストのゲッターセッター
    /// </summary>
    public GameObject SpecialLimitCountText
    {
        get => specialLimitCountText;
        set => specialLimitCountText = value;
    }

    /// <summary>
    /// UIマネージャーのUIを一度、非表示にするメソッド
    /// </summary>
    public void StartUI()
    {
        //プレイヤーターンUIと敵のターンUIを非表示で初期化
        playerTurnUI.SetActive(false);
        enemyTurnUI.SetActive(false);

        //クリアとゲームオーバーUIを非表示で初期化
        gameClearUI.SetActive(false);
        gameOverUI.SetActive(false);

        //再挑戦と終了ボタンとタイトルボタンとステージセレクトボタンを非表示で初期化
        exitButton.SetActive(false);
        reTryButton.SetActive(false);
        titleButton.SetActive(false);
        stageSelectButton.SetActive(false);

        /*各プレイヤーターン時に表示されるUIを非表示で初期化*/
        aKey.SetActive(false);
        sKey.SetActive(false);
        fKey.SetActive(false);

        attackUI.SetActive(false);
        skillUI.SetActive(false);
        specialUI.SetActive(false);

        //ステータスウィンドウを非表示
        statusWindow.SetActive(false);

        //ステータスウィンドウを閉じるボタンを非表示
        closeStatusWindowButton.SetActive(false);

        //ステータスウィンドウを開くボタンを表示
        closeStatusWindowButton.SetActive(true);

        //ポーズ中に表示するパネル、ボタンを非表示
        pausePanel.SetActive(false);
        pauseExitButton.SetActive(false);
        pauseTitleButton.SetActive(false);

        skillLimitCountText.SetActive(false);
        specialLimitCountText.SetActive(false);
    }

    /// <summary>
    /// プレイヤーが操作するUIを表示するメソッド
    /// </summary>
    public void StartPlayerTurnUI()
    {
        aKey.SetActive(true);
        sKey.SetActive(true);
        fKey.SetActive(true);

        attackUI.SetActive(true);
        skillUI.SetActive(true);
        specialUI.SetActive(true);
    }

    /// <summary>
    /// 敵のターンに入る時にプレイヤーが操作するUIを非表示にするメソッド
    /// </summary>
    public void StartEnemyTurnUI()
    {
        aKey.SetActive(false);
        sKey.SetActive(false);
        fKey.SetActive(false);

        attackUI.SetActive(false);
        skillUI.SetActive(false);
        specialUI.SetActive(false);
    }

    /// <summary>
    /// ゲームクリア時に表示するメソッド
    /// </summary>
    public void GameClearUI()
    {
        aKey.SetActive(false);
        sKey.SetActive(false);
        fKey.SetActive(false);

        attackUI.SetActive(false);
        skillUI.SetActive(false);
        specialUI.SetActive(false);

        //タイトルボタン、終了ボタン、ステージセレクトボタンを表示
        exitButton.SetActive(true);
        titleButton.SetActive(true);
        stageSelectButton.SetActive(true);

        //ゲームクリアUIを表示
        gameClearUI.SetActive(true);
    }

    /// <summary>
    /// ゲームオーバー時に表示するメソッド
    /// </summary>
    public void GameOverUI()
    {
        aKey.SetActive(false);
        sKey.SetActive(false);
        fKey.SetActive(false);

        attackUI.SetActive(false);
        skillUI.SetActive(false);
        specialUI.SetActive(false);

        //終了、再挑戦、タイトル、ステージセレクトボタンを表示
        exitButton.SetActive(true);
        titleButton.SetActive(true);
        reTryButton.SetActive(true);
        stageSelectButton.SetActive(true);

        //ゲームオーバーUIを表示
        gameOverUI.SetActive(true);
    }

    /// <summary>
    /// ステータスウィンドウを閉じるメソッド
    /// </summary>
    public void CloseStatusWindow()
    {
        //ステータスウィンドウと閉じるボタンを非表示
        statusWindow.SetActive(false);
        closeStatusWindowButton.SetActive(false);

        //ステータスウィンドウを開くボタンを表示
        openStatusWindowButton.SetActive(true);
    }

    /// <summary>
    /// 中断画面を表示するメソッド
    /// </summary>
    public void ShowPauseMode()
    {
        pausePanel.SetActive(true);

        //タイトルボタン終了ボタン表示
        pauseTitleButton.SetActive(true);
        pauseExitButton.SetActive(true);
    }

    /// <summary>
    /// ポーズ画面を非表示にするメソッド
    /// </summary>
    public void HidePauseMode()
    {
        pausePanel.SetActive(false);

        //タイトルボタン終了ボタン非表示
        pauseTitleButton.SetActive(false);
        pauseExitButton.SetActive(false);
    }
}
