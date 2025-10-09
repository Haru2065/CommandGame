using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのスクリタブルオブジェクトを作成
/// </summary>
[CreateAssetMenu(fileName ="PlayerDataBase",menuName = "ScriptableObject/PlayerDataBase")]
public class PlayerDataBase : ScriptableObject
{
    //プレイヤーのパラメータのリスト
    public List<PlayerParameters> PlayerParameters = new List<PlayerParameters>();
}
