using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Healer : BasePlayerStatus
{
    // Constants for magic numbers
    private const float EFFECT_DESTROY_TIME_SHORT = 0.2f;
    private const float EFFECT_DESTROY_TIME_MEDIUM = 3f;
    private const float EFFECT_DESTROY_TIME_LONG = 4f;
    private const int SKILL_LIMIT_TURNS = 5;
    private const int SPECIAL_LIMIT_TURNS = 6;
    private const int LEVEL_UP_HEAL_POWER_INCREASE = 100;

    [SerializeField]
    [Tooltip("�ΏێґI���}�l�[�W���[")]
    private HealTargetWindow healTargetWindow;

    [SerializeField]
    [Tooltip("�A�^�b�J�[")]
    private Attacker attacker;

    [SerializeField]
    [Tooltip("�o�b�t�@�[")]
    private Buffer buffer;

    [SerializeField]
    [Tooltip("�q�[���[�̍U���G�t�F�N�g")]
    private GameObject healerNormalEffect;

    [SerializeField]
    [Tooltip("�q�[���[�̍U���e�L�X�g�G�t�F�N�g")]
    private GameObject healer_AttackTextEffect;

    [SerializeField]
    [Tooltip("�q�[���[�̉񕜃G�t�F�N�g")]
    private GameObject healer_HeelEffect;

    [SerializeField]
    [Tooltip("�A�^�b�J�[�̉񕜃G�t�F�N�g��\��������ʒu")]
    private Transform attacker_HeelEffect_SpawnPoint;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̉񕜃G�t�F�N�g��\��������ʒu")]
    private Transform buffer_HeelEffect_SpawnPoint;

    [SerializeField]
    [Tooltip("�q�[���[�̉񕜃G�t�F�N�g��\��������ʒu")]
    private Transform healer_HeelEffect_SpawnPoint;

    //�q�[���[���s��������
    private bool isHealerAction;

    /// <summary>
    /// �q�[���[�̍s���t���O�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool IsHealerAction
    {
        get => isHealerAction;
        set => isHealerAction = value;
    }

    

    // Start is called before the first frame update
    protected override void Start()
    {
        //�p�����[�^��ݒ�
        base.Start();

        //�q�[���[�̍s���t���O��false��
        isHealerAction = false;

        //���ʂ̃f�o�t�Ɠ���f�o�t�p���J�E���g�̏�����
        IsDebuff = false;
        IsSpecialDebuff = false;

        IsUseSkill = false;

        //�ŏ��͕K�E�͎g���Ȃ��悤�ɂ���
        IsUseSpecial = true;

        //���ʂ̃f�o�t�Ɠ���f�o�t�p���J�E���g�̏�����
        DebuffCount = 0;
        SpecialDebuffCount = 0;
    }

    protected override void Update()
    {
        //�v���C���[�L������HP���\��
        PlayerHPUGUI.text = $"{PlayerCurrentHP}/ {PlayerMaxHP}";
    }

    /// <summary>
    /// �v���C���[�̍s���I���������i�q�[���[���s���������j�̃t���O�����Z�b�g�ɂ��郁�\�b�h
    /// </summary>
    public override void ResetActionFlag()
    {
        //�x�[�X�̃��\�b�h����v���C���[�s���t���O�����Z�b�g
        base.ResetActionFlag();

        //�q�[���[�̍s���t���O��false
        IsHealerAction = false;
    }

    /// <summary>
    /// �p�����[�^��ݒ肷�郁�\�b�h
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

            //�q�[���[�̌��݂�HP���ő�ɐݒ肵��HP�o�[���ő�ɐݒ�
            PlayerCurrentHP = PlayerMaxHP;
            PlayerHPBar.maxValue = PlayerCurrentHP;
            PlayerHPBar.value = PlayerCurrentHP;
            PlayerHPBar.minValue = 0;

            //�U���͂��q�[���[�̃f�[�^�̍U���͂�ǂݍ���
            AttackPower = playerData.PlayerAttackPowerData;

            //�����U���͂��q�[���[�̍U���͂ɐݒ�
            PlayerResetAttackPower = AttackPower;

            //�q�[���[�̉񕜗͂��q�[���[�̃f�[�^�̉񕜗͂���ǂݍ���
            HealPower = playerData.HealPowerData;

            IsAlive = true;
        }
    }

    /// <summary>
    /// Josn�f�[�^�����鎞�ɓǂݍ��ރ��\�b�h
    /// </summary>
    /// <param name="data">Json�̕ۑ��f�[�^</param>
    protected override void ApplySaveData(PlayerSaveData data)
    {
        //��{�f�[�^�͐e�N���X�̃��\�b�h������s
        base.ApplySaveData(data);

        //�񕜗͂��Z�[�u�f�[�^����ǂݍ���
        HealPower = data.healerHealPower_saveData;
    }

    /// <summary>
    /// �q�[���[�̒ʏ�U��
    /// </summary>
    public override void NormalAttack()
    {
        //�v���C���[���I�񂾓G���U���Ώۂɐݒ肷��
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        if (target != null)
        {
            //�q�[���[�̒ʏ�U�����Đ�
            PlayerSE.Instance.Play_healerNormalAttackSE();

            //�q�[���[�̒ʏ�U�����̃G�t�F�N�g�𐶐�
            GameObject effectInstance = Instantiate(healerNormalEffect, PlayerEffect_SpawnPoint.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(healer_AttackTextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            //�^�[�Q�b�g�Ƀ_���[�W��^����
            target.EnemyOnDamage(AttackPower);

            //�q�[���[�̃G�t�F�N�g������
            Destroy(effectInstance, EFFECT_DESTROY_TIME_SHORT);
            Destroy(textEffectInstance, EFFECT_DESTROY_TIME_LONG);
        }

        //(�q�[���[�ɂ͍U���X�L�����Ȃ����߁A�ʏ�U���Ń��Z�b�g�j
        if (HasBuff)
        {
            // LINQ���g���A�v���C���[�f�[�^�x�[�X����PlayerID�ƈ�v����v���C���[�����擾
            var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);

            //�U���͂����o�����f�[�^�̍U���͂ɐݒ肷��
            AttackPower = playerData.PlayerAttackPowerData;

            HasBuff = false;
        }

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                // �U���͂����̒l�ɖ߂�
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                // JSON�t�@�C���Őݒ肳�ꂽ�q�[���[�̃f�o�t�����ʒm��\��
                BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");

                //�f�o�t�������̏󋵃e�L�X�g���\���ɂ���
                StartCoroutine(PlayerOffDebuffText());
            }
        }

        if (IsSpecialDebuff)
        {
            SpecialDebuffCount--;

            if (SpecialDebuffCount <= 0)
            {
                // �U���͂����̒l�ɖ߂�
                AttackPower = PlayerResetAttackPower;

                IsSpecialDebuff = false;

                // JSON�t�@�C���Őݒ肳�ꂽ�q�[���[�̓���f�o�t�����ʒm��\��
                BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");

                // �f�o�t�������̏󋵃e�L�X�g���\���ɂ���
                StartCoroutine(PlayerOffDebuffText());
            }
        }

        //�q�[���[�̍s���t���O��true��
        IsHealerAction = true;
    }

    /// <summary>
    /// �q�[���[�̃X�L��
    /// </summary>
    public override void PlayerSkill()
    {
        //�ΏێґI���}�l�[�W���[����񕜑ΏێґI����ʂ�\��
        healTargetWindow.ShowHealTargetWindow();

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");
                StartCoroutine(PlayerOffDebuffText());
            }

        }

        if (IsSpecialDebuff)
        {
            SpecialDebuffCount--;

            if (SpecialDebuffCount <= 0)
            {
                //�U���͂����̒l�ɖ߂�
                AttackPower = PlayerResetAttackPower;

                IsSpecialDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");
                StartCoroutine(PlayerOffDebuffText());
            }
        }
    }

    /// <summary>
    /// �q�[���[�̕K�E�Z
    /// </summary>
    public override void SpecialSkill()
    { 

        //�q�[���[�̕K�E�̌��ʉ��Đ�
        PlayerSE.Instance.Play_healerSpecialSE();

        //���������G�t�F�N�g���i�[���郊�X�g
        List<GameObject> specialEffects = new List<GameObject>();

        //�e���X�g���i�[���A�e�L�����̏ꏊ�ɃG�t�F�N�g�𐶐�

        if (attacker.IsAlive)
        {
            specialEffects.Add(Instantiate(healer_HeelEffect, attacker_HeelEffect_SpawnPoint.position, Quaternion.identity));
        }

        if (buffer.IsAlive)
        {
            specialEffects.Add(Instantiate(healer_HeelEffect, buffer_HeelEffect_SpawnPoint.position, Quaternion.identity));
        }

        if (IsAlive)
        {
            specialEffects.Add(Instantiate(healer_HeelEffect, healer_HeelEffect_SpawnPoint.position, Quaternion.identity));
        }

        PlayerSE.Instance.Play_healerSkillSE();

        //������S����
        attacker.PlayerCurrentHP += HealPower;
        buffer.PlayerCurrentHP += HealPower;
        PlayerCurrentHP += HealPower;

        //3�b��S�ẴG�t�F�N�g������
        foreach (var effect in specialEffects)
        {
            Destroy(effect, EFFECT_DESTROY_TIME_MEDIUM);
        }

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("healerOffDebuff");
                StartCoroutine(PlayerOffDebuffText());
            }
        }

        if (IsSpecialDebuff)
        {
            SpecialDebuffCount--;

            if (SpecialDebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;
                
                IsSpecialDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("healerOffDebuff");
                StartCoroutine(PlayerOffDebuffText());
            }
        }
        IsUseSpecial = true;

        //�K�E�����J�E���g��6�^�[���ɐݒ�
        SpecialLimitCount = SPECIAL_LIMIT_TURNS;

        //�q�[���[�̍s���t���O��true
        IsHealerAction = true;
        IsPlayerAction = true;
    }

    /// <summary>
    /// �񕜃��\�b�h
    /// </summary>
    /// <param name="target">�񕜂���Ώێ�</param>
    public void OnHeal(BasePlayerStatus target)
    {
        //�ΏێґI���}�l�[�W���[����ΏۑI���E�B���h�E���\��
        healTargetWindow.HideHealTargetWindow();

        //�q�[���[�̃X�L�����ʉ��Đ�
        PlayerSE.Instance.Play_healerSkillSE();

        GameObject specialEffectInstance = Instantiate(healer_HeelEffect, target.transform.position, Quaternion.identity);
        Destroy(specialEffectInstance, EFFECT_DESTROY_TIME_MEDIUM);

        Debug.Log(target + "��HP" + target.PlayerCurrentHP);

        //�I�������^�[�Q�b�g�ɉ�
        target.PlayerCurrentHP += HealPower;

        Debug.Log(target + "��HP" + target.PlayerCurrentHP);

        //�ő�̗͂𒴂��Ȃ��悤�ɂ���
        if (target.PlayerCurrentHP > target.PlayerMaxHP)
        {
            target.PlayerCurrentHP = target.PlayerMaxHP;

        }

        //�񕜑Ώێ҂�HPUI���X�V
        target.PlayerHPUGUI.text = $"{target.PlayerCurrentHP}/ {target.PlayerMaxHP}";

        //HP�o�[���X�V
        target.PlayerHPBar.value = target.PlayerCurrentHP;

        IsUseSkill = true;

        //�X�L�������J�E���g���T�^�[���ɐݒ�
        SkillLimitCount = SKILL_LIMIT_TURNS;

        //�q�[���[�̍s���t���O��true
        IsHealerAction = true;
        IsPlayerAction = true;
    }

    /// <summary>
    /// �q�[���[�̃_���[�W���\�b�h
    /// </summary>
    /// <param name="damage">�G����̃_���\�W�ʁi�G�̍U����)</param>
    public override void PlayerOnDamage(int damage)
    {
        PlayerCurrentHP -= damage;
        PlayerHPBar.value = PlayerCurrentHP;

        //0��艺�ɂ͍s���Ȃ��悤�ɂ���
        if (PlayerHPBar.value <= 0)
        {
            //�������X�g���炱�̃I�u�W�F�N�g������
            BaseBattleManager.Instance.AlivePlayers.Remove(this);

            PlayerCurrentHP = 0;
            PlayerHPBar.value = 0;

            IsAlive = false;
        }
    }

    /// <summary>
    /// �q�[���[�̃��x���A�b�v���\�b�h
    /// </summary>
    public override void LevelUP()
    {
        base.LevelUP();

        //�񕜃p���[��100�v���X���ă��x���A�b�v
        HealPower += LEVEL_UP_HEAL_POWER_INCREASE;
    }
}
