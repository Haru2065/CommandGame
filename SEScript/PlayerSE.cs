using UnityEngine;

/// <summary>
/// プレイヤーの効果音マネージャー
/// </summary>
public class PlayerSE : MonoBehaviour
{
    //プレイヤーの効果音のインスタンス化用
    private static PlayerSE instance;

    /// <summary>
    /// プレイヤーの効果音のインスタンスのゲッター
    /// </summary>
    public static PlayerSE Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("効果音のオーディオソース")]
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip("アタッカーの通常攻撃音")]
    private AudioClip attackerNormalAttackSE;

    [SerializeField]
    [Tooltip("アタッカーのスキル攻撃音")]
    private AudioClip attackerSkillSE;

    [SerializeField]
    [Tooltip("アタッカーの必殺効果音")]
    private AudioClip attackerSpecialSE;

    [SerializeField]
    [Tooltip("バッファーの通常攻撃音")]
    private AudioClip bufferAttackSE;

    [SerializeField]
    [Tooltip("バッファーのステータス上昇音")]
    private AudioClip bufferUPStatusSE;

    [SerializeField]
    [Tooltip("バッファーのステータス特大上昇音")]
    private AudioClip bufferSpecialSE;

    [SerializeField]
    [Tooltip("ヒーラーの通常攻撃音")]
    private AudioClip healerAttackSE;

    [SerializeField]
    [Tooltip("ヒーラーの通常回復音")]
    private AudioClip healerNormalHealSE;

    [SerializeField]
    [Tooltip("ヒーラーの特大回復音")]
    private AudioClip healerSpecialSE;

    /// <summary>
    /// プレイヤーの効果音マネージャーをインスタンス化
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
    
    /// <summary>
    /// アタッカーの通常攻撃音再生メソッド
    /// </summary>
    public void Play_AttackerNormalAttackSE()
    {
        //通常攻撃音再生
        audioSource.PlayOneShot(attackerNormalAttackSE);
    }

    /// <summary>
    /// アタッカーのスキル音再生メソッド
    /// </summary>
    public void Play_AttackerSkillSE()
    {
        //スキル音再生
        audioSource.PlayOneShot(attackerSkillSE);

        
    }

    /// <summary>
    /// アタッカーの必殺音再生メソッド
    /// </summary>
    public void Play_AttackerSpecialSE()
    {
        //必殺音再生
        audioSource.PlayOneShot(attackerSpecialSE);
    }

    /// <summary>
    /// バッファーの通常攻撃音再生メソッド
    /// </summary>
    public void Play_BufferNormalAttackSE()
    {
        //通常攻撃音再生
        audioSource.PlayOneShot(bufferAttackSE);
    }

    /// <summary>
    /// バッファーのスキル音再生メソッド
    /// </summary>
    public void Play_BufferSkillSE()
    {
        //スキル音再生
        audioSource.PlayOneShot(bufferUPStatusSE);
    }

    /// <summary>
    /// バッファーの必殺音再生メソッド
    /// </summary>
    public void Play_bufferSpecialSE()
    {
        //必殺音再生
        audioSource.PlayOneShot(bufferSpecialSE);
    }

    /// <summary>
    /// ヒーラーの通常攻撃を再生
    /// </summary>
    public void Play_healerNormalAttackSE()
    {
        //通常攻撃音再生
        audioSource.PlayOneShot(healerAttackSE);
    }

    /// <summary>
    /// ヒーラーのスキル音再生
    /// </summary>
    public void Play_healerSkillSE()
    {
        //スキル音再生
        audioSource.PlayOneShot(healerNormalHealSE);
    }

    /// <summary>
    /// ヒーラーの必殺音再生
    /// </summary>
    public void Play_healerSpecialSE()
    {
        //必殺音再生
        audioSource.PlayOneShot(healerSpecialSE);
    }
}
