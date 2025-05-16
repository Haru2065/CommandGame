using UnityEngine;

/// <summary>
/// �v���C���[�̌��ʉ��}�l�[�W���[
/// </summary>
public class PlayerSE : MonoBehaviour
{
    //�v���C���[�̌��ʉ��̃C���X�^���X���p
    private static PlayerSE instance;

    /// <summary>
    /// �v���C���[�̌��ʉ��̃C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static PlayerSE Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("���ʉ��̃I�[�f�B�I�\�[�X")]
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip("�A�^�b�J�[�̒ʏ�U����")]
    private AudioClip attackerNormalAttackSE;

    [SerializeField]
    [Tooltip("�A�^�b�J�[�̃X�L���U����")]
    private AudioClip attackerSkillSE;

    [SerializeField]
    [Tooltip("�A�^�b�J�[�̕K�E���ʉ�")]
    private AudioClip attackerSpecialSE;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̒ʏ�U����")]
    private AudioClip bufferAttackSE;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̃X�e�[�^�X�㏸��")]
    private AudioClip bufferUPStatusSE;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̃X�e�[�^�X����㏸��")]
    private AudioClip bufferSpecialSE;

    [SerializeField]
    [Tooltip("�q�[���[�̒ʏ�U����")]
    private AudioClip healerAttackSE;

    [SerializeField]
    [Tooltip("�q�[���[�̒ʏ�񕜉�")]
    private AudioClip healerNormalHealSE;

    [SerializeField]
    [Tooltip("�q�[���[�̓���񕜉�")]
    private AudioClip healerSpecialSE;

    /// <summary>
    /// �v���C���[�̌��ʉ��}�l�[�W���[���C���X�^���X��
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
    
    /// <summary>
    /// �A�^�b�J�[�̒ʏ�U�����Đ����\�b�h
    /// </summary>
    public void Play_AttackerNormalAttackSE()
    {
        //�ʏ�U�����Đ�
        audioSource.PlayOneShot(attackerNormalAttackSE);
    }

    /// <summary>
    /// �A�^�b�J�[�̃X�L�����Đ����\�b�h
    /// </summary>
    public void Play_AttackerSkillSE()
    {
        //�X�L�����Đ�
        audioSource.PlayOneShot(attackerSkillSE);

        
    }

    /// <summary>
    /// �A�^�b�J�[�̕K�E���Đ����\�b�h
    /// </summary>
    public void Play_AttackerSpecialSE()
    {
        //�K�E���Đ�
        audioSource.PlayOneShot(attackerSpecialSE);
    }

    /// <summary>
    /// �o�b�t�@�[�̒ʏ�U�����Đ����\�b�h
    /// </summary>
    public void Play_BufferNormalAttackSE()
    {
        //�ʏ�U�����Đ�
        audioSource.PlayOneShot(bufferAttackSE);
    }

    /// <summary>
    /// �o�b�t�@�[�̃X�L�����Đ����\�b�h
    /// </summary>
    public void Play_BufferSkillSE()
    {
        //�X�L�����Đ�
        audioSource.PlayOneShot(bufferUPStatusSE);
    }

    /// <summary>
    /// �o�b�t�@�[�̕K�E���Đ����\�b�h
    /// </summary>
    public void Play_bufferSpecialSE()
    {
        //�K�E���Đ�
        audioSource.PlayOneShot(bufferSpecialSE);
    }

    /// <summary>
    /// �q�[���[�̒ʏ�U�����Đ�
    /// </summary>
    public void Play_healerNormalAttackSE()
    {
        //�ʏ�U�����Đ�
        audioSource.PlayOneShot(healerAttackSE);
    }

    /// <summary>
    /// �q�[���[�̃X�L�����Đ�
    /// </summary>
    public void Play_healerSkillSE()
    {
        //�X�L�����Đ�
        audioSource.PlayOneShot(healerNormalHealSE);
    }

    /// <summary>
    /// �q�[���[�̕K�E���Đ�
    /// </summary>
    public void Play_healerSpecialSE()
    {
        //�K�E���Đ�
        audioSource.PlayOneShot(healerSpecialSE);
    }
}
