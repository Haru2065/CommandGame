using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �X�e�[�W�Z���N�g�{�^��
/// </summary>
public class PushStageSelectButton : MonoBehaviour
{
    /// <summary>
    /// �{�^���������ꂽ��A�X�e�[�W�Z���N�g�V�[�������[�h
    /// </summary>
    public void PushStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
