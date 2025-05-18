using UnityEngine;

/// <summary>
/// UI�֌W���ꊇ�Ǘ�
/// </summary>
public class UIManager : MonoBehaviour
{
    //UI�}�l�[�W���[�̃C���X�^���X���p
    private static UIManager instance;

    /// <summary>
    /// �C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static UIManager Instance
    {
        get => instance;
    }

    /// <summary>
    /// UI�}�l�[�W���[���C���X�^���X��
    /// </summary>
    void Awake()
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

    [SerializeField]
    [Tooltip("�v���C���[�̃^�[��UI")]
    private GameObject playerTurnUI;
    /// <summary>
    /// �v���C���[�^�[����UI�̃Q�b�^�[
    /// </summary>
    public GameObject PlayerTurnUI
    {
        get => playerTurnUI;
    }








    [SerializeField]
    [Tooltip("�G�̃^�[��UI")]
    private GameObject enemyTurnUI;

    /// <summary>
    /// �G�̃^�[����UI�̃Q�b�^�[
    /// </summary>
    public GameObject EnemyTurnUI
    {
        get => enemyTurnUI;
    }

    [SerializeField]
    [Tooltip("�L�[�{�[�hA")]
    private GameObject aKey;

    [SerializeField]
    [Tooltip("�L�[�{�[�hS")]
    private GameObject sKey;

    [SerializeField]
    [Tooltip("�L�[�{�[�hF")]
    private GameObject fKey;

    [SerializeField]
    [Tooltip("�ʏ�U��UI")]
    private GameObject attackUI;

    [SerializeField]
    [Tooltip("�X�L��UI")]
    private GameObject skillUI;

    [SerializeField]
    [Tooltip("�K�EUI")]
    private GameObject specialUI;

    [SerializeField]
    [Tooltip("�Q�[���N���AUI")]
    private GameObject gameClearUI;

    [SerializeField]
    [Tooltip("�Q�[���I�[�o�[UI")]
    private GameObject gameOverUI;

    [SerializeField]
    [Tooltip("�I���{�^��")]
    private GameObject exitButton;

    [SerializeField]
    [Tooltip("�Ē���{�^��")]
    private GameObject reTryButton;

    [SerializeField]
    [Tooltip("�^�C�g���{�^��")]
    private GameObject titleButton;

    [SerializeField]
    [Tooltip("�X�e�[�^�X�E�B���h�E")]
    private GameObject statusWindow;

    /// <summary>
    /// �X�e�[�^�X�E�B���h�E�̃Q�b�^�[
    /// </summary>
    public GameObject StatusWindow
    {
        get => statusWindow;
    }

    [SerializeField]
    [Tooltip("�X�e�[�^�X�E�B���h�E���J���{�^��")]
    private GameObject openStatusWindowButton;

    /// <summary>
    /// �X�e�[�^�X�E�B���h�E���J���{�^���̃Q�b�^�[
    /// </summary>
    public GameObject OpenStatusWindowButton
    {
        get => openStatusWindowButton;
    }


    [SerializeField]
    [Tooltip("�X�e�[�^�X�E�B���h�E�����{�^��")]
    private GameObject closeStatusWindowButton;

    /// <summary>
    /// �X�e�[�^�X�E�B���h�E�����{�^���̃Q�b�^�[
    /// </summary>
    public GameObject CloseStatusWindowButton
    {
        get => closeStatusWindowButton;
    }

    [SerializeField]
    [Tooltip("�|�[�Y���̃p�l��")]
    private GameObject pausePanel;

    [SerializeField]
    [Tooltip("�|�[�Y���̏I���{�^��")]
    private GameObject pauseExitButton;

    [SerializeField]
    [Tooltip("�|�[�Y���̃^�C�g���{�^��")]
    private GameObject pauseTitleButton;

    [SerializeField]
    [Tooltip("�X�e�[�W�Z���N�g��ʃ{�^��")]
    private GameObject stageSelectButton;

    [SerializeField]
    [Tooltip("�X�L�������J�E���g�̃e�L�X�g")]
    private GameObject skillLimitCountText;

    /// <summary>
    /// �A�^�b�J�[�̃X�L�������J�E���g�̃e�L�X�g�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public GameObject SkillLimitCountText
    {
        get => skillLimitCountText;
        set => skillLimitCountText = value;
    }

    [SerializeField]
    [Tooltip("�K�E�����J�E���g�̃e�L�X�g")]
    private GameObject specialLimitCountText;

    /// <summary>
    /// �K�E�����J�E���g�̃e�L�X�g�̃Q�b�^�[�Z�b�^�[
    /// </summary>
    public GameObject SpecialLimitCountText
    {
        get => specialLimitCountText;
        set => specialLimitCountText = value;
    }

