using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// �o�g���}�l�[�W���[
/// </summary>
public abstract class BaseBattleManager : MonoBehaviour
{
    //�x�[�X�̃o�g���}�l�[�W���[�C���X�^���X���p
    private static BaseBattleManager instance;

    /// <summary>
    /// �C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static BaseBattleManager Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("�A�^�b�J�[")]
    protected Attacker attacker;

    [SerializeField]
    [Tooltip("�o�b�t�@�[")]
    protected Buffer buffer;

    [SerializeField]
    [Tooltip("�q�[���[")]
    protected Healer healer;

    [SerializeField]
    [Tooltip("�v���C���[�̐������X�g")]
    protected List<BasePlayerStatus> alivePlayers = new List<BasePlayerStatus>();

    /// <summary>
    /// �v���C���[�̐������X�g�̃Q�b�^�[
    /// </summary>
    public List<BasePlayerStatus> AlivePlayers
    {
        get => alivePlayers;
    }

    [SerializeField]
    [Tooltip("���x���A�b�v���s�����X�g")]
    protected List<BasePlayerStatus> LevelUPPlayerList = new List<BasePlayerStatus>();

    [SerializeField]
    [Tooltip("�G�̐������X�g")]
    public List<BaseEnemyStatus> aliveEnemies = new List<BaseEnemyStatus>();

    [SerializeField]
    [Tooltip("�^�[���J�n�G�t�F�N�g")]
    private GameObject startTurnEffect;

    /// <summary>
    /// �^�[���J�n�G�t�F�N�g�̃Q�b�^�[
    /// </summary>
    public GameObject StartTurnEffect
    {
        get => startTurnEffect;
    }

    [SerializeField]
    [Tooltip("First�^�[���J�n�G�t�F�N�g�����ʒu")]
    private Transform firstTurnEffect_SpawnPoint;

    /// <summary>
    /// First�^�[���J�n�G�t�F�N�g�����ʒu�̃Q�b�^�[
    /// </summary>
    public Transform FirstTurnEffect_SpawnPoint
    {
        get => firstTurnEffect_SpawnPoint;
    }

    [SerializeField]
    [Tooltip("Second�^�[���J�n�G�t�F�N�g�����ʒu")]
    private Transform secondTurnEffect_SpawnPoint;

    /// <summary>
    /// Second�^�[���J�n�G�t�F�N�g�����ʒu�̃Q�b�^�[
    /// </summary>
    public Transform SecondTurnEffect_SpawnPoint
    {
        get => secondTurnEffect_SpawnPoint;
    }

    [SerializeField]
    [Tooltip("Third�^�[���J�n�G�t�F�N�g�����ʒu")]
    private Transform thirdTurnEffect_SpawnPoint;

    /// <summary>
    /// Third�^�[���J�n�G�t�F�N�g�����ʒu�̃Q�b�^�[
    /// </summary>
    public Transform ThirdTurnEffect_SpawnPoint
    {
        get => thirdTurnEffect_SpawnPoint;
    }

    //�v���C���[�^�[����
    protected bool IsPlayerTurn;

    //�Q�[���N���A��
    protected bool isGameClear;

    //�Q�[���I�[�o�[��
    protected bool isGameOver;
    
    //�|�[�Y���[�h�ɂł��邩
    protected bool canPoseMode;

    //�X�e�[�W2��������ꂽ��
    private bool isUnlockStage2;

    /// <summary>
    /// �X�e�[�W2������t���O�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool IsUnlockStage2
    {
        get => isUnlockStage2;
        set => isUnlockStage2 = value;
    }

    //�X�e�[�W3��������ꂽ��
    private bool isUnlockStage3;

    /// <summary>
    /// �X�e�[�W3������t���O�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public bool IsUnlockStage3
    {
        get => isUnlockStage3;
        set => isUnlockStage3 = value;
    }

    protected const float TurnDelay = 1f;

    //
    protected const float EffectDelay = 2f;

    protected const int JsonTextDelay = 1;

    /// <summary>
    /// �x�[�X�̃o�g���}�l�[�W���[���C���X�^���X��
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //���ʂ̏����̓x�[�X�ł̃o�g���}�l�[�W���[�ŏ�����
    // Start is called before the first frame update
    protected virtual void Start()
    {
        //UI�}�l�[�W���[�����xUI��S�Ĕ�\��
        UIManager.Instance.StartUI();

        //�ŏ��̓v���C���[�^�[������J�n
        IsPlayerTurn = true;

        //�e�v���C���[�L�����𐶑���Ԃ�true��
        attacker.IsAlive = true;
        buffer.IsAlive = true;
        healer.IsAlive = true;

        //�Q�[���N���A�A�Q�[���I�[�o�[�̔���t���O��false��
        isGameClear = false;
        isGameOver = false;

        //�o�g���J�n���Ƀ^�[�Q�b�g��ݒ�
        PlayerTargetSelect.Instance.SetStartBattleTarget();

        //�ŏ��̓X�e�[�^�X�{�^�����J���{�^���������Ȃ��悤�ɂ���
        PushOpenStatusWindow.Instance.TransparentStatusButton();
    }

