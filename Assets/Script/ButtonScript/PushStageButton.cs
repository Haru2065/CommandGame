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
    /// ステージ2のボタンが押されたらステージ２のシーンをロード
    /// </summary>
    public void PushStage2Button()
    {
        SceneManager.LoadScene("Stage2");
    }
}
