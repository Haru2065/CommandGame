using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Jsonファイルのid情報とtext情報
/// </summary>
[Serializable]
public class TutorialText
{
    //チュートリアルのID
    public string id;

    //チュートリアルのテキスト
    public string text;
}

/// <summary>
/// AddressablesでロードしたJsonをC#に変換した後に表示するスクリプト
/// </summary>
public abstract class TutorialManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Jsonロードするスクリプト")]
    private JsonLoadr jsonLoder;

    [SerializeField]
    [Tooltip("読み込みたいJsonを入力")]
    private string ScenarioJsonKey;

    [SerializeField]
    [Tooltip("テキストウィンドウ")]
    protected GameObject TextWindow;

    [SerializeField]
    [Tooltip("Jsonファイルを表示するUGUI")]
    protected TextMeshProUGUI TutorialTextUGUI;

    //変換したテキストを順番に表示するリスト
    protected List<TutorialText> tutorialList;

    //表示インデックス
    protected int currentIndex;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Json読み込みスクリプトからロード開始
        //受け取ったJsonをパース
        StartCoroutine(jsonLoder.LoadJsonText(json =>
        {
            //Json全体をDictionaryに変換
            var dict = JsonConvert.DeserializeObject<Dictionary<string, List<TutorialText>>>(json);

            //jsonキーがあれば表示リストに代入
            if (dict.TryGetValue(ScenarioJsonKey, out tutorialList))
            {
                currentIndex = 0;

                //テキストウィンドウを表示
                TextWindow.SetActive(true);

                //チュートリアルテキストを表示準備
                StartTutorial();
            }
            else
            {
                Debug.LogWarning($"指定したキーが見つかりませんでした。{ScenarioJsonKey}");
            }
        }));
    }

    /// <summary>
    /// チュートリアルを表示準備するメソッド
    /// </summary>
    protected void StartTutorial()
    {
        //もし読み込んだリストが空だったら警告文を出す
        if (tutorialList.Count == 0)
        {
            Debug.LogWarning("チュートリアルテキストが読み込まれていません!");
            return;
        }

        //チュートリアルを表示(最初はid;Welcomeから表示
        ShowTutorialText(tutorialList[currentIndex]);
    }

    /// <summary>
    /// テキストクリック操作
    /// </summary>
    protected void Update()
    {
        //マウス左クリックされたらテキストの表示を進める
        if (Input.GetMouseButtonDown(0))
        {
            OnClickNextText();
        }
    }

    /// <summary>
    /// クリックされたらテキストの表示を進めるメソッド
    /// </summary>
    protected virtual void OnClickNextText()
    {
        //表示インデックスを次に進める
        currentIndex++;

        //表示インデックスが最後まで表示するまで表示
        if (currentIndex < tutorialList.Count)
        {
            //表示インデックスの数に入っているJsonテキストを表示
            ShowTutorialText(tutorialList[currentIndex]);
        }
    }

    /// <summary>
    /// 変換したJsonシナリオを表示
    /// </summary>
    public abstract void ShowTutorialText(TutorialText texts);
}
