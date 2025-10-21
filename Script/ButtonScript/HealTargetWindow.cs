using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ヒーラーの対象者を選択するスクリプト
/// </summary>
public class HealTargetWindow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("回復ターゲットボタンアタッカー")]
    private Button healTargetAttackerButton;

    [SerializeField]
    [Tooltip("回復ターゲットボタンバッファー")]
    private Button healTargetBufferButton;

    [SerializeField]
    [Tooltip("回復ターゲットボタンヒーラー")]
    private Button healTargetHealerButton;

    [SerializeField]
    [Tooltip("アタッカー")]
    private Attacker attacker;

    [SerializeField]
    [Tooltip("バッファー")]
    private Buffer buffer;

    [SerializeField]
    [Tooltip("ヒーラー")]
    private Healer healer;

    [SerializeField]
    [Tooltip("回復ターゲットウィンドウ")]
    private GameObject healTargetWindow;

    // Start is called before the first frame update
    void Start()
    {
        //最初は各ボタンを非表示
        healTargetAttackerButton.gameObject.SetActive(false);
        healTargetBufferButton.gameObject.SetActive(false);
        healTargetHealerButton.gameObject.SetActive(false);

        //選択ウィンドウを非表示
        healTargetWindow.SetActive(false);
    }

    /// <summary>
    /// 回復対象者を選択ウィンドウを表示、ボタンを押すメソッド
    /// </summary>
    public void ShowHealTargetWindow()
    {
        //選択ウィンドウを表示
        healTargetWindow.SetActive(true);

        //各対象者ボタンを各キャラが生存していたら表示する
        healTargetAttackerButton.gameObject.SetActive(attacker.IsAlive);
        healTargetBufferButton.gameObject.SetActive(buffer.IsAlive);
        healTargetHealerButton.gameObject.SetActive(buffer.IsAlive);



        //各キャラの対象者ボタンが押されたら、その対象者に回復を実行
        healTargetAttackerButton.onClick.AddListener(() => healer.OnHeal(attacker));
        healTargetBufferButton.onClick.AddListener(() => healer.OnHeal(buffer));
        healTargetHealerButton.onClick.AddListener(() => healer.OnHeal(healer));

    }


    /// <summary>
    /// 選択ウィンドウとボタンを非表示にするメソッド
    /// </summary>
    public void HideHealTargetWindow()
    {
        //選択ウィンドウを非表示
        healTargetWindow.SetActive(false);

        //リスナーの重複を防ぐために削除
        healTargetAttackerButton.onClick.RemoveAllListeners();
        healTargetBufferButton.onClick.RemoveAllListeners();
        healTargetHealerButton.onClick.RemoveAllListeners();

        //各対象者ボタンを非表示
        healTargetAttackerButton.gameObject.SetActive(false);
        healTargetBufferButton.gameObject.SetActive(false);
        healTargetHealerButton.gameObject.SetActive(false);
    }
}
