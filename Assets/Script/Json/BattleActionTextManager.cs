using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class BattleText
{
    //JSON�t�@�C���̃e�L�X�g
    public string text;

    //JSON�t�@�C����ID
    public string id;
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
    [Tooltip("Json���[�h����X�N���v�g")]
    private JsonLoadr jsonLoadr;

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
    private TextMeshProUGUI characterBattleActionText;

    //JSON����ǂݍ��񂾃f�[�^������郊�X�g�iC#�`���ɕϊ������f�[�^���󂯎�郊�X�g�j
    private List<BattleText> battleTextConvertedList;

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
        StartCoroutine(jsonLoadr.LoadJsonText(json =>
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, List<BattleText>>>(json);

            if (dict != null && dict.ContainsKey("BattleTexts"))
            {
                battleTextConvertedList = dict["BattleTexts"];
                Debug.Log($"�ϊ������I�o�g���e�L�X�g�f�[�^��: {battleTextConvertedList.Count}");
            }
            else
            {
                Debug.LogWarning("�ϊ����s�I�f�[�^���󂩃L�[���Ԉ���Ă��܂�");
            }
        }));

        //�e�L�X�g�E�B���h�E���\��
        textWindow.SetActive(false);
    }

    public void ShowBattleActionText(string id)
    {
        //JSON����ǂݍ��񂾃f�[�^������郊�X�g����id���擾
        BattleText battleText = battleTextConvertedList.Find(text => text.id == id);

        if (battleText != null)
        {
            //ID�����ƂɃe�L�X�g��\�����e�L�X�g�E�B���h�E���\��
            characterBattleActionText.text = battleText.text;
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
