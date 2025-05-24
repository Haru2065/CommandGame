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

    [SerializeField]
    [Tooltip("ドラゴンのデバフ力(HPを削る値)")]
    private int hpDebuffPower;

    /// <summary>
    /// ドラゴンのデバフ力のゲッターセッター
    /// </summary>
    public int HPDebuffPower
    {
        get => hpDebuffPower;
        set => hpDebuffPower = value;
    }

    [SerializeField]
    [Tooltip("ドラゴンの単体攻撃エフェクト")]
    private GameObject onleyAttackEffect;

    [SerializeField]
    [Tooltip("ドラゴンのブレス攻撃のエフェクト")]
    private GameObject breathEffect;

    [SerializeField]
    [Tooltip("ドラゴン側の攻撃エフェクトのスポーン位置")]
    private Transform dragonEffectSpawnPoint;

    [SerializeField]
    [Tooltip("必殺の攻撃エフェクト")]
    private GameObject specialEffect;

    [SerializeField]
    [Tooltip("必殺の攻撃エフェクトのスポーン位置")]
    private Transform specialEffectSpawnPoint;

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

    /// <summary>
    /// ドラゴンの行動
    /// </summary>
    /// <param name="playerParty">攻撃対象者のリスト</param>
    /// <returns></returns>
    public async UniTask DragonAction(List<BasePlayerStatus> playerParty)
    {
        //ドラゴンが生存していなかったら実行しない
        if (!EnemyIsAlive) return;

        //ドラゴンの攻撃パターン通り攻撃を実行
        await DoAttack(playerParty);
    }

    /// <summary>
    /// ドラゴンのパターン攻撃
    /// </summary>
    /// <param name="playerParty"></param>
    /// <returns></returns>
    public async UniTask DoAttack(List<BasePlayerStatus> playerParty)
    {
        //ターンのカウントを1増やす
        turnCount++;

        //
        int puttern = (turnCount - 1) % 3;

        //攻撃パターン
        switch (puttern)
        {
            //パターン1ランダムに敵に攻撃を行う
            case 0:

                BattleActionTextManager.Instance.ShowBattleActionText(″DragonNormalAttack“)；
                
                await UniTask.Delay(TimeSpan.FromSeconds(1f)；

                BattleActionTextManager.Instance.

                RandomSelect();
                break;
            
            //パターン2ブレス攻撃全体攻撃
            case 1:
                BreathAllAttack(playerParty);

                //2フレーム待つ
                await UniTask.Delay(TimeSpan.FromSeconds(2f));
                break;

            //パターン3必殺攻撃プレイヤーに全体攻撃+ダメージデバフ付与
            case 2:
                SpecialAllAttack(playerParty);

                //2フレーム待つ
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

            HPDebuffPower = enemyData.DebuffPowerData;
        }
        else
        {
            Debug.LogError($"{EnemyID}のデータがデータベースに存在しません!");
        }
    }

    /// <summary>
    /// ランダムでプレイヤー単体に攻撃するメソッド
    /// </summary>
    public override BasePlayerStatus RandomSelect()
    {
        //ドラゴンの単体攻撃音を再生
        EnemySE.Instance.Play_DragonSingleAttackSE();

        //ベースのランダムセレクトメソッドを実行
        BasePlayerStatus target = base.RandomSelect();

        //攻撃対象に単体攻撃エフェクトを生成
        if (target != null)
        {
            GameObject effectInstance = Instantiate(onleyAttackEffect,target.transform.position, Quaternion.identity);

            //2フレーム後エフェクトを消去
            Destroy(effectInstance, 2f);
        }

        return target;
    }

    /// <summary>
    /// 通常の全体攻撃
    /// </summary>
    /// <param name="players"></param>
    private void BreathAllAttack(List<BasePlayerStatus> playerParty)
    {
        //ドラゴンのブレス攻撃を再生
        EnemySE.Instance.Play_DragonBreathSE();

        //通常全体攻撃のエフェクトを生成
        GameObject effectInstance = Instantiate(breathEffect,dragonEffectSpawnPoint.position, Quaternion.Euler(0f,0f,-160f));

        //2フレーム後エフェクトを消去
        Destroy(effectInstance,2f);

        //対象のプレイヤーが生存していたら、リストにいれる（生存者を取得)
        List<BasePlayerStatus> attackTargetPlayers = StartAlivePlayers.FindAll(player => player.IsAlive);


        //攻撃対象がいれば攻撃対象リストに入っているプレイヤーに攻撃
        if (attackTargetPlayers.Count > 0)
        {
            foreach (var player in attackTargetPlayers)
            {

                //プレイヤーにダメージを与える
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

        //必殺エフェクトをプレイヤーキャラの位置に生成
        GameObject dragonEffectInstance = Instantiate(specialEffect, specialEffectSpawnPoint.position, Quaternion.identity);

        // エフェクトスケールを変更（2倍）
        dragonEffectInstance.transform.localScale = new Vector3(2f, 2f, 2f);

        //3フレーム後消去
        Destroy(dragonEffectInstance, 3f);

        //対象のプレイヤーが生存していたら、リストにいれる（生存者を取得)
        List<BasePlayerStatus> attackPlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        // 攻撃対象がいれば攻撃対象リストに入っているプレイヤーに攻撃
        if (attackPlayers.Count > 0)
        {
            foreach (var player in attackPlayers)
            {
                //プレイヤーにダメージを与えて、やけどのデバフ（毎ターンHPを減らす2ターン継続）
                player.PlayerOnDamage(dragonSpecialAttackPower);
                player.IsHPDebuff = true;

                //デバフ継続カウントを2に設定
                player.DebuffCount = 2;
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

        //ドラゴンが生存していなかったら現在のHPは0
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
