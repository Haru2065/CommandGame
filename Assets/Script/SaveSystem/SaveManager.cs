using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor;
using System;

/// <summary>
/// Player�̃p�����[�^��ۑ�����X�N���v�g
/// </summary>
public static class SaveManager
{
    /// <summary>
    /// Json�Ƀ��x���A�b�v���������^�C���̃p�����[�^�f�[�^��ۑ����郁�\�b�h
    /// </summary>
    /// <param name="players">���x���A�b�v����v���C���[</param>
    public static void SavePlayers(List<BasePlayerStatus> players)
    {
        //���x���A�b�v����v���C���[�̃��X�g�ɓ����Ă���L�����̃����^�C���f�[�^��ۑ�
        foreach (var player in players)
        {
            var data = new PlayerSaveData
            {
                playerID_SaveData = player.PlayerID,
                level_SaveData = player.Level,
                attackPower_SaveData = player.AttackPower,
                playerMaxHP_SaveData = player.PlayerMaxHP,
            };

            //Json�̕�����ɕϊ����ĕۑ��i�o��Json���C���f���g����j
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            //�ۑ��p�X�A�t�@�C�����w��
            string path = Application.persistentDataPath + $"/{player.PlayerID}_save.json";

            //�w�肵���ۑ��p�X��Json����������
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
                Debug.Log("�ۑ�����: " + path);
            }
            catch (Exception ex)
            {
                Debug.LogError("�ۑ����s: " + ex.Message);
            }

        }
    }
}
