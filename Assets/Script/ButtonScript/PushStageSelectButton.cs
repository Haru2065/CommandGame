using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �X�e�[�W�Z���N�g�{�^���X�N���v�g
/// </summary>
public class PushStageSelectButton : MonoBehaviour
{
    /// <summary>
    /// �{�^���������ꂽ��X�e�[�W�Z���N�g�V�[�������[�h
    /// </summary>
    public void StageSelectButton()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
