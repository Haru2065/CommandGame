using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Healer : BasePlayerStatus
{
    [SerializeField]
    [Tooltip("対象者選択マネージャー")]
    private HealTargetWindow healTargetWindow;

    [SerializeField]
    [Tooltip("アタッカー")]
    private Attacker attacker;

    [SerializeField]
    [Tooltip("バッファー")]
    private Buffer buffer;

    [SerializeField]
    [Tooltip("ヒーラーの攻撃エフェクト")]
    private GameObject healerNormalEffect;

    [SerializeField]
    [Tooltip("ヒーラーの攻撃テキストエフェクト")]
    private GameObject healer_AttackTextEffect;

    [SerializeField]
    [Tooltip("ヒーラーの回復エフェクト")]
    private GameObject healer_HeelEffect;

    [SerializeField]
    [Tooltip("アタッカーの回復エフェクトを表示させる位置")]
    private Transform attacker_HeelEffect_SpawnPoint;

    [SerializeField]
    [Tooltip("バッファーの回復エフェクトを表示させる位置")]
    private Transform buffer_HeelEffect_SpawnPoint;

    [SerializeField]
    [Tooltip("ヒーラーの回復エフェクトを表示させる位置")]
    private Transform healer_HeelEffect_SpawnPoint;

    //回復力
    public int healPower;

    //ヒーラーが行動したか
    private bool isHealerAction;

    /// <summary>
    /// ヒーラーの行動フラグのゲッターセッター
    /// </summary>
    public bool IsHealerAction
    {
        get => isHealerAction;
        set => isHealerAction = value;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        //パラメータを設定
        base.Start();

        //ヒーラーの行動フラグをfalseに
        isHealerAction = false;

        //普通のデバフと特殊デバフ継続カウントの初期化
        IsDebuff = false;
        IsSpecialDebuff = false;

        IsUseSkill = false;

        //最初は必殺は使えないようにする
        IsUseSpecial = true;

        //普通のデバフと特殊デバフ継続カウントの初期化
        DebuffCount = 0;
        SpecialDebuffCount = 0;
    }

    protected override void Update()
    {
        //プレイヤーキャラのHP数表示
        PlayerHPUGUI.text = $"{PlayerCurrentHP}/ {PlayerMaxHP}";
    }

    /// <summary>
    /// プレイヤーの行動終了したか（ヒーラーが行動したか）のフラグをリセットにするメソッド
    /// </summary>
    public override void ResetActionFlag()
    {
        //ベースのメソッドからプレイヤー行動フラグをリセット
        base.ResetActionFlag();

        //ヒーラーの行動フラグもfalse
        IsHealerAction = false;
    }

    /// <summary>
    /// パラメータを設定するメソッド
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

            //ヒーラーの現在のHPを最大に設定してHPバーも最大に設定
            PlayerCurrentHP = PlayerMaxHP;
            PlayerHPBar.maxValue = PlayerCurrentHP;
            PlayerHPBar.value = PlayerCurrentHP;
            PlayerHPBar.minValue = 0;

            //攻撃力をヒーラーのデータの攻撃力を読み込む
            AttackPower = playerData.PlayerAttackPowerData;

            //初期攻撃力もヒーラーの攻撃力に設定
            PlayerResetAttackPower = AttackPower;

            //ヒーラーの回復力をヒーラーのデータの回復力から読み込み
            healPower = playerData.HealPowerData;

            IsAlive = true;
        }
    }

    /// <summary>
    /// ヒーラーの通常攻撃
    /// </summary>
    public override void NormalAttack()
    {
        //プレイヤーが選んだ敵を攻撃対象に設定する
        BaseEnemyStatus target = PlayerTargetSelect.Instance.GetAttackTargetEnemy();

        if (target != null)
        {
            //ヒーラーの通常攻撃音再生
            PlayerSE.Instance.Play_healerNormalAttackSE();

            //ヒーラーの通常攻撃時のエフェクトを生成
            GameObject effectInstance = Instantiate(healerNormalEffect, PlayerEffect_SpawnPoint.position, Quaternion.identity);
            GameObject textEffectInstance = Instantiate(healer_AttackTextEffect, PlayerTextEfferct_SpawnPoint.position, Quaternion.identity);

            //ターゲットにダメージを与える
            target.EnemyOnDamage(AttackPower);

            //ヒーラーのエフェクトを消去
            Destroy(effectInstance, 0.2f);
            Destroy(textEffectInstance, 4f);
        }

        //(ヒーラーには攻撃スキルがないため、通常攻撃でリセット）
        if (HasBuff)
        {
            // LINQを使い、プレイヤーデータベースからPlayerIDと一致するプレイヤー情報を取得
            var playerData = PlayerDataBase.PlayerParameters.FirstOrDefault(p => p.PlayerNameData == PlayerID);

            //攻撃力を取り出したデータの攻撃力に設定する
            AttackPower = playerData.PlayerAttackPowerData;

            HasBuff = false;
        }

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                // 攻撃力を元の値に戻す
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                // JSONファイルで設定されたヒーラーのデバフ解除通知を表示
                BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");

                //デバフ解除時の状況テキストを非表示にする
                StartCoroutine(PlayerOffDebuffText());
            }
        }

        if (IsSpecialDebuff)
        {
            SpecialDebuffCount--;

            if (SpecialDebuffCount <= 0)
            {
                // 攻撃力を元の値に戻す
                AttackPower = PlayerResetAttackPower;

                IsSpecialDebuff = false;

                // JSONファイルで設定されたヒーラーの特殊デバフ解除通知を表示
                BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");

                // デバフ解除時の状況テキストを非表示にする
                StartCoroutine(PlayerOffDebuffText());
            }
        }

        //ヒーラーの行動フラグをtrueに
        IsHealerAction = true;
    }

    /// <summary>
    /// ヒーラーのスキル
    /// </summary>
    public override void PlayerSkill()
    {
        //対象者選択マネージャーから回復対象者選択画面を表示
        healTargetWindow.ShowHealTargetWindow();

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");
                StartCoroutine(PlayerOffDebuffText());
            }

        }

        if (IsSpecialDebuff)
        {
            SpecialDebuffCount--;

            if (SpecialDebuffCount <= 0)
            {
                //攻撃力を元の値に戻す
                AttackPower = PlayerResetAttackPower;

                IsSpecialDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("AttackerOffDebuff");
                StartCoroutine(PlayerOffDebuffText());
            }
        }
    }

    /// <summary>
    /// ヒーラーの必殺技
    /// </summary>
    public override void SpecialSkill()
    { 

        //ヒーラーの必殺の効果音再生
        PlayerSE.Instance.Play_healerSpecialSE();

        //生成したエフェクトを格納するリスト
        List<GameObject> specialEffects = new List<GameObject>();

        //各リストを格納し、各キャラの場所にエフェクトを生成

        if (attacker.IsAlive)
        {
            specialEffects.Add(Instantiate(healer_HeelEffect, attacker_HeelEffect_SpawnPoint.position, Quaternion.identity));
        }

        if (buffer.IsAlive)
        {
            specialEffects.Add(Instantiate(healer_HeelEffect, buffer_HeelEffect_SpawnPoint.position, Quaternion.identity));
        }

        if (IsAlive)
        {
            specialEffects.Add(Instantiate(healer_HeelEffect, healer_HeelEffect_SpawnPoint.position, Quaternion.identity));
        }

        PlayerSE.Instance.Play_healerSkillSE();

        //味方を全員回復
        attacker.PlayerCurrentHP += healPower;
        buffer.PlayerCurrentHP += healPower;
        PlayerCurrentHP += healPower;

        //3秒後全てのエフェクトを消去
        foreach (var effect in specialEffects)
        {
            Destroy(effect, 3f);
        }

        if (IsDebuff)
        {
            DebuffCount--;

            if (DebuffCount <= 0)
            {
                AttackPower = PlayerResetAttackPower;

                IsDebuff = false;

                BattleActionTextManager.Instance.ShowBattleActionText("healerOffDebuff");
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

                BattleActionTextManager.Instance.ShowBattleActionText("healerOffDebuff");
                StartCoroutine(PlayerOffDebuffText());
            }
        }
        IsUseSpecial = true;

        //必殺制限カウントを6ターンに設定
        SpecialLimitCount = 6;

        //ヒーラーの行動フラグをtrue
        IsHealerAction = true;
        IsPlayerAction = true;
    }

    /// <summary>
    /// 回復メソッド
    /// </summary>
    /// <param name="target">回復する対象者</param>
    public void OnHeal(BasePlayerStatus target)
    {
        //対象者選択マネージャーから対象選択ウィンドウを非表示
        healTargetWindow.HideHealTargetWindow();

        //ヒーラーのスキル効果音再生
        PlayerSE.Instance.Play_healerSkillSE();

        GameObject specialEffectInstance = Instantiate(healer_HeelEffect, target.transform.position, Quaternion.identity);
        Destroy(specialEffectInstance, 3f);

        //選択したターゲットに回復
        target.PlayerCurrentHP += healPower;

        //最大体力を超えないようにする
        if (target.PlayerCurrentHP > target.PlayerMaxHP)
        {
            target.PlayerCurrentHP = target.PlayerMaxHP;

        }

        //回復対象者のHPUIも更新
        target.PlayerHPUGUI.text = $"{target.PlayerCurrentHP}/ {target.PlayerMaxHP}";

        //HPバーを更新
        target.PlayerHPBar.value = target.PlayerCurrentHP;

        IsUseSkill = true;

        //スキル制限カウントを５ターンに設定
        SkillLimitCount = 5;

        //ヒーラーの行動フラグをtrue
        IsHealerAction = true;
        IsPlayerAction = true;
    }

    /// <summary>
    /// ヒーラーのダメージメソッド
    /// </summary>
    /// <param name="damage">敵からのダメ―ジ量（敵の攻撃力)</param>
    public override void PlayerOnDamage(int damage)
    {
        PlayerCurrentHP -= damage;
        PlayerHPBar.value = PlayerCurrentHP;

        //0より下には行かないようにする
        if (PlayerHPBar.value <= 0)
        {
            //生存リストからこのオブジェクトを消去
            BaseBattleManager.Instance.AlivePlayers.Remove(this);

            PlayerCurrentHP = 0;
            PlayerHPBar.value = 0;

            IsAlive = false;
        }
    }

    /// <summary>
    /// ヒーラーのレベルアップメソッド
    /// </summary>
    public override void LevelUP()
    {
        base.LevelUP();

        //回復パワーを100プラスしてレベルアップ
        healPower += 100;
    }
}
