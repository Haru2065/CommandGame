using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージ3のバトルシステム
/// </summary>
public class Stage3BattleSystem : BaseBattleManager
{
    

    [SerializeField]
    [Tooltip("必殺制限カウントのTMProUGUI")]
    private TextMeshProUGUI specialLimitCountUGUI;    

    [SerializeField]
    [Tooltip("ドラゴン")]
    private Dragon dragon;

    [SerializeField]
    [Tooltip("ラストバトルUI")]
    private GameObject LastButtleUI;

    //バトル開始特殊演出（ステージ3限定でスタート演出が終了したかのフラグ）
    private bool isEndDirection;

    //攻撃対象者のリスト
    private List<BasePlayerStatus> playerParty;

    // Start is called before the first frame update
    protected override async void Start()
    {
        //親クラスでUIの初期化と敵の攻撃対象を設定
        base.Start();

        //バトルの開始演出終了フラグをfalse
        isEndDirection = false;

        //最初はポーズ画面にはできない
        canPoseMode = false;

        //攻撃対象を生存リストに設定
        playerParty = alivePlayers;

        //キャンセルトークンソースを生成
        cts = new CancellationTokenSource();

        //ステージ3開始特殊演出を開始
        await BattleStartAction(cts.Token);

        //バトルループの開始
        await BattleLoop(cts.Token);
    }

    // Update is called once per frame
    protected override void Update()
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

