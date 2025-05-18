using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 再挑戦ボタン
/// </summary>
public class PushReTryButton : MonoBehaviour
{
    /// <summary>
    /// ボタンが押されたらステージ１をシーンを再リロード
    /// </summary>
    public void OnButtonReLoadStage1()
    {
        SceneManager.LoadScene("Stage1");
    }
}
