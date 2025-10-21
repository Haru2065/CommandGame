using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGMをコントロールするスクリプト
/// </summary>
public class BGMControl : MonoBehaviour
{
    //BGMControlのインスタンス
    private static BGMControl instance;

    /// <summary>
    /// インスタンスのゲッター
    /// </summary>
    public static BGMControl Instance
    {
        get => instance;
    }

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

        [SerializeField]
    [Tooltip("BGM用オーディオソース")]
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip("ステージ3のBGM")]
    private AudioClip stage3BGM;

    /// <summary>
    /// ステージ3のBGMを再生するメソッド
    /// </summary>
    public void PlayStage3BGM()
    {
        //クリップをStage3BGMに設定
        audioSource.clip = stage3BGM;

        //オーディオソースをループし続ける
        audioSource.loop = true;

        //BGM再生
        audioSource.Play();
    }
}
