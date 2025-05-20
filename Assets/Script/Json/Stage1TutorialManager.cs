using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1TutorialManager : TutorialManager
{
    [SerializeField]
    [Tooltip("�^�[���̃G�t�F�N�g��\������摜")]
    private GameObject turnEffectImage;

    [SerializeField]
    [Tooltip("�G�̃^�[�Q�b�g��I��ł���摜")]
    private GameObject enemyTargetImage;

    [SerializeField]
    [Tooltip("�v���C���[�̑���L�[�̉摜")]
    private GameObject keyBordImage;

    [SerializeField]
    [Tooltip("�L�����̃X�e�[�^�X�Љ�̉摜")]
    private GameObject statusCharaImage;

    [SerializeField]
    [Tooltip("�v���C���[�̃X�e�[�^�X���J���{�^���̉摜")]
    private GameObject statusButtonImage;

    // Start is called before the first frame update
    protected override void Start()
    {
        //�S�Ă̐����摜���\��
        HideStartImage();

        base.Start();
    }
    
    /// <summary>
    /// �S�Ă̐����摜���\���ɂ��郁�\�b�h
    /// </summary>
    void HideStartImage()
    {
        turnEffectImage.SetActive(false);
        enemyTargetImage.SetActive(false);
        keyBordImage.SetActive(false);

        statusCharaImage.SetActive(false);
        statusButtonImage.SetActive(false);
    }



    /// <summary>
    /// �N���b�N���ꂽ��e�L�X�g�̕\����i�߂郁�\�b�h
    /// </summary>
    protected override void OnClickNextText()
    {
        //�\���C���f�b�N�X�����ɐi�߂�
        currentIndex++;

        //�\���C���f�b�N�X���Ō�܂ŕ\������܂ŕ\��
        if (currentIndex < tutorialList.Count)
        {
            //�\���C���f�b�N�X�̐��ɓ����Ă���Json�e�L�X�g��\��
            ShowTutorialText(tutorialList[currentIndex]);
        }

        //�Ō�܂ŕ\��������I��
        else
        {
            Debug.Log("�`���[�g���A���I��!");

            //�Ō�܂ŕ\�������玩���G�ɃX�e�[�W�Z���N�g��ʂɈړ�
            SceneManager.LoadScene("StageSelect");

        }
    }

    /// <summary>
    /// JSON�̃`���[�g���A����\���R���g���[�����郁�\�b�h
    /// </summary>
    /// <param name="scenarios">Json�̃`���[�g���A��</param>
    public override void ShowTutorialText(TutorialText scenarios)
    {
        //Json�ɏ����ꂽ�e�L�X�g������UI�Ƃ��ĕ\��
        TutorialTextUGUI.text = scenarios.text;

        //�`���[�g���A����id�𒲂ׂĂ���ɉ����ăe�L�X�g��UI��\��
        switch (scenarios.id)
        {
            case "Welocome":
                break;

            case "Battle_1":

                //�o�g�������P�̎��Ƀ^�[���G�t�F�N�g�̉摜��\��
                turnEffectImage.SetActive(true);
                break;

            case "Battle_2":
                break;

            case "Battle_3":
                break;

            case "Operation_1":

                //�����Ń^�[���G�t�F�N�g�̉摜���\��
                turnEffectImage.SetActive(false);
                break;

            case "Operation_2":

                //��������Q�ő�����@�摜��\��
                keyBordImage.SetActive(true);
                break;

            case "Operation_3":
                break;

            case "Operation_4":

                //������@�摜���\���ɂ��ēG�^�[�Q�b�g��\��
                keyBordImage.SetActive(false);
                enemyTargetImage.SetActive(true);

                break;

            case "Status_1":

                //�G�^�[�Q�b�g���\��
                enemyTargetImage.SetActive(false);
                break;

            case "Status_2":

                //�X�e�[�^�X�Љ�̉摜��\��
                statusCharaImage.SetActive(true);
                break;

            case "Status_3":

                break;
            case "Status_4":

                ////�X�e�[�^�X�Љ�̉摜���\���ɂ��A�X�e�[�^�X�{�^����\��
                statusCharaImage.SetActive(false);
                statusButtonImage.SetActive(true);
                break;

            case "End":

                //�S�Ẳ摜��\��
                statusButtonImage.SetActive(false);
                statusCharaImage.SetActive(false);
                break;

            default:
                break;
        }
    }
}
