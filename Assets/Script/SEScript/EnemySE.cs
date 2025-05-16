using UnityEngine;

/// <summary>
/// 敵の効果音
/// </summary>
public class EnemySE : MonoBehaviour
{
    //敵の効果音をインスタンス化用
    private static EnemySE instance;

    /// <summary>
    /// 敵の効果音インスタンスのゲッター
    /// </summary>
    public static EnemySE Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("効果音用のオーディオソース")]
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip("スライムの攻撃音")]
    private AudioClip slimeAttackSE;

    [SerializeField]
    [Tooltip("スケルトンの攻撃音")]
    private AudioClip skeletonAttackSE;

    /// <summary>
    /// 敵の効果音マネージャーをインスタンス化
    /// </summary>
    void Awake()
    {
        //インスタンスがなければインスタンス
        if (instance == null)
        {

            instance = this;
        }
        //あればオブジェクトを消去
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// スライムの攻撃音を再生メソッド
    /// </summary>
    public void Play_slimeAttackSE()
    {
        //スライムの攻撃音再生
        audioSource.PlayOneShot(slimeAttackSE);
    }

    /// <summary>
    /// スケルトンの攻撃音を再生メソッド
    /// </summary>
    public void Play_SkeletonAttackSE()
    {
        //スケルトンの攻撃音再生
        audioSource.PlayOneShot(skeletonAttackSE);
    }
}
