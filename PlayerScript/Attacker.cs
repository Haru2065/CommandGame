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
    /// プレイヤーの行動フラグをリセットするメソッド
    /// ターン終了時に呼び出され、次のターンに備えて行動フラグをfalseに戻します
    /// </summary>
    public override void ResetActionFlag()
    {
        // 基底クラスのフラグをリセット
        base.ResetActionFlag();

        // アタッカー固有の行動フラグもリセット
        IsAttackerAction = false;
    }

    /// <summary>
    /// アタッカーのパラメータを初期設定するメソッド
    /// ScriptableObjectのデータベースから初期ステータスを読み込みます
    /// セーブデータが存在しない場合に呼び出されます
    /// </summary>
    protected override void SetPlayerParameters()
    {
        // LINQを使用してデータベースから該当するプレイヤー情報を検索
        var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);

        // データが見つかった場合、各パラメータを初期化
        if (playerData != null)
        {
            // 最大HPをデータベースから読み込み
            PlayerMaxHP = playerData.PlayerMaxHPData;

            // 現在のHPを最大HPに設定（バトル開始時は全回復状態）
            PlayerCurrentHP = PlayerMaxHP;

            Debug.Log($"PlayerCuurentHP:{PlayerCurrentHP},PlayerMaxHP:{PlayerMaxHP}");
            
            // HPバーのUI設定（最大値、現在値、最小値）
            PlayerHPBar.maxValue = PlayerCurrentHP;
            PlayerHPBar.value = PlayerCurrentHP;
            PlayerHPBar.minValue = 0;

            // 攻撃力をデータベースから読み込み
            AttackPower = playerData.PlayerAttackPowerData;

            // デバフ解除時の復元用に初期攻撃力を保存
            PlayerResetAttackPower = AttackPower;

            // 生存フラグをtrueに設定
            IsAlive = true;
        }
    }

    protected override void Update()
    {
        //プレイヤーキャラのHP数表示
        PlayerHPUGUI.text = $"{PlayerCurrentHP}/ {PlayerMaxHP}";
    }

    /// <summary>
    /// 通常攻撃を実行するメソッド
    /// 選択中の敵に対して単体攻撃を行います（Aキーで実行）
    /// </summary>
    public override void NormalAttack()
    {
        // プレイヤーが選択している敵をターゲットとして取得
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        // ターゲットが有効な場合のみ攻撃を実行
        if (target != null)
        {
            // 通常攻撃の効果音を再生
            PlayerSE.Instance.Play_AttackerNormalAttackSE();

            // 攻撃エフェクトとテキストエフェクトを生成
            GameObject effectInstance = Instantiate(attacker_NormalEffect, PlayerEffect_SpawnPoint.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(attacker_TextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            // ターゲットの敵にダメージを与える
            target.EnemyOnDamage(AttackPower);

            // エフェクトを指定時間後に削除（0.2秒後に攻撃エフェクト、2秒後にテキスト）
            Destroy(effectInstance, 0.2f);
            Destroy(textEffectInstance, 2f);
        }

        // アタッカーの行動完了フラグを立てる
        IsAttackerAction = true;
    }

    /// <summary>
    /// アタッカーのスキル攻撃を実行するメソッド
    /// 攻撃力を2倍にして2連続攻撃を行います（Sキーで実行）
    /// 使用後は3ターン再使用不可になります
    /// </summary>
    public override void PlayerSkill()
    {
        // スキル発動時は攻撃力を2倍に強化
        AttackPower *= 2;

        Debug.Log("攻撃力" + AttackPower);

        // プレイヤーが選択している敵をターゲットとして取得
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        // ターゲットが有効な場合のみ攻撃を実行
        if (target != null)
        {
            // スキル攻撃の効果音を再生
            PlayerSE.Instance.Play_AttackerSkillSE();

            // 1回目の攻撃エフェクトを生成
            GameObject effectInstance = Instantiate(attacker_NormalEffect, transform.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(attacker_TextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            // 0.3秒後に2回目の攻撃エフェクトを遅延生成（2連続攻撃の演出）
            Invoke("DelayEffect", 0.3f);

            // ターゲットに2倍の攻撃力でダメージを与える
            target.EnemyOnDamage(AttackPower);

            // エフェクトを指定時間後に削除
            Destroy(effectInstance, 0.2f);
            Destroy(textEffectInstance, 4f);
        }

        // 攻撃力を通常値に戻す（データベースから再読み込み）
        var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);
        AttackPower = playerData.PlayerAttackPowerData;

        // スキル使用フラグを立てる
        IsUseSkill = true;

        // スキルの再使用待機ターン数を設定（3ターン後に再使用可能）
        SkillLimitCount = 3;

        // 行動完了フラグを立てる
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

            //ターゲットまたはオブジェクトが存在しなかったら処理をスキップ
            if (enemy == null || enemy.gameObject == null) continue;

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
    /// 敵のインデックスに対応するスポーン位置を返します
    /// </summary>
    /// <param name="index">エフェクトを生成する敵のインデックス（0-2）</param>
    /// <returns>対応するスポーンポイントのTransform。範囲外の場合はnullを返す。</returns>
    private Transform GetSpawnPoint(int index)
    {
        switch (index)
        {
            case 0: return specialAttackEffect_SpawnPoint1;

            case 1: return specialAttackEffect_SpawnPoint2;

            case 2: return specialAttackEffect_SpawnPoint3;

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