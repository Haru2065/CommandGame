using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2TutorialManager : TutorialManager
{
    [SerializeField]
    [Tooltip("�e�L�����̍s�����������摜")]
    private GameObject actionCountImage;

    [SerializeField]
    [Tooltip("�X�e�[�^�X�󋵂��������摜")]
    private GameObject statusWindowImage;

    // Start is called before the first frame update
    protected override void Start()
    {
        //�e�����摜���\�� 
        actionCountImage.SetActive(false);
        statusWindowImage.SetActive(false);

        base.Start();
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
            case "welcome":

            case "DebuffEnemy_1":

                break;

            case "DebuffEmemy_2":

                break;

            case "DebuffEnemy_3":

                ////�X�e�[�^�X�󋵂��������摜��\��
                statusWindowImage.SetActive(true);
                break;

            case "DebuffEnemy_4":

                //�X�e�[�^�X�󋵂��������摜���\��
                statusWindowImage.SetActive(false);
                break;

            case "DebuffEnemy_5":
                break;

            case "End":

                //�e�L�����̍s�����������摜
                actionCountImage.SetActive(true);
                break;

            default:
                break;
        }
    }
}

