using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Stage2Tutorial
{
    //チュートリアルのID
    public string id;

    //チュートリアルのテキスト
    public string text;
}

[Serializable]
public class Stage2TutorialData
{
    //チュートリアルをリストで管理（JSONデータを変換するために受け取るリスト)
    public List<Stage2Tutorial> Stage2Tutorials;
}

public class Stage2TutorialManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ステージ２のチュートリアルで表示するテキスト")]
    private TextMeshProUGUI stage2TutorialText;

    [SerializeField]
    [Tooltip("各キャラの行動制限説明画像")]
    private GameObject actionCountImage;

    [SerializeField]
    [Tooltip("ステータス状況を説明する画像")]
    private GameObject statusWindowImage;

    //JSONから読み込んだデータをいれるリスト（C#形式に変換したデータを受け取るリスト）
    private List<Stage2Tutorial> stage2ConvertedList;

    //表示インデックス（今どのチュートリアルを表示しているかを記録する）
    private int currentIndex = 0;



    // Start is called before the first frame update
    void Start()
    {
        //各説明画像を非表示 
        actionCountImage.SetActive(false);
        statusWindowImage.SetActive(false);

        //ステージ2のチュートリアルテキストをJsonからロード
        Stage2Tutorial_LoadJson();

        //ステージ2のチュートリアルテキストを表示準備
        StartStage2Tutorial();
    }

    /// <summary>
    /// ステージ2のチュートリアルJsonファイルをロードするメソッド
    /// </summary>
    void Stage2Tutorial_LoadJson()
    {
        //アセットフォルダのResourcesにあるStage2_TutorialText.jsonを読み込む
        TextAsset jsonFile = Resources.Load<TextAsset>("Stage2_TutorialText");

        //Jsonファイルがあれば変換
        if (jsonFile != null)
        {
            Debug.Log($"JSONの内容: {jsonFile.text}");

            //ステージ2のチュートリアルデータ型に変換(JSONファイルの中身を文字列で取得）
            Stage2TutorialData data = JsonUtility.FromJson<Stage2TutorialData>(jsonFile.text);

            // JSONをC#に変換したデータをステージ2の受け取りリストにいれる
            stage2ConvertedList = data.Stage2Tutorials;


            //Jsonのデータがあれば変換完了と表示
            if (data != null && data.Stage2Tutorials != null)
            {
                Debug.Log($"変換完了！チュートリアルデータ数: {stage2ConvertedList.Count}");
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

    /// <summary>
    /// ステージ2のチュートリアルを表示準備するメソッド
    /// </summary>
    void StartStage2Tutorial()
    {
        //もし読み込んだリストが空だったら警告文を出す
        if (stage2ConvertedList.Count == 0)
        {
            Debug.LogWarning("チュートリアルが読み込まれていません!");
            return;
        }

        //チュートリアルを表示(最初はid;Welcomeから表示
        ShowTutorial(stage2ConvertedList[currentIndex]);
    }

    /// <summary>
    /// クリックしたら、次のテキストを表示するメソッド
    /// </summary>
    public void OnClickNext()
    {
        //表示インデックスを次に進める
        currentIndex++;

        //表示インデックスが最後まで表示するまで表示
        if (currentIndex < stage2ConvertedList.Count)
        {
            //表示インデックスの数に入っているJsonテキストを表示
            ShowTutorial(stage2ConvertedList[currentIndex]);
        }

        //最後まで表示したら終了
        else
        {
            Debug.Log("チュートリアル終了!");

            //最後まで表示したら自動敵にステージセレクト画面に移動
            SceneManager.LoadScene("StageSelect");
            
        }
    }

    /// <summary>
    /// JSONのチュートリアルを表示コントロールするメソッド
    /// </summary>
    /// <param name="tutorials">Jsonのチュートリアル</param>
    void ShowTutorial(Stage2Tutorial tutorials)
    {
        //Jsonに書かれたテキスト部分をUIとして表示
        stage2TutorialText.text = tutorials.text;

        //チュートリアルのidを調べてそれに応じてテキストとUIを表示
        switch (tutorials.id)
        {
            case "welcome":

            case "DebuffEnemy_1":

                break ;

            case "DebuffEmemy_2":

                break ;

            case "DebuffEnemy_3":

                ////ステータス状況を説明する画像を表示
                statusWindowImage.SetActive(true);
                break ;

            case "DebuffEnemy_4":

                //ステータス状況を説明する画像を非表示
                statusWindowImage.SetActive(false);
                break ;

            case "DebuffEnemy_5":
                break ;

            case "End":

                //各キャラの行動制限説明画像
                actionCountImage.SetActive(true);
                break ;

            default:
                break ;
        }
    }

    /// <summary>
    /// マウスクリック処理
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        //マウス左クリックで次のテキスト表示
        if (Input.GetMouseButtonDown(0))
        {
            //次のテキスト表示
            OnClickNext();        
        }
    }
}

