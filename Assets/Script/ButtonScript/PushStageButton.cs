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
    /// �X�e�[�W2�̃{�^���������ꂽ��X�e�[�W2�̃V�[�������[�h
    /// </summary>
    public void PushStage2Button()
    {
        SceneManager.LoadScene("Stage2");
    }

    /// <summary>
    /// �X�e�[�W3�̃{�^���������ꂽ��X�e�[�W3�̃V�[�������[�h
    /// </summary>
    public void PushStage3Button()
    {
        SceneManager.LoadScene("Stage3");
    }
}
