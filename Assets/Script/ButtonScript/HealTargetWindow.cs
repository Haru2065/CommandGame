using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �q�[���[�̑Ώێ҂�I������X�N���v�g
/// </summary>
public class HealTargetWindow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�񕜃^�[�Q�b�g�{�^���A�^�b�J�[")]
    private Button healTargetAttackerButton;

    [SerializeField]
    [Tooltip("�񕜃^�[�Q�b�g�{�^���o�b�t�@�[")]
    private Button healTargetBufferButton;

    [SerializeField]
    [Tooltip("�񕜃^�[�Q�b�g�{�^���q�[���[")]
    private Button healTargetHealerButton;

    [SerializeField]
    [Tooltip("�A�^�b�J�[")]
    private Attacker attacker;

    [SerializeField]
    [Tooltip("�o�b�t�@�[")]
    private Buffer buffer;

    [SerializeField]
    [Tooltip("�q�[���[")]
    private Healer healer;

    [SerializeField]
    [Tooltip("�񕜃^�[�Q�b�g�E�B���h�E")]
    private GameObject healTargetWindow;

    // Start is called before the first frame update
    void Start()
    {
        //�ŏ��͊e�{�^�����\��
        healTargetAttackerButton.gameObject.SetActive(false);
        healTargetBufferButton.gameObject.SetActive(false);
        healTargetHealerButton.gameObject.SetActive(false);

        //�I���E�B���h�E���\��
        healTargetWindow.SetActive(false);
    }

    /// <summary>
    /// �񕜑Ώێ҂�I���E�B���h�E��\���A�{�^�����������\�b�h
    /// </summary>
    public void ShowHealTargetWindow()
    {
        //�I���E�B���h�E��\��
        healTargetWindow.SetActive(true);

        //�e�Ώێ҃{�^�����e�L�������������Ă�����\������
        healTargetAttackerButton.gameObject.SetActive(attacker.IsAlive);
        healTargetBufferButton.gameObject.SetActive(buffer.IsAlive);
        healTargetHealerButton.gameObject.SetActive(buffer.IsAlive);



        //�e�L�����̑Ώێ҃{�^���������ꂽ��A���̑Ώێ҂ɉ񕜂����s
        healTargetAttackerButton.onClick.AddListener(() => healer.OnHeal(attacker));
        healTargetBufferButton.onClick.AddListener(() => healer.OnHeal(buffer));
        healTargetHealerButton.onClick.AddListener(() => healer.OnHeal(healer));

    }


    /// <summary>
    /// �I���E�B���h�E�ƃ{�^�����\���ɂ��郁�\�b�h
    /// </summary>
    public void HideHealTargetWindow()
    {
        //�I���E�B���h�E���\��
        healTargetWindow.SetActive(false);

        //���X�i�[�̏d����h�����߂ɍ폜
        healTargetAttackerButton.onClick.RemoveAllListeners();
        healTargetBufferButton.onClick.RemoveAllListeners();
        healTargetHealerButton.onClick.RemoveAllListeners();

        //�e�Ώێ҃{�^�����\��
        healTargetAttackerButton.gameObject.SetActive(false);
        healTargetBufferButton.gameObject.SetActive(false);
        healTargetHealerButton.gameObject.SetActive(false);
    }
}
