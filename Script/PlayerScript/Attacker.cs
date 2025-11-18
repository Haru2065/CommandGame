using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// アタッカーのステータス
/// スクリプタブルオブジェクトからデータを読み込む
/// ベースプレイヤーステータスを継承
/// </summary>
public class Attacker : BasePlayerStatus
{
    //アタッカーの行動が終了したか
    public bool IsAttackerAction;

    [SerializeField]
    [Tooltip("敵のリスト全体攻撃必殺に使用")]
    private List<BaseEnemyStatus> targetEnemys;

    [SerializeField]
    [Tooltip("アタッカー用通常攻撃用エフェクト")]
    private GameObject attacker_NormalEffect;

    [SerializeField]
    [Tooltip("アタッカーの必殺攻撃用エフェクト")]
    private GameObject attacker_SpecialEffect;

    [SerializeField]
    [Tooltip("アタッカーの文字エフェクト")]
    private GameObject attacker_TextEffect;

    [SerializeField]
    [Tooltip("敵に攻撃するエフェクトを表示させる位置1")]
    private Transform specialAttackEffect_SpawnPoint1;

    [SerializeField]
    [Tooltip("敵に攻撃するエフェクトを表示させる位置2")]
    private Transform specialAttackEffect_SpawnPoint2;

    [SerializeField]
    [Tooltip("敵に攻撃するエフェクトを表示させる位置3")]
    private Transform specialAttackEffect_SpawnPoint3;

    // Start is called before the first frame update
    protected override void Start()
    {
        //パラメータを設定
        base.Start();

        IsUseSkill = false;

        //最初は必殺は使えないようにする
        IsUseSpecial = true;

        //普通のデバフと特殊デバフ継続カウントの初期化
        DebuffCount = 0;
        SpecialDebuffCount = 0;

        IsDebuff = false;
        IsSpecialDebuff = false;

        //SkillLimitCount = 3;
        //SpecialLimitCount = 5;

        //アタッカーの行動フラグをfalseに
        IsAttackerAction = false;
    }

    /// <summary>
    /// プレイヤーの行動終了したか（アタッカーが行動したか）のフラグをリセットにするメソッド
    /// </summary>
    public override void ResetActionFlag()
    {
        //ベースのメソッドからプレイヤー行動フラグをリセット
        base.ResetActionFlag();

        //アタッカーの行動フラグもfalse
        IsAttackerAction = false;
    }

    /// <summary>
    /// パラメータを設定するメソッド
    /// プレイヤーのデータベースから読み込み
    /// </summary>
    protected override void SetPlayerParameters()
    {
        // LINQを使い、プレイヤーデータベースからPlayerIDと一致するプレイヤー情報を取得
        var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);