    /// <summary>
    /// �|�[�Y���[�h�ɂ���
    /// </summary>
    // Update is called once per frame
    protected virtual void Update()
    {
        //�G�X�P�[�v�L�[�������ꂽ��I���{�^���ƃ^�C�g���{�^����\��
        if (canPoseMode)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.ShowPauseMode();
            }
            //�^�u�L�[�ŕ���
            else if (Input.GetKeyDown(KeyCode.Tab) && canPoseMode)
            {
                UIManager.Instance.HidePauseMode();
            }
        }
    }

    /// <summary>
    /// �v���C���[�^�[���̋��ʏ���
    /// </summary>
    /// <param name="player">�v���C���[�̃N���X��</param>
    /// <param name="offDebuffTextID">�f�o�t�����e�L�X�g</param>
    /// <param name="normalKey">�ʏ�U���L�[</param>
    /// <param name="skillKey">�X�L���L�[</param>
    /// <param name="specialKey">�K�E�L�[</param>
    /// <param name="token">�L�����Z���ł��鏈��</param>
    /// <returns>�v���C���[���s������܂ŏ�����҂�</returns>
    protected abstract UniTask PlayerTurn(BasePlayerStatus player, string offDebuffTextID, KeyCode normalKey, KeyCode skillKey, KeyCode specialKey, CancellationToken token);

    /// <summary>
    /// �x��ăN���AUI�\���ƃf�[�^�ۑ����郁�\�b�h
    /// </summary>
    protected virtual void DelayGameClearUI()
    {
        //�v���C���[�̃��x���A�b�v�������s��
        StartCoroutine(PlayerLevelUP());
    }

    /// <summary>
    /// �Q�[���N���A�������̊m�F���郁�\�b�h
    /// </summary>
    /// <returns>�o�g���I���t���O</returns>
    protected abstract bool GameClearCheck();


    /// <summary>
    /// �v���C���[�̃��x���A�b�v�ƃp�����[�^��ۑ�����R�[���`��
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator PlayerLevelUP()
    {
        Debug.Log($"�A�^�b�J�[{attacker.AttackPower},{attacker.PlayerMaxHP}");
        Debug.Log($"�o�b�t�@�[{buffer.AttackPower},{buffer.PlayerMaxHP},{buffer.buffPower}");
        Debug.Log($"�q�[���[{healer.AttackPower},{healer.PlayerMaxHP},{healer.healPower}");

        //���x���A�b�v����v���C���[�̃��X�g�̃L���������x���A�b�v
        foreach (var player in LevelUPPlayerList)
        {
            player.LevelUP();
        }

        Debug.Log($"�A�^�b�J�[{attacker.AttackPower},{attacker.PlayerMaxHP}");
        Debug.Log($"�o�b�t�@�[{buffer.AttackPower},{buffer.PlayerMaxHP},{buffer.buffPower}");
        Debug.Log($"�q�[���[{healer.AttackPower},{healer.PlayerMaxHP},{healer.healPower}");

        //���x���A�b�v�����L�����̃p�����[�^��ۑ�
        SaveManager.SavePlayers(LevelUPPlayerList);

        //�ۑ��p�X��\��
        Debug.Log("�ۑ��p�X�F" + Application.persistentDataPath);

        //���x���A�b�v�������Ƃ��E�B���h�E�\��
        BattleActionTextManager.Instance.ShowBattleActionText("LevelUPText");

        //2�t���[���҂�
        yield return new WaitForSeconds(2);

        //���x���A�b�v�������Ƃ�ʒm����E�B���h�E���\��
        StartCoroutine(HidePlayerActionText());

        // UI�}�l�[�W���[����Q�[���N���AUI��\��
        UIManager.Instance.GameClearUI();
    }

    /// <summary>
    /// �Q�[���I�[�o�[�������̊m�F���郁�\�b�h
    /// </summary>
    /// <returns></returns>
    protected virtual bool GameOverCheck()
    {
        //�����������S�ł�����Q�[���I�[�o�[
        if(alivePlayers.Count == 0)
        {
            //�Q�[���I�[�o�[�t���O��true
            isGameOver = true;

            //UI�}�l�[�W���[����Q�[���I�[�o�[��\��
            UIManager.Instance.GameOverUI();

            //�Q�[���I�[�o�[�Ȃ̂�true��Ԃ�
            return true;
        }
        //�����������Ă���̂�false��Ԃ�
        return false;
    }

    /// <summary>
    /// �w�肵���ʒu�Ƀ^�[���J�n�G�t�F�N�g�𐶐�����R�[���`��
    /// </summary>
    /// <param name="spawnPoint">�G�t�F�N�g�𐶐�����ʒu</param>
    /// <returns>�G�t�F�N�g�\����̑ҋ@����</returns>
    protected IEnumerator ShowStartTurnEffect(Transform spawnPoint)
    {
        //�w�肳�ꂽ�ʒu�Ƀ^�[���J�n�G�t�F�N�g�𐶐�
        GameObject startTurnEffectInstance = Instantiate(StartTurnEffect, spawnPoint.position, Quaternion.identity);

        //�^�[���J�n�G�t�F�N�g��2�b�����
        Destroy(startTurnEffectInstance, EffectDelay);

        //0.5�b�ҋ@
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// JSON�t�@�C���̃v���C���[�󋵒ʒm�e�L�X�g���\���ɂ���R�[���`��
    /// </summary>
    /// <returns></returns>
    protected IEnumerator HidePlayerActionText()
    {
        //1�b�҂�
        yield return new WaitForSeconds(JsonTextDelay);

        //JSON�t�@�C���̏󋵒ʒm�e�L�X�g���\��
        BattleActionTextManager.Instance.TextDelayHide();

        //1�t���[���҂�
        yield return null;
    }

    /// <summary>
    /// �X�e�[�W�f�[�^�����[�h���郁�\�b�h
    /// </summary>
    /// <param name="data">Json�ɕۑ�����Ă���X�e�[�W�f�[�^</param>
    protected void LoadStageData(StageSaveData data)
    {
        isUnlockStage2 = data.Stage2UnLock_SaveData;

        //�X�e�[�W3����̃t���O�f�[�^���X�e�[�W�Z�[�u�f�[�^���烍�[�h
        isUnlockStage3 = data.Stage3UnLock_SaveData;
    }
}
