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
        enemyHPUGUI.text = $"{EnemyCurrentHP}/2000";
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
    public IEnumerator SlimeAction()
    {
        //ランダムでプレイヤーに攻撃する対象を選択して攻撃するメソッドを実行
        RandomSelect();
        
        //スライムの攻撃効果音再生
        EnemySE.Instance.Play_slimeAttackSE();

        //1フレーム待つ
        return null;
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
            BasePlayerStatus target = TargetAlivePlayers[Random.Range(0, TargetAlivePlayers.Count)];

            Debug.Log(target.PlayerID + "に攻撃");

            target.PlayerOnDamage(EnemyAttackPower);

            // ターゲットの種類に応じて、適切なバトルアクションテキストを表示する
            switch (target)
            {
                // ターゲットが Attacker の場合
                case var _ when target == Attacker:
                    
                    //ダメージを受けたことを示すテキストを表示
                    BattleActionTextManager.Instance.ShowBattleActionText("DamageAttacker");

                    // 一定時間後に敵のアクションテキストを非表示にする
                    StartCoroutine(HideEnemyActionText());
                    break;

                // ターゲットが Buffer の場合
                case var _ when target == Buffer:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageBuffer");

                    StartCoroutine(HideEnemyActionText());
                    break;

                // ターゲットが Healer の場合
                case var _ when target == Healer:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageHealer");

                    StartCoroutine(HideEnemyActionText());

                    break;
            }
        }
        else
        {
            Debug.Log("攻撃対象がいません");
        }
    }

    /// <summary>
    /// スライムのダメージ処理
    /// </summary>
    /// <param name="damage">プレイヤーからの攻撃＝ダメージ数</param>
    public override void EnemyOnDamage(int damage)
    {
        EnemyCurrentHP -= damage;
        slimeHPBar.value = EnemyCurrentHP;
        

        if (EnemyCurrentHP <= 0)
        {
            //現在のHPを０に設定してHPBarも更新
            EnemyCurrentHP = 0;
            slimeHPBar.value = EnemyCurrentHP;

            EnemyIsAlive = false;

            //生存リストと初期ターゲットを設定するリストの消去
            Stage1BattleSystem.Instance.aliveEnemies.Remove(this);
            PlayerTargetSelect.Instance.RemoveSetTarget(this);

            //アタッカーの全体攻撃のリストから削除
            Attacker.RemoveDeadEnemies();

            //自身のオブジェクトを消去するコールチンスタート
            StartCoroutine(DestroyObject());
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

