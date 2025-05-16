using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Stage1Tutorial
{
    //チュートリアルのID
    public string id;
    
    //チュートリアルのテキスト
    public string text;
}

[Serializable]
public class Stage1TutorialData
{
    //チュートリアルをリストで管理(JSONデータを変換するために受け取るリスト）
    public List<Stage1Tutorial> Stage1Tutorials;
}

public class Stage1TutorialManager : MonoBehaviour
{

    [SerializeField]
    [Tooltip("ステージ１のチュートリアルで表示するテキスト")]
    private TextMeshProUGUI stage1TutorialText;

    [SerializeField]
    [Tooltip("ターンのエフェクトを表示する画像")]
    private GameObject turnEffectImage;

    [SerializeField]
    [Tooltip("敵のターゲットを選んでいる画像")]
    private GameObject enemyTargetImage;

    [SerializeField]
    [Tooltip("プレイヤーの操作キーの画像")]
    private GameObject keyBordImage;

    [SerializeField]
    [Tooltip("キャラのステータス紹介の画像")]
    private GameObject statusCharaImage;

    [SerializeField]
    [Tooltip("プレイヤーのステータスを開くボタンの画像")]
    private GameObject statusButtonImage;

    //JSONから読み込んだデータをいれるリスト（C#形式に変換したデータを受け取るリスト）
    private List<Stage1Tutorial> stage1ConvertedList;

    //表示インデックス（今どのチュートリアルを表示しているかを記録する）
    private int CurrentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        //全ての説明画像を非表示
        HideStartImage();

        //ステージ１のチュートリアルテキストをJsonからロード
        Stage1Tutorial_LoadJson();

        //ステージ１のチュートリアルテキストを表示準備
        StartStage1Tutorial();
    }
    
    /// <summary>
    /// 全ての説明画像を非表示にするメソッド
    /// </summary>
    void HideStartImage()
    {
        turnEffectImage.SetActive(false);
        enemyTargetImage.SetActive(false);
        keyBordImage.SetActive(false);

        statusCharaImage.SetActive(false);
        statusButtonImage.SetActive(false);
    }

    /// <summary>
    /// ステージ１のチュートリアルJsonファイルをロードするメソッド
    /// </summary>
    void Stage1Tutorial_LoadJson()
    {
        //アセットフォルダのResourcesにあるStage1_TutorialText.jsonを読み込む
        TextAsset jsonFile = Resources.Load<TextAsset>("Stage1_TutorialText");

        //Jsonファイルがあれば変換
        if (jsonFile != null)
        {
            Debug.Log($"JSONの内容: {jsonFile.text}");

            //ステージ1のチュートリアルデータ型に変換(JSONファイルの中身を文字列で取得）
            Stage1TutorialData data = JsonUtility.FromJson<Stage1TutorialData>(jsonFile.text);

            // JSONをC#に変換したデータをステージ1の受け取りリストにいれる
            stage1ConvertedList = data.Stage1Tutorials;

            //Jsonのデータがあれば変換完了と表示
            if (data != null && data.Stage1Tutorials != null)
            {
                Debug.Log($"変換完了！チュートリアルデータ数: {stage1ConvertedList.Count}");
            }
            else
            {
                Debug.LogWarning("変換失敗！データが空かキーが間違っています");
            }
        }
        else
        {
            Debug.Log("JSONファイルが見つかりません!");
        }
    }

    /// <summary>
    /// ステージ１のチュートリアルを表示準備するメソッド
    /// </summary>
    void StartStage1Tutorial()
    {
        //もし読み込んだリストが空だったら警告文を出す
        if(stage1ConvertedList.Count == 0)
        {
            Debug.LogWarning("チュートリアルテキストが読み込まれていません!");
            return;
        }

        //チュートリアルを表示(最初はid;Welcomeから表示
        ShowTutorial(stage1ConvertedList[CurrentIndex]);
    }

    /// <summary>
    /// クリックしたら、次のテキストを表示するメソッド
    /// </summary>
    void OnClickNext()
    {
        //表示インデックスを次に進める
        CurrentIndex++;

        //表示インデックスが最後まで表示するまで表示
        if(CurrentIndex < stage1ConvertedList.Count)
        {
            //表示インデックスの数に入っているJsonテキストを表示
            ShowTutorial(stage1ConvertedList[CurrentIndex]);
        }
        //最後まで表示したら終了
        else
        {
            //最後まで表示したら自動敵にステージセレクト画面に移動
            SceneManager.LoadScene("StageSelect");
        }
    }

    /// <summary>
    /// JSONのチュートリアルを表示コントロールするメソッド
    /// </summary>
    /// <param name="tutorials">Jsonのチュートリアル</param>
    void ShowTutorial(Stage1Tutorial tutorials)
    {
        //Jsonに書かれたテキスト部分をUIとして表示
        stage1TutorialText.text = tutorials.text;

        //チュートリアルのidを調べてそれに応じてテキストとUIを表示
        switch (tutorials.id)
        {
            case "Welocome":
                break;

            case "Battle_1":

                //バトル説明１の時にターンエフェクトの画像を表示
                turnEffectImage.SetActive(true);
                break;

            case "Battle_2":
                break ;

            case "Battle_3":
                break ;

            case "Operation_1":

                //ここでターンエフェクトの画像を非表示
                turnEffectImage.SetActive(false);
                break ;

            case "Operation_2":

                //操作説明２で操作方法画像を表示
                keyBordImage.SetActive(true);
                break;

            case "Operation_3":
                break;

            case "Operation_4":

                //操作方法画像を非表示にして敵ターゲットを表示
                keyBordImage.SetActive(false);
                enemyTargetImage.SetActive(true);

                break ;

            case "Status_1":
                
                //敵ターゲットを非表示
                enemyTargetImage.SetActive(false);
                break;

            case "Status_2":

                //ステータス紹介の画像を表示
                statusCharaImage.SetActive(true);
                break;

            case "Status_3":

                break;
            case "Status_4":

                ////ステータス紹介の画像を非表示にし、ステータスボタンを表示
                statusCharaImage.SetActive(false);
                statusButtonImage.SetActive(true);
                break;

            case "End":

                //全ての画像非表示
                statusButtonImage.SetActive(false);
                statusCharaImage.SetActive(false);
                break;

            default: 
                break;

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
