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

    [SerializeField]
    [Tooltip("�h���S���̃f�o�t��(HP�����l)")]
    private int hpDebuffPower;

    /// <summary>
    /// �h���S���̃f�o�t�͂̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int HPDebuffPower
    {
        get => hpDebuffPower;
        set => hpDebuffPower = value;
    }

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

    /// <summary>
    /// �h���S���̍s��
    /// </summary>
    /// <param name="playerParty">�U���Ώێ҂̃��X�g</param>
    /// <returns></returns>
    public async UniTask DragonAction(List<BasePlayerStatus> playerParty)
    {
        //�h���S�����������Ă��Ȃ���������s���Ȃ�
        if (!EnemyIsAlive) return;

        //�h���S���̍U���p�^�[���ʂ�U�������s
        await DoAttack(playerParty);
    }

    /// <summary>
    /// �h���S���̃p�^�[���U��
    /// </summary>
    /// <param name="playerParty"></param>
    /// <returns></returns>
    public async UniTask DoAttack(List<BasePlayerStatus> playerParty)
    {
        //�^�[���̃J�E���g��1���₷
        turnCount++;

        //
        int puttern = (turnCount - 1) % 3;

        //�U���p�^�[��
        switch (puttern)
        {
            //�p�^�[��1�����_���ɓG�ɍU�����s��
            case 0:
                RandomSelect();
                break;
            
            //�p�^�[��2�u���X�U���S�̍U��
            case 1:
                BreathAllAttack(playerParty);

                //2�t���[���҂�
                await UniTask.Delay(TimeSpan.FromSeconds(2f));
                break;

            //�p�^�[��3�K�E�U���v���C���[�ɑS�̍U��+�_���[�W�f�o�t�t�^
            case 2:
                SpecialAllAttack(playerParty);

                //2�t���[���҂�
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

            HPDebuffPower = enemyData.DebuffPowerData;
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

        //�Ώۂ̃v���C���[���������Ă�����A���X�g�ɂ����i�����҂��擾)
        List<BasePlayerStatus> attackTargetPlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        //�U���Ώۂ�����΍U���Ώۃ��X�g�ɓ����Ă���v���C���[�ɍU��
        if (attackTargetPlayers.Count > 0)
        {
            foreach (var player in attackTargetPlayers)
            {
                //�v���C���[�Ƀ_���[�W��^����
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

        //�Ώۂ̃v���C���[���������Ă�����A���X�g�ɂ����i�����҂��擾)
        List<BasePlayerStatus> attackPlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        // �U���Ώۂ�����΍U���Ώۃ��X�g�ɓ����Ă���v���C���[�ɍU��
        if (attackPlayers.Count > 0)
        {
            foreach (var player in attackPlayers)
            {
                //�v���C���[�Ƀ_���[�W��^���āA�₯�ǂ̃f�o�t�i���^�[��HP�����炷2�^�[���p���j
                player.PlayerOnDamage(dragonSpecialAttackPower);
                player.IsHPDebuff = true;

                //�f�o�t�p���J�E���g��2�ɐݒ�
                player.DebuffCount = 2;
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

        //�h���S�����������Ă��Ȃ������猻�݂�HP��0
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
