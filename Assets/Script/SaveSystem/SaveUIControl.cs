using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �X�^�[�g�{�^���������ꂽ��Z�[�u�f�[�^������΃Z�[�u�f�[�^��I������X�N���v�g
/// �Z�[�u�f�[�^������΃Z�[�u�f�[�^����������{�^�����\��
/// </summary>
public class SaveUIControl : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�Z�[�u�f�[�^�����{�^��")]
    private GameObject deleteButton;

    [SerializeField]
    [Tooltip("�Z�[�u�f�[�^�I���{�^��")]
    private GameObject SaveDataSelectButton;

    [SerializeField]
    [Tooltip("�Z�[�u�f�[�^�I���p�l��")]
    private GameObject SaveSelectPanel;

    /// <summary>
    /// �Z�[�u�f�[�^�����鎞�ɕ\������{�^������x��\���ŏ�����
    /// </summary>
    void Start()
    {
        deleteButton.SetActive(false);
        SaveDataSelectButton.SetActive(false);
        SaveSelectPanel.SetActive(false);
    }

    private void Update()
    {
        //�G�X�P�[�v�L�[�������ƃZ�[�u�f�[�^��ʂ����
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //�Z�[�u�f�[�^�I���p�l�����J���Ă��鎞�͕���
            if (SaveSelectPanel.activeSelf)
            {
                deleteButton.SetActive(false);
                SaveDataSelectButton.SetActive(false);
                SaveSelectPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// �X�^�[�g�{�^���������ꂽ���̃��\�b�h
    /// </summary>
    public void PushStartButton()
    {
        //�Z�[�u�f�[�^�����鎞�̓Z�[�u�f�[�^�I���{�^����\��
        if (SaveManager.HasAnySaveData())
        {
            deleteButton.SetActive(true);

            SaveDataSelectButton.SetActive(true);
            SaveSelectPanel.SetActive(true);
        }
        //�Ȃ���΃Z�[�u�f�[�^�I���{�^�����\���ɂ��āA�V�[�������[�h
        else
        {
            deleteButton.SetActive(false);
            SaveDataSelectButton.SetActive(false);
            SaveSelectPanel.SetActive(false);

            SceneManager.LoadScene("StageSelect");
        }
    }
}
