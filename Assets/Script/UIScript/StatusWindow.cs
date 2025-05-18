using TMPro;
using UnityEngine;

/// <summary>
/// �L�����̃X�e�[�^�X�E�B���h�E
/// </summary>
public class StatusWindow : MonoBehaviour
{
    //�L�����̃X�e�[�^�X�E�B���h�E�̃C���X�^���X���p
    private static StatusWindow instance;

    /// <summary>
    /// �C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static StatusWindow Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("�A�^�b�J�[")]
    private BasePlayerStatus attacker;

    [SerializeField]
    [Tooltip("�o�b�t�@�[")]
    private BasePlayerStatus buffer;

    [SerializeField]
    [Tooltip("�q�[���[")]
    private BasePlayerStatus healer;


    [SerializeField]
    [Tooltip("�A�^�b�J�[�̍U���͐��ltext")]
    private TextMeshProUGUI attackerAttackpowerText;

    [SerializeField]
    [Tooltip("�A�^�b�J�[�̃f�o�t�J�E���gtext")]
    private TextMeshProUGUI attacker_DebuffCount_Text;

    [SerializeField]
    [Tooltip("�A�^�b�J�[�̓���f�o�t�J�E���gtext")]
    private TextMeshProUGUI attacker_SpecialDebuffCount_Text;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̍U���͐��ltext")]
    private TextMeshProUGUI bufferAttackPowerText;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̃f�o�t�J�E���gtext")]
    private TextMeshProUGUI buffer_DebuffCount_Text;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̓���f�o�t�J�E���gtext")]
    private TextMeshProUGUI buffer_SpecialDebuffCount_Text;

    [SerializeField]
    [Tooltip("�q�[���[�̍U���͐��ltext")]
    private TextMeshProUGUI healerAttackPowerText;

    [SerializeField]
    [Tooltip("�q�[���[�̃f�o�t�J�E���gtext")]
    private TextMeshProUGUI healer_DebuffCount_Text;

    [SerializeField]
    [Tooltip("�q�[���[�̓���f�o�t�J�E���gtext")]
    private TextMeshProUGUI healer_SpecialDebuffCount_Text;

    /// <summary>
    /// �L�����̃X�e�[�^�X�E�B���h�E���C���X�^���X��
    /// </summary>
    public void Awake()
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

    /// <summary>
    /// �X�e�[�^�X�E�B���h�E���J�����\�b�h
    /// </summary>
    public void OpenStatusWindow()
    {
        //�X�e�[�^�X�E�B���h�E���J��
        UIManager.Instance.StatusWindow.SetActive(true);

        //�X�e�[�^�X�X�V�e�L�X�g�\��
        UPdateStatusWindow();
    }

    /// <summary>
    /// �X�e�[�^�X�X�V���ăe�L�X�g��\�����郁�\�b�h
    /// </summary>
    private void UPdateStatusWindow()
    {
        //�A�^�b�J�[�̍U���͂ƃf�o�t�Ɠ���f�o�t�J�E���g�𐔒l�Ƃ��ĕ\��
        attackerAttackpowerText.text = $"{attacker.AttackPower}";
        attacker_DebuffCount_Text.text = $"{attacker.DebuffCount}";
        attacker_SpecialDebuffCount_Text.text = $"{attacker.SpecialDebuffCount}";

        //�o�b�t�@�[�̍U���͂ƃf�o�t�Ɠ���f�o�t�J�E���g�𐔒l�Ƃ��ĕ\��
        bufferAttackPowerText.text = $"{buffer.AttackPower}";
        buffer_DebuffCount_Text.text = $"{buffer.DebuffCount}";
        buffer_SpecialDebuffCount_Text.text = $"{buffer.SpecialDebuffCount}";

        //�q�[���[�̍U���͂ƃf�o�t�Ɠ���f�o�t�J�E���g�𐔒l�Ƃ��ĕ\��
        healerAttackPowerText.text = $"{healer.AttackPower}";
        healer_DebuffCount_Text.text = $"{healer.DebuffCount}";
        healer_SpecialDebuffCount_Text.text = $"{healer.SpecialDebuffCount}";
    }
}
