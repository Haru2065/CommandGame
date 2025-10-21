using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// バフする対象者を選択するスクリプト
/// </summary>
public class BuffTargetWindow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("バフターゲットボタンアタッカー")]
    private Button buffTargetAttackerButton;

    [SerializeField]
    [Tooltip("バフターゲットボタンバッファー")]
    private Button buffTargetBufferButton;

    [SerializeField]
    [Tooltip("バフターゲットボタンヒーラー")]
    private Button buffTargetHealerButton;

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
    [Tooltip("バフターゲットウィンドウ")]
    private GameObject buffTargetWindow;


    // Start is called before the first frame update
    void Start()
    {
        //最初は各ボタンを非表示
        buffTargetAttackerButton.gameObject.SetActive(false);
        buffTargetBufferButton.gameObject.SetActive(false);
        buffTargetHealerButton.gameObject.SetActive(false);

        //選択ウィンドウを非表示
        buffTargetWindow.SetActive(false);
    }

    /// <summary>
    /// バフ対象者を選択する画面を表示、ボタンを押すメソッド
    /// </summary>
    public void ShowBuffTargetWindow()
    {
        //選択ウィンドウを表示
        buffTargetWindow.SetActive(true);

        //各対象者ボタンを各キャラが生存していたら表示する
        buffTargetAttackerButton.gameObject.SetActive(attacker.IsAlive);
        buffTargetBufferButton.gameObject.SetActive(buffer.IsAlive);
        buffTargetHealerButton.gameObject.SetActive(buffer.IsAlive);



        //各キャラの対象ボタンが押されたら、その対象者にバフを実行
        buffTargetAttackerButton.onClick.AddListener(() => buffer.OnBuff(attacker));
        buffTargetBufferButton.onClick.AddListener(() => buffer.OnBuff(buffer));
        buffTargetHealerButton.onClick.AddListener(() => buffer.OnBuff(healer));
    }

    /// <summary>
    /// 選択ウィンドウとボタンを非表示にするメソッド
    /// </summary>
    public void HideBuffTargetWindow()
    {
        //選択ウィンドウを非表示
        buffTargetWindow.SetActive(false);

        // リスナーの重複を防ぐために削除
        buffTargetAttackerButton.onClick.RemoveAllListeners();
        buffTargetBufferButton.onClick.RemoveAllListeners();
        buffTargetHealerButton.onClick.RemoveAllListeners();

        //各対象者ボタンを非表示
        buffTargetAttackerButton.gameObject.SetActive(false);
        buffTargetBufferButton.gameObject.SetActive(false);
        buffTargetHealerButton.gameObject.SetActive(false);
    }
}
