using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor;
using System;

/// <summary>
/// Playerのパラメータを保存するスクリプト
/// </summary>
public static class SaveManager
{
    /// <summary>
    /// Jsonにレベルアップしたランタイムのパラメータデータを保存するメソッド
    /// </summary>
    /// <param name="players">レベルアップするプレイヤー</param>
    public static void SavePlayers(List<BasePlayerStatus> players)
    {
        //レベルアップするプレイヤーのリストに入っているキャラのランタイムデータを保存
        foreach (var player in players)
        {
            var data = new PlayerSaveData
            {
                playerID_SaveData = player.PlayerID,
                level_SaveData = player.Level,
                attackPower_SaveData = player.AttackPower,
                playerMaxHP_SaveData = player.PlayerMaxHP,
            };

            //Jsonの文字列に変換して保存（出力Jsonをインデントする）
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            //保存パス、ファイル名指定
            string path = Application.persistentDataPath + $"/{player.PlayerID}_save.json";

            //指定した保存パスにJsonを書き込み
            File.WriteAllText(path, json);
        }
    }

    public static void SaveStage()
    {
        if (BaseBattleManager.Instance.IsUnlockStage3)
        {
            var data = new StageSaveData
            {
                Stage3UnLock_SaveData = BaseBattleManager.Instance.IsUnlockStage3
            };

            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                string path = Application.persistentDataPath + "/StageSaveData.json";
                File.WriteAllText(path, json);
                Debug.Log("保存成功: " + path);
            }
            catch (Exception ex)
            {
                Debug.LogError("保存失敗: " + ex.Message);
            }

        }
    }
}
