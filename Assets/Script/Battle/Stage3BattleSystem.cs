using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// ステージ3のバトルシステム
/// </summary>
public class Stage3BattleSystem : BaseBattleManager
{
    private CancellationTokenSource cts;

    [SerializeField]
    [Tooltip("必殺制限カウントのTMProUGUI")]
    private TextMeshProUGUI specialLimitCountUGUI;

    [SerializeField]
    [Tooltip("終了ボタンスクリプト")]
    protected PushExitButton pushExitButton;

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

        //2秒待つ（キャンセル可能処理)
        await UniTask.Delay(TimeSpan.FromSeconds(2),cancellationToken: token);

        //ラストバトルUIを表示
        LastButtleUI.SetActive(true);

        //Stage3のBGMを再生
        BGMControl.Instance.PlayStage3BGM();

        //2秒待つ（キャンセル可能処理）
        await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

        //ラストバトルUIを非表示
        LastButtleUI.SetActive(false);

        //2秒待つ（キャンセル可能処理）
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

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

                //2秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                //UIマネージャーからプレイヤーターン非表示
                UIManager.Instance.PlayerTurnUI.SetActive(false);

                //UIマネージャーからプレイヤーターン時に表示するUIを表示
                UIManager.Instance.StartPlayerTurnUI();


                //アタッカーが生存していたら処理を実行
                if (attacker.IsAlive)
                {
                    //指定した位置にターン開始エフェクト生成
                    StartCoroutine(ShowStartTurnEffect(FirstTurnEffect_SpawnPoint));

                    //1秒待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(1),cancellationToken: token);

                    //アタッカーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //アタッカーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //2秒待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //もしアタッカーにHPデバフが付与されていたらダメージを減らし、デバフカウントも減らす
                    if (attacker.IsHPDebuff)
                    {
                        //ダメージを減らし、デバフカウントも減らす
                        attacker.PlayerCurrentHP -= dragon.HPDebuffPower;
                        attacker.DebuffCount--;

                        //デバフカウントが0になったらHPデバフを解除、また解除したことを通知する
                        if(attacker.DebuffCount <= 0)
                        {
                            attacker.IsHPDebuff = false;
                            BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");

                            //デバフ解除時の状況テキストを非表示にする
                            StartCoroutine(attacker.PlayerOffDebuffText());

                            //2秒待つ
                            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
                        }
                    }

                    //アタッカーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //アタッカーターン開始
                    await AttackerTurn(token);

                    //2フレーム待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);

                    //敵の生存状況を確認（生存リストが空ならループを止める）
                    if (GameClearCheck())
                    {
                        return;
                    }
                }

                if (buffer.IsAlive)
                {
                    //指定した位置にターン開始エフェクト生成
                    StartCoroutine(ShowStartTurnEffect(SecondTurnEffect_SpawnPoint));

                    //1秒待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //バッファーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("BufferTurnText");

                    //バッファーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //2秒待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //もしバッファーにHPデバフが付与されていたらダメージを減らし、デバフカウントも減らす
                    if (buffer.IsHPDebuff)
                    {
                        //ダメージを減らし、デバフカウントも減らす
                        buffer.PlayerCurrentHP -= dragon.HPDebuffPower;
                        buffer.DebuffCount--;

                        //デバフカウントが0になったらHPデバフを解除、また解除したことを通知する
                        if (buffer.DebuffCount <= 0)
                        {
                            buffer.IsHPDebuff = false;
                            BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                            //デバフ解除時の状況テキストを非表示にする
                            StartCoroutine(buffer.PlayerOffDebuffText());

                            //2秒待つ
                            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
                        }
                    }

                    //バッファーのターン開始
                    await BufferTurn(token);

                    await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);

