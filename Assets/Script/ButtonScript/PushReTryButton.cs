using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �Ē���{�^��
/// </summary>
public class PushReTryButton : MonoBehaviour
{
    /// <summary>
    /// �{�^���������ꂽ��X�e�[�W1�V�[�����ă����[�h
    /// </summary>
    public void OnButtonReLoadStage1()
    {
        SceneManager.LoadScene("Stage1");
    }

    /// <summary>
    /// �{�^���������ꂽ��X�e�[�W2�̃V�[�����ă����[�h
    /// </summary>
    public void OnButtonReLoadStage2()
    {
        SceneManager.LoadScene("Stage2");
    }

    /// <summary>
    /// �{�^���������ꂽ��X�e�[�W3�̃V�[�����ă����[�h
    /// </summary>
    public void OnButtonReLoadStage3()
    {
        SceneManager.LoadScene("Stage3");
    }
}
