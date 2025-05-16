using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �{�^���������ƃv���C���[�̃X�e�[�^�X�E�B���h�E���J��
/// </summary>
public class PushOpenStatusWindow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�X�e�[�^�X�E�B���h�E���J���{�^��")]
    private Button statusWindowButton;

    public void Update()
    {
        //�v���C���[�^�[�����̂݃{�^����������悤�ɂ���
        //�����X�e�[�W�P�������̓X�e�[�W�Q�̃C���X�^���X�����肩�v���C���[�^�[���Ȃ���s
        if (Stage1BattleSystem.Instance != null && Stage1BattleSystem.Instance.IsPlayerTurn ||
            Stage2BattleSystem.Instance != null && Stage2BattleSystem.Instance.IsPlayerTurn)
        {
            //�{�^����������悤�ɂ���
            statusWindowButton.interactable = true;
        }

        else
        {
            //�{�^���������Ȃ��悤�ɓ����ɂ���
            statusWindowButton.interactable = false;
        }
    }

    /// <summary>
    /// �{�^���������ƃX�e�[�^�X�E�B���h�E���J��
    /// </summary>
    public void OnButton()
    {
        //�X�e�[�^�X�E�B���h�E��\������
        StatusWindow.Instance.OpenStatusWindow();

        //�X�e�[�^�X�E�B���h�E�����{�^����\��
        UIManager.Instance.CloseStatusWindowButton.SetActive(true);

        //�X�e�[�^�X�E�B���h�E���J���{�^�����\��
        UIManager.Instance.OpenStatusWindowButton.SetActive(false);
    }
}
