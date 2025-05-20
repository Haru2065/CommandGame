using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スケルトンのステータス
/// </summary>
public class Skeleton : BaseEnemyStatus
{
    [SerializeField]
    [Tooltip("スケルトンのデバフパワー")]
    private int debuffPower;

    [SerializeField]
    [Tooltip("スケルトンのHPバー")]
    public Slider SkeltonHPBar;

    // Start is called before the first frame update
    void Start()
    {
        //スケルトンのパラメータを設定
        SetEnemyParameters();

        //スケルトンを生存状態に
        EnemyIsAlive = true;
    }

    /// <summary>
    /// スケルトンのHPを表示する処理
    /// </summary>
    protected override void Update()
    {
        enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";
    }

    /// <summary>
    /// パラメータを設定するメソッド
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

            //スケルトンの現在の体力も最大に設定
            EnemyCurrentHP = EnemyMaxHP;

            //スケルトンのHPバーを最大体力に設定
            SkeltonHPBar.maxValue = EnemyCurrentHP;
            SkeltonHPBar.value = EnemyCurrentHP;

            //スケルトンのHPバーの最小は０に設定
            SkeltonHPBar.minValue = 0;

            //スケルトンの攻撃力をエネミーデータの攻撃力に設定
            EnemyAttackPower = enemyData.EnemyAttackPowerData;

            //スケルトンのデバフ力をエネミーデータのデバフ力に設定
            debuffPower = enemyData.DebuffPowerData;
        }
        else
        {
            Debug.LogError($"{EnemyID} のデータがデータベースに存在しません！");
        }
    }

    /// <summary>
    /// スケルトンが攻撃するUniTask
    /// </summary>
    public async UniTask SkeletonAction(Skeleton skelton)
    {
        //プレイヤーも攻撃するターゲットをランダムで設定
        RandomSelect();

        //2フレーム待つ
        await UniTask.Delay(TimeSpan.FromSeconds(2));

    }

    /// <summary>
    /// プレイヤーにランダムで攻撃するメソッド
    /// </summary>
    public override void RandomSelect()
    {
        //一度生存しているキャラのみでリストを整理する
        List<BasePlayerStatus> TargetAlivePlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        //リストにキャラがいれば実行
        if (TargetAlivePlayers.Count > 0)
        {
            //リストの中にあるプレイヤーキャラを選択してターゲットに設定
            BasePlayerStatus target = TargetAlivePlayers[UnityEngine.Random.Range(0, TargetAlivePlayers.Count)];

            target.PlayerOnDamage(EnemyAttackPower);

            switch (target)
            {
                case var _ when target == Attacker:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageAttacker");

                    StartCoroutine(HideEnemyActionText());

                    break;

                case var _ when target == Buffer:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageBuffer");

                    StartCoroutine(HideEnemyActionText());

                    break;

                case var _ when target == Healer:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageHealer");

                    StartCoroutine(HideEnemyActionText());

                    break;
            }

            //スケルトンの効果音再生
            EnemySE.Instance.Play_SkeletonAttackSE();

            //５０パーセントの確率かつプレイヤーがデバフ状態がfalseならデバフ付与
            if (UnityEngine.Random.Range(0, 100) < 50 && target.IsDebuff == false)
            {
                target.IsDebuff = true;

                target.DebuffCount = 3;

                //ターゲットの攻撃力をデバフ力分下げる
                target.AttackPower -= debuffPower;

                if (target.AttackPower < 50)
                {
                    target.AttackPower = 50;
                }

                //ターゲットがアタッカーかバッファーかヒーラーいずれかのどれかだった場合そのキャラの行動通知表示
                switch (target)
                {
                    //case文のwhenを使い追加条件を行う
                    case var _ when target == Attacker:

                        //JSONアタッカーの特殊デバフ付与通知表示
                        BattleActionTextManager.Instance.ShowBattleActionText("AttackerOnDebuff");

                        //行動通知UIを非表示
                        StartCoroutine(HideEnemyActionText());

                        break;

                    case var _ when target == Buffer:

                        //JSONバッファーの特殊デバフ付与通知表示
                        BattleActionTextManager.Instance.ShowBattleActionText("BufferOnDebuff");

                        //行動通知UIを非表示
                        StartCoroutine(HideEnemyActionText());

                        break;

                    case var _ when target == Healer:

                        //JSONヒーラーの特殊デバフ付与通知表示
                        BattleActionTextManager.Instance.ShowBattleActionText("HealerOnDebuff");

                        //行動通知UIを非表示
                        StartCoroutine(HideEnemyActionText());

                        break;

                    default:
                        break;

                }
            }

            //10%の確率でかつ、プレイヤーがデバフ状態で、特殊デバフ状態でなければ実行
            if (UnityEngine.Random.Range(0, 100) < 10 && target.IsDebuff && target.IsSpecialDebuff == false)
            {
                target.SpecialDebuffCount = 5;

                target.IsSpecialDebuff = true;

                //ターゲットの攻撃力をデバフ力分下げる
                target.AttackPower -= debuffPower;

                if (target.AttackPower < 20)
                {
                    target.AttackPower = 20;
                }

                //ターゲットがアタッカーかバッファーかヒーラーいずれかのどれかだった場合そのキャラの行動通知表示
                switch (target)
                {
                    // ターゲットが Attacker の場合
                    case var _ when target == Attacker:

                        //ダメージを受けたことを示すテキストを表示
                        BattleActionTextManager.Instance.ShowBattleActionText("AttackerOnSpecialDebuff");

                        // 一定時間後に敵のアクションテキストを非表示にする
                        StartCoroutine(HideEnemyActionText());
                        break;

                    // ターゲットが Attacker の場合
                    case var _ when target == Buffer:

                        BattleActionTextManager.Instance.ShowBattleActionText("BufferOnSpecialDebuff");

                        StartCoroutine(HideEnemyActionText());
                        break;

                    // ターゲットが Attacker の場合
                    case var _ when target == Healer:

                        BattleActionTextManager.Instance.ShowBattleActionText("HealerOnSpecialDebuff");

                        StartCoroutine(HideEnemyActionText());
                        break;

                    default:

                        break;
                }
            }
        }
        else
        {
            Debug.Log("攻撃対象がいません");
        }
    }

    /// <summary>
    /// スケルトンのダメージ処理
    /// </summary>
    /// <param name="damage">プレイヤーからの攻撃＝ダメージすう</param>
    public override void EnemyOnDamage(int damage)
    {
        EnemyCurrentHP -= damage;

        //スケルトンのHPが０より下にいかないようにする
        if (EnemyCurrentHP <= 0)
        {
            EnemyCurrentHP = 0;

            // UI更新
            SkeltonHPBar.value = EnemyCurrentHP;
            enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";

            //生存フラグをfalse
            EnemyIsAlive = false;

            //生存リストと初期ターゲットを設定するリストの消去
            Stage2BattleSystem.Instance.aliveEnemies.Remove(this);
            PlayerTargetSelect.Instance.RemoveSetTarget(this);

            //アタッカーの全体攻撃のリストから削除
            Attacker.RemoveDeadEnemies();

            //自身のオブジェクトを消去するコールチンスタート
            StartCoroutine(DestroyObject());
        }
        else
        {
            // 通常のダメージ時のUI更新
            SkeltonHPBar.value = EnemyCurrentHP;
            enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";
        }
    }


    /// <summary>
    /// オブジェクトを消去するコールチン
    /// </summary>
    /// <returns>1フレーム待つ</returns>
    protected override IEnumerator DestroyObject()
    {
        //スライムのオブジェクトを消去
        Destroy(gameObject);

        yield return null;
    }
}
