using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �A�^�b�J�[�̃X�e�[�^�X
/// �X�N���v�^�u���I�u�W�F�N�g����f�[�^��ǂݍ���
/// �x�[�X�v���C���[�X�e�[�^�X���p��
/// </summary>
public class Attacker : BasePlayerStatus
{
    //�A�^�b�J�[�̍s�����I��������
    public bool IsAttackerAction;

    [SerializeField]
    [Tooltip("�G�̃��X�g�S�̍U���K�E�Ɏg�p")]
    private List<BaseEnemyStatus> targetEnemys;

    [SerializeField]
    [Tooltip("�A�^�b�J�[�p�ʏ�U���p�G�t�F�N�g")]
    private GameObject attacker_NormalEffect;

    [SerializeField]
    [Tooltip("�A�^�b�J�[�̕K�E�U���p�G�t�F�N�g")]
    private GameObject attacker_SpecialEffect;

    [SerializeField]
    [Tooltip("�A�^�b�J�[�̕����G�t�F�N�g")]
    private GameObject attacker_TextEffect;

    [SerializeField]
    [Tooltip("�G�ɍU������G�t�F�N�g��\��������ʒu1")]
    private Transform specialAttackEffect_SpawnPoint1;

    [SerializeField]
    [Tooltip("�G�ɍU������G�t�F�N�g��\��������ʒu2")]
    private Transform specialAttackEffect_SpawnPoint2;

    [SerializeField]
    [Tooltip("�G�ɍU������G�t�F�N�g��\��������ʒu3")]
    private Transform specialAttackEffect_SpawnPoint3;

    // Start is called before the first frame update
    protected override void Start()
    {
        //�p�����[�^��ݒ�
        base.Start();

        IsUseSkill = false;

        //�ŏ��͕K�E�͎g���Ȃ��悤�ɂ���
        IsUseSpecial = true;

        //���ʂ̃f�o�t�Ɠ���f�o�t�p���J�E���g�̏�����
        DebuffCount = 0;
        SpecialDebuffCount = 0;

        IsDebuff = false;
        IsSpecialDebuff = false;

        //SkillLimitCount = 3;
        //SpecialLimitCount = 5;

        //�A�^�b�J�[�̍s���t���O��false��
        IsAttackerAction = false;
    }

    /// <summary>
    /// �v���C���[�̍s���I���������i�A�^�b�J�[���s���������j�̃t���O�����Z�b�g�ɂ��郁�\�b�h
    /// </summary>
    public override void ResetActionFlag()
    {
        //�x�[�X�̃��\�b�h����v���C���[�s���t���O�����Z�b�g
        base.ResetActionFlag();

        //�A�^�b�J�[�̍s���t���O��false
        IsAttackerAction = false;
    }

    /// <summary>
    /// �p�����[�^��ݒ肷�郁�\�b�h
    /// �v���C���[�̃f�[�^�x�[�X����ǂݍ���
    /// </summary>
    protected override void SetPlayerParameters()
    {
        // LINQ���g���A�v���C���[�f�[�^�x�[�X����PlayerID�ƈ�v����v���C���[�����擾
        var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);

        // ��v����v���C���[��񂪌��������ꍇ�A�p�����[�^��ݒ�
        if (playerData != null)
        {

            //�ő�̗͂̃f�[�^��ǂݍ���
            PlayerMaxHP = playerData.PlayerMaxHPData;

            //�A�^�b�J�[�̌��݂�HP���ő�ɐݒ肵��HP�o�[���ő�ɐݒ�
            PlayerCurrentHP = PlayerMaxHP;

            Debug.Log($"PlayerCuurentHP:{PlayerCurrentHP},PlayerMaxHP:{PlayerMaxHP}");
            
            PlayerHPBar.maxValue = PlayerCurrentHP;
            PlayerHPBar.value = PlayerCurrentHP;
            PlayerHPBar.minValue = 0;

            //�U���͂��A�^�b�J�[�̃f�[�^�̍U���͂�ǂݍ���
            AttackPower = playerData.PlayerAttackPowerData;

            //�����U���͂��A�^�b�J�[�̍U���͂ɐݒ�
            PlayerResetAttackPower = AttackPower;

            //������Ԃɂ���
            IsAlive = true;
        }
    }

    /// <summary>
    /// �ʏ�U��
    /// </summary>
    public override void NormalAttack()
    {
        //�v���C���[���I�񂾓G���U���Ώۂɐݒ肷��
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        if (target != null)
        {
            //�ʏ�U�����Đ�
            PlayerSE.Instance.Play_AttackerNormalAttackSE();

            //�A�^�b�J�[�̒ʏ�U�����̃G�t�F�N�g�𐶐�
            GameObject effectInstance = Instantiate(attacker_NormalEffect, PlayerEffect_SpawnPoint.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(attacker_TextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            //�U������Ώۂ̓G�Ƀ_���[�W��^����
            target.EnemyOnDamage(AttackPower);

            //�A�^�b�J�[�̃G�t�F�N�g������
            Destroy(effectInstance, 0.2f);
            Destroy(textEffectInstance, 2f);
        }

        //�A�^�b�J�[�̍s���t���O��true��
        IsAttackerAction = true;
    }

    /// <summary>
    /// �A�^�b�J�[�X�L��
    /// </summary>
    public override void PlayerSkill()
    {
        //�U���͂��Q�{�ɂ���
        AttackPower *= 2;

        Debug.Log("�U����" + AttackPower);

        //�v���C���[���I�񂾓G���U���Ώۂɐݒ肷��
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        //�^�[�Q�b�g���ݒ肳��Ă����珈�������s
        if (target != null)
        {
            //�A�^�b�J�[�̃X�L�����ʉ��Đ�
            PlayerSE.Instance.Play_AttackerSkillSE();

            //�A�^�b�J�[�̍U���G�t�F�N�g�ƃe�L�X�g�G�t�F�N�g�𐶐�
            GameObject effectInstance = Instantiate(attacker_NormalEffect, transform.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(attacker_TextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            //�x��Ă�����G�t�F�N�g�𐶐�
            Invoke("DelayEffect", 0.3f);

            target.EnemyOnDamage(AttackPower);

            //�A�^�b�J�[�̃G�t�F�N�g������
            Destroy(effectInstance, 0.2f);
            Destroy(textEffectInstance, 4f);
        }

        var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);
        AttackPower = playerData.PlayerAttackPowerData;

        //�X�L�����g�������̃t���O��true
        IsUseSkill = true;

        //�X�L���g�p�����J�E���g��3�^�[���ɐݒ�
        SkillLimitCount = 3;

        IsAttackerAction = true;
    }

    /// <summary>
    /// �A�^�b�J�[�K�E
    /// </summary>
    public override void SpecialSkill()
    {
        // ���X�g targetEnemys �̍Ō�̗v�f���珇�ɏ������s��
        // �v�f���폜����ۂɃC���f�b�N�X�������̂�h�����ߌ�납��s��
        for (int i = targetEnemys.Count - 1; i >= 0; i--)
        {
            // ���݂̃^�[�Q�b�g�ƂȂ�G���擾
            BaseEnemyStatus enemy = targetEnemys[i];

            //�^�[�Q�b�g�ƃI�u�W�F�N�g�����݂��Ȃ������珈�����X�L�b�v
            if (enemy == null && enemy.gameObject == null) continue;

            //�A�^�b�J�[�K�E�U�������Đ�
            PlayerSE.Instance.Play_AttackerSpecialSE();

            //���������U���G�t�F�N�g���i�[���郊�X�g
            List<GameObject> specialEffects = new List<GameObject>();

            //���������e�L�X�g�G�t�F�N�g���i�[���郊�X�g
            List<GameObject> textEffects = new List<GameObject>();

            //�^�[�Q�b�g���������Ă�����G�t�F�N�g�𐶐�
            if (enemy.EnemyIsAlive)
            {
                //�U���G�t�F�N�g�̐����ʒu���擾
                Transform spawnPoint = GetSpawnPoint(i);

                //�X�|�[���ʒu�����݂���ꍇ�̂ݐ���
                if (spawnPoint != null)
                {
                    //�G�t�F�N�g�����X�g�Ɋi�[������X�|�[���ʒu�ɐ���
                    specialEffects.Add(Instantiate(attacker_SpecialEffect, spawnPoint.position, Quaternion.identity));
                    textEffects.Add(Instantiate(attacker_TextEffect, spawnPoint.position, Quaternion.identity));
                }

                //�G�ɍU�����s��
                enemy.EnemyOnDamage(AttackPower);

                //3�b��S�Ă̍U���G�t�F�N�g������
                foreach (var effect in specialEffects)
                {
                    Destroy(effect, 3f);
                }

                //3�b��S�Ẵe�L�X�g�G�t�F�N�g������
                foreach (var textEffect in textEffects)
                {
                    Destroy(textEffect, 3f);
                }
            }

            //�K�E���g�p�����̂ŕK�E�t���O��true
            IsUseSpecial = true;

            //�K�E�̐����J�E���g��3�ɐݒ�
            SpecialLimitCount = 3;

            IsAttackerAction = true;
        }
    }



    /// <summary>
    /// �v���C���[�̃_���[�W����
    /// </summary>
    /// <param name="damage">�G����̃_���\�W�ʁi�G�̍U����)</param>
    public override void PlayerOnDamage(int damage)
    {
        PlayerCurrentHP -= damage;
�@�@�@�@PlayerHPBar.value = PlayerCurrentHP;

        //�������݂�HP��HP�o�[��0�ɂȂ����琶���t���O��false��
        if (PlayerCurrentHP <= 0)
        {
            //�������X�g���炱�̃I�u�W�F�N�g������
            BaseBattleManager.Instance.AlivePlayers.Remove(this);

            PlayerCurrentHP = 0;
            PlayerHPBar.value = 0;

            IsAlive = false;
        }
    }

    /// <summary>
    /// �S�̍U���p���X�g�̏������\�b�h
    /// </summary>
    public void RemoveDeadEnemies()
    {
        //�S�̍U���p���X�g�ɓ����Ă���G���P���炷
        for (int i = targetEnemys.Count - 1; i >= 0; i--)
        {
            // �G������ł���Ȃ�S�̍U���p���X�g����G���폜
            if (targetEnemys[i] == null || !targetEnemys[i].EnemyIsAlive)
            {
                targetEnemys.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// �x��ăG�t�F�N�g�𐶐����郁�\�b�h
    /// </summary>
    public void DelayEffect()
    {
        //�A�^�b�J�[�̃G�t�F�N�g�ƃe�L�X�g�G�t�F�N�g��x��Đ���
        GameObject EffectInstance = Instantiate(attacker_NormalEffect, PlayerEffect_SpawnPoint.position, Quaternion.identity);
        GameObject TextEffectInstance = Instantiate(attacker_TextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

        //�x��ăG�t�F�N�g������
        Destroy(EffectInstance, 0.2f);
        Destroy(TextEffectInstance, 4f);
    }

    /// <summary>
    /// �G�t�F�N�g�̐����ʒu���擾���郁�\�b�h
    /// </summary>
    /// <param name="index">�G�t�F�N�g�𐶐�����C���f�b�N�X</param>
    /// <returns>�Ή�����X�|�[���|�C���g��Transform�B�͈͊O�̏ꍇ��null��Ԃ��B</returns>
    private Transform GetSpawnPoint(int index)
    {
        switch (index)
        {
            case 0: return specialAttackEffect_SpawnPoint1;

            case 2: return specialAttackEffect_SpawnPoint2;

            case 3: return specialAttackEffect_SpawnPoint3;

            default: return null;
        }
    }

    /// <summary>
    /// �A�^�b�J�[�̃��x���A�b�v���\�b�h
    /// </summary>
    public override void LevelUP()
    {
        base.LevelUP();
    }
}