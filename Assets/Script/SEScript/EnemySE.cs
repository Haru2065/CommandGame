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
}