    /// <summary>
    /// UI�}�l�[�W���[��UI����x�A��\���ɂ��郁�\�b�h
    /// </summary>
    public void StartUI()
    {
        //�v���C���[�^�[��UI�ƓG�̃^�[��UI���\���ŏ�����
        playerTurnUI.SetActive(false);
        enemyTurnUI.SetActive(false);

        //�N���A�ƃQ�[���I�[�o�[UI���\���ŏ�����
        gameClearUI.SetActive(false);
        gameOverUI.SetActive(false);

        //�Ē���ƏI���{�^���ƃ^�C�g���{�^���ƃX�e�[�W�Z���N�g�{�^�����\���ŏ�����
        exitButton.SetActive(false);
        reTryButton.SetActive(false);
        titleButton.SetActive(false);
        stageSelectButton.SetActive(false);

        /*�e�v���C���[�^�[�����ɕ\�������UI���\���ŏ�����*/
        aKey.SetActive(false);
        sKey.SetActive(false);
        fKey.SetActive(false);

        attackUI.SetActive(false);
        skillUI.SetActive(false);
        specialUI.SetActive(false);

        //�X�e�[�^�X�E�B���h�E���\��
        statusWindow.SetActive(false);

        //�X�e�[�^�X�E�B���h�E�����{�^�����\��
        closeStatusWindowButton.SetActive(false);

        //�X�e�[�^�X�E�B���h�E���J���{�^����\��
        closeStatusWindowButton.SetActive(true);

        //�|�[�Y���ɕ\������p�l���A�{�^�����\��
        pausePanel.SetActive(false);
        pauseExitButton.SetActive(false);
        pauseTitleButton.SetActive(false);

        skillLimitCountText.SetActive(false);
        specialLimitCountText.SetActive(false);
    }

    /// <summary>
    /// �v���C���[�����삷��UI��\�����郁�\�b�h
    /// </summary>
    public void StartPlayerTurnUI()
    {
        aKey.SetActive(true);
        sKey.SetActive(true);
        fKey.SetActive(true);

        attackUI.SetActive(true);
        skillUI.SetActive(true);
        specialUI.SetActive(true);
    }

    /// <summary>
    /// �G�̃^�[���ɓ��鎞�Ƀv���C���[�����삷��UI���\���ɂ��郁�\�b�h
    /// </summary>
    public void StartEnemyTurnUI()
    {
        aKey.SetActive(false);
        sKey.SetActive(false);
        fKey.SetActive(false);

        attackUI.SetActive(false);
        skillUI.SetActive(false);
        specialUI.SetActive(false);
    }

    /// <summary>
    /// �Q�[���N���A���ɕ\�����郁�\�b�h
    /// </summary>
    public void GameClearUI()
    {
        aKey.SetActive(false);
        sKey.SetActive(false);
        fKey.SetActive(false);

        attackUI.SetActive(false);
        skillUI.SetActive(false);
        specialUI.SetActive(false);

        //�^�C�g���{�^���A�I���{�^���A�X�e�[�W�Z���N�g�{�^����\��
        exitButton.SetActive(true);
        titleButton.SetActive(true);
        stageSelectButton.SetActive(true);

        //�Q�[���N���AUI��\��
        gameClearUI.SetActive(true);
    }

    /// <summary>
    /// �Q�[���I�[�o�[���ɕ\�����郁�\�b�h
    /// </summary>
    public void GameOverUI()
    {
        aKey.SetActive(false);
        sKey.SetActive(false);
        fKey.SetActive(false);

        attackUI.SetActive(false);
        skillUI.SetActive(false);
        specialUI.SetActive(false);

        //�I���A�Ē���A�^�C�g���A�X�e�[�W�Z���N�g�{�^����\��
        exitButton.SetActive(true);
        titleButton.SetActive(true);
        reTryButton.SetActive(true);
        stageSelectButton.SetActive(true);

        //�Q�[���I�[�o�[UI��\��
        gameOverUI.SetActive(true);
    }

    /// <summary>
    /// �X�e�[�^�X�E�B���h�E����郁�\�b�h
    /// </summary>
    public void CloseStatusWindow()
    {
        //�X�e�[�^�X�E�B���h�E�ƕ���{�^�����\��
        statusWindow.SetActive(false);
        closeStatusWindowButton.SetActive(false);

        //�X�e�[�^�X�E�B���h�E���J���{�^����\��
        openStatusWindowButton.SetActive(true);
    }

    /// <summary>
    /// ���f��ʂ�\�����郁�\�b�h
    /// </summary>
    public void ShowPauseMode()
    {
        pausePanel.SetActive(true);

        //�^�C�g���{�^���I���{�^���\��
        pauseTitleButton.SetActive(true);
        pauseExitButton.SetActive(true);
    }

    /// <summary>
    /// �|�[�Y��ʂ��\���ɂ��郁�\�b�h
    /// </summary>
    public void HidePauseMode()
    {
        pausePanel.SetActive(false);

        //�^�C�g���{�^���I���{�^����\��
        pauseTitleButton.SetActive(false);
        pauseExitButton.SetActive(false);
    }
}
