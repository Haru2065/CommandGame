using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("�G�̐������X�g")]
    public List<BaseEnemyStatus> aliveEnemies = new List<BaseEnemyStatus>();


    [SerializeField]
    [Tooltip("���x���A�b�v���s�����X�g")]
    protected List<BasePlayerStatus> LevelUPPlayerList = new List<BasePlayerStatus>();

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
    public bool IsPlayerTurn;

    //�Q�[���N���A��
    protected bool isGameClear;

    //�Q�[���I�[�o�[��
    protected bool isGameOver;
    
    //�|�[�Y���[�h�ɂł��邩
    protected bool canPoseMode;

    //�X�e�[�W3��������ꂽ��
    public bool IsUnlockStage3;

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
    /// �x��ăN���AUI��\�����郁�\�b�h
    /// </summary>
    protected virtual void DelayGameClearUI()
    {
        //UI�}�l�[�W���[����Q�[���N���AUI��\��
        UIManager.Instance.GameClearUI();

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
        //���x���A�b�v�������Ƃ��E�B���h�E�\��
        BattleActionTextManager.Instance.ShowBattleActionText("LevelUPText");
        

        //2�t���[���҂�
        yield return new WaitForSeconds(2f);

        //���x���A�b�v�������Ƃ�ʒm����E�B���h�E���\��
        StartCoroutine(HidePlayerActionText());

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
        Destroy(startTurnEffectInstance, 2f);

        //0.5�b�ҋ@
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// JSON�t�@�C���̏󋵒ʒm�e�L�X�g���\���ɂ���R�[���`��
    /// </summary>
    /// <returns></returns>
    protected IEnumerator HidePlayerActionText()
    {
        //2�t���[���҂�
        yield return new WaitForSeconds(2f);

        //JSON�t�@�C���̏󋵒ʒm�e�L�X�g���\��
        BattleActionTextManager.Instance.TextDelayHide();

        //1�t���[���҂�
        yield return null;
    }
}
