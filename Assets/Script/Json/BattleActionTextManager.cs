using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class BattleText
{
    //JSONファイルのテキスト
    public string text;

    //JSONファイルのID
    public string id;
}

/// <summary>
/// バトル時の行動通知のスクリプト
/// JSONから変換
/// </summary>
public class BattleActionTextManager : MonoBehaviour
{
    //バトル時の行動通知のスクリプトをインスタンス化用
    private static BattleActionTextManager instance;

    /// <summary>
    /// インスタンスのゲッター
    /// </summary>
    public static BattleActionTextManager Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("Jsonロードするスクリプト")]
    private JsonLoadr jsonLoadr;

    [SerializeField]
    [Tooltip("テキストウィンドウ")]
    private GameObject textWindow;

    /// <summary>
    /// テキストウィンドウのゲッター
    /// </summary>
    public GameObject TextWindow
    {
        get => textWindow;
    }

    [SerializeField]
    [Tooltip("バトルでキャラの状況を表示するテキスト")]
    private TextMeshProUGUI characterBattleActionText;

    //JSONから読み込んだデータをいれるリスト（C#形式に変換したデータを受け取るリスト）
    private List<BattleText> battleTextConvertedList;

    /// <summary>
    /// インスタンス化
    /// </summary>
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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(jsonLoadr.LoadJsonText(json =>
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, List<BattleText>>>(json);

            if (dict != null && dict.ContainsKey("BattleTexts"))
            {
                battleTextConvertedList = dict["BattleTexts"];
                Debug.Log($"変換完了！バトルテキストデータ数: {battleTextConvertedList.Count}");
            }
            else
            {
                Debug.LogWarning("変換失敗！データが空かキーが間違っています");
            }
        }));

        //テキストウィンドウを非表示
        textWindow.SetActive(false);
    }

    public void ShowBattleActionText(string id)
    {
        //JSONから読み込んだデータをいれるリストからidを取得
        BattleText battleText = battleTextConvertedList.Find(text => text.id == id);

        if (battleText != null)
        {
            //IDをもとにテキストを表示しテキストウィンドウも表示
            characterBattleActionText.text = battleText.text;
            textWindow.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"指定された ID ({id}) に対応するテキストが見つかりません");
        }
    }

    /// <summary>
    /// テキストウィンドウを非表示にするメソッド
    /// </summary>
    public void TextDelayHide()
    {
        TextWindow.SetActive(false);
    }
}
