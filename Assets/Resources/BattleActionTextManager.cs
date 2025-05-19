using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class BattleText
{
    //JSON�t�@�C���̃e�L�X�g
    public string text;

    //JSON�t�@�C����ID
    public string id;
}

[Serializable]
public class BattleTextData
{
    //�o�g�����̍s���ʒm�̃e�L�X�g���X�g
    public List<BattleText> BattleTexts;
}

/// <summary>
/// �o�g�����̍s���ʒm�̃X�N���v�g
/// JSON����ϊ�
/// </summary>
public class BattleActionTextManager : MonoBehaviour
{
    //�o�g�����̍s���ʒm�̃X�N���v�g���C���X�^���X���p
    private static BattleActionTextManager instance;

    /// <summary>
    /// �C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static BattleActionTextManager Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("�e�L�X�g�E�B���h�E")]
    private GameObject textWindow;

    /// <summary>
    /// �e�L�X�g�E�B���h�E�̃Q�b�^�[
    /// </summary>
    public GameObject TextWindow
    {
        get => textWindow;
    }

    [SerializeField]
    [Tooltip("�o�g���ŃL�����̏󋵂�\������e�L�X�g")]
    private TextMeshProUGUI character_BattleActionText;

    //JSON����ǂݍ��񂾃f�[�^������郊�X�g�iC#�`���ɕϊ������f�[�^���󂯎�郊�X�g�j
    private List<BattleText> battleText_ConvertedList;

    /// <summary>
    /// �C���X�^���X��
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //JSON��ǂݍ��ݕϊ�����
        BattleText_LoadJsonText();

        //�e�L�X�g�E�B���h�E���\��
        textWindow.SetActive(false);
    }

    /// <summary>
    /// Json��ǂݍ��ݕϊ����郁�\�b�h
    /// </summary>
    public void BattleText_LoadJsonText()
    {
        //�A�Z�b�g�t�H���_��Resources�ɂ���BattleText.json��ǂݍ���
        TextAsset jsonFile = Resources.Load<TextAsset>("BattleText");

        if (jsonFile != null)
        {
            Debug.Log($"JSON�̓��e: {jsonFile.text}");

            //JSON�̒��g�𕶎���Ŏ擾
            BattleTextData data = JsonUtility.FromJson<BattleTextData>(jsonFile.text);

            //JSON����ǂݍ��񂾃f�[�^������郊�X�g�ɂ����
            battleText_ConvertedList = data.BattleTexts;

            if (data != null && data.BattleTexts != null)
            {
                Debug.Log($"�ϊ������I�o�g���e�L�X�g�f�[�^��: {battleText_ConvertedList.Count}");
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

    public void ShowBattleActionText(string id)
    {
        //JSON����ǂݍ��񂾃f�[�^������郊�X�g����id���擾
        BattleText battleText = battleText_ConvertedList.Find(text => text.id == id);

        if (battleText != null)
        {
            //ID�����ƂɃe�L�X�g��\�����e�L�X�g�E�B���h�E���\��
            character_BattleActionText.text = battleText.text;
            textWindow.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"�w�肳�ꂽ ID ({id}) �ɑΉ�����e�L�X�g��������܂���");
        }
    }

    /// <summary>
    /// �e�L�X�g�E�B���h�E���\���ɂ��郁�\�b�h
    /// </summary>
    public void TextDelayHide()
    {
        TextWindow.SetActive(false);
    }
}