        //もし終了ボタンが押されたら安全に終了するためにキャンセル処理を行う
        if (pushExitButton.IsQuitGame)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    /// <summary>
    /// ステージ3開始特殊演出
    /// </summary>
    /// <param name="token">キャンセルできる処理</param>
    /// <returns></returns>
    async UniTask BattleStartAction(CancellationToken token)
    {
        //ドラゴンの鳴き声を実行
        EnemySE.Instance.Play_DragonRourSE();

        //2フレーム待つ（キャンセル可能処理)
        await UniTask.Delay(TimeSpan.FromSeconds(2f),cancellationToken: token);

        //ラストバトルUIを表示
        LastButtleUI.SetActive(true);

        //Stage3のBGMを再生
        BGMControl.Instance.PlayStage3BGM();

        //1フレーム待つ（キャンセル可能処理）
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

        //ラストバトルUIを非表示
        LastButtleUI.SetActive(false);

        //1秒待つ（キャンセル可能処理）
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

        //開始演出終了
        isEndDirection = true;

        //開始演出が終了するまで待つ（キャンセル可能処理）
        await UniTask.WaitUntil((() => isEndDirection), cancellationToken: token) ;
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

                    //
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay),cancellationToken: token);

                    //アタッカーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //アタッカーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //1フレーム待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //ステータスウィンドウを開くボタンを押せるようにする
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //アタッカーターン開始(キャンセルできる処理)
                    await PlayerTurnAction(attacker, KeyCode.A, KeyCode.S, KeyCode.F, token);
                    
                    //もしアタッカーにHPデバフが付与されていたらダメージを減らし、デバフカウントも減らす
                    if (attacker.IsHPDebuff)
                    {
                        //アタッカーのデバフ処理（デバフ解除されたら、デバフ解除通知を表示）
                        await HPDebuff(attacker, "AttackerOffDebuff", token);
                    }

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
                    await PlayerTurnAction(buffer, KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //もしバッファーにHPデバフが付与されていたらダメージを減らし、デバフカウントも減らす
                    if (buffer.IsHPDebuff)
                    {
                        //バッファーのデバフ処理（デバフ解除されたら、デバフ解除通知を表示）
                        await HPDebuff(buffer, "BufferOffDebuff", token);
                    }

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
                    await PlayerTurnAction(healer, KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //もしヒーラーにHPデバフが付与されていたらダメージを減らし、デバフカウントも減らす
                    if (healer.IsHPDebuff)
                    {
                        //ヒーラーのデバフ処理（デバフ解除されたら、デバフ解除通知を表示）
                        await HPDebuff(healer, "HealerOffDebuff", token);
                    }

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
    protected override async UniTask PlayerTurnAction(BasePlayerStatus player,KeyCode normalKey, KeyCode skillKey, KeyCode specialKey, CancellationToken token)
    {
        //必殺制限中ならUIを表示して制限カウントを減らす
        if (player.IsUseSpecial)
        {
            UIManager.Instance.SpecialLimitCountText.SetActive(true);
            player.SpecialLimitCount--;

            //必殺制限カウントが0になったら必殺を使用可能にする
            if (player.SpecialLimitCount <= 0)
            {
                player.SpecialLimitCount = 0;
                player.IsUseSpecial = false;

                //必殺制限カウントのUI非表示
                UIManager.Instance.SpecialLimitCountText.SetActive(false);
            }

            //制限継続中ならそのままUIを表示する
            else
            {
                specialLimitCountUGUI.text = $"({player.SpecialLimitCount})";
            }
        }

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

        ////デバフが付与されていたらHPを減らす
        //if(player.IsDebuff)
        //{
        //    await HPDebuff(player, offDebuffTextID, token);
        //}

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
    /// <param name="token">キャンセルできる処理</param>
    /// <returns>ユニタスクを止める処理</returns>
    async UniTask EnemyTurn(CancellationToken token)
    {
        //スケルトンドラゴンのターン
        await DragonTurn(token);

        if (GameOverCheck()) return;

        //1フレーム待つ
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay));

        IsPlayerTurn = true;
    }

    /// <summary>
    /// ユニタスクドラゴンターン
    /// </summary>
    /// <param name="Token">キャンセルできる処理/param>
    /// <returns>次の処理を待つ</returns>
    async UniTask DragonTurn(CancellationToken Token)
    {
        await dragon.DragonAction(playerParty);

        //1フレーム待つ
        await UniTask.Yield();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="debuffHPTextkey"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    async UniTask HPDebuff(BasePlayerStatus player, string debuffHPTextkey, CancellationToken token)
    {
        if (player == null) return;

        player.PlayerCurrentHP -= dragon.HPDebuffPower;

        player.PlayerHPBar.value = player.PlayerCurrentHP;

        player.DebuffCount--;

        if(player.DebuffCount  <= 0)
        {
            player.IsDebuff = false;

            BattleActionTextManager.Instance.ShowBattleActionText(debuffHPTextkey);

            StartCoroutine(player.PlayerOffDebuffText());

            await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);
        }
       
    }

    /// <summary>
    /// プレイヤーが全滅を確認し、ゲームオーバー処理を実行する。
    /// </summary>
    /// <returns>全滅していたらtrue</returns>
    protected override bool GameClearCheck()
    {
        //もし敵が全滅したらクリア
        if (aliveEnemies.Count == 0)
        {
            //ゲームクリアフラグをtrue
            isGameClear = true;

            //キャンセル処理を実行し、キャンセルトークンを破棄しリソースを解放
            cts.Cancel();
            cts.Dispose();

            //ゲームクリアシーンをロード
            SceneManager.LoadScene("GameClear");

            //クリアしたのでtrueを返す
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
        //もし味方が全滅したらゲームオーバー
        if (alivePlayers.Count == 0)
        {
            //ゲームオーバーフラグをtrue
            isGameOver = true;

            //UIマネージャーからゲームオーバーを表示
            UIManager.Instance.GameOverUI();

            //レベルアップしたことをウィンドウ表示
            BattleActionTextManager.Instance.ShowBattleActionText("Retry");

            //キャンセル処理を実行し、キャンセルトークンを破棄しリソースを解放
            cts.Cancel();
            cts.Dispose();

            //ゲームオーバーなのでtrueを返す
            return true;
        }
        //味方が生きているのでfalseを返す
        return false;
    }
}
