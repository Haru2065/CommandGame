using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �Ē���{�^��
/// </summary>
public class PushReTryButton : MonoBehaviour
{
    /// <summary>
    /// �{�^���������ꂽ��X�e�[�W�P���V�[�����ă����[�h
    /// </summary>
    public void OnButtonReLoadStage1()
    {
        SceneManager.LoadScene("Stage1");
    }
}
