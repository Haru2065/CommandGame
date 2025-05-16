using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// �x�[�X�̓G�X�e�[�^�X
/// �݌v�}�ɂ���
/// </summary>
public abstract class BaseEnemyStatus : MonoBehaviour
{
    //�G�̍U����
    protected int EnemyAttackPower;

    //�G�̍ő�̗�
    protected int EnemyMaxHP;

    //�G�̌��݂�HP
    private int enemyCurrentHP;

    /// <summary>
    /// �G�̌��݂�HP�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int EnemyCurrentHP
    {
        get => enemyCurrentHP;
        set => enemyCurrentHP = value;
    }

    [SerializeField]
    [Tooltip("�G��HPUI")]
    protected TextMeshProUGUI enemyHPUGUI;

    [SerializeField]
    [Tooltip("�^�[�Q�b�gUI��\������ʒu")]
    private Transform targetPosition;

    [SerializeField]
    [Tooltip("�G�ɍU������Ώۃ^�[�Q�b�g")]
    private GameObject targetUI;

    public GameObject TargetUI
    {
        get => targetUI;
        set => targetUI = value;
    }

    [SerializeField]
    [Tooltip("�A�^�b�J�[�S�̍U���G���X�g�p")]
    private Attacker attacker;

    public Attacker Attacker
    {
        get => attacker;
    }

    [SerializeField]
    [Tooltip("�o�b�t�@�[�e�L�X�g���b�Z�[�W�p")]
    private Buffer buffer;

    public Buffer Buffer
    {
        get => buffer;
    }

    [SerializeField]
    [Tooltip("�q�[���[�e�L�X�g���b�Z�[�W�p")]
    private Healer healer;

    public  Healer Healer
    {
        get => healer;
    }

    [SerializeField]
    [Tooltip("�G�̃f�[�^�x�[�X")]
    private EnemyDataBase enemyDataBase;

    /// <summary>
    /// �G�f�[�^�x�[�X�̃Q�b�^�[
    /// </summary>
    public EnemyDataBase EnemyDataBase
    {
        get => enemyDataBase;
    }

    //�G���������Ă��邩
    private bool enemyIsAlive;

    //�G���������Ă��邩�̃Q�b�^�[�Z�b�^�[
    public bool EnemyIsAlive
    {
        get => enemyIsAlive;
        set => enemyIsAlive = value;
    }

    [SerializeField]
    [Tooltip("�GID")]
    string enemyID;

    //�GID�̃Q�b�^�[�Z�b�^�[
    public string EnemyID
    {
        get => enemyID;
        set => enemyID = value;
    }

    //�������Ă���L�������������ɐݒ肷�郊�X�g
    public List<BasePlayerStatus> StartAlivePlayers;

    /// <summary>
    /// HPUI�\���p�q�N���X�ŏ���
    /// </summary>
    protected abstract void Update();

    /// <summary>
    /// �^�[�Q�b�g��\�����郁�\�b�h
    /// </summary>
    /// <param name="show">�\��������</param>
    public void ShowTargetUI(bool show)
    {
        //�^�[�Q�b�gUI��\������
        targetUI.SetActive(show);

        //�^�[�Q�b�g��UI���^�[�Q�b�g��\������ʒu�ɕ\��
        targetUI.transform.position = targetPosition.position;
    }

    /// <summary>
    ///�}�E�X���N���b�N���ꂽ��^�[�Q�b�g�ɐݒ�
    /// </summary>
    public void OnMouseDown()
    {
        //�G���������Ă���΃^�[�Q�b�g��ݒ�
        if (enemyIsAlive)
        {
            PlayerTargetSelect.Instance.SetTarget(this);
        }
    }

    /// <summary>
    /// �X�P���g���̃f�o�t�ʒm���\���ɂ���R�[���`��
    /// </summary>
    /// <returns>�҂�����</returns>
    protected IEnumerator HideEnemyActionText()
    {
        //2�t���[���҂�
        yield return new WaitForSeconds(2f);
        
        //�e�L�X�g�E�B���h�E���\��
        BattleActionTextManager.Instance.TextDelayHide();

    �@�@//1�t���[���҂�
        yield return null;
    }

    /// <summary>
    /// �����_���Ńv���C���[�ɍU���͂���Ώۂ����߂郁�\�b�h
    /// </summary>
    public virtual void RandomSelect()
    {
        //��x�������Ă���L�����݂̂Ń��X�g�𐮗�����
        List<BasePlayerStatus> TargetAlivePlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        //���X�g�ɃL����������Ύ��s
        if(TargetAlivePlayers.Count > 0)
        {
            //���X�g�̒��ɂ���v���C���[�L������I�����ă^�[�Q�b�g�ɐݒ�
            BasePlayerStatus target = TargetAlivePlayers[Random.Range(0,TargetAlivePlayers.Count)];

            //�ݒ肵���^�[�Q�b�g�Ƀ_���[�W��^����
            target.PlayerOnDamage(EnemyAttackPower);
        }
        else 
        {
            Debug.Log("�U���Ώۂ����܂���");
        }
    }


    /// <summary>
    /// �G�̃_���[�W����
    /// �q�N���X�Ŏ��Ԃ���邽��abstract�ɂ���
    /// </summary>
    /// <param name="damage">�v���C���[�̍U���͕��̃_���[�W</param>
    public abstract void EnemyOnDamage(int damage);

    /// <summary>
    /// �G�̃p�����[�^����ݒ肷�郁�\�b�h
    /// �q�N���X�Ŏ��Ԃ���邽��abstract�ɂ���
    /// </summary>
    protected abstract void SetEnemyParameters();

    protected abstract IEnumerator DestroyObject();
}
