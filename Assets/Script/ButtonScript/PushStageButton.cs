using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �X�e�[�W�{�^����I������X�N���v�g(�o�g���X�e�[�W�̃{�^���̂݁j
/// </summary>
public class PushStageButton : MonoBehaviour
{
    /// <summary>
    /// �X�e�[�W1�̃{�^���������ꂽ��X�e�[�W1�̃V�[�������[�h
    /// </summary>
    public void PushStage1Button()
    {
        SceneManager.LoadScene("Stage1");
    }

    /// <summary>
    /// �X�e�[�W2�̃{�^���������ꂽ��X�e�[�W�Q�̃V�[�������[�h
    /// </summary>
    public void PushStage2Button()
    {
        SceneManager.LoadScene("Stage2");
    }
}
