using System;
using UnityEngine;

/// <summary>
/// ステージの解放状況を保存するデータ
/// </summary>
[Serializable]
public class StageSaveData
{
    //ステージ2,3は解放されたか
    public bool Stage2UnLock_SaveData;
    public bool Stage3UnLock_SaveData;
}
