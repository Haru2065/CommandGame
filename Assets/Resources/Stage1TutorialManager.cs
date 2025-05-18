using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Stage1Tutorial
{
    //�`���[�g���A����ID
    public string id;
    
    //�`���[�g���A���̃e�L�X�g
    public string text;
}

[Serializable]
public class Stage1TutorialData
{
    //�`���[�g���A�������X�g�ŊǗ�(JSON�f�[�^��ϊ����邽�߂Ɏ󂯎�郊�X�g�j
    public List<Stage1Tutorial> Stage1Tutorials;
}

public class Stage1TutorialManager : MonoBehaviour
{

    [SerializeField]
    [Tooltip("�X�e�[�W�P�̃`���[�g���A���ŕ\������e�L�X�g")]
    private TextMeshProUGUI stage1TutorialText;

    [SerializeField]
    [Tooltip("�^�[���̃G�t�F�N�g��\������摜")]
    private GameObject turnEffectImage;

    [SerializeField]
    [Tooltip("�G�̃^�[�Q�b�g��I��ł���摜")]
    private GameObject enemyTargetImage;

    [SerializeField]
    [Tooltip("�v���C���[�̑���L�[�̉摜")]
    private GameObject keyBordImage;

    [SerializeField]
    [Tooltip("�L�����̃X�e�[�^�X�Љ�̉摜")]
    private GameObject statusCharaImage;

    [SerializeField]
    [Tooltip("�v���C���[�̃X�e�[�^�X���J���{�^���̉摜")]
    private GameObject statusButtonImage;

    //JSON����ǂݍ��񂾃f�[�^������郊�X�g�iC#�`���ɕϊ������f�[�^���󂯎�郊�X�g�j
    private List<Stage1Tutorial> stage1ConvertedList;

    //�\���C���f�b�N�X�i���ǂ̃`���[�g���A����\�����Ă��邩���L�^����j
    private int CurrentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        //�S�Ă̐����摜���\��
        HideStartImage();

        //�X�e�[�W�P�̃`���[�g���A���e�L�X�g��Json���烍�[�h
        Stage1Tutorial_LoadJson();

