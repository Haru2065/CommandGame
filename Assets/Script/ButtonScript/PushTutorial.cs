using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �`���[�g���A���V�[���Ɉړ�����{�^���X�N���v�g
/// </summary>
public class PushTutorial : MonoBehaviour
{
    /// <summary>
    /// �`���[�g���A���P�̃{�^���������ꂽ��`���[�g���A���P�̃V�[�������[�h
    /// </summary>
    public void OnTutorial1Button()
    {
        SceneManager.LoadScene("Tutorial1");
    }

    /// <summary>
    /// �`���[�g���A���Q�̃{�^���������ꂽ��`���[�g���A���Q�̃V�[�������[�h
    /// </summary>
    public void OnTutorial2Button()
    {
        SceneManager.LoadScene("Tutorial2");
    }
}
