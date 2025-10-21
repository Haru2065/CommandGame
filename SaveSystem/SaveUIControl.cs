using UnityEngine;

/// <summary>
/// スタートボタンが押されたらセーブデータがあればセーブデータを選択するスクリプト
/// セーブデータがあればセーブデータを消去するボタンも表示
/// </summary>
public class SaveUIControl : MonoBehaviour
{
    //SaveUIControlのインスタンス
    private static SaveUIControl instance;

    /// <summary>
    /// インスタンスのゲッター
    /// </summary>
    public static SaveUIControl Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("セーブデータ消去ボタン")]
    private GameObject deleteButton;

    /// <summary>
    /// インスタンス化インスタンスがなければオブジェクトを消去
    /// </summary>
    void Awake()
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

    void Start()
    {
        // セーブデータがある時はセーブデータ選択ボタンを表示
        if (SaveManager.HasAnySaveData())
        {
            deleteButton.SetActive(true);
        }
        //なければセーブデータ選択ボタンを非表示
        else
        {
            deleteButton.SetActive(false);
        }
    }

    /// <summary>
    /// 消去ボタンが押されたら消去ボタンを非表示にするメソッド
    /// </summary>
    public void HideSaveDeleteButton()
    {
        deleteButton.SetActive(false);
    }
}
