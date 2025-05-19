using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;�@
using System;

/// <summary>
/// Player�̃p�����[�^�A�X�e�[�W�̏󋵂�ۑ�����X�N���v�g
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

            //Json�̃f�[�^�����邩���m�F���A�ۑ�
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                string path = Application.persistentDataPath + "/StageSaveData.json";
                File.WriteAllText(path, json);
                Debug.Log("�ۑ�����: " + path);
            }
            //�Ȃ���Ύ��s
            catch (Exception ex)
            {
                Debug.LogError("�ۑ����s: " + ex.Message);
            }

        }
    }

    /// <summary>
    /// �Z�[�u�f�[�^���폜���郁�\�b�h
    /// </summary>
    public static void DeleteSavedata(string fileName)
    {
        //�Z�[�u�f�[�^�̃p�X���w��
        string path = Application.persistentDataPath + "/" + fileName;

        //�Z�[�u�f�[�^�����݂���Ώ���
        if(File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("�Z�[�u�f�[�^�폜����: " + path);
        }
        else
        {
            Debug.LogWarning("�Z�[�u�f�[�^���폜�ł��܂���ł���: " + path);
        }
    }

    /// <summary>
    /// �v���C���[�̃p�����[�^�Z�[�u�f�[�^�����݂��邩
    /// </summary>
    /// <param name="playerID">�v���C���[��ID</param>
    /// <returns>�Z�[�u�f�[�^�����݂���΃Z�[�u�f�[�^�ƃt���O��true�ŕԂ�</returns>
    public static bool HasPlayerSave(string playerID)
    {
        //�Z�[�u�f�[�^�̃p�X���w��
        string path = Application.persistentDataPath + $"/{playerID}_save.json";
        return File.Exists(path);
    }

    /// <summary>
    /// �X�e�[�W�̃Z�[�u�f�[�^�����݂��邩
    /// </summary>
    /// <returns>�Z�[�u�f�[�^�����݂���΃Z�[�u�f�[�^�ƃt���O��true�ŕԂ�</returns>
    public static bool HasStageSave()
    {
        //�Z�[�u�f�[�^�̃p�X���w��
        string path = Application.persistentDataPath + "/StageSaveData.json";
        return File.Exists(path);
    }

    /// <summary>
    /// ���ׂẴZ�[�u�f�[�^�����݂��邩�܂Ƃ߂ă`�F�b�N���s��
    /// </summary>
    /// <returns></returns>
    public static bool HasAnySaveData()
    {
        return HasStageSave() || HasPlayerSave("Attacker") || HasPlayerSave("Buffer") || HasPlayerSave("Healer");
    }
}
