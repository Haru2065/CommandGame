using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �o�b�t�@�[�̃X�e�[�^�X
/// �X�N���v�^�u���I�u�W�F�N�g����f�[�^��ǂݍ���
/// �x�[�X�v���C���[�X�e�[�^�X���p��
/// </summary>
public class Buffer : BasePlayerStatus
{
    // Constants for magic numbers
    private const float EFFECT_DESTROY_TIME_SHORT = 0.2f;
    private const float EFFECT_DESTROY_TIME_MEDIUM = 3f;
    private const float EFFECT_DESTROY_TIME_LONG = 4f;
    private const int SKILL_LIMIT_TURNS = 3;
    private const int SPECIAL_LIMIT_TURNS = 6;
    private const int LEVEL_UP_BUFF_POWER_INCREASE = 100;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�^�[�Q�b�g�}�l�[�W���[")]
    private BuffTargetWindow bufferTargetWindow;

    [SerializeField]
    [Tooltip("�A�^�b�J�[")]
    private Attacker attacker;

    [SerializeField]
    [Tooltip("�q�[���[")]
    private Healer healer;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̒ʏ�U���G�t�F�N�g")]
    private GameObject bufferNormalEffect;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̃X�L���G�t�F�N�g")]
    private GameObject bufferSkillEffect;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̃e�L�X�g�G�t�F�N�g")]
    private GameObject bufferTextEffect;


    [SerializeField]
    [Tooltip("�A�^�b�J�[�̃o�t�G�t�F�N�g��\��������ʒu")]
    private Transform attacker_BuffEffect_SpawnPoint;

    [SerializeField]
    [Tooltip("�o�b�t�@�[�̃o�t�G�t�F�N�g��\��������ʒu")]
    private Transform buffer_BuffEffect_SpawnPoint;

    [SerializeField]
    [Tooltip("�q�[���[�̃o�t�G�t�F�N�g��\��������ʒu")]
    private Transform healer_BuffEffect_SpawnPoint;

    //�o�b�t�@�[�̍s�����I��������
    public bool IsBufferAction;


    // Start is called before the first frame update
    protected override void Start()
    {
        //�p�����[�^��ݒ�
        base.Start();

        IsBufferAction = false;

        IsUseSkill = false;

        IsUseSpecial = true;

        IsDebuff = false;

        IsSpecialDebuff = false;
    }

    protected override void Update()
    {
        //�v���C���[�L������HP���\��
        PlayerHPUGUI.text = $"{PlayerCurrentHP}/ {PlayerMaxHP}";
    }

    /// <summary>
    /// �v���C���[�̍s���I���������i�o�b�t�@�[���s���������j�̃t���O�����Z�b�g�ɂ��郁�\�b�h
    /// </summary>
    public override void ResetActionFlag()
    {
        //�x�[�X�̃��\�b�h����v���C���[�s���t���O�����Z�b�g
        base.ResetActionFlag();

        //�o�b�t�@�[�̍s���t���O��false
        IsBufferAction = false;
    }

    /// <summary>
    /// �p�����[�^��ݒ肷�郁�\�b�h
    /// �v���C���[�̃f�[�^�x�[�X����ǂݍ���
    /// </summary>
    protected override void SetPlayerParameters()
    {

        // LINQ���g���A�v���C���[�f�[�^�x�[�X����PlayerID�ƈ�v����v���C���[�����擾
        var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);

        //�v���C���[��ID�ƍ��v����΃p�����[�^��ݒ�
        if (playerData != null)
        {
            //�ő�̗͂̃f�[�^��ǂݍ���
            PlayerMaxHP = playerData.PlayerMaxHPData;

            //�o�b�t�@�[�̌��݂�HP���ő�ɐݒ肵��HP�o�[���ő�ɐݒ�
            PlayerCurrentHP = PlayerMaxHP;
            PlayerHPBar.maxValue = PlayerCurrentHP;
            PlayerHPBar.value = PlayerCurrentHP;
            PlayerHPBar.minValue = 0;

            //HPBar�̍ŏ��͂O
            PlayerHPBar.minValue = 0;

            //�U���͂��o�b�t�@�[�̃f�[�^�̍U���͂�ǂݍ���
            AttackPower = playerData.PlayerAttackPowerData;

            //�����U���͂��A�^�b�J�[�̍U���͂ɐݒ�
            PlayerResetAttackPower = AttackPower;

            //�o�t�f�[�^��ǂݍ���
            BuffPower = playerData.BuffPowerData;

            //������Ԃɂ���
            IsAlive = true;
        }
    }

    /// <summary>
    /// Json�f�[�^�����鎞�ɓǂݍ��݃��\�b�h
    /// </summary>
    /// <param name="data">�ۑ��f�[�^</param>
    protected override void ApplySaveData(PlayerSaveData data)
    {
        //��{�f�[�^�͐e�N���X�̃��\�b�h�Ŏ��s
        base.ApplySaveData(data);

        //�o�t�͂��Z�[�u�f�[�^����ǂݍ���
        BuffPower = data.bufferBuffPower_saveData;
    }

    /// <summary>
    /// �o�b�t�@�[�̒ʏ�U��
    /// </summary>
    public override void NormalAttack()
    {
        //�v���C���[���I�񂾓G���U���Ώۂɐݒ肷��
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        if (target != null)
        {
            //�o�b�t�@�̒ʏ�U�����Đ�
            PlayerSE.Instance.Play_BufferNormalAttackSE();

            //�o�b�t�@�̒ʏ�U���̃G�t�F�N�g�𐶐�
            GameObject effectInstance = Instantiate(bufferNormalEffect, PlayerEffect_SpawnPoint.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(bufferTextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            //�U������Ώۂ̓G�Ƀ_���[�W��^����
            target.EnemyOnDamage(AttackPower);

            //�G�t�F�N�g����������
            Destroy(effectInstance, EFFECT_DESTROY_TIME_SHORT);
            Destroy(textEffectInstance, EFFECT_DESTROY_TIME_LONG);
        }

        //�����o�t���ʂ�����΍U���͂����Z�b�g
        if (HasBuff)
        {
            var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p =>p.PlayerNameData == PlayerID);

            //�o�b�t�@�[�̍U���͂��f�[�^�̍U���͂ɐݒ�
            AttackPower = playerData.PlayerAttackPowerData;

            HasBuff = false;
        }

        if (IsDebuff)
        {
            DebuffCount--;

            //�f�o�t�p���J�E���g��0�ɂȂ�����t���O��false�A�e�L�X�g��\��
            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                // JSON�t�@�C���Őݒ肳�ꂽ�A�^�b�J�[�̃f�o�t�����ʒm��\��
                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                // �f�o�t�������̏󋵃e�L�X�g���\���ɂ���
                StartCoroutine(PlayerOffDebuffText());
            }
        }

        if (IsSpecialDebuff)
        {
            SpecialDebuffCount--;

            //����f�o�t�p���J�E���g��0�ɂȂ�����t���O��false�A�e�L�X�g��\��
            if (SpecialDebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsSpecialDebuff = false;

                // JSON�t�@�C���Őݒ肳�ꂽ�A�^�b�J�[�̓���f�o�t�����ʒm��\��
                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                StartCoroutine(PlayerOffDebuffText());
            }
        }

        IsBufferAction = true;
    }

    /// <summary>
    /// �o�b�t�@�[�̃X�L��
    /// </summary>
    public override void PlayerSkill()
    {
        //�o�t����^�[�Q�b�g��I������E�B���h�E���J��
        bufferTargetWindow.ShowBuffTargetWindow();

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

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

                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                StartCoroutine(PlayerOffDebuffText());
            }
        }

        IsUseSkill = true;

        //�X�L�������J�E���g���R�ɐݒ�
        SkillLimitCount = SKILL_LIMIT_TURNS;
    }

    /// <summary>
    /// �o�b�t�@�[�̕K�E�Z
    /// </summary>
    public override void SpecialSkill()
    {
        //�S�L�����o�t�����̂Ńt���O��true��
        attacker.HasBuff = true;
        HasBuff = true;
        healer.HasBuff = true;

        //�o�b�t�@�[�̕K�E���ʉ��Đ�
        PlayerSE.Instance.Play_bufferSpecialSE();

        //���������G�t�F�N�g���i�[���郊�X�g
        List<GameObject> specialEffects = new List<GameObject>();

        //�e���X�g���i�[���A�e�L�����̏ꏊ�ɃG�t�F�N�g�𐶐�

        if (attacker.IsAlive)
        { 
            specialEffects.Add(Instantiate(bufferSkillEffect, attacker_BuffEffect_SpawnPoint.position, Quaternion.identity));
        }

        //���g���������Ă�����G�t�F�N�g�𐶐�
        if (IsAlive)
        {
            specialEffects.Add(Instantiate(bufferSkillEffect, buffer_BuffEffect_SpawnPoint.position, Quaternion.identity));
        }


        if (healer.IsAlive)
        {
            specialEffects.Add(Instantiate(bufferSkillEffect, healer_BuffEffect_SpawnPoint.position, Quaternion.identity));
        }

        //3�b��S�ẴG�t�F�N�g������
        foreach (var effect in specialEffects)
        {
            Destroy(effect, EFFECT_DESTROY_TIME_MEDIUM);
        }


        //�S�L�����o�t����
        attacker.AttackPower += BuffPower;
        AttackPower += BuffPower;
        healer.AttackPower += BuffPower;

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

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

                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                StartCoroutine(PlayerOffDebuffText());
            }
        }
        IsUseSpecial = true;

        //�o�b�t�@�[�̍s���t���O��true
        IsBufferAction = true;
        IsPlayerAction = true;

        //�K�E�����J�E���g���U�ɐݒ�
        SpecialLimitCount = SPECIAL_LIMIT_TURNS;

    }

    /// <summary>
    /// �o�t���s���\�b�h
    /// </summary>
    /// <param name="target">�v���C���[���擾����</param>
    public void OnBuff(BasePlayerStatus target)
    {
        //�o�t�Ώۂ�I������E�B���h�E���\��
        bufferTargetWindow.HideBuffTargetWindow();

        Debug.LogWarning(target.PlayerID + "�̍U����" + target.AttackPower);

        //�o�t�X�L�����ʉ��Đ�
        PlayerSE.Instance.Play_BufferSkillSE();

        //�o�b�t�@�[�̃X�L���G�t�F�N�g�𐶐�
        GameObject effectInstance = Instantiate(bufferSkillEffect, target.transform.position,Quaternion.identity);

        //�o�t�͂��^�[�Q�b�g�̍U���͂����Z
        target.AttackPower += BuffPower;

        Debug.LogWarning($"{target.PlayerID} �o�t��̍U����: {target.AttackPower} (InstanceID: {target.GetInstanceID()})");

        //�G�t�F�N�g������
        Destroy(effectInstance, EFFECT_DESTROY_TIME_MEDIUM);

        //�o�b�t�@�[���s�������̂�true��
        IsBufferAction = true;

        IsPlayerAction = true;

        //�o�b�t�@�[�̃^�[�����I��������X�L�������ƕK�E�����J�E���g��UI���\��
        if (IsBufferAction)
        {
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);
        }

        //�^�[�Q�b�g�̃o�t�������̃t���O��true�ɂ���
        target.HasBuff = true;
    }

    /// <summary>
    /// �_���[�W���\�b�h
    /// </summary>
    /// <param name="damage">�G�̍U����</param>
    public override void PlayerOnDamage(int damage)
    {
        //�o�b�t�@�[�̗̑͂�G�U���͕����炷
        PlayerCurrentHP -= damage;
        PlayerHPBar.value = PlayerCurrentHP;

        //�������݂�HP��HP�o�[��0�ɂȂ�����IsAlive��false
        if(PlayerCurrentHP <= 0)
        {
            //�����t���O��false��
            BaseBattleManager.Instance.AlivePlayers.Remove(this);

            PlayerCurrentHP = 0;
            PlayerHPBar.value = 0;

            IsAlive = false;
        }
    }

    /// <summary>
    /// �o�b�t�@�[�̃��x���A�b�v����
    /// </summary>
    public override void LevelUP()
    {
        base.LevelUP();

        //�o�t�p���[��100�v���X���ă��x���A�b�v
        BuffPower += LEVEL_UP_BUFF_POWER_INCREASE;
    }
}