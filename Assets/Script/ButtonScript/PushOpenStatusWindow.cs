using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �{�^���������ƃv���C���[�̃X�e�[�^�X�E�B���h�E���J��
/// </summary>
public class PushOpenStatusWindow : MonoBehaviour
{
    //�X�e�[�^�X�E�B���h�E���J���X�N���v�g�̃C���X�^���X
    private static PushOpenStatusWindow instance;

    /// <summary>
    /// �C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static PushOpenStatusWindow Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("�X�e�[�^�X�E�B���h�E���J���{�^��")]
    private Button statusWindowButton;

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

    //�{�^����������悤�ɂ��郁�\�b�h
    public void CanPushStatusButton()
    {
        //�{�^����������悤�ɂ���
        statusWindowButton.interactable = true;
    }

    //�{�^���������Ȃ��悤�ɓ����ɂ��郁�\�b�h
    public void TransparentStatusButton()
    {
        statusWindowButton.interactable = false;
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
