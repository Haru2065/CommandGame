using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using System.IO;

public class Stage2BattleSystem : BaseBattleManager
{
    //UniTask�̃L�����Z������g�[�N���\�[�X
    private CancellationTokenSource cts;

    [SerializeField]
    [Tooltip("3�̂̃X�P���g��")]
    private Skeleton skeleton1, skeleton2, skeleton3;

    [SerializeField]
    [Tooltip("�X�L�������J�E���g��TMProUGUI")]
    private TextMeshProUGUI skillLimitCountUGUI;

    [SerializeField]
    [Tooltip("�K�E�����J�E���g��TMProUGUI")]
    private TextMeshProUGUI specialLimitCountUGUI;

    [SerializeField]
    [Tooltip("�I���{�^���X�N���v�g")]
    protected PushExitButton pushExitButton;

    

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
        if(pushExitButton.IsQuitGame)
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

                //�Q�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
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

                    //1�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //�A�^�b�J�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("AttackerTurnText");

                    //�A�^�b�J�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //2�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //�A�^�b�J�[�^�[���J�n
                    await AttackerTurn(token);

                    //2�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

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

                    //1�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //�o�b�t�@�[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("BufferTurnText");

                    //�o�b�t�@�[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //2�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

                    //�o�b�t�@�[�̃^�[���J�n
                    await BufferTurn(token);

                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

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

                    //1�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                    //�q�[���[�̃^�[���J�n�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("HealerTurnText");

                    //�q�[���[�̃^�[���J�n�ʒm��\��
                    StartCoroutine(HidePlayerActionText());

                    //2�b�҂i�L�����Z���g�[�N�����Ă΂ꂽ��L�����Z���j
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);


                    //�q�[���[�̃^�[���J�n
                    await HealerTurn(token);

                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

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
    /// <param name="token">�L�����Z���ł��鏈��</param>
    /// <returns>UniTask���~�߂鏈��</returns>
    async UniTask AttackerTurn(CancellationToken token)
    {
        //�A�^�b�J�[���X�L�����g���Ă�����J�E���g�����炷
        if (attacker.IsUseSkill)
        {
            //UI�}�l�[�W���[����X�L���J�E���gUI��\��
            UIManager.Instance.SkillLimitCountText.SetActive(true);

            attacker.SkillLimitCount--;

            //�X�L�������J�E���g��0�Ȃ�����X�L�������t���Ofalse
            if (attacker.SkillLimitCount <= 0)
            {
                attacker.SkillLimitCount = 0;

                //UI�}�l�[�W���[����X�L�������J�E���gUI��\��
                UIManager.Instance.SkillLimitCountText.SetActive(false);
                attacker.IsUseSkill = false;
            }
            else
            {
                //�X�L�������J�E���g����\��
                skillLimitCountUGUI.text = $"{attacker.SkillLimitCount}";
            }
        }

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
                if (attacker.IsUseSkill == false) attacker.PlayerSkill();

                else
                {
                    //JSON�t�@�C������X�L���͎g�p�s�ʒm�\��
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSkill");

                    //�X�L���g�p�s��UI���\���ɂ���R�[���`��
                    StartCoroutine(HidePlayerActionText());
                }
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
    /// <param name="token">�L�����Z���ł��鏈��</param>
    /// <returns>���j�^�X�N���~�߂鏈��</returns>
    async UniTask BufferTurn(CancellationToken token)
    {
        //�o�b�t�@�[���X�L�����g���Ă�����J�E���g�����炷
        if (buffer.IsUseSkill)
        {
            //�o�b�t�@�[�̃X�L�������J�E���g��\��
            UIManager.Instance.SkillLimitCountText.SetActive(true);

            buffer.SkillLimitCount--;

            if (buffer.SkillLimitCount <= 0)
            {
                buffer.SkillLimitCount = 0;

                //�o�b�t�@�[�̃X�L�������J�E���g���\��
                UIManager.Instance.SkillLimitCountText.SetActive(false);
                buffer.IsUseSkill = false;
            }
            else
            {
                //�o�b�t�@�[�̃X�L�������J�E���g����\��
                skillLimitCountUGUI.text = $"{buffer.SkillLimitCount}";
            }
        }

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
                //�X�L�������t���O��false�Ȃ�X�L�������s
                if (buffer.IsUseSkill == false)
                {
                    //�o�b�t�@�[�̃X�L��
                    buffer.PlayerSkill();
                    break;
                }
                else
                {
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSkill");
                    StartCoroutine(HidePlayerActionText());
                }
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
    /// <param name="token">�L�����Z���ł��鏈��</param>
    /// <returns>���j�^�X�N���~�߂鏈��</returns>
    async UniTask HealerTurn(CancellationToken token)
    {
        //�q�[���[�̃X�L�������t���O��true�Ȃ�X�L�������J�E���g���炷
        if (healer.IsUseSkill)
        {
            //�q�[���[�̃X�L�������J�E���g��\��
            UIManager.Instance.SkillLimitCountText.SetActive(true);
            healer.SkillLimitCount--;
            
            
            //�X�L�������J�E���g��0�ɂȂ�����
            if (healer.SkillLimitCount <= 0)
            {

                healer.SkillLimitCount = 0;
                
                UIManager.Instance.SkillLimitCountText.SetActive(false);
                healer.IsUseSkill = false;
            }
            else
            {
                skillLimitCountUGUI.text = $"{healer.SkillLimitCount}";
            }
        }

        //�q�[���[�̕K�E�����t���O��true�Ȃ�K�v�����J�E���g���炷
        if (healer.IsUseSpecial)
        {
            //�q�[���[�̕K�E�����J�E���g��\��
            UIManager.Instance.SpecialLimitCountText.SetActive(true);
�@�@�@�@�@�@healer.SpecialLimitCount--;

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
                if (healer.IsUseSkill == false)
                {
                    //�q�[���[�̃X�L��
                    healer.PlayerSkill();

                    break;
                }
                else
                {
                    BattleActionTextManager.Instance.ShowBattleActionText("Can't_UseSkill");
                    StartCoroutine(HidePlayerActionText());
                }
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
}
