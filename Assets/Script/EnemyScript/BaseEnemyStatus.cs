using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ベースの敵ステータス
/// 設計図にする
/// </summary>
public abstract class BaseEnemyStatus : MonoBehaviour
{
    //敵の攻撃力
    protected int EnemyAttackPower;

    //敵の最大体力
    protected int EnemyMaxHP;

    //敵の現在のHP
    private int enemyCurrentHP;

    /// <summary>
    /// 敵の現在のHPのゲッターセッター
    /// </summary>
    public int EnemyCurrentHP
    {
        get => enemyCurrentHP;
        set => enemyCurrentHP = value;
    }

    [SerializeField]
    [Tooltip("敵のHPUI")]
    protected TextMeshProUGUI enemyHPUGUI;

    [SerializeField]
    [Tooltip("ターゲットUIを表示する位置")]
    private Transform targetPosition;

    [SerializeField]
    [Tooltip("敵に攻撃する対象ターゲット")]
    private GameObject targetUI;

    public GameObject TargetUI
    {
        get => targetUI;
        set => targetUI = value;
    }

    [SerializeField]
    [Tooltip("アタッカー全体攻撃敵リスト用")]
    private Attacker attacker;

    public Attacker Attacker
    {
        get => attacker;
    }

    [SerializeField]
    [Tooltip("バッファーテキストメッセージ用")]
    private Buffer buffer;

    public Buffer Buffer
    {
        get => buffer;
    }

    [SerializeField]
    [Tooltip("ヒーラーテキストメッセージ用")]
    private Healer healer;

    public  Healer Healer
    {
        get => healer;
    }

    [SerializeField]
    [Tooltip("敵のデータベース")]
    private EnemyDataBase enemyDataBase;

    /// <summary>
    /// 敵データベースのゲッター
    /// </summary>
    public EnemyDataBase EnemyDataBase
    {
        get => enemyDataBase;
    }

    //敵が生存しているか
    private bool enemyIsAlive;

    //敵が生存しているかのゲッターセッター
    public bool EnemyIsAlive
    {
        get => enemyIsAlive;
        set => enemyIsAlive = value;
    }

    [SerializeField]
    [Tooltip("敵ID")]
    string enemyID;

    //敵IDのゲッターセッター
    public string EnemyID
    {
        get => enemyID;
        set => enemyID = value;
    }

    [SerializeField]
    [Tooltip("ドラゴンの単体攻撃エフェクト")]
    private GameObject onlyAttackEffect;

    public GameObject OnlyAttackEffect
    {
        get => onlyAttackEffect;
        set => onlyAttackEffect = value;
    }


    //生存しているキャラを初期時に設定するリスト
    public List<BasePlayerStatus> StartAlivePlayers;

    /// <summary>
    /// HPUI表示用子クラスで書く
    /// </summary>
    protected abstract void Update();

    /// <summary>
    /// ターゲットを表示するメソッド
    /// </summary>
    /// <param name="show">表示したか</param>
    public void ShowTargetUI(bool show)
    {
        //ターゲットUIを表示する
        targetUI.SetActive(show);

        //ターゲットをUIをターゲットを表示する位置に表示
        targetUI.transform.position = targetPosition.position;
    }

    /// <summary>
    ///マウスがクリックされたらターゲットに設定
    /// </summary>
    public void OnMouseDown()
    {
        //敵が生存していればターゲットを設定
        if (enemyIsAlive)
        {
            PlayerTargetSelect.Instance.SetTarget(this);
        }
    }

    protected IEnumerator ShowEnemyActionText(string EnemyActiuon)
    {
        yield return new WaitForSeconds(1f);

        BattleActionTextManager.Instance.ShowBattleActionText(EnemyActiuon);

        yield return null;
    }

    /// <summary>
    /// スケルトンのデバフ通知を非表示にするコールチン
    /// </summary>
    /// <returns>待ち時間</returns>
    protected IEnumerator HideEnemyActionText()
    {
        //2フレーム待つ
        yield return new WaitForSeconds(1f);
        
        //テキストウィンドウを非表示
        BattleActionTextManager.Instance.TextDelayHide();

    　　//1フレーム待つ
        yield return null;
    }

    /// <summary>
    /// ランダムでプレイヤーに攻撃力する対象を決めるメソッド
    /// </summary>
    public virtual BasePlayerStatus RandomSelect()
    {
        //一度生存しているキャラのみでリストを整理する
        List<BasePlayerStatus> TargetAlivePlayers = StartAlivePlayers.FindAll(player => player.IsAlive);

        //リストにキャラがいれば実行
        if(TargetAlivePlayers.Count > 0)
        {
            //リストの中にあるプレイヤーキャラを選択してターゲットに設定
            BasePlayerStatus target = TargetAlivePlayers[Random.Range(0,TargetAlivePlayers.Count)];

            //設定したターゲットにダメージを与える
            target.PlayerOnDamage(EnemyAttackPower);

            return target;
        }
        else 
        {
            Debug.Log("攻撃対象がいません");

            return null;
        }
    }


    /// <summary>
    /// 敵のダメージ処理
    /// 子クラスで実態を作るためabstractにする
    /// </summary>
    /// <param name="damage">プレイヤーの攻撃力分のダメージ</param>
    public abstract void EnemyOnDamage(int damage);

    /// <summary>
    /// 敵のパラメータをを設定するメソッド
    /// 子クラスで実態を作るためabstractにする
    /// </summary>
    protected abstract void SetEnemyParameters();

    /// <summary>
    /// 敵を消去するコールチン
    /// </summary>
    /// <returns>1フレーム待つ</returns>
    protected abstract IEnumerator DestroyObject();
}
