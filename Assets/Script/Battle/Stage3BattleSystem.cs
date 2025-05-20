using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// �X�e�[�W3�̃o�g���V�X�e��
/// </summary>
public class Stage3BattleSystem : BaseBattleManager
{
    private CancellationTokenSource cts;

    [SerializeField]
    [Tooltip("�K�E�����J�E���g��TMProUGUI")]
    private TextMeshProUGUI specialLimitCountUGUI;

    [SerializeField]
    [Tooltip("�I���{�^���X�N���v�g")]
    protected PushExitButton pushExitButton;

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

        //2�b�҂i�L�����Z���\����)
        await UniTask.Delay(TimeSpan.FromSeconds(2),cancellationToken: token);

        //���X�g�o�g��UI��\��
        LastButtleUI.SetActive(true);

        //Stage3��BGM���Đ�
        BGMControl.Instance.PlayStage3BGM();

        //2�b�҂i�L�����Z���\�����j
        await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

        //���X�g�o�g��UI���\��
        LastButtleUI.SetActive(false);

        //2�b�҂i�L�����Z���\�����j
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

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

                //2�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                //UI�}�l�[�W���[����v���C���[�^�[����\��
                UIManager.Instance.PlayerTurnUI.SetActive(false);

                //UI�}�l�[�W���[����v���C���[�^�[�����ɕ\������UI��\��
                UIManager.Instance.StartPlayerTurnUI();


                //�A�^�b�J�[���������Ă����珈�������s
                if (attacker.IsAlive)
                {
                    //�w�肵���ʒu�Ƀ^�[���J�n�G�t�F�N�g����
                    StartCoroutine(ShowStartTurnEffect(FirstTurnEffect_SpawnPoint));

                    //1�b�҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(1),cancellationToken: token);

                    //�A�^�b�J�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //�A�^�b�J�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //2�b�҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //�����A�^�b�J�[��HP�f�o�t���t�^����Ă�����_���[�W�����炵�A�f�o�t�J�E���g�����炷
                    if (attacker.IsHPDebuff)
                    {
                        //�_���[�W�����炵�A�f�o�t�J�E���g�����炷
                        attacker.PlayerCurrentHP -= dragon.HPDebuffPower;
                        attacker.DebuffCount--;

                        //�f�o�t�J�E���g��0�ɂȂ�����HP�f�o�t�������A�܂������������Ƃ�ʒm����
                        if(attacker.DebuffCount <= 0)
                        {
                            attacker.IsHPDebuff = false;
                            BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");

                            //�f�o�t�������̏󋵃e�L�X�g���\���ɂ���
                            StartCoroutine(attacker.PlayerOffDebuffText());

                            //2�b�҂�
                            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
                        }
                    }

                    //�A�^�b�J�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //�A�^�b�J�[�^�[���J�n
                    await AttackerTurn(token);

                    //2�t���[���҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);

                    //�G�̐����󋵂��m�F�i�������X�g����Ȃ烋�[�v���~�߂�j
                    if (GameClearCheck())
                    {
                        return;
                    }
                }

                if (buffer.IsAlive)
                {
                    //�w�肵���ʒu�Ƀ^�[���J�n�G�t�F�N�g����
                    StartCoroutine(ShowStartTurnEffect(SecondTurnEffect_SpawnPoint));

                    //1�b�҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //�o�b�t�@�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("BufferTurnText");

                    //�o�b�t�@�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //2�b�҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //�����o�b�t�@�[��HP�f�o�t���t�^����Ă�����_���[�W�����炵�A�f�o�t�J�E���g�����炷
                    if (buffer.IsHPDebuff)
                    {
                        //�_���[�W�����炵�A�f�o�t�J�E���g�����炷
                        buffer.PlayerCurrentHP -= dragon.HPDebuffPower;
                        buffer.DebuffCount--;

                        //�f�o�t�J�E���g��0�ɂȂ�����HP�f�o�t�������A�܂������������Ƃ�ʒm����
                        if (buffer.DebuffCount <= 0)
                        {
                            buffer.IsHPDebuff = false;
                            BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                            //�f�o�t�������̏󋵃e�L�X�g���\���ɂ���
                            StartCoroutine(buffer.PlayerOffDebuffText());

                            //2�b�҂�
                            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
                        }
                    }

                    //�o�b�t�@�[�̃^�[���J�n
                    await BufferTurn(token);

                    await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);

