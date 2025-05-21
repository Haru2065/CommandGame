using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using System.IO;

/// <summary>
/// �X�e�[�W�P�̃o�g���V�X�e��
/// </summary>
public class Stage1BattleSystem : BaseBattleManager
{
    [SerializeField]
    [Tooltip("�X���C��1")]
    private Slime slime1;

    [SerializeField]
    [Tooltip("�X���C���Q")]
    private Slime slime2;

    [SerializeField]
    [Tooltip("�X���C���R")]
    private Slime slime3;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // �X�e�[�W�̃Z�[�u�f�[�^��ǂݍ���
        string path = Application.persistentDataPath + $"/StageSaveData.Json";

        //�Z�[�u�f�[�^�����݂���Ȃ�X�e�[�W�f�[�^�����[�h
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StageSaveData saveData = JsonConvert.DeserializeObject<StageSaveData>(json);
            LoadStageData(saveData);
        }

        //�Z�[�u�f�[�^�����݂��Ȃ��Ȃ�X�e�[�W�f�[�^��������
        else
        {
            IsUnlockStage2 = false;
        }

        PlayerTargetSelect.Instance.SetStartBattleTarget();

        canPoseMode = false;

        //�o�g�����[�v�J�n
        StartCoroutine(BattleLoop());
    }

    /// <summary>
    /// �o�g�����[�v
    /// </summary>
    /// <returns></returns>
    IEnumerator BattleLoop()
    {
        //�Q�[���N���A�t���O���A�Q�[���I�[�o�[�t���O��true�ɂȂ�܂Ń��[�v��������
        while (true)
        {
            if (isGameClear || isGameOver) yield break;

            if (IsPlayerTurn)
            {
                //UI�}�l�[�W���[����v���C���[�^�[��UI��\��
                UIManager.Instance.PlayerTurnUI.SetActive(true);

                yield return new WaitForSeconds(2f);

                //UI�}�l�[�W���[����v���C���[�^�[��UI���\��
                UIManager.Instance.PlayerTurnUI.SetActive(false);

                //UI�}�l�[�W���[����v���C���[�^�[�����ɕ\������UI��\��
                UIManager.Instance.StartPlayerTurnUI();

                //�A�^�b�J�[���������Ă����珈�������s
                if (attacker.IsAlive)
                {
                    //�A�^�b�J�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //�w�肵���ʒu�Ƀ^�[���J�n�G�t�F�N�g����
                    yield return ShowStartTurnEffect(FirstTurnEffect_SpawnPoint);

                    //�A�^�b�J�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //�X�e�[�^�X�E�B���h�E���J���{�^����������悤�ɂ���
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //�A�^�b�J�[�^�[���J�n
                    yield return AttackerTurn();

                    yield return new WaitForSeconds(2f);

                    //�G�̐����󋵂��m�F����
                    GameClearCheck();

                    //�X�e�[�^�X�{�^�����J���{�^���������Ȃ��悤�ɂ���
                    PushOpenStatusWindow.Instance.TransparentStatusButton();

                    //�Q�[���N���A�Ȃ烋�[�v���~�߂�
                    if (isGameClear)yield break;
                }

                //�o�b�t�@�[���������Ă����珈�������s
                if (buffer.IsAlive)
                {
                    //�o�b�t�@�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("BufferTurnText");

                    yield return ShowStartTurnEffect(SecondTurnEffect_SpawnPoint);

                    //�o�b�t�@�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //�o�b�t�@�[�^�[���J�n
                    yield return BufferTurn();

                    yield return new WaitForSeconds(2);

                    GameClearCheck();

                    PushOpenStatusWindow.Instance.TransparentStatusButton();

                    if (isGameClear) yield break;
                }

                //�q�[���[���������Ă����珈�������s
                if (healer.IsAlive)
                {

                    //�q�[���[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("HealerTurnText");

                    yield return ShowStartTurnEffect(ThirdTurnEffect_SpawnPoint);

                    //�q�[���[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //�q�[���[�^�[���J�n
                    yield return HealerTurn();

                    yield return new WaitForSeconds(2);

                    GameClearCheck();

                    PushOpenStatusWindow.Instance.TransparentStatusButton();

                    if (isGameClear) yield break ;
                }
            }

            //�G�^�[��
            else
            {
                //�e�v���C���[�̍s���t���O��false�ɂ���
                attacker.IsAttackerAction = false;
                buffer.IsBufferAction = false;
                healer.IsHealerAction = false;

                //UI�}�l�[�W���[����G�^�[��UI��\��
                UIManager.Instance.EnemyTurnUI.SetActive(true);

                yield return new WaitForSeconds(3f);

                //UI�}�l�[�W���[����G�^�[��UI���\��
                UIManager.Instance.EnemyTurnUI.SetActive(false);

                //�G�^�[���J�n
                yield return EnemyTurn();
            }
        }

    }

    /// <summary>
    /// �A�^�b�J�[�^�[������
    /// </summary>
    /// <returns>�������s����܂őҋ@</returns>
    IEnumerator AttackerTurn()
    {
        //�|�[�Y���[�h�ɂł��邩�̃t���O��true��
        canPoseMode = true;

        //A�ES�EF�L�[�̂����ꂩ���������܂őҋ@���A�����ꂽ�L�[�ɉ������U�������s����B
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F));

        if (Input.GetKeyDown(KeyCode.A))
        {
            //�A�^�b�J�[�̒ʏ�U��
            attacker.NormalAttack();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            //�A�^�b�J�[�̃X�L��
            attacker.PlayerSkill();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            //�A�^�b�J�[�̕K�E
            attacker.SpecialSkill();
        }

        //�A�^�b�J�[�̍s�����I���܂ŏ�����҂�
        yield return new WaitUntil(() => attacker.IsAttackerAction);

        //�|�[�Y���[�h�̂ł��邩�̃t���O��false
        canPoseMode = false;
    }

    /// <summary>
    /// �o�b�t�@�[�^�[������
    /// </summary>
    /// <returns>�������s����܂őҋ@</returns>
    IEnumerator BufferTurn()
    {
        canPoseMode = true;

        //A�ES�EF�L�[�̂����ꂩ���������܂őҋ@���A�����ꂽ�L�[�ɉ������U�������s����B
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F));

        if (Input.GetKeyDown(KeyCode.A))
        {
            //�o�b�t�@�[�ʏ�U��
            buffer.NormalAttack();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            //�o�b�t�@�[�̃X�L��
            buffer.PlayerSkill();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            //�o�b�t�@�[�̕K�E
            buffer.SpecialSkill();
        }

        //�o�b�t�@�[�̍s�����I���܂ŏ�����҂�
        yield return new WaitUntil(() => buffer.IsBufferAction);

        canPoseMode = false;
    }

    /// <summary>
    /// �q�[���[�̃^�[������
    /// </summary>
    /// <returns>�������s����܂őҋ@</returns>
    IEnumerator HealerTurn()
    {
        canPoseMode = true;

        //A�ES�EF�L�[�̂����ꂩ���������܂őҋ@���A�����ꂽ�L�[�ɉ������U�������s����B
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F));

        if (Input.GetKeyDown(KeyCode.A))
        {
            //�q�[���[�̒ʏ�U��
            healer.NormalAttack();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            //�q�[���[�̃X�L��
            healer.PlayerSkill();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            //�q�[���[�̕K�E
            healer.SpecialSkill();
        }

        //�q�[���[�̍s�����I���܂ŏ�����҂�
        yield return new WaitUntil(() => healer.IsHealerAction);

        canPoseMode = false;
        
        //�v���C���[�̃^�[���I��
        IsPlayerTurn = false;
    }

    /// <summary>
    /// �G�^�[��
    /// </summary>
    /// <returns>2�t���[���҂�</returns>
    IEnumerator EnemyTurn()
    {
        //�X���C�����������Ă����珈�������s
        if (slime1.EnemyIsAlive)
        {
            //�X���C���P�̃^�[���J�n
            yield return SlimeTurn(slime1);

            //�v���C���[�̐����󋵂��m�F
            GameOverCheck();

            //�Q�[���I�[�o�[�t���O��true�Ȃ烋�[�v���~�߂�
            if (isGameOver) yield break;
        }

        if (slime2.EnemyIsAlive)
        {
            yield return new WaitForSeconds(2f);

            //�X���C��2�̃^�[���J�n
            yield return SlimeTurn(slime2);

            //�v���C���[�̐����󋵂��m�F
            GameOverCheck();

            if (isGameOver) yield break;
        }

        if (slime3.EnemyIsAlive)
        {
            yield return new WaitForSeconds(2f);

            //�X���C��3�̃^�[���J�n
            yield return SlimeTurn(slime3);

            //�v���C���[�̐����󋵂��m�F
            GameOverCheck();

            if (isGameOver) yield break;
        }

        //�G�^�[���I��
        IsPlayerTurn = true;

        //1�t���[���҂�
        yield return null;
    }

    /// <summary>
    /// �X���C���^�[������
    /// </summary>
    /// <param name="slime">�X���C���̃X�e�[�^�X</param>
    /// <returns></returns>
    IEnumerator SlimeTurn(Slime slime)
    {
        //�X���C���̍U���J�n
        slime.SlimeAction();

        yield return null;
    }

    /// <summary>
    /// �v���C���[���S�ł��m�F���A�Q�[���I�[�o�[���������s����B
    /// </summary>
    /// <returns>�S�ł��Ă�����true</returns>
    protected override bool GameClearCheck()
    {
        //�G�������X�g����ɂȂ�����l��Ԃ�
        if (aliveEnemies.Count == 0)
        {
            isGameClear = true;

            if (!IsUnlockStage2)
            {
                //�X�e�[�W3�����
                IsUnlockStage2 = true;

                //�X�e�[�W�f�[�^��ۑ�
                SaveManager.SaveStage();
            }

            //2�b�x��ăN���AUI��\�����A���x���A�b�v�������s��
            Invoke("DelayGameClearUI", 2);
            return true;
        }
        //�G�������Ă���̂�false��Ԃ�
        return false;
    }

    /// <summary>
    /// �v���C���[���S�ł��m�F���A�Q�[���I�[�o�[���������s����B
    /// </summary>
    /// <returns>�S�ł��Ă�����true</returns>
    protected override bool GameOverCheck()
    {
        //�v���C���[�̐������X�g����ɂȂ�����l��Ԃ�
        if (alivePlayers.Count == 0)
        {
            isGameOver = true;

            //2�b�x��ăQ�[���I�[�o�[�t�h��\��
            Invoke("DelayGameOverUI", 2);
            return true;
        }
        //�v���C���[�������Ă���̂�false��Ԃ�
        return false;
    }

    /// <summary>
    /// �x��ăQ�[���I�[�o�[UI��\�����鏈��B
    /// </summary>
    void DelayGameOverUI()
    {
        UIManager.Instance.GameOverUI();
    }
}
