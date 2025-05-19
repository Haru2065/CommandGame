using Newtonsoft.Json;
using System.IO;
using UnityEngine;

/// <summary>
/// ステージの解放状況に応じてボタンを表示非表示するスクリプト
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    

    [SerializeField]
    [Tooltip("ステージボタン")]
    private GameObject stage3Button;

    //ステージ3が解放されたかどうか
    private bool isStage3Unlocked;

    // Start is called before the first frame update
    void Start()
    {
        //ステージ3のボタンを非表示にする
        stage3Button.SetActive(false);

        //ステージのセーブデータを読み込む
        string path = Application.persistentDataPath + $"/StageSaveData.Json";

        //セーブデータが存在するならステージデータをロード
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StageSaveData saveData = JsonConvert.DeserializeObject<StageSaveData>(json);
            isStage3Unlocked = saveData.Stage3UnLock_SaveData;
        }
        else
        {
            //セーブデータが存在しない場合はステージ3は解放されていない
            isStage3Unlocked = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
