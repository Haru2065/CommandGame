using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class BattleText
{
    //JSONファイルのテキスト
    public string text;

    //JSONファイルのID
    public string id;
}

[Serializable]
public class BattleTextData
{
    //バトル時の行動通知のテキストリスト
    public List<BattleText> BattleTexts;
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
    private TextMeshProUGUI character_BattleActionText;

    //JSONから読み込んだデータをいれるリスト（C#形式に変換したデータを受け取るリスト）
    private List<BattleText> battleText_ConvertedList;

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
        //JSONを読み込み変換する
        BattleText_LoadJsonText();

        //テキストウィンドウを非表示
        textWindow.SetActive(false);
    }

    /// <summary>
    /// Jsonを読み込み変換するメソッド
    /// </summary>
    public void BattleText_LoadJsonText()
    {
        //アセットフォルダのResourcesにあるBattleText.jsonを読み込む
        TextAsset jsonFile = Resources.Load<TextAsset>("BattleText");

        if (jsonFile != null)
        {
            Debug.Log($"JSONの内容: {jsonFile.text}");

            //JSONの中身を文字列で取得
            BattleTextData data = JsonUtility.FromJson<BattleTextData>(jsonFile.text);

            //JSONから読み込んだデータをいれるリストにいれる
            battleText_ConvertedList = data.BattleTexts;

            if (data != null && data.BattleTexts != null)
            {
                Debug.Log($"変換完了！バトルテキストデータ数: {battleText_ConvertedList.Count}");
            }
            else
            {
                Debug.LogWarning("変換失敗！データが空かキーが間違っています");
            }
        }

        else
        {
            Debug.LogError("Jsonファイルが見つかりません");
        }
    }

    public void ShowBattleActionText(string id)
    {
        //JSONから読み込んだデータをいれるリストからidを取得
        BattleText battleText = battleText_ConvertedList.Find(text => text.id == id);

        if (battleText != null)
        {
            //IDをもとにテキストを表示しテキストウィンドウも表示
            character_BattleActionText.text = battleText.text;
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
