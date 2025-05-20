using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1TutorialManager : TutorialManager
{
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

    // Start is called before the first frame update
    protected override void Start()
    {
        //全ての説明画像を非表示
        HideStartImage();

        base.Start();
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
    /// クリックされたらテキストの表示を進めるメソッド
    /// </summary>
    protected override void OnClickNextText()
    {
        //表示インデックスを次に進める
        currentIndex++;

        //表示インデックスが最後まで表示するまで表示
        if (currentIndex < tutorialList.Count)
        {
            //表示インデックスの数に入っているJsonテキストを表示
            ShowTutorialText(tutorialList[currentIndex]);
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
    /// <param name="scenarios">Jsonのチュートリアル</param>
    public override void ShowTutorialText(TutorialText scenarios)
    {
        //Jsonに書かれたテキスト部分をUIとして表示
        TutorialTextUGUI.text = scenarios.text;

        //チュートリアルのidを調べてそれに応じてテキストとUIを表示
        switch (scenarios.id)
        {
            case "Welocome":
                break;

            case "Battle_1":

                //バトル説明１の時にターンエフェクトの画像を表示
                turnEffectImage.SetActive(true);
                break;

            case "Battle_2":
                break;

            case "Battle_3":
                break;

            case "Operation_1":

                //ここでターンエフェクトの画像を非表示
                turnEffectImage.SetActive(false);
                break;

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

                break;

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
}
