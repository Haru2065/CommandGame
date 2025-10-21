using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージボタンを選択するスクリプト(バトルステージのボタンのみ）
/// </summary>
public class PushStageButton : MonoBehaviour
{
    /// <summary>
    /// ステージ1のボタンが押されたらステージ1のシーンをロード
    /// </summary>
    public void PushStage1Button()
    {
        SceneManager.LoadScene("Stage1");
    }

    /// <summary>
    /// ステージ2のボタンが押されたらステージ2のシーンをロード
    /// </summary>
    public void PushStage2Button()
    {
        SceneManager.LoadScene("Stage2");
    }

    /// <summary>
    /// ステージ3のボタンが押されたらステージ3のシーンをロード
    /// </summary>
    public void PushStage3Button()
    {
        SceneManager.LoadScene("Stage3");
    }
}
