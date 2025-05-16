using UnityEngine;

/// <summary>
/// �I���{�^��
/// </summary>
public class PushExitButton : MonoBehaviour
{
    private bool isQuitGame = false;

    public bool IsQuitGame
    {
        get => isQuitGame;
    }

    /// <summary>
    /// �{�^���������ꂽ��A�v���P�[�V�������I������
    /// </summary>
    public void OnButton()
    {
        isQuitGame = true;

        //Unity���I���i�Q�[�����I��)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}
