using UnityEngine;

/// <summary>
/// 終了ボタン
/// </summary>
public class PushExitButton : MonoBehaviour
{
    private bool isQuitGame = false;

    public bool IsQuitGame
    {
        get => isQuitGame;
    }

    /// <summary>
    /// ボタンが押されたらアプリケーションを終了する
    /// </summary>
    public void OnButton()
    {
        isQuitGame = true;

        //Unityを終了（ゲームを終了)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}
