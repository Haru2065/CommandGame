using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

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
    protected override async void Start()
    {
        base.Start();

        canPoseMode = false;

        cts = new CancellationTokenSource();        

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
        await BattleLoop(cts.Token);
    }

    protected override void Update()
    {
        base.Update();

        // �����I���{�^���������ꂽ����S�ɏI�����邽�߂ɃL�����Z���������s��
        if (pushExitButton.IsQuitGame)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    /// <summary>
    /// UniTask�o�g���̃��[�v����
    /// </summary>
    /// <param name="token">�L�����Z���ł��鏈��/param>
    /// <returns>�v���C���[���s������܂ő҂�</returns>
    async UniTask BattleLoop(CancellationToken token)
    {
        //�Q�[���N���A�E�Q�[���I�[�o�[�̃t���O��true�Ȃ烋�[�v���~�߂ăL�����Z������
        while (!(isGameClear || token.IsCancellationRequested))
        {
            if (IsPlayerTurn)
            {
                //UI�}�l�[�W���[����v���C���[�^�[���\��
                UIManager.Instance.PlayerTurnUI.SetActive(true);

                //1�t���[���҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                //�S�v���C���[�̍s���t���O�����Z�b�g
                attacker.ResetActionFlag();
                buffer.ResetActionFlag();
                healer.ResetActionFlag();

                //UI�}�l�[�W���[����v���C���[�^�[����\��
                UIManager.Instance.PlayerTurnUI.SetActive(false);

                //UI�}�l�[�W���[����v���C���[�^�[�����ɕ\������UI��\��
                UIManager.Instance.StartPlayerTurnUI();

                //�A�^�b�J�[���������Ă����珈�������s
                if (attacker.IsAlive)
                {
                    //�w�肵���ʒu�Ƀ^�[���J�n�G�t�F�N�g����
                    StartCoroutine(ShowStartTurnEffect(FirstTurnEffect_SpawnPoint));

                    //1�t���[���҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�A�^�b�J�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //�A�^�b�J�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //1�t���[���҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�X�e�[�^�X�E�B���h�E���J���{�^����������悤�ɂ���
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //�A�^�b�J�[�^�[���J�n(�L�����Z���ł��鏈��)
                    await PlayerTurnAction(attacker, "AttackeroffDebuff", KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //�G�̐����󋵂��m�F�i�������X�g����Ȃ烋�[�v���~�߂�j
                    if (GameClearCheck())
                    {
                        return;
                    }

                    //�X�e�[�^�X�{�^�����J���{�^���������Ȃ��悤�ɂ���
                    PushOpenStatusWindow.Instance.TransparentStatusButton();
                }

                if (buffer.IsAlive)
                {
                    //�w�肵���ʒu�Ƀ^�[���J�n�G�t�F�N�g����
                    StartCoroutine(ShowStartTurnEffect(SecondTurnEffect_SpawnPoint));

                    //1�t���[���҂�(�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z��)
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�o�b�t�@�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("BufferTurnText");

                    //�o�b�t�@�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //1�t���[���҂�(�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //�o�b�t�@�[�̃^�[���J�n(�L�����Z���ł��鏈��)
                    await PlayerTurnAction(buffer, "BufferOffDebuff", KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //�G�̐����󋵂��m�F�i�������X�g����Ȃ烋�[�v���~�߂�j
                    if (GameClearCheck())
                    {
                        return;
                    }
                    PushOpenStatusWindow.Instance.TransparentStatusButton();
                }

                if (healer.IsAlive)
                {
                    //�w�肵���ʒu�Ƀ^�[���J�n�G�t�F�N�g����
                    StartCoroutine(ShowStartTurnEffect(ThirdTurnEffect_SpawnPoint));

                    //1�t���[���҂�(�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z��)
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�q�[���[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("HealerTurnText");

                    //�q�[���[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //1�t���[���҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�X�e�[�^�X�E�B���h�E���J���{�^����������悤�ɂ���
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //�q�[���[�̃^�[���J�n(�L�����Z���ł��鏈���j
                    await PlayerTurnAction(healer, "HealerOffdebuff", KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //�G�̐����󋵂��m�F�i�������X�g����Ȃ烋�[�v���~�߂�j
                    if (GameClearCheck())
                    {
                        return;
                    }

                    //�X�e�[�^�X�{�^�����J���{�^���������Ȃ��悤�ɂ���
                    PushOpenStatusWindow.Instance.TransparentStatusButton();
                }
            }

            //�G�̃^�[��
            else
            {
                //�e�v���C���[�̍s���t���O��false�ɂ���
                attacker.IsAttackerAction = false;
                buffer.IsBufferAction = false;
                healer.IsHealerAction = false;

                //UI�}�l�[�W���[����G�^�[��UI��\��
                UIManager.Instance.EnemyTurnUI.SetActive(true);

                await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                //UI�}�l�[�W���[����G�^�[��UI���\��
                UIManager.Instance.EnemyTurnUI.SetActive(false);

                //�G�^�[���J�n
                await EnemyTurn(token);
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
    protected override async UniTask PlayerTurnAction(BasePlayerStatus player, string offDebuffTextID, KeyCode normalKey, KeyCode skillKey, KeyCode specialKey, CancellationToken token)
    {
        //�v���C���[�̍s���t���O��true�ɂȂ�܂Ń��[�v��������
        while (!player.IsPlayerAction)
        {
            //�|�[�Y��ʂɐ؂�ւ��\�ɂ���
            canPoseMode = true;

            //�����ꂩ�̃L�[���������܂ő҂�(�ʏ�A�X�L���A�K�E)
            await UniTask.WaitUntil(() =>
                Input.GetKeyDown(normalKey) ||
                Input.GetKeyDown(skillKey) ||
                Input.GetKeyDown(specialKey),
                cancellationToken: token);

            if (Input.GetKeyDown(normalKey))
            {
                //�v���C���[�̒ʏ�U�����s
                player.NormalAttack();

                //�ʏ�U���ł͂����Ƀv���C���[�s���t���O��true�ɂ�
                player.IsPlayerAction = true;
            }
            else if (Input.GetKeyDown(skillKey))
            {
                //�v���C���[�̃X�L�������s
                player.PlayerSkill();
                break;
            }
            else if (Input.GetKeyDown(specialKey))
            {
                if (!player.IsUseSpecial)
                {
                    //�K�E�����t���O��false�Ȃ�X�y�V���������s
                    player.SpecialSkill();
                    break;
                }
                else
                {
                    //JSON�t�@�C������K�E�g�p�s�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSpecial");

                    //�K�E�g�p�s��UI���\���ɂ���R�[���`��
                    StartCoroutine(HidePlayerActionText());
                }
            }
        }

        //�v���C���[�^�[�����I��������|�[�Y��ʂɂ͐؂�ւ���Ȃ�����
        if (player.IsPlayerAction)
        {
            //�v���C���[�̕K�E�����J�E���g�̃e�L�X�g���\��
            UIManager.Instance.SpecialLimitCountText.SetActive(false);
            canPoseMode = false;
        }

        //1�t���[���҂�(�L�����Z���ł��鏈���j
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

        //TryCatch���g���G���[��h��
        try
        {
            //�v���C���[���s�����I������܂ŏ�����҂�
            await UniTask.WaitUntil(() => player.IsPlayerAction, cancellationToken: token);

            //�v���C���[�S�̂̍s�����I�������v���C���[�^�[���I��
            if (attacker.IsPlayerAction && buffer.IsPlayerAction && healer.IsPlayerAction)
            {
                IsPlayerTurn = false;
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("�L�����Z�����ꂽ");
        }

        //1�t���[���҂�
        await UniTask.Yield();
    }

    /// <summary>
    /// ���j�^�X�N�G�̃^�[��
    /// </summary>
    /// <param name="token">�L�����Z���ł��鏈��/param>
    /// <returns>���j�^�X�N���~�߂鏈��</returns>
    async UniTask EnemyTurn(CancellationToken token)
    {
        if (slime1.EnemyIsAlive)
        {
            //�X���C��1�^�[��
            await SlimeTurn(slime1, token);
            
        }
        if (slime2.EnemyIsAlive)
        {
            //�X���C��2�^�[��
            await SlimeTurn(slime2, token);
        }

        if (slime3.EnemyIsAlive)
        {
            //�X���C��3�^�[��
            await SlimeTurn(slime3, token);
        }

        GameOverCheck();

        // �S�X���C�����s�����I������őS�Ń`�F�b�N
        if (isGameOver)
        {
            return;
        }



        //�G�̃^�[���I��
        IsPlayerTurn = true;
    }

    /// <summary>
    /// �X���C���^�[������
    /// </summary>
    /// <param name="slime">�X���C���̃X�e�[�^�X</param>
    /// <returns></returns>
    async UniTask SlimeTurn(Slime slime, CancellationToken token)
    {
        //�X���C���̍U���J�n
        await slime.SlimeAction();

        await UniTask.Yield();
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
