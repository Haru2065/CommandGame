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
        enemyHPUGUI.text = $"{EnemyCurrentHP}/2000";
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
    public IEnumerator SlimeAction()
    {
        //�����_���Ńv���C���[�ɍU������Ώۂ�I�����čU�����郁�\�b�h�����s
        RandomSelect();
        
        //�X���C���̍U�����ʉ��Đ�
        EnemySE.Instance.Play_slimeAttackSE();

        //1�t���[���҂�
        return null;
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
            BasePlayerStatus target = TargetAlivePlayers[Random.Range(0, TargetAlivePlayers.Count)];

            Debug.Log(target.PlayerID + "�ɍU��");

            target.PlayerOnDamage(EnemyAttackPower);

            // �^�[�Q�b�g�̎�ނɉ����āA�K�؂ȃo�g���A�N�V�����e�L�X�g��\������
            switch (target)
            {
                // �^�[�Q�b�g�� Attacker �̏ꍇ
                case var _ when target == Attacker:
                    
                    //�_���[�W���󂯂����Ƃ������e�L�X�g��\��
                    BattleActionTextManager.Instance.ShowBattleActionText("DamageAttacker");

                    // ��莞�Ԍ�ɓG�̃A�N�V�����e�L�X�g���\���ɂ���
                    StartCoroutine(HideEnemyActionText());
                    break;

                // �^�[�Q�b�g�� Buffer �̏ꍇ
                case var _ when target == Buffer:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageBuffer");

                    StartCoroutine(HideEnemyActionText());
                    break;

                // �^�[�Q�b�g�� Healer �̏ꍇ
                case var _ when target == Healer:

                    BattleActionTextManager.Instance.ShowBattleActionText("DamageHealer");

                    StartCoroutine(HideEnemyActionText());

                    break;
            }
        }
        else
        {
            Debug.Log("�U���Ώۂ����܂���");
        }
    }

    /// <summary>
    /// �X���C���̃_���[�W����
    /// </summary>
    /// <param name="damage">�v���C���[����̍U�����_���[�W��</param>
    public override void EnemyOnDamage(int damage)
    {
        EnemyCurrentHP -= damage;
        slimeHPBar.value = EnemyCurrentHP;
        

        if (EnemyCurrentHP <= 0)
        {
            //���݂�HP���O�ɐݒ肵��HPBar���X�V
            EnemyCurrentHP = 0;
            slimeHPBar.value = EnemyCurrentHP;

            EnemyIsAlive = false;

            //�������X�g�Ə����^�[�Q�b�g��ݒ肷�郊�X�g�̏���
            Stage1BattleSystem.Instance.aliveEnemies.Remove(this);
            PlayerTargetSelect.Instance.RemoveSetTarget(this);

            //�A�^�b�J�[�̑S�̍U���̃��X�g����폜
            Attacker.RemoveDeadEnemies();

            //���g�̃I�u�W�F�N�g����������R�[���`���X�^�[�g
            StartCoroutine(DestroyObject());
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

