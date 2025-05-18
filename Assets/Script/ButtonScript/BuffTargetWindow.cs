using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �o�t����Ώێ҂�I������X�N���v�g
/// </summary>
public class BuffTargetWindow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�o�t�^�[�Q�b�g�{�^���A�^�b�J�[")]
    private Button buffTargetAttackerButton;

    [SerializeField]
    [Tooltip("�o�t�^�[�Q�b�g�{�^���o�b�t�@�[")]
    private Button buffTargetBufferButton;

    [SerializeField]
    [Tooltip("�o�t�^�[�Q�b�g�{�^���q�[���[")]
    private Button buffTargetHealerButton;

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
    [Tooltip("�o�t�^�[�Q�b�g�E�B���h�E")]
    private GameObject buffTargetWindow;


    // Start is called before the first frame update
    void Start()
    {
        //�ŏ��͊e�{�^�����\��
        buffTargetAttackerButton.gameObject.SetActive(false);
        buffTargetBufferButton.gameObject.SetActive(false);
        buffTargetHealerButton.gameObject.SetActive(false);

        //�I���E�B���h�E���\��
        buffTargetWindow.SetActive(false);
    }

    /// <summary>
    /// �o�t�Ώێ҂�I�������ʂ�\���A�{�^�����������\�b�h
    /// </summary>
    public void ShowBuffTargetWindow()
    {
        //�I���E�B���h�E��\��
        buffTargetWindow.SetActive(true);

        //�e�Ώێ҃{�^�����e�L�������������Ă�����\������
        buffTargetAttackerButton.gameObject.SetActive(attacker.IsAlive);
        buffTargetBufferButton.gameObject.SetActive(buffer.IsAlive);
        buffTargetHealerButton.gameObject.SetActive(buffer.IsAlive);



        //�e�L�����̑Ώۃ{�^���������ꂽ��A���̑Ώێ҂Ƀo�t�����s
        buffTargetAttackerButton.onClick.AddListener(() => buffer.OnBuff(attacker));
        buffTargetBufferButton.onClick.AddListener(() => buffer.OnBuff(buffer));
        buffTargetHealerButton.onClick.AddListener(() => buffer.OnBuff(healer));
    }

    /// <summary>
    /// �I���E�B���h�E�ƃ{�^�����\���ɂ��郁�\�b�h
    /// </summary>
    public void HideBuffTargetWindow()
    {
        //�I���E�B���h�E���\��
        buffTargetWindow.SetActive(false);

        // ���X�i�[�̏d����h�����߂ɍ폜
        buffTargetAttackerButton.onClick.RemoveAllListeners();
        buffTargetBufferButton.onClick.RemoveAllListeners();
        buffTargetHealerButton.onClick.RemoveAllListeners();

        //�e�Ώێ҃{�^�����\��
        buffTargetAttackerButton.gameObject.SetActive(false);
        buffTargetBufferButton.gameObject.SetActive(false);
        buffTargetHealerButton.gameObject.SetActive(false);
    }
}