        //�X�e�[�W�P�̃`���[�g���A���e�L�X�g��\������
        StartStage1Tutorial();
    }
    
    /// <summary>
    /// �S�Ă̐����摜���\���ɂ��郁�\�b�h
    /// </summary>
    void HideStartImage()
    {
        turnEffectImage.SetActive(false);
        enemyTargetImage.SetActive(false);
        keyBordImage.SetActive(false);

        statusCharaImage.SetActive(false);
        statusButtonImage.SetActive(false);
    }

    /// <summary>
    /// �X�e�[�W�P�̃`���[�g���A��Json�t�@�C�������[�h���郁�\�b�h
    /// </summary>
    void Stage1Tutorial_LoadJson()
    {
        //�A�Z�b�g�t�H���_��Resources�ɂ���Stage1_TutorialText.json��ǂݍ���
        TextAsset jsonFile = Resources.Load<TextAsset>("Stage1_TutorialText");

        //Json�t�@�C��������Εϊ�
        if (jsonFile != null)
        {
            Debug.Log($"JSON�̓��e: {jsonFile.text}");

            //�X�e�[�W1�̃`���[�g���A���f�[�^�^�ɕϊ�(JSON�t�@�C���̒��g�𕶎���Ŏ擾�j
            Stage1TutorialData data = JsonUtility.FromJson<Stage1TutorialData>(jsonFile.text);

            // JSON��C#�ɕϊ������f�[�^���X�e�[�W1�̎󂯎�胊�X�g�ɂ����
            stage1ConvertedList = data.Stage1Tutorials;

            //Json�̃f�[�^������Εϊ������ƕ\��
            if (data != null && data.Stage1Tutorials != null)
            {
                Debug.Log($"�ϊ������I�`���[�g���A���f�[�^��: {stage1ConvertedList.Count}");
            }
            else
            {
                Debug.LogWarning("�ϊ����s�I�f�[�^���󂩃L�[���Ԉ���Ă��܂�");
            }
        }
        else
        {
            Debug.Log("JSON�t�@�C����������܂���!");
        }
    }

    /// <summary>
    /// �X�e�[�W�P�̃`���[�g���A����\���������郁�\�b�h
    /// </summary>
    void StartStage1Tutorial()
    {
        //�����ǂݍ��񂾃��X�g���󂾂�����x�������o��
        if(stage1ConvertedList.Count == 0)
        {
            Debug.LogWarning("�`���[�g���A���e�L�X�g���ǂݍ��܂�Ă��܂���!");
            return;
        }

        //�`���[�g���A����\��(�ŏ���id;Welcome����\��
        ShowTutorial(stage1ConvertedList[CurrentIndex]);
    }

    /// <summary>
    /// �N���b�N������A���̃e�L�X�g��\�����郁�\�b�h
    /// </summary>
    void OnClickNext()
    {
        //�\���C���f�b�N�X�����ɐi�߂�
        CurrentIndex++;

        //�\���C���f�b�N�X���Ō�܂ŕ\������܂ŕ\��
        if(CurrentIndex < stage1ConvertedList.Count)
        {
            //�\���C���f�b�N�X�̐��ɓ����Ă���Json�e�L�X�g��\��
            ShowTutorial(stage1ConvertedList[CurrentIndex]);
        }
        //�Ō�܂ŕ\��������I��
        else
        {
            //�Ō�܂ŕ\�������玩���G�ɃX�e�[�W�Z���N�g��ʂɈړ�
            SceneManager.LoadScene("StageSelect");
        }
    }

    /// <summary>
    /// JSON�̃`���[�g���A����\���R���g���[�����郁�\�b�h
    /// </summary>
    /// <param name="tutorials">Json�̃`���[�g���A��</param>
    void ShowTutorial(Stage1Tutorial tutorials)
    {
        //Json�ɏ����ꂽ�e�L�X�g������UI�Ƃ��ĕ\��
        stage1TutorialText.text = tutorials.text;

        //�`���[�g���A����id�𒲂ׂĂ���ɉ����ăe�L�X�g��UI��\��
        switch (tutorials.id)
        {
            case "Welocome":
                break;

            case "Battle_1":

                //�o�g�������P�̎��Ƀ^�[���G�t�F�N�g�̉摜��\��
                turnEffectImage.SetActive(true);
                break;

            case "Battle_2":
                break ;

            case "Battle_3":
                break ;

            case "Operation_1":

                //�����Ń^�[���G�t�F�N�g�̉摜���\��
                turnEffectImage.SetActive(false);
                break ;

            case "Operation_2":

                //��������Q�ő�����@�摜��\��
                keyBordImage.SetActive(true);
                break;

            case "Operation_3":
                break;

            case "Operation_4":

                //������@�摜���\���ɂ��ēG�^�[�Q�b�g��\��
                keyBordImage.SetActive(false);
                enemyTargetImage.SetActive(true);

                break ;

            case "Status_1":
                
                //�G�^�[�Q�b�g���\��
                enemyTargetImage.SetActive(false);
                break;

            case "Status_2":

                //�X�e�[�^�X�Љ�̉摜��\��
                statusCharaImage.SetActive(true);
                break;

            case "Status_3":

                break;
            case "Status_4":

                ////�X�e�[�^�X�Љ�̉摜���\���ɂ��A�X�e�[�^�X�{�^����\��
                statusCharaImage.SetActive(false);
                statusButtonImage.SetActive(true);
                break;

            case "End":

                //�S�Ẳ摜��\��
                statusButtonImage.SetActive(false);
                statusCharaImage.SetActive(false);
                break;

            default: 
                break;

        }
    }

    /// <summary>
    /// �}�E�X�N���b�N����
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        //�}�E�X���N���b�N�Ŏ��̃e�L�X�g�\��
        if (Input.GetMouseButtonDown(0))
        {
            //���̃e�L�X�g�\��
            OnClickNext();
        }
    }
}
