using System;

/// <summary>
/// プレイヤーのパラメータの保存データ
/// </summary>
[Serializable]
public class PlayerSaveData
{
    //プレイヤーのIDセーブデータ
    public string playerID_SaveData;

    //プレイヤーのレベルセーブデータ
    public int level_SaveData;

    //プレイヤーの攻撃力と最大体力のセーブデータ
    public int attackPower_SaveData;
    public int playerMaxHP_SaveData;
}
