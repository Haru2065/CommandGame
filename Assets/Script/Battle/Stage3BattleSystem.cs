using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// �X�e�[�W3�̃o�g���V�X�e��
/// </summary>
public class Stage3BattleSystem : BaseBattleManager
{
    

    [SerializeField]
    [Tooltip("�K�E�����J�E���g��TMProUGUI")]
    private TextMeshProUGUI specialLimitCountUGUI;    

    [SerializeField]
    [Tooltip("�h���S��")]
    private Dragon dragon;

    [SerializeField]
    [Tooltip("���X�g�o�g��UI")]
    private GameObject LastButtleUI;

    //�o�g���J�n���ꉉ�o�i�X�e�[�W3����ŃX�^�[�g���o���I���������̃t���O�j
    private bool isEndDirection;

    //�U���Ώێ҂̃��X�g
    private List<BasePlayerStatus> playerParty;

    // Start is called before the first frame update
    protected override async void Start()
    {
        //�e�N���X��UI�̏������ƓG�̍U���Ώۂ�ݒ�
        base.Start();

        //�o�g���̊J�n���o�I���t���O��false
        isEndDirection = false;

        //�ŏ��̓|�[�Y��ʂɂ͂ł��Ȃ�
        canPoseMode = false;

        //�U���Ώۂ𐶑����X�g�ɐݒ�
        playerParty = alivePlayers;

        //�L�����Z���g�[�N���\�[�X�𐶐�
        cts = new CancellationTokenSource();

        //�X�e�[�W3�J�n���ꉉ�o���J�n
        await BattleStartAction(cts.Token);

        //�o�g�����[�v�̊J�n
        await BattleLoop(cts.Token);
    }

    // Update is called once per frame
    protected override void Update()
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

