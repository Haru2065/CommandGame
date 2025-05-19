using Newtonsoft.Json;
using System.IO;
using UnityEngine;

/// <summary>
/// �X�e�[�W�̉���󋵂ɉ����ă{�^����\����\������X�N���v�g
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    

    [SerializeField]
    [Tooltip("�X�e�[�W�{�^��")]
    private GameObject stage3Button;

    //�X�e�[�W3��������ꂽ���ǂ���
    private bool isStage3Unlocked;

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
            isStage3Unlocked = saveData.Stage3UnLock_SaveData;
        }
        else
        {
            //�Z�[�u�f�[�^�����݂��Ȃ��ꍇ�̓X�e�[�W3�͉������Ă��Ȃ�
            isStage3Unlocked = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�����X�e�[�W3���������Ă�����{�^����\��
        if (isStage3Unlocked)
        {
            stage3Button.SetActive(true);
        }
        else
        {
            stage3Button.SetActive(false);
        }
    }
}
