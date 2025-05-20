using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGM���R���g���[������X�N���v�g
/// </summary>
public class BGMControl : MonoBehaviour
{
    //BGMControl�̃C���X�^���X
    private static BGMControl instance;

    /// <summary>
    /// �C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static BGMControl Instance
    {
        get => instance;
    }

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

        [SerializeField]
    [Tooltip("BGM�p�I�[�f�B�I�\�[�X")]
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip("�X�e�[�W3��BGM")]
    private AudioClip stage3BGM;

    /// <summary>
    /// �X�e�[�W3��BGM���Đ����郁�\�b�h
    /// </summary>
    public void PlayStage3BGM()
    {
        //�N���b�v��Stage3BGM�ɐݒ�
        audioSource.clip = stage3BGM;

        //�I�[�f�B�I�\�[�X�����[�v��������
        audioSource.loop = true;

        //BGM�Đ�
        audioSource.Play();
    }
}
