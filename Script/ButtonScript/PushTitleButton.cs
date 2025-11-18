using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトルボタン
/// </summary>
public class PushTitleButton : MonoBehaviour
{
    /// <summary>
    /// ボタンが押されたらタイトルシーンに移動
    /// </summary>
    public void OnButton()
    {
        SceneManager.LoadScene("Title");
    }
}
