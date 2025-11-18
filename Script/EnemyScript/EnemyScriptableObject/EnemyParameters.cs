using System;
using UnityEngine;

/// <summary>
/// 敵のデータベースで利用するパラメータ
/// </summary>
[Serializable]
public class EnemyParameters
{
    [Tooltip("敵ID")]
    public string EnemyNameData;

    [Tooltip("敵の最大体力")]
    public int EnemyMaxHPData;

    [SerializeField]
    [Tooltip("敵の攻撃力")]
    public int EnemyAttackPowerData;

    [Tooltip("デバフ力")]
    public int DebuffPowerData;
}
