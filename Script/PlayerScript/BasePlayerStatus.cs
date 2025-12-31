using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �x�[�X�̃v���C���[�X�e�[�^�X
/// �݌v�}�ɂ���
/// </summary>
public abstract class BasePlayerStatus : MonoBehaviour
{
    // Constants for magic numbers
    private const float DEBUFF_TEXT_WAIT_TIME = 2f;
    private const int LEVEL_UP_ATTACK_POWER_INCREASE = 100;
    private const int LEVEL_UP_MAX_HP_INCREASE = 500;

    [SerializeField]
    [Tooltip("�v���C���[�L�����̃f�[�^�x�[�X")]
    private PlayerDataBase playerDataBase;

    /// <summary>
    /// �v���C���[�̃f�[�^�x�[�X�̃Q�b�^�[
    /// </summary>
    public PlayerDataBase PlayerDataBase
    {
        get => playerDataBase;
    }

    [SerializeField]
    [Tooltip("�v���C���[ID��")]
    private string playerID;

    /// <summary>
    /// �v���C���[ID�̃Q�b�^�[
    /// </summary>
    public string PlayerID
    {
        get => playerID;
        set => playerID = value;
    }

    //�v���C���[���������Ă��邩
    private bool isAlive;

    /// <summary>
    /// ���C���[���������Ă��邩�Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool IsAlive
    {
        get => isAlive;
        set => isAlive = value;
    }

    /// <summary>
    /// �X�L�����g������
    /// </summary>
    private bool isUseSkill;

    /// <summary>
    /// �X�L�����g�������̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool IsUseSkill
    {
        get => isUseSkill;
        set => isUseSkill = value;
    }

    //�K�E���g������
    private bool isUseSpecial;

    /// <summary>
    /// �K�E���g�������̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool IsUseSpecial
    {
        get => isUseSpecial;
        set => isUseSpecial = value;
    }

    //�o�t����Ă��邩
    private bool hasBuff;

    /// <summary>
    /// �o�t����Ă��邩�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool HasBuff
    {
        get => hasBuff;
        set => hasBuff = value;
    }

    //�f�o�t��Ԃ�
    private bool isDebuff;

    /// <summary>
    /// �f�o�t��Ԃ��̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool IsDebuff
    {
        get => isDebuff;
        set => isDebuff = value;
    }

    //HP������f�o�t��Ԃ�
    private bool isHPDebuff;

    /// <summary>
    /// HP������f�o�t��Ԃ��̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool IsHPDebuff
    {
        get => isHPDebuff;
        set => isHPDebuff = value;
    }

    //����f�o�t��Ԃ�
    private bool isSpecialDebuff;

    /// <summary>
    /// ����f�o�t��Ԃ��̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool IsSpecialDebuff
    {
        get => isSpecialDebuff;
        set => isSpecialDebuff = value;
    }

    private bool isPlayerAction;

    public bool IsPlayerAction
    {
        get => isPlayerAction;
        set => isPlayerAction = value;
    }

    //�v���C���[�̍U����
    private int attackPower;

    /// <summary>
    /// �v���C���[�̍U���͂̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int AttackPower
    {
        get => attackPower;
        set => attackPower = value;
    }

    //�v���C���[�̍ő�̗�
    private int playerMaxHP;

    /// <summary>
    /// �v���C���[�̍ő�̗̑͂̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int PlayerMaxHP
    {
        get => playerMaxHP;
        set => playerMaxHP = value;
    }

    /// <summary>
    /// �v���C���[�̌��݂̗̑�
    /// </summary>
    private int playerCurrentHP;

    /// <summary>
    /// �v���C���[�̌��݂̗̑͂̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int PlayerCurrentHP
    {
        get =>playerCurrentHP;
        set => playerCurrentHP = value;
    }

    //�v���C���[�̏����U����(�f�o�t�����p�j
    private int playerResetAttackPower;

    /// <summary>
    /// �v���C���[�p�̏����U���͂̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int PlayerResetAttackPower
    {
        get => playerResetAttackPower;
        set => playerResetAttackPower = value;
    }

    //�f�o�t�p���J�E���g
    private int debuffCount;

    /// <summary>
    /// �f�o�t�p���J�E���g�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int DebuffCount
    {
        get => debuffCount;
        set => debuffCount = value;
    }

    //����f�o�t�p���J�E���g
    private int specialDebuffCount;

    /// <summary>
    /// ����f�o�t�p���J�E���g�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int SpecialDebuffCount
    {
        get => specialDebuffCount;
        set => specialDebuffCount = value;
    }

    [SerializeField]
    [Tooltip("�X�L���g�p�����J�E���g")]
    private int skillLimitCount;

    /// <summary>
    /// �X�L���g�p�����J�E���g�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int SkillLimitCount
    {
        get => skillLimitCount;
        set => skillLimitCount = value;
    }
    
    [SerializeField]
    [Tooltip("�K�E�g�p�����̃J�E���g")]
    private int specialLimitCount;

    /// <summary>
    /// �K�E�g�p�����̃J�E���g�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int SpecialLimitCount
    {
        get => specialLimitCount;
        set => specialLimitCount = value;
    }

    [SerializeField]
    [Tooltip("�v���C���[��HP�o�[")]
    private Slider playerHPBar;

    /// <summary>
    /// �v���C���[��HP�o�[�̃Q�b�^�[
    /// </summary>
    public Slider PlayerHPBar
    {
        get => playerHPBar;
        set => playerHPBar = value;
    }

    [SerializeField]
    [Tooltip("�G�t�F�N�g��\��������ʒu")]
    private Transform playerEffect_SpawnPoint;

    /// <summary>
    /// �G�t�F�N�g��\��������ʒu�̃Q�b�^�[
    /// </summary>
    public Transform PlayerEffect_SpawnPoint
    {
        get => playerEffect_SpawnPoint;
    }

    [SerializeField]
    [Tooltip("�����G�t�F�N�g��\��������ʒu")]
    private Transform playerTextEfferct_SpawnPoint;

    /// <summary>
    /// �����G�t�F�N�g��\��������ʒu�̃Q�b�^�[
    /// </summary>
    public Transform PlayerTextEfferct_SpawnPoint
    {
        get => playerEffect_SpawnPoint;
    }

    [SerializeField]
    [Tooltip("�v���C���[��HP��UI")]
    private TextMeshProUGUI playerHPUGUI;

    /// <summary>
    /// �v���C���[��HP��UI�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public TextMeshProUGUI PlayerHPUGUI
    {
        get => playerHPUGUI;
        set => playerHPUGUI = value;
    }

    //�v���C���[�̃��x��
    private int level = 0; 

    /// <summary>
    /// �v���C���[�̃��x���̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public int Level
    {
        get => level;
        set => level = value;
    }

    //���ۂ̃o�t��
    public int BuffPower;

    //�񕜗�
    public int HealPower;

    // Update is called once per frame
    protected abstract void Update();

    /// <summary>
    /// �󋵃e�L�X�g���\���ɂ���R�[���`��
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayerOffDebuffText()
    {
        //2�t���[���҂�
        yield return new WaitForSeconds(DEBUFF_TEXT_WAIT_TIME);

        //�e�L�X�g���\��
        BattleActionTextManager.Instance.TextDelayHide();

        //1�t���[���҂�
        yield return null;
    }

    /// <summary>
    /// �Z�[�u�f�[�^������΃Z�[�u�����v���C���[�f�[�^����X�e�[�^�X��ǂݍ���
    /// </summary>
    protected virtual void Start()
    {
        //�v���C���[ID�̃Z�[�u�f�[�^���擾
        string path = Application.persistentDataPath + $"/{PlayerID}_save.Json";

        //�Z�[�u�f�[�^������Γǂ݁AC#�ɕϊ�������A�X�e�[�^�X���[�h�����J�n
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerSaveData saveData = JsonConvert.DeserializeObject<PlayerSaveData>(json);
            
            //�Z�[�u�f�[�^����X�e�[�^�X��ǂݍ���
            ApplySaveData(saveData);
        }

        //�Z�[�u�f�[�^���Ȃ���΃X�N���v�^�u���I�u�W�F�N�g���烍�[�h
        else
        {
            SetPlayerParameters();
        }
    }

    public virtual void ResetActionFlag()
    {
        isPlayerAction = false;
    }

    /// <summary>
    /// Json�ŕۑ����ꂽ�p�����[�^�����[�h
    /// </summary>
    /// <param name="data">�p�����[�^�̕ۑ��f�[�^</param>
    protected virtual void ApplySaveData(PlayerSaveData data)
    {
        //�v���C���[�Z�[�u�f�[�^�ɕۑ�����Ă��郌�x���A�U���́A�ő�̗͂�ǂݍ���
        level = data.level_SaveData;
        AttackPower = data.attackPower_SaveData;
        playerMaxHP = data.playerMaxHP_SaveData;
        playerCurrentHP = playerMaxHP;

        //HPBar���ő�ɐݒ�
        playerHPBar.maxValue = playerCurrentHP;
        playerHPBar.value = playerCurrentHP;
        playerHPBar.minValue = 0;

        //���Z�b�g�p�U���͂��Z�[�u�f�[�^����ǂݍ���Őݒ�
        playerResetAttackPower = data.attackPower_SaveData;
    }


    /// <summary>
    /// �v���C���[�̃��x���A�b�v���A�p�����[�^���グ�郁�\�b�h
    /// </summary>
    public virtual void LevelUP()
    {
        //�U���́A�ő�̗͂̐��l���グ��
        attackPower += LEVEL_UP_ATTACK_POWER_INCREASE;
        playerMaxHP += LEVEL_UP_MAX_HP_INCREASE;

        //���x�����v���X����
        level++;

    }
    

    /// <summary>
    /// �v���C���[�̃_���[�W���\�b�h
    /// </summary>
    /// <param name="damage">�G�̍U���͂��_���[�W�ɂ���</param>
    public abstract void PlayerOnDamage(int damage);

    /// <summary>
    /// �v���C���[�̒ʏ�U�����\�b�h
    /// </summary>
    public abstract void NormalAttack();

    /// <summary>
    /// �v���C���[�̃X�L�����\�b�h
    /// </summary>
    public abstract void PlayerSkill();

    /// <summary>
    /// �v���C���[�̕K�E�Z���\�b�h
    /// </summary>
    public abstract void SpecialSkill();

    /// <summary>
    /// �X�N���v�^�u���I�u�W�F�N�g����p�����[�^��ݒ肷�郁�\�b�h
    /// </summary>
    protected abstract void SetPlayerParameters();
}
