using Newtonsoft.Json;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ベースのプレイヤーステータス
/// 設計図にする
/// </summary>
public abstract class BasePlayerStatus : MonoBehaviour
{
    [SerializeField]
    [Tooltip("プレイヤーキャラのデータベース")]
    private PlayerDataBase playerDataBase;

    /// <summary>
    /// プレイヤーのデータベースのゲッター
    /// </summary>
    public PlayerDataBase PlayerDataBase
    {
        get => playerDataBase;
    }

    [SerializeField]
    [Tooltip("プレイヤーID名")]
    private string playerID;

    /// <summary>
    /// プレイヤーIDのゲッター
    /// </summary>
    public string PlayerID
    {
        get => playerID;
        set => playerID = value;
    }

    //プレイヤーが生存しているか
    private bool isAlive;

    /// <summary>
    /// レイヤーが生存しているかゲッターセッター
    /// </summary>
    public bool IsAlive
    {
        get => isAlive;
        set => isAlive = value;
    }

    /// <summary>
    /// スキルを使ったか
    /// </summary>
    private bool isUseSkill;

    /// <summary>
    /// スキルを使ったかのゲッターセッター
    /// </summary>
    public bool IsUseSkill
    {
        get => isUseSkill;
        set => isUseSkill = value;
    }

    //必殺を使ったか
    private bool isUseSpecial;

    /// <summary>
    /// 必殺を使ったかのゲッターセッター
    /// </summary>
    public bool IsUseSpecial
    {
        get => isUseSpecial;
        set => isUseSpecial = value;
    }

    //バフされているか
    private bool hasBuff;

    /// <summary>
    /// バフされているかのゲッターセッター
    /// </summary>
    public bool HasBuff
    {
        get => hasBuff;
        set => hasBuff = value;
    }

    //デバフ状態か
    private bool isDebuff;

    /// <summary>
    /// デバフ状態かのゲッターセッター
    /// </summary>
    public bool IsDebuff
    {
        get => isDebuff;
        set => isDebuff = value;
    }

    //HPが減るデバフ状態か
    private bool isHPDebuff;

    /// <summary>
    /// HPが減るデバフ状態かのゲッターセッター
    /// </summary>
    public bool IsHPDebuff
    {
        get => isHPDebuff;
        set => isHPDebuff = value;
    }

    //特殊デバフ状態か
    private bool isSpecialDebuff;

    /// <summary>
    /// 特殊デバフ状態かのゲッターセッター
    /// </summary>
    public bool IsSpecialDebuff
    {
        get => isSpecialDebuff;
        set => isSpecialDebuff = value;
    }

    //プレイヤーの攻撃力
    private int attackPower;

    /// <summary>
    /// プレイヤーの攻撃力のゲッターセッター
    /// </summary>
    public int AttackPower
    {
        get => attackPower;
        set => attackPower = value;
    }

    //プレイヤーの最大体力
    private int playerMaxHP;

    /// <summary>
    /// プレイヤーの最大の体力のゲッターセッター
    /// </summary>
    public int PlayerMaxHP
    {
        get => playerMaxHP;
        set => playerMaxHP = value;
    }

    /// <summary>
    /// プレイヤーの現在の体力
    /// </summary>
    private int playerCurrentHP;

    /// <summary>
    /// プレイヤーの現在の体力のゲッターセッター
    /// </summary>
    public int PlayerCurrentHP
    {
        get =>playerCurrentHP;
        set => playerCurrentHP = value;
    }

    //プレイヤーの初期攻撃力(デバフ解除用）
    private int playerResetAttackPower;

    /// <summary>
    /// プレイヤー用の初期攻撃力のゲッターセッター
    /// </summary>
    public int PlayerResetAttackPower
    {
        get => playerResetAttackPower;
        set => playerResetAttackPower = value;
    }

    //デバフ継続カウント
    private int debuffCount;

    /// <summary>
    /// デバフ継続カウントのゲッターセッター
    /// </summary>
    public int DebuffCount
    {
        get => debuffCount;
        set => debuffCount = value;
    }

    //特殊デバフ継続カウント
    private int specialDebuffCount;

    /// <summary>
    /// 特殊デバフ継続カウントのゲッターセッター
    /// </summary>
    public int SpecialDebuffCount
    {
        get => specialDebuffCount;
        set => specialDebuffCount = value;
    }

