using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタンを押すとプレイヤーのステータスウィンドウを開く
/// </summary>
public class PushOpenStatusWindow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ステータスウィンドウを開くボタン")]
    private Button statusWindowButton;

    public void Update()
    {
        //プレイヤーターン時のみボタンを押せるようにする
        //もしステージ１もしくはステージ２のインスタンスがありかつプレイヤーターンなら実行
        if (Stage1BattleSystem.Instance != null && Stage1BattleSystem.Instance.IsPlayerTurn ||
            Stage2BattleSystem.Instance != null && Stage2BattleSystem.Instance.IsPlayerTurn)
        {
            //ボタンが押せるようにする
            statusWindowButton.interactable = true;
        }

        else
        {
            //ボタンが押せないように透明にする
            statusWindowButton.interactable = false;
        }
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
