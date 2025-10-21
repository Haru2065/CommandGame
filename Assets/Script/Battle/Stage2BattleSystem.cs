using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using System.IO;

public class Stage2BattleSystem : BaseBattleManager
{
    [SerializeField]
    [Tooltip("3体のスケルトン")]
    private Skeleton skeleton1, skeleton2, skeleton3;

    [SerializeField]
    [Tooltip("スキル制限カウントのTMProUGUI")]
    private TextMeshProUGUI skillLimitCountUGUI;

    [SerializeField]
    [Tooltip("必殺制限カウントのTMProUGUI")]
    private TextMeshProUGUI specialLimitCountUGUI;

    // Start is called before the first frame update
    protected override async void Start()
    {
        //ベースのバトルシステムから初期設定を行う
        base.Start();

        //ステージのセーブデータを読み込む
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
            IsUnlockStage3 = false;
        }

        //UNiTaskをキャンセルするトークンを生成
        cts = new CancellationTokenSource();

        canPoseMode = false;

        //バトルループ開始
        //キャンセルトークンを渡す
        await BattleLoop(cts.Token);
    }

    /// <summary>
    /// ポーズモードにする
    /// </summary>
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
    /// UniTaskバトルのループ処理
    /// </summary>
    /// <param name="token">キャンセルできる処理/param>
    /// <returns>プレイヤーが行動するまで待つ</returns>
    async UniTask BattleLoop(CancellationToken token)
    {
        //ゲームクリア・ゲームオーバーのフラグがtrueならループを止めてキャンセルする
        while (!(isGameClear || isGameOver || token.IsCancellationRequested))
        {
            if (IsPlayerTurn)
            {
                //UIマネージャーからプレイヤーターン表示
                UIManager.Instance.PlayerTurnUI.SetActive(true);

                //1フレーム待つ(キャンセルトークンが呼ばれたらキャンセル）
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

                    //1秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //アタッカーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //アタッカーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //2秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //ステータスウィンドウを開くボタンを押せるようにする
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //アタッカーターン開始
                    await PlayerTurnAction(attacker, KeyCode.A,KeyCode.S, KeyCode.F, token);

                    //デバフが付与されているかチェック
                    statusDebuffCheck(attacker,"AttackerOffDebuff");

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

                    //バッファーのターン開始
                    await PlayerTurnAction(buffer, KeyCode.A, KeyCode.S, KeyCode.F,token);

                    //デバフが付与されているかチェック
                    statusDebuffCheck(attacker, "AttackerOffDebuff");

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

                    //2秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //ステータスウィンドウを開くボタンを押せるようにする
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //ヒーラーのターン開始
                    await PlayerTurnAction(healer, KeyCode.A, KeyCode.S, KeyCode.F, token);

                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //敵の生存状況を確認（生存リストが空ならループを止める）
                    if (GameClearCheck())
                    {
                        return;
                    }

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

                await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);

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
    protected override async UniTask PlayerTurnAction(BasePlayerStatus player, KeyCode normalKey, KeyCode skillKey, KeyCode specialKey, CancellationToken token)
    {
        //スキン制限中ならUIを表示にして制限カウントを減らす
        if (player.IsUseSkill)
        {
            //UIマネージャーからスキルカウントUIを表示
            UIManager.Instance.SkillLimitCountText.SetActive(true);

            player.SkillLimitCount--;

            //スキン制限カウントが0になったらUIを非表示にして使用可能にする
            if (player.SkillLimitCount <= 0)
            {
                player.SkillLimitCount = 0;

                //UIマネージャーからスキル制限カウントUI非表示
                UIManager.Instance.SkillLimitCountText.SetActive(false);
                player.IsUseSkill = false;
            }

            //制限継続中ならそのままUIを表示する
            else
            {
                //スキル制限カウント数を表示
                skillLimitCountUGUI.text = $"{"{" + player.SkillLimitCount + "}" }";
            }
        }

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

                //通常攻撃ではすぐにプレイヤー行動フラグをtrueにする
                player.IsPlayerAction = true;
            }
            else if (Input.GetKeyDown(skillKey))
            {
                //スキル制限フラグがfalseならスキルを実行
                if (player.IsUseSkill == false)
                {
                    player.PlayerSkill();
                    break;
                }

                else
                {
                    //JSONファイルからスキルは使用不可通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSkill");

                    //スキル使用不可UIを非表示にするコールチン
                    StartCoroutine(HidePlayerActionText());
                }
            }

            else if (Input.GetKeyDown(specialKey))
            {
                //必殺制限フラグがfalseならスペシャルを実行
                if (player.IsUseSpecial == false)
                {
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
            //プレイヤーのスキル制限カウントと必殺制限カウントのテキスト非表示
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);
            canPoseMode = false;
        }

        //1フレーム待つ(キャンセルできる処理）
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

        //tryCatchを使いエラーを防ぐ
        try
        {
            //プレイヤーが行動を終了するまで処理を待つ
            await UniTask.WaitUntil(() => player.IsPlayerAction, cancellationToken: token);

            //プレイヤー全体の行動が終わったらプレイヤーターン終了
            if(attacker.IsPlayerAction && buffer.IsPlayerAction &&healer.IsPlayerAction)
            {
                IsPlayerTurn = false;
            }
        }
        catch(OperationCanceledException)
        {
            Debug.Log("キャンセルされた");
        }

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

            if (!IsUnlockStage3)
            {
                //ステージ3を解放
                IsUnlockStage3 = true;

                //ステージデータを保存
                SaveManager.SaveStage();
            }

            //2秒遅れてクリアUIを表示し、レベルアップ処理を行う
            Invoke("DelayGameClearUI", 2);

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

            //キャンセル処理を実行し、キャンセルトークンを破棄しリソースを解放
            cts.Cancel();
            cts.Dispose();

            //ゲームオーバーなのでtrueを返す
            return true;
        }
        //味方が生きているのでfalseを返す
        return false;
    }

    /// <summary>
    /// ユニタスク敵のターン
    /// </summary>
    /// <param name="token">キャンセルできる処理/param>
    /// <returns>ユニタスクを止める処理</returns>
    async UniTask EnemyTurn(CancellationToken token)
    {
        if (skeleton1.EnemyIsAlive)
        {
            //スケルトン1ターン
            await SkeltonTurn(skeleton1, token);
            if (GameOverCheck()) return;
        }
        if (skeleton2.EnemyIsAlive)
        {
            //スケルトン2ターン
            await SkeltonTurn(skeleton2, token);
            if (GameOverCheck()) return;
        }

        if (skeleton3.EnemyIsAlive)
        {
            //スケルトン3ターン
            await SkeltonTurn(skeleton3, token);
            if (GameOverCheck()) return;
        }

        IsPlayerTurn = true;
    }

    /// <summary>
    /// スケルトンターン
    /// </summary>
    /// <param name="skeleton">スケルトンステータス</param>
    /// <param name="token">キャンセルできる処理</param>
    async UniTask SkeltonTurn(Skeleton skeleton, CancellationToken token)
    {
        //スケルトンアクション
        await skeleton.SkeletonAction(skeleton);

        //1フレーム待つ
        await UniTask.Yield();
    }

    /// <summary>
    /// プレイヤーのデバフ状況を確認するメソッド
    /// </summary>
    /// <param name="debuffPlayer">デバフ対象者</param>
    /// <param name="playerOffDebuffText">Jsonのデバフ解除通知ID</param>
    private void statusDebuffCheck(BasePlayerStatus debuffPlayer, string playerOffDebuffText)
    {
        //プレイヤーがデバフ付与されていなかったらなにもしない
        if (!debuffPlayer.IsDebuff) return;

        //もしプレイヤーがデバフが付与されていたらデバフカウントを減らす
        else if (debuffPlayer.IsDebuff)
        {
            debuffPlayer.DebuffCount--;

            //デバフカウントが0になったら攻撃力をもとに戻して、デバフ解除
            if (debuffPlayer.DebuffCount <= 0)
            {
                debuffPlayer.AttackPower = debuffPlayer.PlayerResetAttackPower;
                debuffPlayer.IsDebuff = false;

                // JSONファイルで設定されたアタッカーのデバフ解除通知を表示
                BattleActionTextManager.Instance.ShowBattleActionText(playerOffDebuffText);

                //デバフ解除時の状況テキストを非表示にする
                StartCoroutine(debuffPlayer.PlayerOffDebuffText());
            }
        }

        //もし特殊デバフが付与されていなかったら何もしない
        if(!debuffPlayer.IsSpecialDebuff) return ;
        
        //もし特殊デバフが付与されていたら特殊デバフのカウントを減らす
        else if(debuffPlayer.IsSpecialDebuff)
        {
            debuffPlayer.SpecialDebuffCount--;

            // 特殊デバフのカウントが0になったら、攻撃力を元に戻してデバフを解除
            if (debuffPlayer.SpecialDebuffCount <= 0)
            {
                debuffPlayer.AttackPower = debuffPlayer.PlayerResetAttackPower;
                debuffPlayer.IsSpecialDebuff = false;

                // JSONファイルで設定されたアタッカーの特殊デバフ解除通知を表示
                BattleActionTextManager.Instance.ShowBattleActionText(playerOffDebuffText);

                // デバフ解除時の状況テキストを非表示にする
                StartCoroutine(debuffPlayer.PlayerOffDebuffText());
            }
        }
    }


}
