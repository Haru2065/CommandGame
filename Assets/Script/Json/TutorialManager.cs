using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Json�t�@�C����id����text���
/// </summary>
[Serializable]
public class TutorialText
{
    //�`���[�g���A����ID
    public string id;

    //�`���[�g���A���̃e�L�X�g
    public string text;
}

/// <summary>
/// Addressables�Ń��[�h����Json��C#�ɕϊ�������ɕ\������X�N���v�g
/// </summary>
public abstract class TutorialManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Json���[�h����X�N���v�g")]
    private JsonLoadr jsonLoder;

    [SerializeField]
    [Tooltip("�ǂݍ��݂���Json�����")]
    private string ScenarioJsonKey;

    [SerializeField]
    [Tooltip("�e�L�X�g�E�B���h�E")]
    protected GameObject TextWindow;

    [SerializeField]
    [Tooltip("Json�t�@�C����\������UGUI")]
    protected TextMeshProUGUI TutorialTextUGUI;

    //�ϊ������e�L�X�g�����Ԃɕ\�����郊�X�g
    protected List<TutorialText> tutorialList;

    //�\���C���f�b�N�X
    protected int currentIndex;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Json�ǂݍ��݃X�N���v�g���烍�[�h�J�n
        //�󂯎����Json���p�[�X
        StartCoroutine(jsonLoder.LoadJsonText(json =>
        {
            //Json�S�̂�Dictionary�ɕϊ�
            var dict = JsonConvert.DeserializeObject<Dictionary<string, List<TutorialText>>>(json);

            //json�L�[������Ε\�����X�g�ɑ��
            if (dict.TryGetValue(ScenarioJsonKey, out tutorialList))
            {
                currentIndex = 0;

                //�e�L�X�g�E�B���h�E��\��
                TextWindow.SetActive(true);

                //�`���[�g���A���e�L�X�g��\������
                StartTutorial();
            }
            else
            {
                Debug.LogWarning($"�w�肵���L�[��������܂���ł����B{ScenarioJsonKey}");
            }
        }));
    }

    /// <summary>
    /// �`���[�g���A����\���������郁�\�b�h
    /// </summary>
    protected void StartTutorial()
    {
        //�����ǂݍ��񂾃��X�g���󂾂�����x�������o��
        if (tutorialList.Count == 0)
        {
            Debug.LogWarning("�`���[�g���A���e�L�X�g���ǂݍ��܂�Ă��܂���!");
            return;
        }

        //�`���[�g���A����\��(�ŏ���id;Welcome����\��
        ShowTutorialText(tutorialList[currentIndex]);
    }

    /// <summary>
    /// �e�L�X�g�N���b�N����
    /// </summary>
    protected void Update()
    {
        //�}�E�X���N���b�N���ꂽ��e�L�X�g�̕\����i�߂�
        if (Input.GetMouseButtonDown(0))
        {
            OnClickNextText();
        }
    }

    /// <summary>
    /// �N���b�N���ꂽ��e�L�X�g�̕\����i�߂郁�\�b�h
    /// </summary>
    protected virtual void OnClickNextText()
    {
        //�\���C���f�b�N�X�����ɐi�߂�
        currentIndex++;

        //�\���C���f�b�N�X���Ō�܂ŕ\������܂ŕ\��
        if (currentIndex < tutorialList.Count)
        {
            //�\���C���f�b�N�X�̐��ɓ����Ă���Json�e�L�X�g��\��
            ShowTutorialText(tutorialList[currentIndex]);
        }
    }

    /// <summary>
    /// �ϊ�����Json�V�i���I��\��
    /// </summary>
    public abstract void ShowTutorialText(TutorialText texts);
}
