using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2TutorialManager : TutorialManager
{
    [SerializeField]
    [Tooltip("各キャラの行動制限説明画像")]
    private GameObject actionCountImage;

    [SerializeField]
    [Tooltip("ステータス状況を説明する画像")]
    private GameObject statusWindowImage;

    // Start is called before the first frame update
    protected override void Start()
    {
        //各説明画像を非表示 
        actionCountImage.SetActive(false);
        statusWindowImage.SetActive(false);

        base.Start();
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
            case "welcome":

            case "DebuffEnemy_1":

                break;

            case "DebuffEmemy_2":

                break;

            case "DebuffEnemy_3":

                ////ステータス状況を説明する画像を表示
                statusWindowImage.SetActive(true);
                break;

            case "DebuffEnemy_4":

                //ステータス状況を説明する画像を非表示
                statusWindowImage.SetActive(false);
                break;

            case "DebuffEnemy_5":
                break;

            case "End":

                //各キャラの行動制限説明画像
                actionCountImage.SetActive(true);
                break;

            default:
                break;
        }
    }
}

