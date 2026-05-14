using Newtonsoft.Json;
using System.IO;
using UnityEngine;

/// <summary>
/// ステージの解放状況に応じてボタンを表示非表示するスクリプト
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ステージ2ボタン")]
    private GameObject stage2Button;

    [SerializeField]
    [Tooltip("チュートリアル2ボタン")]
    private GameObject tutorial2Button;

    [SerializeField]
    [Tooltip("ステージ3ボタン")]
    private GameObject stage3Button;

    //ステージ2が解放されたかどうか
    private bool isStage2Unlocked;

    //ステージ3が解放されたかどうか
    private bool isStage3Unlocked;

    // Start is called before the first frame update
    void Start()
    {
        //ステージ2,3のボタン、チュートリアル2ボタンを非表示にする
        tutorial2Button.SetActive(false);
        stage2Button.SetActive(false);
        stage3Button.SetActive(false);

        //ステージのセーブデータを読み込む
        string path = Application.persistentDataPath + $"/StageSaveData.Json";

        //セーブデータが存在するならステージデータをロード
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StageSaveData saveData = JsonConvert.DeserializeObject<StageSaveData>(json);

            isStage2Unlocked = saveData.Stage2UnLock_SaveData;
            isStage3Unlocked = saveData.Stage3UnLock_SaveData;
        }
        else
        {
            //セーブデータが存在しない場合はステージ2,3は解放されていない
            isStage2Unlocked = false;
            isStage3Unlocked = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //もしステージ2が解放されていたらボタンを表示
        if (isStage2Unlocked)
        {
            tutorial2Button.SetActive(true);
            stage2Button.SetActive(true);
        }
        else
        {
            tutorial2Button.SetActive(false);
            stage2Button.SetActive(false);
        }

        //もしステージ3が解放されていたらボタンを表示
        if (isStage3Unlocked)
        {
            stage3Button.SetActive(true);
        }
        else
        {
            stage3Button.SetActive(false);
        }
    }
}