    [SerializeField]
    [Tooltip("スキル使用制限カウント")]
    private int skillLimitCount;

    /// <summary>
    /// スキル使用制限カウントのゲッターセッター
    /// </summary>
    public int SkillLimitCount
    {
        get => skillLimitCount;
        set => skillLimitCount = value;
    }
    
    [SerializeField]
    [Tooltip("必殺使用制限のカウント")]
    private int specialLimitCount;

    /// <summary>
    /// 必殺使用制限のカウントのゲッターセッター
    /// </summary>
    public int SpecialLimitCount
    {
        get => specialLimitCount;
        set => specialLimitCount = value;
    }

    [SerializeField]
    [Tooltip("プレイヤーのHPバー")]
    private Slider playerHPBar;

    /// <summary>
    /// プレイヤーのHPバーのゲッター
    /// </summary>
    public Slider PlayerHPBar
    {
        get => playerHPBar;
        set => playerHPBar = value;
    }

    [SerializeField]
    [Tooltip("エフェクトを表示させる位置")]
    private Transform playerEffect_SpawnPoint;

    /// <summary>
    /// エフェクトを表示させる位置のゲッター
    /// </summary>
    public Transform PlayerEffect_SpawnPoint
    {
        get => playerEffect_SpawnPoint;
    }

    [SerializeField]
    [Tooltip("文字エフェクトを表示させる位置")]
    private Transform playerTextEfferct_SpawnPoint;

    /// <summary>
    /// 文字エフェクトを表示させる位置のゲッター
    /// </summary>
    public Transform PlayerTextEfferct_SpawnPoint
    {
        get => playerEffect_SpawnPoint;
    }

    //プレイヤーのレベル
    private int level = 0; 

    /// <summary>
    /// プレイヤーのレベルのゲッターセッター
    /// </summary>
    public int Level
    {
        get => level;
        set => level = value;
    }

    /// <summary>
    /// 状況テキストを非表示にするコールチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayerOffDebuffText()
    {
        //2フレーム待つ
        yield return new WaitForSeconds(2f);

        //テキストを非表示
        BattleActionTextManager.Instance.TextDelayHide();

        //1フレーム待つ
        yield return null;
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Start()
    {
        string path = Application.persistentDataPath + $"/{PlayerID}_save.Json";

        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerSaveData saveData = JsonConvert.DeserializeObject<PlayerSaveData>(json);
            ApplySaveData(saveData);
            
            

        }
        else
        {
            SetPlayerParameters();
        }
    }

    /// <summary>
    /// Jsonで保存されたパラメータをロード
    /// </summary>
    /// <param name="data">パラメータの保存データ</param>
    private void ApplySaveData(PlayerSaveData data)
    {
        level = data.level_SaveData;
        AttackPower = data.attackPower_SaveData;
        playerMaxHP = data.playerMaxHP_SaveData;
        playerCurrentHP = playerMaxHP;

        playerHPBar.maxValue = playerCurrentHP;
        playerHPBar.value = playerCurrentHP;
        playerHPBar.minValue = 0;

        playerResetAttackPower = data.attackPower_SaveData;
    }


    /// <summary>
    /// プレイヤーのレベルアップし、パラメータを上げるメソッド
    /// </summary>
    public virtual void LevelUP()
    {
        //攻撃力、最大体力の数値を上げる
        attackPower += 100;
        playerMaxHP += 500;

        //レベルをプラスする
        level++;;

    }

    /// <summary>
    /// プレイヤーのダメージメソッド
    /// </summary>
    /// <param name="damage">敵の攻撃力をダメージにする</param>
    public abstract void PlayerOnDamage(int damage);

    /// <summary>
    /// プレイヤーの通常攻撃メソッド
    /// </summary>
    public abstract void NormalAttack();

    /// <summary>
    /// プレイヤーのスキルメソッド
    /// </summary>
    public abstract void PlayerSkill();

    /// <summary>
    /// プレイヤーの必殺技メソッド
    /// </summary>
    public abstract void SpecialSkill();

    /// <summary>
    /// スクリプタブルオブジェクトからパラメータを設定するメソッド
    /// </summary>
    protected abstract void SetPlayerParameters();
}