                    //敵の生存状況を確認（生存リストが空ならループを止める）
                    if (GameClearCheck())
                    {
                        return;
                    }
                }

                if (healer.IsAlive)
                {
                    //指定した位置にターン開始エフェクト生成
                    StartCoroutine(ShowStartTurnEffect(ThirdTurnEffect_SpawnPoint));

                    //1秒待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //ヒーラーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("HealerTurnText");

                    //ヒーラーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //2秒待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //もしヒーラーにHPデバフが付与されていたらダメージを減らし、デバフカウントも減らす
                    if (healer.IsHPDebuff)
                    {
                        //ダメージを減らし、デバフカウントも減らす
                        healer.PlayerCurrentHP -= dragon.HPDebuffPower;
                        healer.DebuffCount--;

                        //デバフカウントが0になったらHPデバフを解除、また解除したことを通知する
                        if (healer.DebuffCount <= 0)
                        {
                            healer.IsHPDebuff = false;
                            BattleActionTextManager.Instance.ShowBattleActionText("HealerOffDebuff");

                            //デバフ解除時の状況テキストを非表示にする
                            StartCoroutine(attacker.PlayerOffDebuffText());

                            //2秒待つ
                            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
                        }
                    }

                    //ヒーラーのターン開始
                    await HealerTurn(token);

                    await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);

                    //敵の生存状況を確認（生存リストが空ならループを止める）
                    if (GameClearCheck())
                    {
                        return;
                    }
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

                await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);

                //UIマネージャーから敵ターンUIを非表示
                UIManager.Instance.EnemyTurnUI.SetActive(false);

                //敵ターン開始
                await EnemyTurn(token);
            }
        }
    }

    /// <summary>
    /// ユニタスクアタッカーターン
    /// </summary>
    /// <param name="token">キャンセルされるトークン</param>
    /// <returns>UniTaskを止める処理</returns>
    async UniTask AttackerTurn(CancellationToken token)
    {
        //アタッカーがを使っていたらカウントを減らす
        if (attacker.IsUseSpecial)
        {
            //UIマネージャーからスキルカウントUIを表示
            UIManager.Instance.SpecialLimitCountText.SetActive(true);

            //必殺制限カウントを減らす
            attacker.SpecialLimitCount--;

            //必殺制限カウントが0になったら表示制限フラグfalse
            if (attacker.SpecialLimitCount <= 0)
            {
                attacker.SpecialDebuffCount = 0;

                //UIマネージャーから必殺カウント制限カウントのUIを非表示
                UIManager.Instance.SpecialLimitCountText.SetActive(false);
                attacker.IsUseSpecial = false;
            }
            else
            {
                //必殺制限カウント数表示
                specialLimitCountUGUI.text = $"{attacker.SpecialLimitCount}";
            }
        }

        //アタッカーの行動が終わるまでループし続ける
        while (!attacker.IsAttackerAction)
        {
            //ポーズモード可能
            canPoseMode = true;

            //A・S・Fキーいずれかのキーが押されるまで処理を待つ（キャンセルトークンが呼ばれたらキャンセルする）
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F),
        cancellationToken: token);

            if (Input.GetKeyDown(KeyCode.A))
            {
                //アタッカーの通常攻撃
                attacker.NormalAttack();
            }

            else if (Input.GetKeyDown(KeyCode.S))
            {
                //スキル制限フラグがfalseならスキルを実行
                attacker.PlayerSkill();
            }

            else if (Input.GetKeyDown(KeyCode.F))
            {
                //必殺制限フラグがfalseならスペシャルを実行
                if (attacker.IsUseSpecial == false) attacker.SpecialSkill();

                else if (attacker.IsUseSpecial)
                {
                    //JSONファイルから必殺使用不可通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSpecial");

                    //必殺使用不可UIを非表示にするコールチン
                    StartCoroutine(HidePlayerActionText());
                }

            }
        }

        //アタッカーの行動フラグがtrueになったら制限UIを非表示
        if (attacker.IsAttackerAction)
        {
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);

            canPoseMode = false;
        }

        //TryCatchを使いエラーを防ぐ
        try
        {
            //アタッカーの行動フラグを待つ（キャンセルトークンが呼ばれたらキャンセルする）
            await UniTask.WaitUntil(() => attacker.IsAttackerAction, cancellationToken: token);

            IsPlayerTurn = false;
        }

        catch (OperationCanceledException)
        {
            return;
        }
    }

    /// <summary>
    /// ユニタスクバッファーターン
    /// </summary>
    /// <param name="token">キャンセルされるトークン</param>
    /// <returns>ユニタスクを止める処理</returns>
    async UniTask BufferTurn(CancellationToken token)
    {
        //バッファーが必殺を使っていたらカウントを減らす
        if (buffer.IsUseSpecial)
        {
            //バッファーの必殺制限カウントを表示
            UIManager.Instance.SpecialLimitCountText.SetActive(true);
            buffer.SpecialLimitCount--;

            if (buffer.SpecialLimitCount <= 0)
            {
                buffer.SpecialDebuffCount = 0;

                //バッファーの必殺制限カウントを非表示
                UIManager.Instance.SpecialLimitCountText.SetActive(false);
                buffer.IsUseSpecial = false;
            }
            else
            {
                specialLimitCountUGUI.text = $"{buffer.SpecialLimitCount}";
            }
        }

        while (!buffer.IsBufferAction)
        {
            canPoseMode = true;

            //A・S・Fキーいずれかのキーが押されるまで処理を待つ（キャンセルトークンが呼ばれたらキャンセルする）
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F),
        cancellationToken: token);

            if (Input.GetKeyDown(KeyCode.A))
            {
                //バッファーの通常攻撃
                buffer.NormalAttack();

                //通常攻撃時はすぐにバッファーターンを終了
                buffer.IsBufferAction = true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                //バッファーのスキル
                buffer.PlayerSkill();
                break;
            }

            else if (Input.GetKeyDown(KeyCode.F))
            {
                //必殺制限フラグがfalseなら必殺を実行
                if (buffer.IsUseSpecial == false)
                {
                    //バッファーの必殺実行
                    buffer.SpecialSkill();

                    //必殺の時はすぐにバッファーターンを終了
                    buffer.IsBufferAction = true;
                }
                else
                {
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSpecial");
                    StartCoroutine(HidePlayerActionText());
                }
            }
        }

        //ポーズフラグを解除して制限カウントUIを非表示
        if (buffer.IsBufferAction)
        {
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);
            canPoseMode = false;
        }

        //TryCatchを使いエラーを防ぐ
        try
        {
            //バッファー行動するまで処理を待つ（キャンセルトークンが呼ばれたらキャンセルする）
            await UniTask.WaitUntil(() => buffer.IsBufferAction, cancellationToken: token);

            IsPlayerTurn = false;
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    /// <summary>
    /// ユニタスクヒーラーターン
    /// </summary>
    /// <param name="token">キャンセルされるトークン</param>
    /// <returns>ユニタスクを止める処理</returns>
    async UniTask HealerTurn(CancellationToken token)
    {

        //ヒーラーの必殺制限フラグがtrueなら必要制限カウント減らす
        if (healer.IsUseSpecial)
        {
            //ヒーラーの必殺制限カウントを表示
            UIManager.Instance.SpecialLimitCountText.SetActive(true);
            healer.SpecialLimitCount--;

            if (healer.SpecialLimitCount <= 0)
            {
                healer.SpecialLimitCount = 0;

                UIManager.Instance.SpecialLimitCountText.SetActive(false);
                healer.IsUseSpecial = false;
            }
            else
            {
                specialLimitCountUGUI.text = $"{healer.SpecialLimitCount}";
            }
        }

        while (!healer.IsHealerAction)
        {
            canPoseMode = true;

            //A・S・Fキーいずれかのキーが押されるまで処理を待つ（キャンセルトークンが呼ばれたらキャンセルする）
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F),
        cancellationToken: token);

            if (Input.GetKeyDown(KeyCode.A))
            {
                //ヒーラーの通常攻撃
                healer.NormalAttack();

                //通常攻撃時はすぐにヒーラーターン終了
                healer.IsHealerAction = true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                //ヒーラーのスキル
                healer.PlayerSkill();
                break;
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                if (healer.IsUseSpecial == false)
                {
                    //ヒーラーの必殺
                    healer.SpecialSkill();

                    //通常攻撃時はすぐにヒーラーターン終了
                    healer.IsHealerAction = true;
                }
                else
                {
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSpecial");
                    StartCoroutine(HidePlayerActionText());
                }

            }
        }

        //ヒーラーの行動が終わればスキルと必殺の制限カウントUIを非表示
        if (healer.IsHealerAction)
        {
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);

            canPoseMode = false;
        }

        try
        {
            await UniTask.WaitUntil(() => healer.IsHealerAction, cancellationToken: token);
            IsPlayerTurn = false;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("キャンセルされた");
            return;
        }
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