        // 一致するプレイヤー情報が見つかった場合、パラメータを設定
        if (playerData != null)
        {

            //最大体力のデータを読み込み
            PlayerMaxHP = playerData.PlayerMaxHPData;

            //アタッカーの現在のHPを最大に設定してHPバーも最大に設定
            PlayerCurrentHP = PlayerMaxHP;

            Debug.Log($"PlayerCuurentHP:{PlayerCurrentHP},PlayerMaxHP:{PlayerMaxHP}");
            
            PlayerHPBar.maxValue = PlayerCurrentHP;
            PlayerHPBar.value = PlayerCurrentHP;
            PlayerHPBar.minValue = 0;

            //攻撃力をアタッカーのデータの攻撃力を読み込む
            AttackPower = playerData.PlayerAttackPowerData;

            //初期攻撃力もアタッカーの攻撃力に設定
            PlayerResetAttackPower = AttackPower;

            //生存状態にする
            IsAlive = true;
        }
    }

    protected override void Update()
    {
        //プレイヤーキャラのHP数表示
        PlayerHPUGUI.text = $"{PlayerCurrentHP}/ {PlayerMaxHP}";
    }

    /// <summary>
    /// 通常攻撃
    /// </summary>
    public override void NormalAttack()
    {
        //プレイヤーが選んだ敵を攻撃対象に設定する
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        if (target != null)
        {
            //通常攻撃音再生
            PlayerSE.Instance.Play_AttackerNormalAttackSE();

            //アタッカーの通常攻撃時のエフェクトを生成
            GameObject effectInstance = Instantiate(attacker_NormalEffect, PlayerEffect_SpawnPoint.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(attacker_TextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            //攻撃する対象の敵にダメージを与える
            target.EnemyOnDamage(AttackPower);

            //アタッカーのエフェクトを消去
            Destroy(effectInstance, 0.2f);
            Destroy(textEffectInstance, 2f);
        }

        //アタッカーの行動フラグをtrueに
        IsAttackerAction = true;
    }

    /// <summary>
    /// アタッカースキル
    /// </summary>
    public override void PlayerSkill()
    {
        //攻撃力を２倍にする
        AttackPower *= 2;

        Debug.Log("攻撃力" + AttackPower);

        //プレイヤーが選んだ敵を攻撃対象に設定する
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        //ターゲットが設定されていたら処理を実行
        if (target != null)
        {
            //アタッカーのスキル効果音再生
            PlayerSE.Instance.Play_AttackerSkillSE();

            //アタッカーの攻撃エフェクトとテキストエフェクトを生成
            GameObject effectInstance = Instantiate(attacker_NormalEffect, transform.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(attacker_TextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            //遅れてもう一個エフェクトを生成
            Invoke("DelayEffect", 0.3f);

            target.EnemyOnDamage(AttackPower);

            //アタッカーのエフェクトを消去
            Destroy(effectInstance, 0.2f);
            Destroy(textEffectInstance, 4f);
        }

        var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);
        AttackPower = playerData.PlayerAttackPowerData;

        //スキルを使ったかのフラグをtrue
        IsUseSkill = true;

        //スキル使用制限カウントを3ターンに設定
        SkillLimitCount = 3;

        //アタッカーの行動フラグをtrue
        IsAttackerAction = true;
        IsPlayerAction = true;
    }

    /// <summary>
    /// アタッカー必殺
    /// </summary>
    public override void SpecialSkill()
    {
        // リスト targetEnemys の最後の要素から順に処理を行う
        // 要素を削除する際にインデックスがずれるのを防ぐため後ろから行う
        for (int i = targetEnemys.Count - 1; i >= 0; i--)
        {
            // 現在のターゲットとなる敵を取得
            BaseEnemyStatus enemy = targetEnemys[i];

            //ターゲットとオブジェクトが存在しなかったら処理をスキップ
            if (enemy == null && enemy.gameObject == null) continue;

            //アタッカー必殺攻撃音声再生
            PlayerSE.Instance.Play_AttackerSpecialSE();

            //生成した攻撃エフェクトを格納するリスト
            List<GameObject> specialEffects = new List<GameObject>();

            //生成したテキストエフェクトを格納するリスト
            List<GameObject> textEffects = new List<GameObject>();

            //ターゲットが生存していたらエフェクトを生成
            if (enemy.EnemyIsAlive)
            {
                //攻撃エフェクトの生成位置を取得
                Transform spawnPoint = GetSpawnPoint(i);

                //スポーン位置が存在する場合のみ生成
                if (spawnPoint != null)
                {
                    //エフェクトをリストに格納した後スポーン位置に生成
                    specialEffects.Add(Instantiate(attacker_SpecialEffect, spawnPoint.position, Quaternion.identity));
                    textEffects.Add(Instantiate(attacker_TextEffect, spawnPoint.position, Quaternion.identity));
                }

                //敵に攻撃を行う
                enemy.EnemyOnDamage(AttackPower);

                //3秒後全ての攻撃エフェクトを消去
                foreach (var effect in specialEffects)
                {
                    Destroy(effect, 3f);
                }

                //3秒後全てのテキストエフェクトを消去
                foreach (var textEffect in textEffects)
                {
                    Destroy(textEffect, 3f);
                }
            }

            //必殺を使用したので必殺フラグをtrue
            IsUseSpecial = true;

            //必殺の制限カウントを3に設定
            SpecialLimitCount = 3;

            //アタッカーの行動フラグをtrue
            IsAttackerAction = true;
            IsPlayerAction = true;
        }
    }



    /// <summary>
    /// プレイヤーのダメージ処理
    /// </summary>
    /// <param name="damage">敵からのダメ―ジ量（敵の攻撃力)</param>
    public override void PlayerOnDamage(int damage)
    {
        PlayerCurrentHP -= damage;
　　　　PlayerHPBar.value = PlayerCurrentHP;

        //もし現在のHPとHPバーが0になったら生存フラグをfalseに
        if (PlayerCurrentHP <= 0)
        {
            //生存リストからこのオブジェクトを消去
            BaseBattleManager.Instance.AlivePlayers.Remove(this);

            PlayerCurrentHP = 0;
            PlayerHPBar.value = 0;

            IsAlive = false;
        }
    }

    /// <summary>
    /// 全体攻撃用リストの消去メソッド
    /// </summary>
    public void RemoveDeadEnemies()
    {
        //全体攻撃用リストに入っている敵を１減らす
        for (int i = targetEnemys.Count - 1; i >= 0; i--)
        {
            // 敵が死んでいるなら全体攻撃用リストから敵を削除
            if (targetEnemys[i] == null || !targetEnemys[i].EnemyIsAlive)
            {
                targetEnemys.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 遅れてエフェクトを生成するメソッド
    /// </summary>
    public void DelayEffect()
    {
        //アタッカーのエフェクトとテキストエフェクトを遅れて生成
        GameObject EffectInstance = Instantiate(attacker_NormalEffect, PlayerEffect_SpawnPoint.position, Quaternion.identity);
        GameObject TextEffectInstance = Instantiate(attacker_TextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

        //遅れてエフェクトを消去
        Destroy(EffectInstance, 0.2f);
        Destroy(TextEffectInstance, 4f);
    }

    /// <summary>
    /// エフェクトの生成位置を取得するメソッド
    /// </summary>
    /// <param name="index">エフェクトを生成するインデックス</param>
    /// <returns>対応するスポーンポイントのTransform。範囲外の場合はnullを返す。</returns>
    private Transform GetSpawnPoint(int index)
    {
        switch (index)
        {
            case 0: return specialAttackEffect_SpawnPoint1;

            case 2: return specialAttackEffect_SpawnPoint2;

            case 3: return specialAttackEffect_SpawnPoint3;

            default: return null;
        }
    }

    /// <summary>
    /// アタッカーのレベルアップメソッド
    /// </summary>
    public override void LevelUP()
    {
        base.LevelUP();
    }
}