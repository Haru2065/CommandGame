using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// チュートリアルシーンに移動するボタンスクリプト
/// </summary>
public class PushTutorial : MonoBehaviour
{
    /// <summary>
    /// チュートリアル１のボタンが押されたらチュートリアル１のシーンをロード
    /// </summary>
    public void OnTutorial1Button()
    {
        SceneManager.LoadScene("Tutorial1");
    }

    /// <summary>
    /// チュートリアル２のボタンが押されたらチュートリアル２のシーンをロード
    /// </summary>
    public void OnTutorial2Button()
    {
        SceneManager.LoadScene("Tutorial2");
    }
}
