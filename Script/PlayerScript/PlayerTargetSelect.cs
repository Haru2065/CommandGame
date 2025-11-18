using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーが敵に攻撃する対象を選択するスクリプト
/// </summary>
public class PlayerTargetSelect : MonoBehaviour
{
    //このスクリプトをインスタンス化
    private static PlayerTargetSelect instance;

    /// <summary>
    /// インスタンスのゲッター
    /// </summary>
    public static PlayerTargetSelect Instance
    {
        get => instance;
    }

    //敵の攻撃するターゲット
    private BaseEnemyStatus attackTarget;

    
    [Tooltip("開始時のみ使用バトル開始時に設定するターゲット")]
    public List<BaseEnemyStatus> StartSetTargets;

    /// <summary>
    /// シングルトンパターンでインスタンス化
    /// </summary>
    private void Awake()
    {
        //インスタンスがなければインスタンス化
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(Instance);
        }
        
    }

    /// <summary>
    /// バトル開始時に攻撃対象を設定するメソッド
    /// </summary>
    public void SetStartBattleTarget()
    {
        //初期設定するターゲットリストにあればリストの要素１つ目の敵をターゲットとして設定
        if (StartSetTargets.Count > 0)
        {
            SetTarget(StartSetTargets[0]);
        }
    }

    /// <summary>
    /// プレイヤーが敵に攻撃する対象を設定するメソッド
    /// </summary>
    /// <param name="newTarget">新しく設定する攻撃対象</param>
    public void SetTarget(BaseEnemyStatus newTarget)
    {
        //敵の攻撃対象が既に設定されていたらUIを非表示
        if(attackTarget != null)
        {
            attackTarget.ShowTargetUI(false);
        }

        //攻撃対象を新しいターゲットに設定
        attackTarget = newTarget;

        //ターゲットUIを表示
        attackTarget.ShowTargetUI(true);
        Debug.Log("ターゲットを変更しました");
    }

    /// <summary>
    /// 指定した敵が倒れた場合にターゲットを再選択するメソッド
    /// </summary>
    /// <param name="deadEnemy">倒された敵</param>
    public void RemoveSetTarget(BaseEnemyStatus deadEnemy)
    {
        // リストに存在する場合、倒れた敵をターゲットリストから削除
        if (StartSetTargets.Contains(deadEnemy))
        {
            StartSetTargets.Remove(deadEnemy);
        }

        // 現在の攻撃対象が倒された敵だった場合、次のターゲットを決定する
        if (attackTarget == deadEnemy)
        {
            if (StartSetTargets.Count > 0)
            {
                // リストの先頭を新しいターゲットとして設定
                SetTarget(StartSetTargets[0]);
            }
            else
            {
                // すべての敵が倒された場合、ターゲットUIを非表示にする
                attackTarget.ShowTargetUI(false);
            }
        }

    }

    /// <summary>
    /// 設定したターゲットを取得するメソッド
    /// </summary>
    /// <returns>取得したターゲットの値を各プレイヤーキャラのステータスに渡す</returns>
    public BaseEnemyStatus GetAttackTargetEnemy()
    {
        return attackTarget;
    }
}
