using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �X�P���g���̃X�e�[�^�X
/// </summary>
public class Skeleton : BaseEnemyStatus
{
    [SerializeField]
    [Tooltip("�X�P���g���̃f�o�t�p���[")]
    private int debuffPower;

    [SerializeField]
    [Tooltip("�X�P���g����HP�o�[")]
    public Slider SkeltonHPBar;

    // Start is called before the first frame update
    void Start()
    {
        //�X�P���g���̃p�����[�^��ݒ�
        SetEnemyParameters();

        //�X�P���g���𐶑���Ԃ�
        EnemyIsAlive = true;
    }

    /// <summary>
    /// �X�P���g����HP��\�����鏈��
    /// </summary>
    protected override void Update()
    {
        enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";
    }

    /// <summary>
    /// �p�����[�^��ݒ肷�郁�\�b�h
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

            //�X�P���g���̌��݂̗̑͂��ő�ɐݒ�
            EnemyCurrentHP = EnemyMaxHP;

            //�X�P���g����HP�o�[���ő�̗͂ɐݒ�
            SkeltonHPBar.maxValue = EnemyCurrentHP;
            SkeltonHPBar.value = EnemyCurrentHP;

            //�X�P���g����HP�o�[�̍ŏ��͂O�ɐݒ�
            SkeltonHPBar.minValue = 0;

            //�X�P���g���̍U���͂��G�l�~�[�f�[�^�̍U���͂ɐݒ�
            EnemyAttackPower = enemyData.EnemyAttackPowerData;

            //�X�P���g���̃f�o�t�͂��G�l�~�[�f�[�^�̃f�o�t�͂ɐݒ�
            debuffPower = enemyData.DebuffPowerData;
        }
        else
        {
            Debug.LogError($"{EnemyID} �̃f�[�^���f�[�^�x�[�X�ɑ��݂��܂���I");
        }
    }

    /// <summary>
    /// �X�P���g�����U������UniTask
    /// </summary>
    public async UniTask SkeletonAction(Skeleton skelton)
    {
        //�v���C���[���U������^�[�Q�b�g�������_���Őݒ�
        RandomSelect();

        //2�t���[���҂�
        await UniTask.Delay(TimeSpan.FromSeconds(2));

    }

    /// <summary>
    /// �v���C���[�Ƀ����_���ōU�����郁�\�b�h
    /// </summary>
    public override void RandomSelect()
    {
        //��x�������Ă���L�����݂̂Ń��X�g�𐮗�����
        List<BasePlayerStatus> TargetAlivePlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        //���X�g�ɃL����������Ύ��s
        if (TargetAlivePlayers.Count > 0)
        {
            //���X�g�̒��ɂ���v���C���[�L������I�����ă^�[�Q�b�g�ɐݒ�
            BasePlayerStatus target = TargetAlivePlayers[UnityEngine.Random.Range(0, TargetAlivePlayers.Count)];

            target.PlayerOnDamage(EnemyAttackPower);

            switch (target)
            {
                case var _ when target == Attacker:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageAttacker");

                    StartCoroutine(HideEnemyActionText());

                    break;

                case var _ when target == Buffer:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageBuffer");

                    StartCoroutine(HideEnemyActionText());

                    break;

                case var _ when target == Healer:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageHealer");

                    StartCoroutine(HideEnemyActionText());

                    break;
            }

            //�X�P���g���̌��ʉ��Đ�
            EnemySE.Instance.Play_SkeletonAttackSE();

            //�T�O�p�[�Z���g�̊m�����v���C���[���f�o�t��Ԃ�false�Ȃ�f�o�t�t�^
            if (UnityEngine.Random.Range(0, 100) < 50 && target.IsDebuff == false)
            {
                target.IsDebuff = true;

                target.DebuffCount = 3;

                //�^�[�Q�b�g�̍U���͂��f�o�t�͕�������
                target.AttackPower -= debuffPower;

                if (target.AttackPower < 50)
                {
                    target.AttackPower = 50;
                }

                //�^�[�Q�b�g���A�^�b�J�[���o�b�t�@�[���q�[���[�����ꂩ�̂ǂꂩ�������ꍇ���̃L�����̍s���ʒm�\��
                switch (target)
                {
                    //case����when���g���ǉ��������s��
                    case var _ when target == Attacker:

                        //JSON�A�^�b�J�[�̓���f�o�t�t�^�ʒm�\��
                        BattleActionTextManager.Instance.ShowBattleActionText("AttackerOnDebuff");

                        //�s���ʒmUI���\��
                        StartCoroutine(HideEnemyActionText());

                        break;

                    case var _ when target == Buffer:

                        //JSON�o�b�t�@�[�̓���f�o�t�t�^�ʒm�\��
                        BattleActionTextManager.Instance.ShowBattleActionText("BufferOnDebuff");

                        //�s���ʒmUI���\��
                        StartCoroutine(HideEnemyActionText());

                        break;

                    case var _ when target == Healer:

                        //JSON�q�[���[�̓���f�o�t�t�^�ʒm�\��
                        BattleActionTextManager.Instance.ShowBattleActionText("HealerOnDebuff");

                        //�s���ʒmUI���\��
                        StartCoroutine(HideEnemyActionText());

                        break;

                    default:
                        break;

                }
            }

            //10%�̊m���ł��A�v���C���[���f�o�t��ԂŁA����f�o�t��ԂłȂ���Ύ��s
            if (UnityEngine.Random.Range(0, 100) < 10 && target.IsDebuff && target.IsSpecialDebuff == false)
            {
                target.SpecialDebuffCount = 5;

                target.IsSpecialDebuff = true;

                //�^�[�Q�b�g�̍U���͂��f�o�t�͕�������
                target.AttackPower -= debuffPower;

                if (target.AttackPower < 20)
                {
                    target.AttackPower = 20;
                }

                //�^�[�Q�b�g���A�^�b�J�[���o�b�t�@�[���q�[���[�����ꂩ�̂ǂꂩ�������ꍇ���̃L�����̍s���ʒm�\��
                switch (target)
                {
                    // �^�[�Q�b�g�� Attacker �̏ꍇ
                    case var _ when target == Attacker:

                        //�_���[�W���󂯂����Ƃ������e�L�X�g��\��
                        BattleActionTextManager.Instance.ShowBattleActionText("AttackerOnSpecialDebuff");

                        // ��莞�Ԍ�ɓG�̃A�N�V�����e�L�X�g���\���ɂ���
                        StartCoroutine(HideEnemyActionText());
                        break;

                    // �^�[�Q�b�g�� Attacker �̏ꍇ
                    case var _ when target == Buffer:

                        BattleActionTextManager.Instance.ShowBattleActionText("BufferOnSpecialDebuff");

                        StartCoroutine(HideEnemyActionText());
                        break;

                    // �^�[�Q�b�g�� Attacker �̏ꍇ
                    case var _ when target == Healer:

                        BattleActionTextManager.Instance.ShowBattleActionText("HealerOnSpecialDebuff");

                        StartCoroutine(HideEnemyActionText());
                        break;

                    default:

                        break;
                }
            }
        }
        else
        {
            Debug.Log("�U���Ώۂ����܂���");
        }
    }

    /// <summary>
    /// �X�P���g���̃_���[�W����
    /// </summary>
    /// <param name="damage">�v���C���[����̍U�����_���[�W����</param>
    public override void EnemyOnDamage(int damage)
    {
        EnemyCurrentHP -= damage;

        //�X�P���g����HP���O��艺�ɂ����Ȃ��悤�ɂ���
        if (EnemyCurrentHP <= 0)
        {
            EnemyCurrentHP = 0;

            // UI�X�V
            SkeltonHPBar.value = EnemyCurrentHP;
            enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";

            //�����t���O��false
            EnemyIsAlive = false;

            //�������X�g�Ə����^�[�Q�b�g��ݒ肷�郊�X�g�̏���
            Stage2BattleSystem.Instance.aliveEnemies.Remove(this);
            PlayerTargetSelect.Instance.RemoveSetTarget(this);

            //�A�^�b�J�[�̑S�̍U���̃��X�g����폜
            Attacker.RemoveDeadEnemies();

            //���g�̃I�u�W�F�N�g����������R�[���`���X�^�[�g
            StartCoroutine(DestroyObject());
        }
        else
        {
            // �ʏ�̃_���[�W����UI�X�V
            SkeltonHPBar.value = EnemyCurrentHP;
            enemyHPUGUI.text = $"{EnemyCurrentHP}/2500";
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
