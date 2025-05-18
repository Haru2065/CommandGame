using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージセレクトボタン
/// </summary>
public class PushStageSelectButton : MonoBehaviour
{
    /// <summary>
    /// ボタンが押されたら、ステージセレクトシーンをロード
    /// </summary>
    public void PushStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
