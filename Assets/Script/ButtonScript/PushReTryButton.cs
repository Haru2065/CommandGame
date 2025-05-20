using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 再挑戦ボタン
/// </summary>
public class PushReTryButton : MonoBehaviour
{
    /// <summary>
    /// ボタンが押されたらステージ1シーンを再リロード
    /// </summary>
    public void OnButtonReLoadStage1()
    {
        SceneManager.LoadScene("Stage1");
    }

    /// <summary>
    /// ボタンが押されたらステージ2のシーンを再リロード
    /// </summary>
    public void OnButtonReLoadStage2()
    {
        SceneManager.LoadScene("Stage2");
    }

    /// <summary>
    /// ボタンが押されたらステージ3のシーンを再リロード
    /// </summary>
    public void OnButtonReLoadStage3()
    {
        SceneManager.LoadScene("Stage3");
    }
}
