using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// バッファーのステータス
/// スクリプタブルオブジェクトからデータを読み込む
/// ベースプレイヤーステータスを継承
/// </summary>
public class Buffer : BasePlayerStatus
{
    [SerializeField]
    [Tooltip("バッファーターゲットマネージャー")]
    private BuffTargetWindow bufferTargetWindow;

    [SerializeField]
    [Tooltip("アタッカー")]
    private Attacker attacker;

    [SerializeField]
    [Tooltip("ヒーラー")]
    private Healer healer;

    [SerializeField]
    [Tooltip("バッファーの通常攻撃エフェクト")]
    private GameObject bufferNormalEffect;

    [SerializeField]
    [Tooltip("バッファーのスキルエフェクト")]
    private GameObject bufferSkillEffect;

    [SerializeField]
    [Tooltip("バッファーのテキストエフェクト")]
    private GameObject bufferTextEffect;


    [SerializeField]
    [Tooltip("アタッカーのバフエフェクトを表示させる位置")]
    private Transform attacker_BuffEffect_SpawnPoint;

    [SerializeField]
    [Tooltip("バッファーのバフエフェクトを表示させる位置")]
    private Transform buffer_BuffEffect_SpawnPoint;

    [SerializeField]
    [Tooltip("ヒーラーのバフエフェクトを表示させる位置")]
    private Transform healer_BuffEffect_SpawnPoint;

    //実際のバフ力
    public int buffPower;

    //バッファーの行動が終了したか
    public bool IsBufferAction;


    // Start is called before the first frame update
    protected override void Start()
    {
        //パラメータを設定
        base.Start();

        IsBufferAction = false;

        //SkillLimitCount = 3;
        //SpecialLimitCount = 6;

        IsUseSkill = false;

        IsUseSpecial = true;

        IsDebuff = false;

        IsSpecialDebuff = false;
    }

    /// <summary>
    /// パラメータを設定するメソッド
    /// プレイヤーのデータベースから読み込み
    /// </summary>
    protected override void SetPlayerParameters()
    {

        // LINQを使い、プレイヤーデータベースからPlayerIDと一致するプレイヤー情報を取得
        var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);

