using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スライムステータススクリプト
/// </summary>
public class Slime : BaseEnemyStatus
{
    [SerializeField]
    [Tooltip("スライムHPバー")]
    private Slider slimeHPBar;

    // Start is called before the first frame update
    void Start()
    {
        //敵のパラメータを設定
        SetEnemyParameters();

        //スライムを生存状態に
        EnemyIsAlive = true;
    }

    /// <summary>
    /// スライムのHPを表示する処理
    /// </summary>
    protected override void Update()
    {
        enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";
    }

    /// <summary>
    /// パラメータを設定するメソッド
    /// 敵のデータベースから読みこみ
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
            
            //現在の敵の体力も最大に設定
            EnemyCurrentHP = EnemyMaxHP;

            //スライムのHPバーを最大体力に設定
            slimeHPBar.maxValue = EnemyCurrentHP;
            slimeHPBar.value = EnemyCurrentHP;

            //スライムのHPバーの最小は０に設定
            slimeHPBar.minValue = 0;

            //スライムの攻撃力をエネミーデータの攻撃力に設定
            EnemyAttackPower = enemyData.EnemyAttackPowerData;
        }
        else
        {
            Debug.LogError($"{EnemyID} のデータがデータベースに存在しません！");
        }
    }

    /// <summary>
    /// スライムのターンに行動するメソッド
    /// </summary>
    public async UniTask SlimeAction()
    {
        //ランダムでプレイヤーに攻撃する対象を選択して攻撃するメソッドを実行
        RandomSelect();
        
        //スライムの攻撃効果音再生
        EnemySE.Instance.Play_slimeAttackSE();

        //2フレーム待つ
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
    }

    /// <summary>
    /// プレイヤーにランダムで攻撃するメソッド
    /// </summary>
    public override BasePlayerStatus RandomSelect()
    {
        //一度生存しているキャラのみでリストを整理する
        List<BasePlayerStatus> TargetAlivePlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        //リストにキャラがいれば実行
        if (TargetAlivePlayers.Count > 0)
        {
            //リストの中にあるプレイヤーキャラを選択してターゲットに設定
            BasePlayerStatus target = TargetAlivePlayers[UnityEngine.Random.Range(0, TargetAlivePlayers.Count)];

            Debug.Log(target.PlayerID + "に攻撃");

            //ターゲットの位置にエフェクトを生成
            GameObject effectInstance = Instantiate(OnlyAttackEffect, target.transform.position, Quaternion.identity);

            target.PlayerOnDamage(EnemyAttackPower);

            //2フレーム後エフェクトを消去
            Destroy(effectInstance, 2f);

            //設定したターゲットを返す
            return target;
        }
        else
        {
            Debug.Log("攻撃対象がいません");
            return null;
        }
    }

    /// <summary>
    /// スライムのダメージ処理
    /// </summary>
    /// <param name="damage">プレイヤーからの攻撃＝ダメージ数</param>
    public override void EnemyOnDamage(int damage)
    {
        EnemyCurrentHP -= damage;

        if (EnemyCurrentHP <= 0)
        {
            //現在のHPを０に設定してHPBarも更新
            EnemyCurrentHP = 0;

            // UI更新
            slimeHPBar.value = EnemyCurrentHP;
            enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";

            //生存フラグをfalse
            EnemyIsAlive = false;

            //生存リストと初期ターゲットを設定するリストの消去
            Stage1BattleSystem.Instance.aliveEnemies.Remove(this);
            PlayerTargetSelect.Instance.RemoveSetTarget(this);

            //アタッカーの全体攻撃のリストから削除
            Attacker.RemoveDeadEnemies();

            //自身のオブジェクトを消去するコールチンスタート
            StartCoroutine(DestroyObject());
        }
        else
        {
            // 通常のダメージ時のUI更新
            slimeHPBar.value = EnemyCurrentHP;
            enemyHPUGUI.text = $"{EnemyCurrentHP}/2000";
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

