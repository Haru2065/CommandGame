using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のスクリプタブルオブジェクトを作成
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemySatusData")]
public class EnemyDataBase : ScriptableObject
{
    //敵のパラメータリスト
    public List<EnemyParameters> EnemyParameters = new List<EnemyParameters>();
}