        //プレイヤーのIDと合致すればパラメータを設定
        if (playerData != null)
        {
            //最大体力のデータを読み込み
            PlayerMaxHP = playerData.PlayerMaxHPData;

            //バッファーの現在のHPを最大に設定してHPバーも最大に設定
            PlayerCurrentHP = PlayerMaxHP;
            PlayerHPBar.maxValue = PlayerCurrentHP;
            PlayerHPBar.value = PlayerCurrentHP;
            PlayerHPBar.minValue = 0;

            //HPBarの最小は０
            PlayerHPBar.minValue = 0;

            //攻撃力をバッファーのデータの攻撃力を読み込む
            AttackPower = playerData.PlayerAttackPowerData;

            //初期攻撃力もアタッカーの攻撃力に設定
            PlayerResetAttackPower = AttackPower;

            //バフデータを読み込み
            buffPower = playerData.BuffPowerData;

            //生存状態にする
            IsAlive = true;
        }
    }

    /// <summary>
    /// バッファーの通常攻撃
    /// </summary>
    public override void NormalAttack()
    {
        //プレイヤーが選んだ敵を攻撃対象に設定する
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        if (target != null)
        {
            //バッファの通常攻撃音再生
            PlayerSE.Instance.Play_BufferNormalAttackSE();

            //バッファの通常攻撃のエフェクトを生成
            GameObject effectInstance = Instantiate(bufferNormalEffect, PlayerEffect_SpawnPoint.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(bufferTextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            //攻撃する対象の敵にダメージを与える
            target.EnemyOnDamage(AttackPower);

            //エフェクトを消去する
            Destroy(effectInstance,0.2f);
            Destroy(textEffectInstance, 4);
        }

        //もしバフ効果があれば攻撃力をリセット
        if (HasBuff)
        {
            var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p =>p.PlayerNameData == PlayerID);

            //バッファーの攻撃力をデータの攻撃力に設定
            AttackPower = playerData.PlayerAttackPowerData;

            HasBuff = false;
        }

        if (IsDebuff)
        {
            DebuffCount--;

            //デバフ継続カウントが0になったらフラグをfalse、テキストを表示
            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                // JSONファイルで設定されたアタッカーのデバフ解除通知を表示
                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                // デバフ解除時の状況テキストを非表示にする
                StartCoroutine(PlayerOffDebuffText());
            }
        }

        if (IsSpecialDebuff)
        {
            SpecialDebuffCount--;

            //特殊デバフ継続カウントが0になったらフラグをfalse、テキストを表示
            if (SpecialDebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsSpecialDebuff = false;

                // JSONファイルで設定されたアタッカーの特殊デバフ解除通知を表示
                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                StartCoroutine(PlayerOffDebuffText());
            }
        }

        IsBufferAction = true;
    }

    /// <summary>
    /// バッファーのスキル
    /// </summary>
    public override void PlayerSkill()
    {
        //バフするターゲットを選択するウィンドウを開く
        bufferTargetWindow.ShowBuffTargetWindow();

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                StartCoroutine(PlayerOffDebuffText());
            }
        }

        if (IsSpecialDebuff)
        {
            SpecialDebuffCount--;

            if (SpecialDebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsSpecialDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                StartCoroutine(PlayerOffDebuffText());
            }
        }

        IsUseSkill = true;

        //スキル制限カウントを３に設定
        SkillLimitCount= 3;
    }

    /// <summary>
    /// バッファーの必殺技
    /// </summary>
    public override void SpecialSkill()
    {
        //全キャラバフしたのでフラグをtrueに
        attacker.HasBuff = true;
        HasBuff = true;
        healer.HasBuff = true;

        //バッファーの必殺効果音再生
        PlayerSE.Instance.Play_bufferSpecialSE();

        //生成したエフェクトを格納するリスト
        List<GameObject> specialEffects = new List<GameObject>();

        //各リストを格納し、各キャラの場所にエフェクトを生成

        if (attacker.IsAlive)
        { 
            specialEffects.Add(Instantiate(bufferSkillEffect, attacker_BuffEffect_SpawnPoint.position, Quaternion.identity));
        }

        //自身が生存していたらエフェクトを生成
        if (IsAlive)
        {
            specialEffects.Add(Instantiate(bufferSkillEffect, buffer_BuffEffect_SpawnPoint.position, Quaternion.identity));
        }


        if (healer.IsAlive)
        {
            specialEffects.Add(Instantiate(bufferSkillEffect, healer_BuffEffect_SpawnPoint.position, Quaternion.identity));
        }

        //3秒後全てのエフェクトを消去
        foreach (var effect in specialEffects)
        {
            Destroy(effect,3f);
        }


        //全キャラバフする
        attacker.AttackPower += buffPower;
        AttackPower += buffPower;
        healer.AttackPower += buffPower;

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                StartCoroutine(PlayerOffDebuffText());
            }
        }

        if (IsSpecialDebuff)
        {
            SpecialDebuffCount--;

            if (SpecialDebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsSpecialDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("BufferOffDebuff");

                StartCoroutine(PlayerOffDebuffText());
            }
        }
        IsUseSpecial = true;
        IsBufferAction = true;

        //必殺制限カウントを６に設定
        SpecialLimitCount = 6;

    }

    /// <summary>
    /// バフ実行メソッド
    /// </summary>
    /// <param name="target">プレイヤーを取得する</param>
    public void OnBuff(BasePlayerStatus target)
    {
        //バフ対象を選択するウィンドウを非表示
        bufferTargetWindow.HideBuffTargetWindow();

        Debug.LogWarning(target.PlayerID + "の攻撃力" + target.AttackPower);

        //バフスキル効果音再生
        PlayerSE.Instance.Play_BufferSkillSE();

        //バッファーのスキルエフェクトを生成
        GameObject effectInstance = Instantiate(bufferSkillEffect, target.transform.position,Quaternion.identity);

        //バフ力をターゲットの攻撃力を加算
        target.AttackPower += buffPower;

        Debug.LogWarning(target.PlayerID + "の攻撃力" + target.AttackPower);

        //エフェクトを消去
        Destroy(effectInstance, 3f);

        //バッファーが行動したのでtrueに
        IsBufferAction = true;

        //バッファーのターンが終了したらスキル制限と必殺制限カウントのUIを非表示
        if (IsBufferAction)
        {
            UIManager.Instance.SkillLimitCountText.SetActive(false);
            UIManager.Instance.SpecialLimitCountText.SetActive(false);
        }

        //ターゲットのバフしたかのフラグをtrueにする
        target.HasBuff = true;
    }

    /// <summary>
    /// ダメージメソッド
    /// </summary>
    /// <param name="damage">敵の攻撃力</param>
    public override void PlayerOnDamage(int damage)
    {
        //バッファーの体力を敵攻撃力分減らす
        PlayerCurrentHP -= damage;
        PlayerHPBar.value = PlayerCurrentHP;

        //もし現在のHPとHPバーが0になったらIsAliveをfalse
        if(PlayerCurrentHP <= 0)
        {
            //生存フラグをfalseに
            BaseBattleManager.Instance.AlivePlayers.Remove(this);

            PlayerCurrentHP = 0;
            PlayerHPBar.value = 0;

            IsAlive = false;
        }
    }

    /// <summary>
    /// バッファーのレベルアップ処理
    /// </summary>
    public override void LevelUP()
    {
        base.LevelUP();

        //バフパワーを100プラスしてレベルアップ
        buffPower += 100;
    }
}