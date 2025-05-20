using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージセレクトボタンスクリプト
/// </summary>
public class PushStageSelectButton : MonoBehaviour
{
    /// <summary>
    /// ボタンが押されたらステージセレクトシーンをロード
    /// </summary>
    public void StageSelectButton()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
