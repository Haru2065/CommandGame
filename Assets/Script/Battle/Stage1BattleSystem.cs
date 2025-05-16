using System.Collections;
using UnityEngine;

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
    protected override void Start()
    {
        base.Start();

        PlayerTargetSelect.Instance.SetStartBattleTarget();

        canPoseMode = false;

        //バトルループ開始
        StartCoroutine(BattleLoop());
    }

    /// <summary>
    /// バトルループ
    /// </summary>
    /// <returns></returns>
    IEnumerator BattleLoop()
    {
        //ゲームクリアフラグか、ゲームオーバーフラグがtrueになるまでループし続ける
        while (true)
        {
            if (isGameClear || isGameOver) yield break;

            if (IsPlayerTurn)
            {
                //UIマネージャーからプレイヤーターンUIを表示
                UIManager.Instance.PlayerTurnUI.SetActive(true);

                yield return new WaitForSeconds(2f);

                //UIマネージャーからプレイヤーターンUIを非表示
                UIManager.Instance.PlayerTurnUI.SetActive(false);

                //UIマネージャーからプレイヤーターン時に表示するUIを表示
                UIManager.Instance.StartPlayerTurnUI();

                //アタッカーが生存していたら処理を実行
                if (attacker.IsAlive)
                {
                    //アタッカーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //指定した位置にターン開始エフェクト生成
                    yield return ShowStartTurnEffect(FirstTurnEffect_SpawnPoint);

                    //アタッカーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //アタッカーターン開始
                    yield return AttackerTurn();

                    yield return new WaitForSeconds(2f);

                    //敵の生存状況を確認する
                    GameClearCheck();

                    //ゲームクリアならループを止める
                    if (isGameClear)yield break;
                }

                //バッファーが生存していたら処理を実行
                if (buffer.IsAlive)
                {
                    //バッファーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("BufferTurnText");

                    yield return ShowStartTurnEffect(SecondTurnEffect_SpawnPoint);

                    //バッファーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //バッファーターン開始
                    yield return BufferTurn();

                    yield return new WaitForSeconds(2);

                    GameClearCheck();

                    if (isGameClear) yield break;
                }

                //ヒーラーが生存していたら処理を実行
                if (healer.IsAlive)
                {

                    //ヒーラーのターン開始通知表示
                    BattleActionTextManager.Instance.ShowBattleActionText("HealerTurnText");

                    yield return ShowStartTurnEffect(ThirdTurnEffect_SpawnPoint);

                    //ヒーラーのターン開始通知非表示
                    StartCoroutine(HidePlayerActionText());

                    //ヒーラーターン開始
                    yield return HealerTurn();

                    yield return new WaitForSeconds(2);

                    GameClearCheck();

                    if (isGameClear) yield break ;
                }
            }

            //敵ターン
            else
            {
                //各プレイヤーの行動フラグをfalseにする
                attacker.IsAttackerAction = false;
                buffer.IsBufferAction = false;
                healer.IsHealerAction = false;

                //UIマネージャーから敵ターンUIを表示
                UIManager.Instance.EnemyTurnUI.SetActive(true);

                yield return new WaitForSeconds(3f);

                //UIマネージャーから敵ターンUIを非表示
                UIManager.Instance.EnemyTurnUI.SetActive(false);

                //敵ターン開始
                yield return EnemyTurn();
            }
        }

    }

    /// <summary>
    /// アタッカーターン処理
    /// </summary>
    /// <returns>処理が行われるまで待機</returns>
    IEnumerator AttackerTurn()
    {
        //ポーズモードにできるかのフラグをtrueに
        canPoseMode = true;

        //A・S・Fキーのいずれかが押されるまで待機し、押されたキーに応じた攻撃を実行する。
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F));

        if (Input.GetKeyDown(KeyCode.A))
        {
            //アタッカーの通常攻撃
            attacker.NormalAttack();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            //アタッカーのスキル
            attacker.PlayerSkill();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            //アタッカーの必殺
            attacker.SpecialSkill();
        }

        //アタッカーの行動が終わるまで処理を待つ
        yield return new WaitUntil(() => attacker.IsAttackerAction);

        //ポーズモードのできるかのフラグをfalse
        canPoseMode = false;
    }

    /// <summary>
    /// バッファーターン処理
    /// </summary>
    /// <returns>処理が行われるまで待機</returns>
    IEnumerator BufferTurn()
    {
        canPoseMode = true;

        //A・S・Fキーのいずれかが押されるまで待機し、押されたキーに応じた攻撃を実行する。
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F));

        if (Input.GetKeyDown(KeyCode.A))
        {
            //バッファー通常攻撃
            buffer.NormalAttack();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            //バッファーのスキル
            buffer.PlayerSkill();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            //バッファーの必殺
            buffer.SpecialSkill();
        }

        //バッファーの行動が終わるまで処理を待つ
        yield return new WaitUntil(() => buffer.IsBufferAction);

        canPoseMode = false;
    }

    /// <summary>
    /// ヒーラーのターン処理
    /// </summary>
    /// <returns>処理が行われるまで待機</returns>
    IEnumerator HealerTurn()
    {
        canPoseMode = true;

        //A・S・Fキーのいずれかが押されるまで待機し、押されたキーに応じた攻撃を実行する。
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F));

        if (Input.GetKeyDown(KeyCode.A))
        {
            //ヒーラーの通常攻撃
            healer.NormalAttack();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            //ヒーラーのスキル
            healer.PlayerSkill();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            //ヒーラーの必殺
            healer.SpecialSkill();
        }

        //ヒーラーの行動が終わるまで処理を待つ
        yield return new WaitUntil(() => healer.IsHealerAction);

        canPoseMode = false;
        
        //プレイヤーのターン終了
        IsPlayerTurn = false;
    }

    /// <summary>
    /// 敵ターン
    /// </summary>
    /// <returns>2フレーム待つ</returns>
    IEnumerator EnemyTurn()
    {
        //スライムが生存していたら処理を実行
        if (slime1.EnemyIsAlive)
        {
            //スライム１のターン開始
            yield return SlimeTurn(slime1);

            //プレイヤーの生存状況を確認
            GameOverCheck();

            //ゲームオーバーフラグがtrueならループを止める
            if (isGameOver) yield break;
        }

        if (slime2.EnemyIsAlive)
        {
            yield return new WaitForSeconds(2f);

            //スライム2のターン開始
            yield return SlimeTurn(slime2);

            //プレイヤーの生存状況を確認
            GameOverCheck();

            if (isGameOver) yield break;
        }

        if (slime3.EnemyIsAlive)
        {
            yield return new WaitForSeconds(2f);

            //スライム3のターン開始
            yield return SlimeTurn(slime3);

            //プレイヤーの生存状況を確認
            GameOverCheck();

            if (isGameOver) yield break;
        }

        //敵ターン終了
        IsPlayerTurn = true;

        //1フレーム待つ
        yield return null;
    }

    /// <summary>
    /// スライムターン処理
    /// </summary>
    /// <param name="slime">スライムのステータス</param>
    /// <returns></returns>
    IEnumerator SlimeTurn(Slime slime)
    {
        //スライムの攻撃開始
        slime.SlimeAction();

        yield return null;
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

            foreach (var player in LevelUPPlayerList)
            {
                player.LevelUP();
            }

            //2秒遅れてクリアUIを表示
            Invoke("DelayGameClearUI", 2);
            return true;
        }
        //敵が生きているのでfalseを返す
        return false;
    }

    /// <summary>
    /// 遅れてクリアUIを表示するメソッド
    /// </summary>
    void DelayGameClearUI()
    {
        UIManager.Instance.GameClearUI();
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
