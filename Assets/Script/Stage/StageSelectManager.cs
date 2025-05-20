using Newtonsoft.Json;
using System.IO;
using UnityEngine;

/// <summary>
/// �X�e�[�W�̉���󋵂ɉ����ă{�^����\����\������X�N���v�g
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�X�e�[�W2�{�^��")]
    private GameObject stage2Button;

    [SerializeField]
    [Tooltip("�`���[�g���A��2�{�^��")]
    private GameObject tutorial2Button;

    [SerializeField]
    [Tooltip("�X�e�[�W3�{�^��")]
    private GameObject stage3Button;

    //�X�e�[�W2��������ꂽ���ǂ���
    private bool isStage2Unlocked;

    //�X�e�[�W3��������ꂽ���ǂ���
    private bool isStage3Unlocked;

    // Start is called before the first frame update
    void Start()
    {
        //�X�e�[�W2,3�̃{�^���A�`���[�g���A��2�{�^�����\���ɂ���
        tutorial2Button.SetActive(false);
        stage2Button.SetActive(false);
        stage3Button.SetActive(false);

        //�X�e�[�W�̃Z�[�u�f�[�^��ǂݍ���
        string path = Application.persistentDataPath + $"/StageSaveData.Json";

        //�Z�[�u�f�[�^�����݂���Ȃ�X�e�[�W�f�[�^�����[�h
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StageSaveData saveData = JsonConvert.DeserializeObject<StageSaveData>(json);

            isStage2Unlocked = saveData.Stage2UnLock_SaveData;
            isStage3Unlocked = saveData.Stage3UnLock_SaveData;
        }
        else
        {
            //�Z�[�u�f�[�^�����݂��Ȃ��ꍇ�̓X�e�[�W2,3�͉������Ă��Ȃ�
            isStage2Unlocked = false;
            isStage3Unlocked = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�����X�e�[�W2���������Ă�����{�^����\��
        if (isStage2Unlocked)
        {
            tutorial2Button.SetActive(true);
            stage2Button.SetActive(true);
        }
        else
        {
            tutorial2Button.SetActive(false);
            stage2Button.SetActive(false);
        }

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
