using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W�̉���󋵂ɉ����ă{�^����\����\������X�N���v�g
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    

    [SerializeField]
    [Tooltip("�X�e�[�W�{�^��")]
    private GameObject stage3Button;

    // Start is called before the first frame update
    void Start()
    {
        //�X�e�[�W3�̃{�^�����\���ɂ���
        stage3Button.SetActive(false);

        //�X�e�[�W�̃Z�[�u�f�[�^��ǂݍ���
        string path = Application.persistentDataPath + $"/StageSaveData.Json";

        //�Z�[�u�f�[�^�����݂���Ȃ�X�e�[�W�f�[�^�����[�h
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StageSaveData saveData = JsonConvert.DeserializeObject<StageSaveData>(json);
            LoadStageData(saveData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�����X�e�[�W3���������Ă�����{�^����\��
        if (BaseBattleManager.Instance != null && BaseBattleManager.Instance.IsUnlockStage3)
        {
            stage3Button.SetActive(true);
        }
        else
        {
            stage3Button.SetActive(false);
        }
    }

    /// <summary>
    /// �X�e�[�W�f�[�^�����[�h���郁�\�b�h
    /// </summary>
    /// <param name="data">Json�ɕۑ�����Ă���X�e�[�W�f�[�^</param>
    public static void LoadStageData(StageSaveData data)
    {
        //�X�e�[�W3����̃t���O�f�[�^���X�e�[�W�Z�[�u�f�[�^���烍�[�h
        BaseBattleManager.Instance.IsUnlockStage3 = data.Stage3UnLock_SaveData;
    }
}
