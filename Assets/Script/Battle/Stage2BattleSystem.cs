using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using System.IO;

public class Stage2BattleSystem : BaseBattleManager
{
    //UniTaskのキャンセルするトークンソース
    private CancellationTokenSource cts;

    [SerializeField]
    [Tooltip("3体のスケルトン")]
    private Skeleton skeleton1, skeleton2, skeleton3;

    [SerializeField]
    [Tooltip("スキル制限カウントのTMProUGUI")]
    private TextMeshProUGUI skillLimitCountUGUI;

    [SerializeField]
    [Tooltip("必殺制限カウントのTMProUGUI")]
    private TextMeshProUGUI specialLimitCountUGUI;

    [SerializeField]
    [Tooltip("終了ボタンスクリプト")]
    protected PushExitButton pushExitButton;

    

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
        if(pushExitButton.IsQuitGame)
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

                //２秒待つ（キャンセルトークンが呼ばれたらキャンセル）
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

                    //1秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //アタッカーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //アタッカーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //2秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //アタッカーターン開始
                    await AttackerTurn(token);

                    //2秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

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

                    //1秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //バッファーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("BufferTurnText");

                    //バッファーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //2秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //バッファーのターン開始
                    await BufferTurn(token);

                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

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

                    //1秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //ヒーラーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("HealerTurnText");

                    //ヒーラーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //2秒待つ（キャンセルトークンが呼ばれたらキャンセル）
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);


                    //ヒーラーのターン開始
                    await HealerTurn(token);

                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

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
    /// <param name="token">キャンセルできる処理</param>
    /// <returns>UniTaskを止める処理</returns>
    async UniTask AttackerTurn(CancellationToken token)
    {
        //アタッカーがスキルを使っていたらカウントを減らす
        if (attacker.IsUseSkill)
        {
            //UIマネージャーからスキルカウントUIを表示
            UIManager.Instance.SkillLimitCountText.SetActive(true);

            attacker.SkillLimitCount--;

            //スキル制限カウントが0なったらスキル制限フラグfalse
            if (attacker.SkillLimitCount <= 0)
            {
                attacker.SkillLimitCount = 0;

                //UIマネージャーからスキル制限カウントUI非表示
                UIManager.Instance.SkillLimitCountText.SetActive(false);
                attacker.IsUseSkill = false;
            }
            else
            {
                //スキル制限カウント数を表示
                skillLimitCountUGUI.text = $"{attacker.SkillLimitCount}";
            }
        }

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
                if (attacker.IsUseSkill == false) attacker.PlayerSkill();

                else
                {
                    //JSONファイルからスキルは使用不可通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSkill");

                    //スキル使用不可UIを非表示にするコールチン
                    StartCoroutine(HidePlayerActionText());
                }
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
    /// <param name="token">キャンセルできる処理</param>
    /// <returns>ユニタスクを止める処理</returns>
    async UniTask BufferTurn(CancellationToken token)
    {
        //バッファーがスキルを使っていたらカウントを減らす
        if (buffer.IsUseSkill)
        {
            //バッファーのスキル制限カウントを表示
            UIManager.Instance.SkillLimitCountText.SetActive(true);

            buffer.SkillLimitCount--;

            if (buffer.SkillLimitCount <= 0)
            {
                buffer.SkillLimitCount = 0;

                //バッファーのスキル制限カウントを非表示
                UIManager.Instance.SkillLimitCountText.SetActive(false);
                buffer.IsUseSkill = false;
            }
            else
            {
                //バッファーのスキル制限カウント数を表示
                skillLimitCountUGUI.text = $"{buffer.SkillLimitCount}";
            }
        }

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
                //スキル制限フラグがfalseならスキルを実行
                if (buffer.IsUseSkill == false)
                {
                    //バッファーのスキル
                    buffer.PlayerSkill();
                    break;
                }
                else
                {
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSkill");
                    StartCoroutine(HidePlayerActionText());
                }
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
    /// <param name="token">キャンセルできる処理</param>
    /// <returns>ユニタスクを止める処理</returns>
    async UniTask HealerTurn(CancellationToken token)
    {
        //ヒーラーのスキル制限フラグがtrueならスキル制限カウント減らす
        if (healer.IsUseSkill)
        {
            //ヒーラーのスキル制限カウントを表示
            UIManager.Instance.SkillLimitCountText.SetActive(true);
            healer.SkillLimitCount--;
            
            
            //スキル制限カウントが0になったら
            if (healer.SkillLimitCount <= 0)
            {

                healer.SkillLimitCount = 0;
                
                UIManager.Instance.SkillLimitCountText.SetActive(false);
                healer.IsUseSkill = false;
            }
            else
            {
                skillLimitCountUGUI.text = $"{healer.SkillLimitCount}";
            }
        }

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
                if (healer.IsUseSkill == false)
                {
                    //ヒーラーのスキル
                    healer.PlayerSkill();

                    break;
                }
                else
                {
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSkill");
                    StartCoroutine(HidePlayerActionText());
                }
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
}
