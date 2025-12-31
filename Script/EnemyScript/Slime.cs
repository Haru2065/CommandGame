using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �X���C���X�e�[�^�X�X�N���v�g
/// </summary>
public class Slime : BaseEnemyStatus
{
    // Constants for magic numbers
    private const int SLIME_MAX_HP_DISPLAY = 2500;
    private const float ACTION_WAIT_TIME = 2f;
    private const float EFFECT_DESTROY_TIME = 2f;

    [SerializeField]
    [Tooltip("�X���C��HP�o�[")]
    private Slider slimeHPBar;

    // Start is called before the first frame update
    void Start()
    {
        //�G�̃p�����[�^��ݒ�
        SetEnemyParameters();

        //�X���C���𐶑���Ԃ�
        EnemyIsAlive = true;
    }

    /// <summary>
    /// �X���C����HP��\�����鏈��
    /// </summary>
    protected override void Update()
    {
        enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";
    }

    /// <summary>
    /// �p�����[�^��ݒ肷�郁�\�b�h
    /// �G�̃f�[�^�x�[�X����ǂ݂���
    /// </summary>
    protected override void SetEnemyParameters()
    {
        //�����N�@�\���g���āA�G�̃f�[�^�x�[�X�̍ŏ��̗v�f����G��ID���擾
        var enemyData = EnemyDataBase.EnemyParameters.FirstOrDefault(e => e.EnemyNameData == EnemyID);

        //���v����΃p�����[�^��ݒ�
        if (enemyData != null)
        {
            //���݂�HP��G�f�[�^�ɐݒ肳��Ă���ő�̗͂ɂ���
            EnemyMaxHP = enemyData.EnemyMaxHPData;
            
            //���݂̓G�̗̑͂��ő�ɐݒ�
            EnemyCurrentHP = EnemyMaxHP;

            //�X���C����HP�o�[���ő�̗͂ɐݒ�
            slimeHPBar.maxValue = EnemyCurrentHP;
            slimeHPBar.value = EnemyCurrentHP;

            //�X���C����HP�o�[�̍ŏ��͂O�ɐݒ�
            slimeHPBar.minValue = 0;

            //�X���C���̍U���͂��G�l�~�[�f�[�^�̍U���͂ɐݒ�
            EnemyAttackPower = enemyData.EnemyAttackPowerData;
        }
        else
        {
            Debug.LogError($"{EnemyID} �̃f�[�^���f�[�^�x�[�X�ɑ��݂��܂���I");
        }
    }

    /// <summary>
    /// �X���C���̃^�[���ɍs�����郁�\�b�h
    /// </summary>
    public async UniTask SlimeAction()
    {
        //�����_���Ńv���C���[�ɍU������Ώۂ�I�����čU�����郁�\�b�h�����s
        RandomSelect();
        
        //�X���C���̍U�����ʉ��Đ�
        EnemySE.Instance.Play_slimeAttackSE();

        //2�t���[���҂�
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
    }

    /// <summary>
    /// �v���C���[�Ƀ����_���ōU�����郁�\�b�h
    /// </summary>
    public override BasePlayerStatus RandomSelect()
    {
        //��x�������Ă���L�����݂̂Ń��X�g�𐮗�����
        List<BasePlayerStatus> TargetAlivePlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        //���X�g�ɃL����������Ύ��s
        if (TargetAlivePlayers.Count > 0)
        {
            //���X�g�̒��ɂ���v���C���[�L������I�����ă^�[�Q�b�g�ɐݒ�
            BasePlayerStatus target = TargetAlivePlayers[UnityEngine.Random.Range(0, TargetAlivePlayers.Count)];

            Debug.Log(target.PlayerID + "�ɍU��");

            //�^�[�Q�b�g�̈ʒu�ɃG�t�F�N�g�𐶐�
            GameObject effectInstance = Instantiate(OnlyAttackEffect, target.transform.position, Quaternion.identity);

            target.PlayerOnDamage(EnemyAttackPower);

            //2�t���[����G�t�F�N�g������
            Destroy(effectInstance, 2f);

            //�ݒ肵���^�[�Q�b�g��Ԃ�
            return target;
        }
        else
        {
            Debug.Log("�U���Ώۂ����܂���");
            return null;
        }
    }

    /// <summary>
    /// �X���C���̃_���[�W����
    /// </summary>
    /// <param name="damage">�v���C���[����̍U�����_���[�W��</param>
    public override void EnemyOnDamage(int damage)
    {
        EnemyCurrentHP -= damage;

        if (EnemyCurrentHP <= 0)
        {
            //���݂�HP���O�ɐݒ肵��HPBar���X�V
            EnemyCurrentHP = 0;

            // UI�X�V
            slimeHPBar.value = EnemyCurrentHP;
            enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";

            //�����t���O��false
            EnemyIsAlive = false;

            //�������X�g�Ə����^�[�Q�b�g��ݒ肷�郊�X�g�̏���
            Stage1BattleSystem.Instance.aliveEnemies.Remove(this);
            PlayerTargetSelect.Instance.RemoveSetTarget(this);

            //�A�^�b�J�[�̑S�̍U���̃��X�g����폜
            Attacker.RemoveDeadEnemies();

            //���g�̃I�u�W�F�N�g����������R�[���`���X�^�[�g
            StartCoroutine(DestroyObject());
        }
        else
        {
            // �ʏ�̃_���[�W����UI�X�V
            slimeHPBar.value = EnemyCurrentHP;
            enemyHPUGUI.text = $"{EnemyCurrentHP}/2000";
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g����������R�[���`��
    /// </summary>
    /// <returns>1�t���[���҂�</returns>
    protected override IEnumerator DestroyObject()
    {
        //�X���C���̃I�u�W�F�N�g������
        Destroy(gameObject);
        
        yield return null;
    }
}