        //�����I���{�^���������ꂽ����S�ɏI�����邽�߂ɃL�����Z���������s��
        if (pushExitButton.IsQuitGame)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    /// <summary>
    /// �X�e�[�W3�J�n���ꉉ�o
    /// </summary>
    /// <param name="token">�L�����Z���ł��鏈��</param>
    /// <returns></returns>
    async UniTask BattleStartAction(CancellationToken token)
    {
        //�h���S���̖��������s
        EnemySE.Instance.Play_DragonRourSE();

        //2�t���[���҂i�L�����Z���\����)
        await UniTask.Delay(TimeSpan.FromSeconds(2f),cancellationToken: token);

        //���X�g�o�g��UI��\��
        LastButtleUI.SetActive(true);

        //Stage3��BGM���Đ�
        BGMControl.Instance.PlayStage3BGM();

        //1�t���[���҂i�L�����Z���\�����j
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

        //���X�g�o�g��UI���\��
        LastButtleUI.SetActive(false);

        //1�b�҂i�L�����Z���\�����j
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

        //�J�n���o�I��
        isEndDirection = true;

        //�J�n���o���I������܂ő҂i�L�����Z���\�����j
        await UniTask.WaitUntil((() => isEndDirection), cancellationToken: token) ;
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

                    //
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay),cancellationToken: token);

                    //�A�^�b�J�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //�A�^�b�J�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //1�t���[���҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�X�e�[�^�X�E�B���h�E���J���{�^����������悤�ɂ���
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //�A�^�b�J�[�^�[���J�n(�L�����Z���ł��鏈��)
                    await PlayerTurnAction(attacker, KeyCode.A, KeyCode.S, KeyCode.F, token);
                    
                    //�����A�^�b�J�[��HP�f�o�t���t�^����Ă�����_���[�W�����炵�A�f�o�t�J�E���g�����炷
                    if (attacker.IsHPDebuff)
                    {
                        //�A�^�b�J�[�̃f�o�t�����i�f�o�t�������ꂽ��A�f�o�t�����ʒm��\���j
                        await HPDebuff(attacker, "AttackerOffDebuff", token);
                    }

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
                    await PlayerTurnAction(buffer, KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //�����o�b�t�@�[��HP�f�o�t���t�^����Ă�����_���[�W�����炵�A�f�o�t�J�E���g�����炷
                    if (buffer.IsHPDebuff)
                    {
                        //�o�b�t�@�[�̃f�o�t�����i�f�o�t�������ꂽ��A�f�o�t�����ʒm��\���j
                        await HPDebuff(buffer, "BufferOffDebuff", token);
                    }

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
                    await PlayerTurnAction(healer, KeyCode.A, KeyCode.S, KeyCode.F, token);

                    //�����q�[���[��HP�f�o�t���t�^����Ă�����_���[�W�����炵�A�f�o�t�J�E���g�����炷
                    if (healer.IsHPDebuff)
                    {
                        //�q�[���[�̃f�o�t�����i�f�o�t�������ꂽ��A�f�o�t�����ʒm��\���j
                        await HPDebuff(healer, "HealerOffDebuff", token);
                    }

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
    protected override async UniTask PlayerTurnAction(BasePlayerStatus player,KeyCode normalKey, KeyCode skillKey, KeyCode specialKey, CancellationToken token)
    {
        //�K�E�������Ȃ�UI��\�����Đ����J�E���g�����炷
        if (player.IsUseSpecial)
        {
            UIManager.Instance.SpecialLimitCountText.SetActive(true);
            player.SpecialLimitCount--;

            //�K�E�����J�E���g��0�ɂȂ�����K�E���g�p�\�ɂ���
            if (player.SpecialLimitCount <= 0)
            {
                player.SpecialLimitCount = 0;
                player.IsUseSpecial = false;

                //�K�E�����J�E���g��UI��\��
                UIManager.Instance.SpecialLimitCountText.SetActive(false);
            }

            //�����p�����Ȃ炻�̂܂�UI��\������
            else
            {
                specialLimitCountUGUI.text = $"({player.SpecialLimitCount})";
            }
        }

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

        ////�f�o�t���t�^����Ă�����HP�����炷
        //if(player.IsDebuff)
        //{
        //    await HPDebuff(player, offDebuffTextID, token);
        //}

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
    /// <param name="token">�L�����Z���ł��鏈��</param>
    /// <returns>���j�^�X�N���~�߂鏈��</returns>
    async UniTask EnemyTurn(CancellationToken token)
    {
        //�X�P���g���h���S���̃^�[��
        await DragonTurn(token);

        if (GameOverCheck()) return;

        //1�t���[���҂�
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay));

        IsPlayerTurn = true;
    }

    /// <summary>
    /// ���j�^�X�N�h���S���^�[��
    /// </summary>
    /// <param name="Token">�L�����Z���ł��鏈��/param>
    /// <returns>���̏�����҂�</returns>
    async UniTask DragonTurn(CancellationToken Token)
    {
        await dragon.DragonAction(playerParty);

        //1�t���[���҂�
        await UniTask.Yield();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="debuffHPTextkey"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    async UniTask HPDebuff(BasePlayerStatus player, string debuffHPTextkey, CancellationToken token)
    {
        if (player == null) return;

        player.PlayerCurrentHP -= dragon.HPDebuffPower;

        player.PlayerHPBar.value = player.PlayerCurrentHP;

        player.DebuffCount--;

        if(player.DebuffCount  <= 0)
        {
            player.IsDebuff = false;

            BattleActionTextManager.Instance.ShowBattleActionText(debuffHPTextkey);

            StartCoroutine(player.PlayerOffDebuffText());

            await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);
        }
       
    }

    /// <summary>
    /// �v���C���[���S�ł��m�F���A�Q�[���I�[�o�[���������s����B
    /// </summary>
    /// <returns>�S�ł��Ă�����true</returns>
    protected override bool GameClearCheck()
    {
        //�����G���S�ł�����N���A
        if (aliveEnemies.Count == 0)
        {
            //�Q�[���N���A�t���O��true
            isGameClear = true;

            //�L�����Z�����������s���A�L�����Z���g�[�N����j�������\�[�X�����
            cts.Cancel();
            cts.Dispose();

            //�Q�[���N���A�V�[�������[�h
            SceneManager.LoadScene("GameClear");

            //�N���A�����̂�true��Ԃ�
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
        //�����������S�ł�����Q�[���I�[�o�[
        if (alivePlayers.Count == 0)
        {
            //�Q�[���I�[�o�[�t���O��true
            isGameOver = true;

            //UI�}�l�[�W���[����Q�[���I�[�o�[��\��
            UIManager.Instance.GameOverUI();

            //���x���A�b�v�������Ƃ��E�B���h�E�\��
            BattleActionTextManager.Instance.ShowBattleActionText("Retry");

            //�L�����Z�����������s���A�L�����Z���g�[�N����j�������\�[�X�����
            cts.Cancel();
            cts.Dispose();

            //�Q�[���I�[�o�[�Ȃ̂�true��Ԃ�
            return true;
        }
        //�����������Ă���̂�false��Ԃ�
        return false;
    }
}