                    //�G�̐����󋵂��m�F�i�������X�g����Ȃ烋�[�v���~�߂�j
                    if (GameClearCheck())
                    {
                        return;
                    }
                }

                if (healer.IsAlive)
                {
                    //�w�肵���ʒu�Ƀ^�[���J�n�G�t�F�N�g����
                    StartCoroutine(ShowStartTurnEffect(ThirdTurnEffect_SpawnPoint));

                    //1�b�҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //�q�[���[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("HealerTurnText");

                    //�q�[���[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //2�b�҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //�����q�[���[��HP�f�o�t���t�^����Ă�����_���[�W�����炵�A�f�o�t�J�E���g�����炷
                    if (healer.IsHPDebuff)
                    {
                        //�_���[�W�����炵�A�f�o�t�J�E���g�����炷
                        healer.PlayerCurrentHP -= dragon.HPDebuffPower;
                        healer.DebuffCount--;

                        //�f�o�t�J�E���g��0�ɂȂ�����HP�f�o�t�������A�܂������������Ƃ�ʒm����
                        if (healer.DebuffCount <= 0)
                        {
                            healer.IsHPDebuff = false;
                            BattleActionTextManager.Instance.ShowBattleActionText("HealerOffDebuff");

                            //�f�o�t�������̏󋵃e�L�X�g���\���ɂ���
                            StartCoroutine(attacker.PlayerOffDebuffText());

                            //2�b�҂�
                            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
                        }
                    }

                    //�q�[���[�̃^�[���J�n
                    await HealerTurn(token);

                    await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);

                    //�G�̐����󋵂��m�F�i�������X�g����Ȃ烋�[�v���~�߂�j
                    if (GameClearCheck())
                    {
                        return;
                    }
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
    /// ���j�^�X�N�A�^�b�J�[�^�[��
    /// </summary>
    /// <param name="token">�L�����Z�������g�[�N��</param>
    /// <returns>UniTask���~�߂鏈��</returns>
    async UniTask AttackerTurn(CancellationToken token)
    {
        //�A�^�b�J�[�����g���Ă�����J�E���g�����炷
        if (attacker.IsUseSpecial)
        {
            //UI�}�l�[�W���[����X�L���J�E���gUI��\��
            UIManager.Instance.SpecialLimitCountText.SetActive(true);

            //�K�E�����J�E���g�����炷
            attacker.SpecialLimitCount--;

            //�K�E�����J�E���g��0�ɂȂ�����\�������t���Ofalse
            if (attacker.SpecialLimitCount <= 0)
            {
                attacker.SpecialDebuffCount = 0;

                //UI�}�l�[�W���[����K�E�J�E���g�����J�E���g��UI���\��
                UIManager.Instance.SpecialLimitCountText.SetActive(false);
                attacker.IsUseSpecial = false;
            }
            else
            {
                //�K�E�����J�E���g���\��
                specialLimitCountUGUI.text = $"{attacker.SpecialLimitCount}";
            }
        }

        //�A�^�b�J�[�̍s�����I���܂Ń��[�v��������
        while (!attacker.IsAttackerAction)
        {
            //�|�[�Y���[�h�\
            canPoseMode = true;

            //A�ES�EF�L�[�����ꂩ�̃L�[���������܂ŏ�����҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z������j
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F),
        cancellationToken: token);

            if (Input.GetKeyDown(KeyCode.A))
            {
                //�A�^�b�J�[�̒ʏ�U��
                attacker.NormalAttack();
            }

            else if (Input.GetKeyDown(KeyCode.S))
            {
                //�X�L�������t���O��false�Ȃ�X�L�������s
                attacker.PlayerSkill();
            }

            else if (Input.GetKeyDown(KeyCode.F))
            {
                //�K�E�����t���O��false�Ȃ�X�y�V���������s
                if (attacker.IsUseSpecial == false) attacker.SpecialSkill();

                else if (attacker.IsUseSpecial)
                {
                    //JSON�t�@�C������K�E�g�p�s�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSpecial");

                    //�K�E�g�p�s��UI���\���ɂ���R�[���`��
                    StartCoroutine(HidePlayerActionText());
                }

            }
        }

        //�A�^�b�J�[�̍s���t���O��true�ɂȂ����琧��UI���\��
        if (attacker.IsAttackerAction)
        {
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);

            canPoseMode = false;
        }

        //TryCatch���g���G���[��h��
        try
        {
            //�A�^�b�J�[�̍s���t���O��҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z������j
            await UniTask.WaitUntil(() => attacker.IsAttackerAction, cancellationToken: token);

            IsPlayerTurn = false;
        }

        catch (OperationCanceledException)
        {
            return;
        }
    }

    /// <summary>
    /// ���j�^�X�N�o�b�t�@�[�^�[��
    /// </summary>
    /// <param name="token">�L�����Z�������g�[�N��</param>
    /// <returns>���j�^�X�N���~�߂鏈��</returns>
    async UniTask BufferTurn(CancellationToken token)
    {
        //�o�b�t�@�[���K�E���g���Ă�����J�E���g�����炷
        if (buffer.IsUseSpecial)
        {
            //�o�b�t�@�[�̕K�E�����J�E���g��\��
            UIManager.Instance.SpecialLimitCountText.SetActive(true);
            buffer.SpecialLimitCount--;

            if (buffer.SpecialLimitCount <= 0)
            {
                buffer.SpecialDebuffCount = 0;

                //�o�b�t�@�[�̕K�E�����J�E���g���\��
                UIManager.Instance.SpecialLimitCountText.SetActive(false);
                buffer.IsUseSpecial = false;
            }
            else
            {
                specialLimitCountUGUI.text = $"{buffer.SpecialLimitCount}";
            }
        }

        while (!buffer.IsBufferAction)
        {
            canPoseMode = true;

            //A�ES�EF�L�[�����ꂩ�̃L�[���������܂ŏ�����҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z������j
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F),
        cancellationToken: token);

            if (Input.GetKeyDown(KeyCode.A))
            {
                //�o�b�t�@�[�̒ʏ�U��
                buffer.NormalAttack();

                //�ʏ�U�����͂����Ƀo�b�t�@�[�^�[�����I��
                buffer.IsBufferAction = true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                //�o�b�t�@�[�̃X�L��
                buffer.PlayerSkill();
                break;
            }

            else if (Input.GetKeyDown(KeyCode.F))
            {
                //�K�E�����t���O��false�Ȃ�K�E�����s
                if (buffer.IsUseSpecial == false)
                {
                    //�o�b�t�@�[�̕K�E���s
                    buffer.SpecialSkill();

                    //�K�E�̎��͂����Ƀo�b�t�@�[�^�[�����I��
                    buffer.IsBufferAction = true;
                }
                else
                {
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSpecial");
                    StartCoroutine(HidePlayerActionText());
                }
            }
        }

        //�|�[�Y�t���O���������Đ����J�E���gUI���\��
        if (buffer.IsBufferAction)
        {
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);
            canPoseMode = false;
        }

        //TryCatch���g���G���[��h��
        try
        {
            //�o�b�t�@�[�s������܂ŏ�����҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z������j
            await UniTask.WaitUntil(() => buffer.IsBufferAction, cancellationToken: token);

            IsPlayerTurn = false;
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    /// <summary>
    /// ���j�^�X�N�q�[���[�^�[��
    /// </summary>
    /// <param name="token">�L�����Z�������g�[�N��</param>
    /// <returns>���j�^�X�N���~�߂鏈��</returns>
    async UniTask HealerTurn(CancellationToken token)
    {

        //�q�[���[�̕K�E�����t���O��true�Ȃ�K�v�����J�E���g���炷
        if (healer.IsUseSpecial)
        {
            //�q�[���[�̕K�E�����J�E���g��\��
            UIManager.Instance.SpecialLimitCountText.SetActive(true);
            healer.SpecialLimitCount--;

            if (healer.SpecialLimitCount <= 0)
            {
                healer.SpecialLimitCount = 0;

                UIManager.Instance.SpecialLimitCountText.SetActive(false);
                healer.IsUseSpecial = false;
            }
            else
            {
                specialLimitCountUGUI.text = $"{healer.SpecialLimitCount}";
            }
        }

        while (!healer.IsHealerAction)
        {
            canPoseMode = true;

            //A�ES�EF�L�[�����ꂩ�̃L�[���������܂ŏ�����҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z������j
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.F),
        cancellationToken: token);

            if (Input.GetKeyDown(KeyCode.A))
            {
                //�q�[���[�̒ʏ�U��
                healer.NormalAttack();

                //�ʏ�U�����͂����Ƀq�[���[�^�[���I��
                healer.IsHealerAction = true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                //�q�[���[�̃X�L��
                healer.PlayerSkill();
                break;
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                if (healer.IsUseSpecial == false)
                {
                    //�q�[���[�̕K�E
                    healer.SpecialSkill();

                    //�ʏ�U�����͂����Ƀq�[���[�^�[���I��
                    healer.IsHealerAction = true;
                }
                else
                {
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSpecial");
                    StartCoroutine(HidePlayerActionText());
                }

            }
        }

        //�q�[���[�̍s�����I���΃X�L���ƕK�E�̐����J�E���gUI���\��
        if (healer.IsHealerAction)
        {
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);

            canPoseMode = false;
        }

        try
        {
            await UniTask.WaitUntil(() => healer.IsHealerAction, cancellationToken: token);
            IsPlayerTurn = false;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("�L�����Z�����ꂽ");
            return;
        }
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
