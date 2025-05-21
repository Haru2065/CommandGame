using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタンを押すとプレイヤーのステータスウィンドウを開く
/// </summary>
public class PushOpenStatusWindow : MonoBehaviour
{
    //ステータスウィンドウを開くスクリプトのインスタンス
    private static PushOpenStatusWindow instance;

    /// <summary>
    /// インスタンスのゲッター
    /// </summary>
    public static PushOpenStatusWindow Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("ステータスウィンドウを開くボタン")]
    private Button statusWindowButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //ボタンが押せるようにするメソッド
    public void CanPushStatusButton()
    {
        //ボタンが押せるようにする
        statusWindowButton.interactable = true;
    }

    //ボタンが押せないように透明にするメソッド
    public void TransparentStatusButton()
    {
        statusWindowButton.interactable = false;
    }
    

    /// <summary>
    /// ボタンを押すとステータスウィンドウを開く
    /// </summary>
    public void OnButton()
    {
        //ステータスウィンドウを表示する
        StatusWindow.Instance.OpenStatusWindow();

        //ステータスウィンドウを閉じるボタンを表示
        UIManager.Instance.CloseStatusWindowButton.SetActive(true);

        //ステータスウィンドウを開くボタンを非表示
        UIManager.Instance.OpenStatusWindowButton.SetActive(false);
    }
}
