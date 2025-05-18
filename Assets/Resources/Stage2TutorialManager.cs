using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Stage2Tutorial
{
    //�`���[�g���A����ID
    public string id;

    //�`���[�g���A���̃e�L�X�g
    public string text;
}

[Serializable]
public class Stage2TutorialData
{
    //�`���[�g���A�������X�g�ŊǗ��iJSON�f�[�^��ϊ����邽�߂Ɏ󂯎�郊�X�g)
    public List<Stage2Tutorial> Stage2Tutorials;
}

public class Stage2TutorialManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�X�e�[�W�Q�̃`���[�g���A���ŕ\������e�L�X�g")]
    private TextMeshProUGUI stage2TutorialText;

    [SerializeField]
    [Tooltip("�e�L�����̍s�����������摜")]
    private GameObject actionCountImage;

    [SerializeField]
    [Tooltip("�X�e�[�^�X�󋵂��������摜")]
    private GameObject statusWindowImage;

    //JSON����ǂݍ��񂾃f�[�^������郊�X�g�iC#�`���ɕϊ������f�[�^���󂯎�郊�X�g�j
    private List<Stage2Tutorial> stage2ConvertedList;

    //�\���C���f�b�N�X�i���ǂ̃`���[�g���A����\�����Ă��邩���L�^����j
    private int currentIndex = 0;



    // Start is called before the first frame update
    void Start()
    {
        //�e�����摜���\�� 
        actionCountImage.SetActive(false);
        statusWindowImage.SetActive(false);

        //�X�e�[�W2�̃`���[�g���A���e�L�X�g��Json���烍�[�h
        Stage2Tutorial_LoadJson();

        //�X�e�[�W2�̃`���[�g���A���e�L�X�g��\������
        StartStage2Tutorial();
    }

    /// <summary>
    /// �X�e�[�W2�̃`���[�g���A��Json�t�@�C�������[�h���郁�\�b�h
    /// </summary>
    void Stage2Tutorial_LoadJson()
    {
        //�A�Z�b�g�t�H���_��Resources�ɂ���Stage2_TutorialText.json��ǂݍ���
        TextAsset jsonFile = Resources.Load<TextAsset>("Stage2_TutorialText");

        //Json�t�@�C��������Εϊ�
        if (jsonFile != null)
        {
            Debug.Log($"JSON�̓��e: {jsonFile.text}");

            //�X�e�[�W2�̃`���[�g���A���f�[�^�^�ɕϊ�(JSON�t�@�C���̒��g�𕶎���Ŏ擾�j
            Stage2TutorialData data = JsonUtility.FromJson<Stage2TutorialData>(jsonFile.text);

            // JSON��C#�ɕϊ������f�[�^���X�e�[�W2�̎󂯎�胊�X�g�ɂ����
            stage2ConvertedList = data.Stage2Tutorials;


            //Json�̃f�[�^������Εϊ������ƕ\��
            if (data != null && data.Stage2Tutorials != null)
            {
                Debug.Log($"�ϊ������I�`���[�g���A���f�[�^��: {stage2ConvertedList.Count}");
            }
            else
            {
                Debug.LogWarning("�ϊ����s�I�f�[�^���󂩃L�[���Ԉ���Ă��܂�");
            }
        }

        else
        {
            Debug.LogError("Json�t�@�C����������܂���");
        }
    }

    /// <summary>
    /// �X�e�[�W2�̃`���[�g���A����\���������郁�\�b�h
    /// </summary>
    void StartStage2Tutorial()
    {
        //�����ǂݍ��񂾃��X�g���󂾂�����x�������o��
        if (stage2ConvertedList.Count == 0)
        {
            Debug.LogWarning("�`���[�g���A�����ǂݍ��܂�Ă��܂���!");
            return;
        }

        //�`���[�g���A����\��(�ŏ���id;Welcome����\��
        ShowTutorial(stage2ConvertedList[currentIndex]);
    }

    /// <summary>
    /// �N���b�N������A���̃e�L�X�g��\�����郁�\�b�h
    /// </summary>
    public void OnClickNext()
    {
        //�\���C���f�b�N�X�����ɐi�߂�
        currentIndex++;

        //�\���C���f�b�N�X���Ō�܂ŕ\������܂ŕ\��
        if (currentIndex < stage2ConvertedList.Count)
        {
            //�\���C���f�b�N�X�̐��ɓ����Ă���Json�e�L�X�g��\��
            ShowTutorial(stage2ConvertedList[currentIndex]);
        }

        //�Ō�܂ŕ\��������I��
        else
        {
            Debug.Log("�`���[�g���A���I��!");

            //�Ō�܂ŕ\�������玩���G�ɃX�e�[�W�Z���N�g��ʂɈړ�
            SceneManager.LoadScene("StageSelect");
            
        }
    }

    /// <summary>
    /// JSON�̃`���[�g���A����\���R���g���[�����郁�\�b�h
    /// </summary>
    /// <param name="tutorials">Json�̃`���[�g���A��</param>
    void ShowTutorial(Stage2Tutorial tutorials)
    {
        //Json�ɏ����ꂽ�e�L�X�g������UI�Ƃ��ĕ\��
        stage2TutorialText.text = tutorials.text;

        //�`���[�g���A����id�𒲂ׂĂ���ɉ����ăe�L�X�g��UI��\��
        switch (tutorials.id)
        {
            case "welcome":

            case "DebuffEnemy_1":

                break ;

            case "DebuffEmemy_2":

                break ;

            case "DebuffEnemy_3":

                ////�X�e�[�^�X�󋵂��������摜��\��
                statusWindowImage.SetActive(true);
                break ;

            case "DebuffEnemy_4":

                //�X�e�[�^�X�󋵂��������摜���\��
                statusWindowImage.SetActive(false);
                break ;

            case "DebuffEnemy_5":
                break ;

            case "End":

                //�e�L�����̍s�����������摜
                actionCountImage.SetActive(true);
                break ;

            default:
                break ;
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

