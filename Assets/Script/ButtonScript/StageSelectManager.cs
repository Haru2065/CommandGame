using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージの解放状況に応じてボタンを表示非表示するスクリプト
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    

    [SerializeField]
    [Tooltip("ステージボタン")]
    private GameObject stage3Button;

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
            LoadStageData(saveData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //もしステージ3が解放されていたらボタンを表示
        if (BaseBattleManager.Instance != null && BaseBattleManager.Instance.IsUnlockStage3)
        {
            stage3Button.SetActive(true);
        }
        else
        {
            stage3Button.SetActive(false);
        }
    }

    /// <summary>
    /// ステージデータをロードするメソッド
    /// </summary>
    /// <param name="data">Jsonに保存されているステージデータ</param>
    public static void LoadStageData(StageSaveData data)
    {
        //ステージ3解放のフラグデータをステージセーブデータからロード
        BaseBattleManager.Instance.IsUnlockStage3 = data.Stage3UnLock_SaveData;
    }
}
