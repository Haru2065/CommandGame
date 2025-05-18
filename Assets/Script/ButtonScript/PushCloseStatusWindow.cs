using UnityEngine;

/// <summary>
/// ステータスウィンドウを閉じるボタン
/// </summary>
public class PushCloseStatusWindow : MonoBehaviour
{
    /// <summary>
    /// ボタンを押すとステータスウィンドウを閉じる
    /// </summary>
    public void OnButton()
    { 
        //UIマネージャーからステータスを非表示にする
        UIManager.Instance.CloseStatusWindow();
    }
}
