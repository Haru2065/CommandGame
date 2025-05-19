using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;　
using System;

/// <summary>
/// Playerのパラメータ、ステージの状況を保存するスクリプト
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

            //Jsonのデータがあるかを確認し、保存
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                string path = Application.persistentDataPath + "/StageSaveData.json";
                File.WriteAllText(path, json);
                Debug.Log("保存成功: " + path);
            }
            //なければ失敗
            catch (Exception ex)
            {
                Debug.LogError("保存失敗: " + ex.Message);
            }

        }
    }

    /// <summary>
    /// セーブデータを削除するメソッド
    /// </summary>
    public static void DeleteSavedata(string fileName)
    {
        //セーブデータのパスを指定
        string path = Application.persistentDataPath + "/" + fileName;

        //セーブデータが存在すれば消去
        if(File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("セーブデータ削除成功: " + path);
        }
        else
        {
            Debug.LogWarning("セーブデータを削除できませんでした: " + path);
        }
    }

    /// <summary>
    /// プレイヤーのパラメータセーブデータが存在するか
    /// </summary>
    /// <param name="playerID">プレイヤーのID</param>
    /// <returns>セーブデータが存在すればセーブデータとフラグをtrueで返す</returns>
    public static bool HasPlayerSave(string playerID)
    {
        //セーブデータのパスを指定
        string path = Application.persistentDataPath + $"/{playerID}_save.json";
        return File.Exists(path);
    }

    /// <summary>
    /// ステージのセーブデータが存在するか
    /// </summary>
    /// <returns>セーブデータが存在すればセーブデータとフラグをtrueで返す</returns>
    public static bool HasStageSave()
    {
        //セーブデータのパスを指定
        string path = Application.persistentDataPath + "/StageSaveData.json";
        return File.Exists(path);
    }

    /// <summary>
    /// すべてのセーブデータが存在するかまとめてチェックを行う
    /// </summary>
    /// <returns></returns>
    public static bool HasAnySaveData()
    {
        return HasStageSave() || HasPlayerSave("Attacker") || HasPlayerSave("Buffer") || HasPlayerSave("Healer");
    }
}
