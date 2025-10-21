using JetBrains.Annotations;
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

    [SerializeField]
    [Tooltip("ドラゴンの鳴き声音")]
    private AudioClip dragonRoarSE;

    [SerializeField]
    [Tooltip("ドラゴンの単体攻撃音")]
    private AudioClip dragonSingleAttackSE;

    [SerializeField]
    [Tooltip("ドラゴンのブレス攻撃音")]
    private AudioClip dragonBreathSE;

    [SerializeField]
    [Tooltip("ドラゴンの必殺攻撃音")]
    private AudioClip dragonSpecialAttackSE;


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

    /// <summary>
    /// ドラゴンの鳴き声再生メソッド
    /// </summary>
    public void Play_DragonRourSE()
    {
        //ドラゴンの鳴き声を再生
        audioSource.PlayOneShot(dragonRoarSE);
    }

    /// <summary>
    /// ドラゴンの単体攻撃音再生メソッド
    /// </summary>
    public void Play_DragonSingleAttackSE()
    {
        //ドラゴンの単体攻撃音を再生
        audioSource.PlayOneShot(dragonSingleAttackSE);
    }

    /// <summary>
    /// ドラゴンのブレス攻撃再生メソッド
    /// </summary>
    public void Play_DragonBreathSE()
    {
        //ドラゴンのブレス攻撃音を再生
        audioSource.PlayOneShot(dragonBreathSE);
    }

    /// <summary>
    /// ドラゴンの必殺攻撃再生メソッド
    /// </summary>
    public void Play_DragonSpecialAttackSE()
    {
        //ドラゴンの必殺攻撃音を再生
        audioSource.PlayOneShot(dragonSpecialAttackSE); 
    }
}
