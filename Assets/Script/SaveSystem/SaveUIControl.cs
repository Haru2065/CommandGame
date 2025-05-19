using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// スタートボタンが押されたらセーブデータがあればセーブデータを選択するスクリプト
/// セーブデータがあればセーブデータを消去するボタンも表示
/// </summary>
public class SaveUIControl : MonoBehaviour
{
    [SerializeField]
    [Tooltip("セーブデータ消去ボタン")]
    private GameObject deleteButton;

    [SerializeField]
    [Tooltip("セーブデータ選択ボタン")]
    private GameObject SaveDataSelectButton;

    [SerializeField]
    [Tooltip("セーブデータ選択パネル")]
    private GameObject SaveSelectPanel;

    /// <summary>
    /// セーブデータがある時に表示するボタンを一度非表示で初期化
    /// </summary>
    void Start()
    {
        deleteButton.SetActive(false);
        SaveDataSelectButton.SetActive(false);
        SaveSelectPanel.SetActive(false);
    }

    private void Update()
    {
        //エスケープキーを押すとセーブデータ画面を閉じる
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //セーブデータ選択パネルが開いている時は閉じる
            if (SaveSelectPanel.activeSelf)
            {
                deleteButton.SetActive(false);
                SaveDataSelectButton.SetActive(false);
                SaveSelectPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// スタートボタンが押された時のメソッド
    /// </summary>
    public void PushStartButton()
    {
        //セーブデータがある時はセーブデータ選択ボタンを表示
        if (SaveManager.HasAnySaveData())
        {
            deleteButton.SetActive(true);

            SaveDataSelectButton.SetActive(true);
            SaveSelectPanel.SetActive(true);
        }
        //なければセーブデータ選択ボタンを非表示にして、シーンをロード
        else
        {
            deleteButton.SetActive(false);
            SaveDataSelectButton.SetActive(false);
            SaveSelectPanel.SetActive(false);

            SceneManager.LoadScene("StageSelect");
        }
    }
}
