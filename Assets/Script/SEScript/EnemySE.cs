using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// �G�̌��ʉ�
/// </summary>
public class EnemySE : MonoBehaviour
{
    //�G�̌��ʉ����C���X�^���X���p
    private static EnemySE instance;

    /// <summary>
    /// �G�̌��ʉ��C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static EnemySE Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("���ʉ��p�̃I�[�f�B�I�\�[�X")]
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip("�X���C���̍U����")]
    private AudioClip slimeAttackSE;

    [SerializeField]
    [Tooltip("�X�P���g���̍U����")]
    private AudioClip skeletonAttackSE;

    [SerializeField]
    [Tooltip("�h���S���̖�����")]
    private AudioClip dragonRoarSE;

    [SerializeField]
    [Tooltip("�h���S���̒P�̍U����")]
    private AudioClip dragonSingleAttackSE;

    [SerializeField]
    [Tooltip("�h���S���̃u���X�U����")]
    private AudioClip dragonBreathSE;

    [SerializeField]
    [Tooltip("�h���S���̕K�E�U����")]
    private AudioClip dragonSpecialAttackSE;


    /// <summary>
    /// �G�̌��ʉ��}�l�[�W���[���C���X�^���X��
    /// </summary>
    void Awake()
    {
        //�C���X�^���X���Ȃ���΃C���X�^���X
        if (instance == null)
        {

            instance = this;
        }
        //����΃I�u�W�F�N�g������
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �X���C���̍U�������Đ����\�b�h
    /// </summary>
    public void Play_slimeAttackSE()
    {
        //�X���C���̍U�����Đ�
        audioSource.PlayOneShot(slimeAttackSE);
    }

    /// <summary>
    /// �X�P���g���̍U�������Đ����\�b�h
    /// </summary>
    public void Play_SkeletonAttackSE()
    {
        //�X�P���g���̍U�����Đ�
        audioSource.PlayOneShot(skeletonAttackSE);
    }

    /// <summary>
    /// �h���S���̖����Đ����\�b�h
    /// </summary>
    public void Play_DragonRourSE()
    {
        //�h���S���̖������Đ�
        audioSource.PlayOneShot(dragonRoarSE);
    }

    /// <summary>
    /// �h���S���̒P�̍U�����Đ����\�b�h
    /// </summary>
    public void Play_DragonSingleAttackSE()
    {
        //�h���S���̒P�̍U�������Đ�
        audioSource.PlayOneShot(dragonSingleAttackSE);
    }

    /// <summary>
    /// �h���S���̃u���X�U���Đ����\�b�h
    /// </summary>
    public void Play_DragonBreathSE()
    {
        //�h���S���̃u���X�U�������Đ�
        audioSource.PlayOneShot(dragonBreathSE);
    }

    /// <summary>
    /// �h���S���̕K�E�U���Đ����\�b�h
    /// </summary>
    public void Play_DragonSpecialAttackSE()
    {
        //�h���S���̕K�E�U�������Đ�
        audioSource.PlayOneShot(dragonSpecialAttackSE); 
    }
}
