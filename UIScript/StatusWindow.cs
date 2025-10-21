using TMPro;
using UnityEngine;

/// <summary>
/// キャラのステータスウィンドウ
/// </summary>
public class StatusWindow : MonoBehaviour
{
    //キャラのステータスウィンドウのインスタンス化用
    private static StatusWindow instance;

    /// <summary>
    /// インスタンスのゲッター
    /// </summary>
    public static StatusWindow Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("アタッカー")]
    private BasePlayerStatus attacker;

    [SerializeField]
    [Tooltip("バッファー")]
    private BasePlayerStatus buffer;

    [SerializeField]
    [Tooltip("ヒーラー")]
    private BasePlayerStatus healer;


    [SerializeField]
    [Tooltip("アタッカーの攻撃力数値text")]
    private TextMeshProUGUI attackerAttackpowerText;

    [SerializeField]
    [Tooltip("アタッカーのデバフカウントtext")]
    private TextMeshProUGUI attacker_DebuffCount_Text;

    [SerializeField]
    [Tooltip("アタッカーの特殊デバフカウントtext")]
    private TextMeshProUGUI attacker_SpecialDebuffCount_Text;

    [SerializeField]
    [Tooltip("バッファーの攻撃力数値text")]
    private TextMeshProUGUI bufferAttackPowerText;

    [SerializeField]
    [Tooltip("バッファーのデバフカウントtext")]
    private TextMeshProUGUI buffer_DebuffCount_Text;

    [SerializeField]
    [Tooltip("バッファーの特殊デバフカウントtext")]
    private TextMeshProUGUI buffer_SpecialDebuffCount_Text;

    [SerializeField]
    [Tooltip("ヒーラーの攻撃力数値text")]
    private TextMeshProUGUI healerAttackPowerText;

    [SerializeField]
    [Tooltip("ヒーラーのデバフカウントtext")]
    private TextMeshProUGUI healer_DebuffCount_Text;

    [SerializeField]
    [Tooltip("ヒーラーの特殊デバフカウントtext")]
    private TextMeshProUGUI healer_SpecialDebuffCount_Text;

    /// <summary>
    /// キャラのステータスウィンドウをインスタンス化
    /// </summary>
    public void Awake()
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

    /// <summary>
    /// ステータスウィンドウを開くメソッド
    /// </summary>
    public void OpenStatusWindow()
    {
        //ステータスウィンドウを開く
        UIManager.Instance.StatusWindow.SetActive(true);

        //ステータス更新テキスト表示
        UPdateStatusWindow();
    }

    /// <summary>
    /// ステータス更新してテキストを表示するメソッド
    /// </summary>
    private void UPdateStatusWindow()
    {
        //アタッカーの攻撃力とデバフと特殊デバフカウントを数値として表示
        attackerAttackpowerText.text = $"{attacker.AttackPower}";
        attacker_DebuffCount_Text.text = $"{attacker.DebuffCount}";
        attacker_SpecialDebuffCount_Text.text = $"{attacker.SpecialDebuffCount}";

        //バッファーの攻撃力とデバフと特殊デバフカウントを数値として表示
        bufferAttackPowerText.text = $"{buffer.AttackPower}";
        buffer_DebuffCount_Text.text = $"{buffer.DebuffCount}";
        buffer_SpecialDebuffCount_Text.text = $"{buffer.SpecialDebuffCount}";

        //ヒーラーの攻撃力とデバフと特殊デバフカウントを数値として表示
        healerAttackPowerText.text = $"{healer.AttackPower}";
        healer_DebuffCount_Text.text = $"{healer.DebuffCount}";
        healer_SpecialDebuffCount_Text.text = $"{healer.SpecialDebuffCount}";
    }
}
