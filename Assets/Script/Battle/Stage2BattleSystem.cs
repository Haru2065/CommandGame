using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using System.IO;

public class Stage2BattleSystem : BaseBattleManager
{
    [SerializeField]
    [Tooltip("3�̂̃X�P���g��")]
    private Skeleton skeleton1, skeleton2, skeleton3;

    [SerializeField]
    [Tooltip("�X�L�������J�E���g��TMProUGUI")]
    private TextMeshProUGUI skillLimitCountUGUI;

    [SerializeField]
    [Tooltip("�K�E�����J�E���g��TMProUGUI")]
    private TextMeshProUGUI specialLimitCountUGUI;

    // Start is called before the first frame update
    protected override async void Start()
    {
        //�x�[�X�̃o�g���V�X�e�����珉���ݒ���s��
        base.Start();

        //�X�e�[�W�̃Z�[�u�f�[�^��ǂݍ���
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
            IsUnlockStage3 = false;
        }

        //UNiTask���L�����Z������g�[�N���𐶐�
        cts = new CancellationTokenSource();

        canPoseMode = false;

        //�o�g�����[�v�J�n
        //�L�����Z���g�[�N����n��
        await BattleLoop(cts.Token);
    }

    /// <summary>
    /// �|�[�Y���[�h�ɂ���
    /// </summary>
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
    /// UniTask�o�g���̃��[�v����
    /// </summary>
    /// <param name="token">�L�����Z���ł��鏈��/param>
    /// <returns>�v���C���[���s������܂ő҂�</returns>
    async UniTask BattleLoop(CancellationToken token)
    {
        //�Q�[���N���A�E�Q�[���I�[�o�[�̃t���O��true�Ȃ烋�[�v���~�߂ăL�����Z������
        while (!(isGameClear || isGameOver || token.IsCancellationRequested))
        {
            if (IsPlayerTurn)
            {
                //UI�}�l�[�W���[����v���C���[�^�[���\��
                UIManager.Instance.PlayerTurnUI.SetActive(true);

                //1�t���[���҂�(�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
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

                    //1�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�A�^�b�J�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //�A�^�b�J�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //2�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�X�e�[�^�X�E�B���h�E���J���{�^����������悤�ɂ���
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //�A�^�b�J�[�^�[���J�n
                    await PlayerTurnAction(attacker, KeyCode.A,KeyCode.S, KeyCode.F, token);

                    //�f�o�t���t�^����Ă��邩�`�F�b�N
                    statusDebuffCheck(attacker,"AttackerOffDebuff");

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

                    //�o�b�t�@�[�̃^�[���J�n
                    await PlayerTurnAction(buffer, KeyCode.A, KeyCode.S, KeyCode.F,token);

                    //�f�o�t���t�^����Ă��邩�`�F�b�N
                    statusDebuffCheck(attacker, "AttackerOffDebuff");

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

                    //2�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�X�e�[�^�X�E�B���h�E���J���{�^����������悤�ɂ���
                    PushOpenStatusWindow.Instance.CanPushStatusButton();

                    //�q�[���[�̃^�[���J�n
                    await PlayerTurnAction(healer, KeyCode.A, KeyCode.S, KeyCode.F, token);

                    await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

                    //�G�̐����󋵂��m�F�i�������X�g����Ȃ烋�[�v���~�߂�j
                    if (GameClearCheck())
                    {
                        return;
                    }

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

                await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);

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
    protected override async UniTask PlayerTurnAction(BasePlayerStatus player, KeyCode normalKey, KeyCode skillKey, KeyCode specialKey, CancellationToken token)
    {
        //�X�L���������Ȃ�UI��\���ɂ��Đ����J�E���g�����炷
        if (player.IsUseSkill)
        {
            //UI�}�l�[�W���[����X�L���J�E���gUI��\��
            UIManager.Instance.SkillLimitCountText.SetActive(true);

            player.SkillLimitCount--;

            //�X�L�������J�E���g��0�ɂȂ�����UI���\���ɂ��Ďg�p�\�ɂ���
            if (player.SkillLimitCount <= 0)
            {
                player.SkillLimitCount = 0;

                //UI�}�l�[�W���[����X�L�������J�E���gUI��\��
                UIManager.Instance.SkillLimitCountText.SetActive(false);
                player.IsUseSkill = false;
            }

            //�����p�����Ȃ炻�̂܂�UI��\������
            else
            {
                //�X�L�������J�E���g����\��
                skillLimitCountUGUI.text = $"{"{" + player.SkillLimitCount + "}" }";
            }
        }

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

                //�ʏ�U���ł͂����Ƀv���C���[�s���t���O��true�ɂ���
                player.IsPlayerAction = true;
            }
            else if (Input.GetKeyDown(skillKey))
            {
                //�X�L�������t���O��false�Ȃ�X�L�������s
                if (player.IsUseSkill == false)
                {
                    player.PlayerSkill();
                    break;
                }

                else
                {
                    //JSON�t�@�C������X�L���͎g�p�s�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSkill");

                    //�X�L���g�p�s��UI���\���ɂ���R�[���`��
                    StartCoroutine(HidePlayerActionText());
                }
            }

            else if (Input.GetKeyDown(specialKey))
            {
                //�K�E�����t���O��false�Ȃ�X�y�V���������s
                if (player.IsUseSpecial == false)
                {
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
            //�v���C���[�̃X�L�������J�E���g�ƕK�E�����J�E���g�̃e�L�X�g��\��
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);
            canPoseMode = false;
        }

        //1�t���[���҂�(�L�����Z���ł��鏈���j
        await UniTask.Delay(TimeSpan.FromSeconds(TurnDelay), cancellationToken: token);

        //tryCatch���g���G���[��h��
        try
        {
            //�v���C���[���s�����I������܂ŏ�����҂�
            await UniTask.WaitUntil(() => player.IsPlayerAction, cancellationToken: token);

            //�v���C���[�S�̂̍s�����I�������v���C���[�^�[���I��
            if(attacker.IsPlayerAction && buffer.IsPlayerAction &&healer.IsPlayerAction)
            {
                IsPlayerTurn = false;
            }
        }
        catch(OperationCanceledException)
        {
            Debug.Log("�L�����Z�����ꂽ");
        }

        //1�t���[���҂�
        await UniTask.Yield();
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

            if (!IsUnlockStage3)
            {
                //�X�e�[�W3�����
                IsUnlockStage3 = true;

                //�X�e�[�W�f�[�^��ۑ�
                SaveManager.SaveStage();
            }

            //2�b�x��ăN���AUI��\�����A���x���A�b�v�������s��
            Invoke("DelayGameClearUI", 2);

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

            //�L�����Z�����������s���A�L�����Z���g�[�N����j�������\�[�X�����
            cts.Cancel();
            cts.Dispose();

            //�Q�[���I�[�o�[�Ȃ̂�true��Ԃ�
            return true;
        }
        //�����������Ă���̂�false��Ԃ�
        return false;
    }

    /// <summary>
    /// ���j�^�X�N�G�̃^�[��
    /// </summary>
    /// <param name="token">�L�����Z���ł��鏈��/param>
    /// <returns>���j�^�X�N���~�߂鏈��</returns>
    async UniTask EnemyTurn(CancellationToken token)
    {
        if (skeleton1.EnemyIsAlive)
        {
            //�X�P���g��1�^�[��
            await SkeltonTurn(skeleton1, token);
            if (GameOverCheck()) return;
        }
        if (skeleton2.EnemyIsAlive)
        {
            //�X�P���g��2�^�[��
            await SkeltonTurn(skeleton2, token);
            if (GameOverCheck()) return;
        }

        if (skeleton3.EnemyIsAlive)
        {
            //�X�P���g��3�^�[��
            await SkeltonTurn(skeleton3, token);
            if (GameOverCheck()) return;
        }

        IsPlayerTurn = true;
    }

    /// <summary>
    /// �X�P���g���^�[��
    /// </summary>
    /// <param name="skeleton">�X�P���g���X�e�[�^�X</param>
    /// <param name="token">�L�����Z���ł��鏈��</param>
    async UniTask SkeltonTurn(Skeleton skeleton, CancellationToken token)
    {
        //�X�P���g���A�N�V����
        await skeleton.SkeletonAction(skeleton);

        //1�t���[���҂�
        await UniTask.Yield();
    }

    /// <summary>
    /// �v���C���[�̃f�o�t�󋵂��m�F���郁�\�b�h
    /// </summary>
    /// <param name="debuffPlayer">�f�o�t�Ώێ�</param>
    /// <param name="playerOffDebuffText">Json�̃f�o�t�����ʒmID</param>
    private void statusDebuffCheck(BasePlayerStatus debuffPlayer, string playerOffDebuffText)
    {
        //�v���C���[���f�o�t�t�^����Ă��Ȃ�������Ȃɂ����Ȃ�
        if (!debuffPlayer.IsDebuff) return;

        //�����v���C���[���f�o�t���t�^����Ă�����f�o�t�J�E���g�����炷
        else if (debuffPlayer.IsDebuff)
        {
            debuffPlayer.DebuffCount--;

            //�f�o�t�J�E���g��0�ɂȂ�����U���͂����Ƃɖ߂��āA�f�o�t����
            if (debuffPlayer.DebuffCount <= 0)
            {
                debuffPlayer.AttackPower = debuffPlayer.PlayerResetAttackPower;
                debuffPlayer.IsDebuff = false;

                // JSON�t�@�C���Őݒ肳�ꂽ�A�^�b�J�[�̃f�o�t�����ʒm��\��
                BattleActionTextManager.Instance.ShowBattleActionText(playerOffDebuffText);

                //�f�o�t�������̏󋵃e�L�X�g���\���ɂ���
                StartCoroutine(debuffPlayer.PlayerOffDebuffText());
            }
        }

        //��������f�o�t���t�^����Ă��Ȃ������牽�����Ȃ�
        if(!debuffPlayer.IsSpecialDebuff) return ;
        
        //��������f�o�t���t�^����Ă��������f�o�t�̃J�E���g�����炷
        else if(debuffPlayer.IsSpecialDebuff)
        {
            debuffPlayer.SpecialDebuffCount--;

            // ����f�o�t�̃J�E���g��0�ɂȂ�����A�U���͂����ɖ߂��ăf�o�t������
            if (debuffPlayer.SpecialDebuffCount <= 0)
            {
                debuffPlayer.AttackPower = debuffPlayer.PlayerResetAttackPower;
                debuffPlayer.IsSpecialDebuff = false;

                // JSON�t�@�C���Őݒ肳�ꂽ�A�^�b�J�[�̓���f�o�t�����ʒm��\��
                BattleActionTextManager.Instance.ShowBattleActionText(playerOffDebuffText);

                // �f�o�t�������̏󋵃e�L�X�g���\���ɂ���
                StartCoroutine(debuffPlayer.PlayerOffDebuffText());
            }
        }
    }


}
