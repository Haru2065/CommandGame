using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

/// <summary>
/// ステージ１のバトルシステム
/// </summary>
public class Stage1BattleSystem : BaseBattleManager
{
    [SerializeField]
    [Tooltip("スライム1")]
    private Slime slime1;

    [SerializeField]
    [Tooltip("スライム２")]
    private Slime slime2;

    [SerializeField]
    [Tooltip("スライム３")]
    private Slime slime3;

    // Start is called before the first frame update
    protected override async void Start()
    {
        base.Start();

        canPoseMode = false;

        cts = new CancellationTokenSource();        

        // ステージのセーブデータを読み込む
        string path = Application.persistentDataPath + $"/StageSaveData.Json";

        //セーブデータが存在するならステージデータをロード
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StageSaveData saveData = JsonConvert.DeserializeObject<StageSaveData>(json);
            LoadStageData(saveData);
        }

        //セーブデータが存在しないならステージデータを初期化
        else
        {
            IsUnlockStage2 = false;
        }

        PlayerTargetSelect.Instance.SetStartBattleTarget();

        canPoseMode = false;

        //バトルループ開始
        await BattleLoop(cts.Token);
    }

    protected override void Update()
    {
        base.Update();

        // もし終了ボタンが押されたら安全に終了するためにキャンセル処理を行う
        if (pushExitButton.IsQuitGame)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    /// <summary>
    /// UniTaskバトルのループ処理
    /// </summary>
    /// <param name="token">キャンセルできる処理/param>
    /// <returns>プレイヤーが行動するまで待つ</returns>
    async UniTask BattleLoop(CancellationToken token)
    {
        //ゲームクリア・ゲームオーバーのフラグがtrueならループを止めてキャンセルする
        while (!(isGameClear || token.IsCancellationRequested))
        {
            if (IsPlayerTurn)
            {
                //UIマネージャーからプレイヤーターン表示
                UIManager.Instance.PlayerTurnUI.SetActive(true);

                //1フレーム待つ（キャンセルトークンが呼ばれたらキャンセル）
                await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                //全プレイヤーの行動フラグをリセット
                attacker.ResetActionFlag();
                buffer.ResetActionFlag();
                healer.ResetActionFlag();

                //UIマネージャーからプレイヤーターン非表示
                UIManager.Instance.PlayerTurnUI.SetActive(false);

                //UIマネージャーからプレイヤーターン時に表示するUIを表示
                UIManager.Instance.StartPlayerTurnUI();

                //アタッカーが生存していたら処理を実行
                if (attacker.IsAlive)
                {
                    //指定した位置にターン開始エフェクト生成
                    StartCoroutine(ShowStartTurnEffect(FirstTurnEffect_SpawnPoint));

                    //1フレーム待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //アタッカーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //アタッカーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //1フレーム待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //ステータスウィンドウを開くボタンを押せるようにする
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //アタッカーターン開始(キャンセルできる処理)
                    await PlayerTurnAction(attacker, "AttackeroffDebuff", KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //敵の生存状況を確認（生存リストが空ならループを止める）
                    if (GameClearCheck())
                    {
                        return;
                    }

                    //ステータスボタンを開くボタンを押せないようにする
                    PushOpenStatusWindow.Instance.TransparentStatusButton();
                }

                if (buffer.IsAlive)
                {
                    //指定した位置にターン開始エフェクト生成
                    StartCoroutine(ShowStartTurnEffect(SecondTurnEffect_SpawnPoint));

                    //1フレーム待つ(キャンセルトークンが呼ばれたらキャンセル)
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //バッファーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("BufferTurnText");

                    //バッファーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //1フレーム待つ(キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //バッファーのターン開始(キャンセルできる処理)
                    await PlayerTurnAction(buffer, "BufferOffDebuff", KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //敵の生存状況を確認（生存リストが空ならループを止める）
                    if (GameClearCheck())
                    {
                        return;
                    }
                    PushOpenStatusWindow.Instance.TransparentStatusButton();
                }

                if (healer.IsAlive)
                {
                    //指定した位置にターン開始エフェクト生成
                    StartCoroutine(ShowStartTurnEffect(ThirdTurnEffect_SpawnPoint));

                    //1フレーム待つ(キャンセルトークンが呼ばれたらキャンセル)
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //ヒーラーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("HealerTurnText");

                    //ヒーラーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //1フレーム待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //ステータスウィンドウを開くボタンを押せるようにする
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //ヒーラーのターン開始(キャンセルできる処理）
                    await PlayerTurnAction(healer, "HealerOffdebuff", KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //敵の生存状況を確認（生存リストが空ならループを止める）
                    if (GameClearCheck())
                    {
                        return;
                    }

                    //ステータスボタンを開くボタンを押せないようにする
                    PushOpenStatusWindow.Instance.TransparentStatusButton();
                }
            }

            //敵のターン
            else
            {
                //各プレイヤーの行動フラグをfalseにする
                attacker.IsAttackerAction = false;
                buffer.IsBufferAction = false;
                healer.IsHealerAction = false;

                //UIマネージャーから敵ターンUIを表示
                UIManager.Instance.EnemyTurnUI.SetActive(true);

                await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                //UIマネージャーから敵ターンUIを非表示
                UIManager.Instance.EnemyTurnUI.SetActive(false);

                //敵ターン開始
                await EnemyTurn(token);
            }
        }
    }

    /// <summary>
    /// プレイヤーターンの共通処理
    /// </summary>
    /// <param name="player">プレイヤーのクラス名</param>
    /// <param name="offDebuffTextID">デバフ解除テキスト</param>
    /// <param name="normalKey">通常攻撃キー</param>
    /// <param name="skillKey">スキルキー</param>
    /// <param name="specialKey">必殺キー</param>
    /// <param name="token">キャンセルできる処理</param>
    /// <returns>プレイヤーが行動するまで処理を待つ</returns>
    protected override async UniTask PlayerTurnAction(BasePlayerStatus player, string offDebuffTextID, KeyCode normalKey, KeyCode skillKey, KeyCode specialKey, CancellationToken token)
    {
        //プレイヤーの行動フラグがtrueになるまでループし続ける
        while (!player.IsPlayerAction)
        {
            //ポーズ画面に切り替え可能にする
            canPoseMode = true;

            //いずれかのキーが押されるまで待つ(通常、スキン、必殺)
            await UniTask.WaitUntil(() =>
                Input.GetKeyDown(normalKey) ||
                Input.GetKeyDown(skillKey) ||
                Input.GetKeyDown(specialKey),
                cancellationToken: token);

            if (Input.GetKeyDown(normalKey))
            {
                //プレイヤーの通常攻撃実行
                player.NormalAttack();

                //通常攻撃ではすぐにプレイヤー行動フラグをtrueにす
                player.IsPlayerAction = true;
            }
            else if (Input.GetKeyDown(skillKey))
            {
                //プレイヤーのスキルを実行
                player.PlayerSkill();
                break;
            }
            else if (Input.GetKeyDown(specialKey))
            {
                if (!player.IsUseSpecial)
                {
                    //必殺制限フラグがfalseならスペシャルを実行
                    player.SpecialSkill();
                    break;
                }
                else
                {
                    //JSONファイルから必殺使用不可通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSpecial");

                    //必殺使用不可UIを非表示にするコールチン
                    StartCoroutine(HidePlayerActionText());
                }
            }
        }

        //プレイヤーターンが終了したらポーズ画面には切り替えれなくする
        if (player.IsPlayerAction)
        {
            //プレイヤーの必殺制限カウントのテキストを非表示
            UIManager.Instance.SpecialLimitCountText.SetActive(false);
            canPoseMode = false;
        }

        //1フレーム待つ(キャンセルできる処理）
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

        //TryCatchを使いエラーを防ぐ
        try
        {
            //プレイヤーが行動を終了するまで処理を待つ
            await UniTask.WaitUntil(() => player.IsPlayerAction, cancellationToken: token);

            //プレイヤー全体の行動が終わったらプレイヤーターン終了
            if (attacker.IsPlayerAction && buffer.IsPlayerAction && healer.IsPlayerAction)
            {
                IsPlayerTurn = false;
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("キャンセルされた");
        }

        //1フレーム待つ
        await UniTask.Yield();
    }

    /// <summary>
    /// ユニタスク敵のターン
    /// </summary>
    /// <param name="token">キャンセルできる処理/param>
    /// <returns>ユニタスクを止める処理</returns>
    async UniTask EnemyTurn(CancellationToken token)
    {
        if (slime1.EnemyIsAlive)
        {
            //スライム1ターン
            await SlimeTurn(slime1, token);
            
        }
        if (slime2.EnemyIsAlive)
        {
            //スライム2ターン
            await SlimeTurn(slime2, token);
        }

        if (slime3.EnemyIsAlive)
        {
            //スライム3ターン
            await SlimeTurn(slime3, token);
        }

        GameOverCheck();

        // 全スライムが行動し終えた後で全滅チェック
        if (isGameOver)
        {
            return;
        }



        //敵のターン終了
        IsPlayerTurn = true;
    }

    /// <summary>
    /// スライムターン処理
    /// </summary>
    /// <param name="slime">スライムのステータス</param>
    /// <returns></returns>
    async UniTask SlimeTurn(Slime slime, CancellationToken token)
    {
        //スライムの攻撃開始
        await slime.SlimeAction();

        await UniTask.Yield();
    }

    /// <summary>
    /// プレイヤーが全滅を確認し、ゲームオーバー処理を実行する。
    /// </summary>
    /// <returns>全滅していたらtrue</returns>
    protected override bool GameClearCheck()
    {
        //敵生存リストが空になったら値を返す
        if (aliveEnemies.Count == 0)
        {
            isGameClear = true;

            if (!IsUnlockStage2)
            {
                //ステージ3を解放
                IsUnlockStage2 = true;

                //ステージデータを保存
                SaveManager.SaveStage();
            }

            //2秒遅れてクリアUIを表示し、レベルアップ処理を行う
            Invoke("DelayGameClearUI", 2);
            return true;
        }
        //敵が生きているのでfalseを返す
        return false;
    }

    /// <summary>
    /// プレイヤーが全滅を確認し、ゲームオーバー処理を実行する。
    /// </summary>
    /// <returns>全滅していたらtrue</returns>
    protected override bool GameOverCheck()
    {
        //プレイヤーの生存リストが空になったら値を返す
        if (alivePlayers.Count == 0)
        {
            isGameOver = true;

            //2秒遅れてゲームオーバーＵＩを表示
            Invoke("DelayGameOverUI", 2);
            return true;
        }
        //プレイヤーが生きているのでfalseを返す
        return false;
    }

    /// <summary>
    /// 遅れてゲームオーバーUIを表示する処理B
    /// </summary>
    void DelayGameOverUI()
    {
        UIManager.Instance.GameOverUI();
    }
}
