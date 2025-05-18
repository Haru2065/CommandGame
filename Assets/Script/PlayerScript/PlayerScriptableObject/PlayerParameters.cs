using System;
using UnityEngine;

/// <summary>
/// プレイヤーのデータベースで利用するパラメータ
/// </summary>
[Serializable]
public class PlayerParameters
{
    [Tooltip("プレイヤーの名前データ")]
    public string PlayerNameData;

    [Tooltip("プレイヤーの名前の最大体力のデータ")]
    public int PlayerMaxHPData;

    [Tooltip("プレイヤー攻撃力データ")]
    public int PlayerAttackPowerData;

    [Tooltip("プレイヤーのバフパワーデータ")]
    public int BuffPowerData;

    [Tooltip("プレイヤーの回復力データ")]
    public int HealPowerData;
}
