using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �h���S���̃X�e�[�^�X
/// </summary>
public class Dragon : BaseEnemyStatus
{
    [SerializeField]
    [Tooltip("�h���S����HPBar")]
    private Slider DragonHPBar;

    [SerializeField]
    [Tooltip("�h���S���̕K�E�̍U����")]
    private int dragonSpecialAttackPower;

    //�h���S���̃^�[���J�E���g
    private int turnCount = 0;

    /// <summary>
    /// �^�[���J�E���g�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int TurnCount
    {
        get => turnCount; set => turnCount = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        //�h���S���̃p�����[�^��ݒ�
        SetEnemyParameters();

        //�h���S���𐶑���Ԃɂ���
        EnemyIsAlive = true;
    }

    public async UniTask DragonAction(List<BasePlayerStatus> playerParty)
    {
        if (!EnemyIsAlive) return;

        await DoAttack(playerParty);
    }

    public async UniTask DoAttack(List<BasePlayerStatus> playerParty)
    {
        TurnCount++;

        int puttern = (turnCount - 1) % 3;

        switch (puttern)
        {
            case 0:
                RandomSelect();
                break;

            case 1:
                BreathAllAttack(playerParty);

                await UniTask.Delay(TimeSpan.FromSeconds(2f));
                break;

            case 2:
                SpecialAllAttack(playerParty);

                await UniTask.Delay(TimeSpan.FromSeconds(2f));
                break;
        }
    }

    /// <summary>
    /// �h���S���̃p�����[�^�ݒ胁�\�b�h
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

            //�h���S���̌��݂̗̑͂��ő�̗͂ɐݒ�
            EnemyCurrentHP = EnemyMaxHP;

            //�h���S����HP�o�[���ő�̗͂ɐݒ�
            DragonHPBar.maxValue = EnemyCurrentHP;
            DragonHPBar.value = EnemyCurrentHP;

            //�h���S����HP�o�[�̍ŏ���0�ɐݒ�
            DragonHPBar.minValue = 0;

            //�h���S���̍U���͂�G�f�[�^�̍U���͂ɐݒ�
            EnemyAttackPower = enemyData.EnemyAttackPowerData;
        }
        else
        {
            Debug.LogError($"{EnemyID}�̃f�[�^���f�[�^�x�[�X�ɑ��݂��܂���!");
        }
    }

    /// <summary>
    /// �����_���Ńv���C���[�P�̂ɍU�����郁�\�b�h
    /// </summary>
    public override void RandomSelect()
    {
        //�h���S���̒P�̍U�������Đ�
        EnemySE.Instance.Play_DragonSingleAttackSE();

        //�x�[�X�̃����_���Z���N�g���\�b�h�����s
        base.RandomSelect();
    }

    /// <summary>
    /// �ʏ�̑S�̍U��
    /// </summary>
    /// <param name="players"></param>
    private void BreathAllAttack(List<BasePlayerStatus> playerParty)
    {
        //�h���S���̃u���X�U�����Đ�
        EnemySE.Instance.Play_DragonBreathSE();

        List<BasePlayerStatus> attackPlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        if (attackPlayers.Count > 0)
        {
            foreach (var player in attackPlayers)
            {
                player.PlayerOnDamage(EnemyAttackPower);
            }
        }
        else
        {
            Debug.Log("�U���Ώۂ����܂���");
        }

    }

    /// <summary>
    /// �K�E�U��(�S�̂ɍU����^���Ă����2�^�[���p���Ńf�o�t��t�^�j
    /// </summary>
    /// <param name="players"></param>
    private void SpecialAllAttack(List<BasePlayerStatus> playerParty)
    {
        //�h���S���̕K�E�U�������Đ�
        EnemySE.Instance.Play_DragonSpecialAttackSE();

        List<BasePlayerStatus> attackPlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        if (attackPlayers.Count > 0)
        {
            foreach (var player in attackPlayers)
            {
                player.PlayerOnDamage(dragonSpecialAttackPower);
            }
        }
        else
        {
            Debug.Log("�U���Ώۂ����܂���");
        }
    }

    /// <summary>
    /// �h���S����HP��\�����鏈��
    /// </summary>
    protected override void Update()
    {
        enemyHPUGUI.text = $"{EnemyCurrentHP} / {EnemyMaxHP}";

        if(!EnemyIsAlive)
        {
            EnemyCurrentHP = 0;
        }
    }

    /// <summary>
    /// �h���S���̃_���[�W����
    /// </summary>
    /// <param name="damage">�v���C���[����̍U�����_���[�W��</param>
    public override void EnemyOnDamage(int damage)
    {
        EnemyCurrentHP -= damage;
        DragonHPBar.value = EnemyCurrentHP;

        //�h���S����HP���O��艺�ɂ����Ȃ��悤�ɂ���
        if (EnemyCurrentHP <= 0)
        {
            EnemyCurrentHP = 0;
            DragonHPBar.value = EnemyCurrentHP;

            EnemyIsAlive = false;

            //�������X�g�Ə����^�[�Q�b�g��ݒ肷�郊�X�g�̏���
            Stage2BattleSystem.Instance.aliveEnemies.Remove(this);
            PlayerTargetSelect.Instance.RemoveSetTarget(this);

            //�A�^�b�J�[�̑S�̍U���̃��X�g����폜
            Attacker.RemoveDeadEnemies();

            //�h���S���̃I�u�W�F�N�g��������R�[���`���X�^�[�g
            StartCoroutine(DestroyObject());
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g����������R�[���`��
    /// </summary>
    /// <returns>1�t���[���҂�</returns>
    protected override IEnumerator DestroyObject()
    {
        //�h���S���̃I�u�W�F�N�g������
        Destroy(gameObject);

        yield return null;
    }
}
