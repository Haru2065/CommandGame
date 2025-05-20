using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ドラゴンのステータス
/// </summary>
public class Dragon : BaseEnemyStatus
{
    [SerializeField]
    [Tooltip("ドラゴンのHPBar")]
    private Slider DragonHPBar;

    [SerializeField]
    [Tooltip("ドラゴンの必殺の攻撃力")]
    private int dragonSpecialAttackPower;

    //ドラゴンのターンカウント
    private int turnCount = 0;

    /// <summary>
    /// ターンカウントのゲッターセッター
    /// </summary>
    public int TurnCount
    {
        get => turnCount; set => turnCount = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        //ドラゴンのパラメータを設定
        SetEnemyParameters();

        //ドラゴンを生存状態にする
        EnemyIsAlive = true;
    }

    public async UniTask DragonAction(List<BasePlayerStatus> playerParty)
    {
        if (!EnemyIsAlive) return;

        await DoAttack(playerParty);
    }

    public async UniTask DoAttack(List<BasePlayerStatus> playerParty)
    {
        TurnCount++;

        int puttern = (turnCount - 1) % 3;

        switch (puttern)
        {
            case 0:
                RandomSelect();
                break;

            case 1:
                BreathAllAttack(playerParty);

                await UniTask.Delay(TimeSpan.FromSeconds(2f));
                break;

            case 2:
                SpecialAllAttack(playerParty);

                await UniTask.Delay(TimeSpan.FromSeconds(2f));
                break;
        }
    }

    /// <summary>
    /// ドラゴンのパラメータ設定メソッド
    /// </summary>
    protected override void SetEnemyParameters()
    {
        //リンク機能を使って、敵のデータベースの最初の要素から敵のIDを取得
        var enemyData = EnemyDataBase.EnemyParameters.FirstOrDefault(e => e.EnemyNameData == EnemyID);

        //合致すればパラメータを設定
        if (enemyData != null)
        {
            //現在のHPを敵データに設定されている最大体力にする
            EnemyMaxHP = enemyData.EnemyMaxHPData;

            //ドラゴンの現在の体力も最大体力に設定
            EnemyCurrentHP = EnemyMaxHP;

            //ドラゴンのHPバーも最大体力に設定
            DragonHPBar.maxValue = EnemyCurrentHP;
            DragonHPBar.value = EnemyCurrentHP;

            //ドラゴンのHPバーの最小は0に設定
            DragonHPBar.minValue = 0;

            //ドラゴンの攻撃力を敵データの攻撃力に設定
            EnemyAttackPower = enemyData.EnemyAttackPowerData;
        }
        else
        {
            Debug.LogError($"{EnemyID}のデータがデータベースに存在しません!");
        }
    }

    /// <summary>
    /// ランダムでプレイヤー単体に攻撃するメソッド
    /// </summary>
    public override void RandomSelect()
    {
        //ドラゴンの単体攻撃音を再生
        EnemySE.Instance.Play_DragonSingleAttackSE();

        //ベースのランダムセレクトメソッドを実行
        base.RandomSelect();
    }

    /// <summary>
    /// 通常の全体攻撃
    /// </summary>
    /// <param name="players"></param>
    private void BreathAllAttack(List<BasePlayerStatus> playerParty)
    {
        //ドラゴンのブレス攻撃を再生
        EnemySE.Instance.Play_DragonBreathSE();

        List<BasePlayerStatus> attackPlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        if (attackPlayers.Count > 0)
        {
            foreach (var player in attackPlayers)
            {
                player.PlayerOnDamage(EnemyAttackPower);
            }
        }
        else
        {
            Debug.Log("攻撃対象がいません");
        }

    }

    /// <summary>
    /// 必殺攻撃(全体に攻撃を与えてさらに2ターン継続でデバフを付与）
    /// </summary>
    /// <param name="players"></param>
    private void SpecialAllAttack(List<BasePlayerStatus> playerParty)
    {
        //ドラゴンの必殺攻撃音を再生
        EnemySE.Instance.Play_DragonSpecialAttackSE();

        List<BasePlayerStatus> attackPlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        if (attackPlayers.Count > 0)
        {
            foreach (var player in attackPlayers)
            {
                player.PlayerOnDamage(dragonSpecialAttackPower);
            }
        }
        else
        {
            Debug.Log("攻撃対象がいません");
        }
    }

    /// <summary>
    /// ドラゴンのHPを表示する処理
    /// </summary>
    protected override void Update()
    {
        enemyHPUGUI.text = $"{EnemyCurrentHP} / {EnemyMaxHP}";

        if(!EnemyIsAlive)
        {
            EnemyCurrentHP = 0;
        }
    }

    /// <summary>
    /// ドラゴンのダメージ処理
    /// </summary>
    /// <param name="damage">プレイヤーからの攻撃＝ダメージ数</param>
    public override void EnemyOnDamage(int damage)
    {
        EnemyCurrentHP -= damage;
        DragonHPBar.value = EnemyCurrentHP;

        //ドラゴンのHPが０より下にいかないようにする
        if (EnemyCurrentHP <= 0)
        {
            EnemyCurrentHP = 0;
            DragonHPBar.value = EnemyCurrentHP;

            EnemyIsAlive = false;

            //生存リストと初期ターゲットを設定するリストの消去
            Stage2BattleSystem.Instance.aliveEnemies.Remove(this);
            PlayerTargetSelect.Instance.RemoveSetTarget(this);

            //アタッカーの全体攻撃のリストから削除
            Attacker.RemoveDeadEnemies();

            //ドラゴンのオブジェクト消去するコールチンスタート
            StartCoroutine(DestroyObject());
        }
    }

    /// <summary>
    /// オブジェクトを消去するコールチン
    /// </summary>
    /// <returns>1フレーム待つ</returns>
    protected override IEnumerator DestroyObject()
    {
        //ドラゴンのオブジェクトを消去
        Destroy(gameObject);

        yield return null;
    }
}
