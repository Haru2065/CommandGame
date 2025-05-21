using TMPro;
using UnityEngine;

/// <summary>
/// HPマネージャー
/// </summary>
public class HPManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("アタッカー")]
    private Attacker attacker;

    [SerializeField]
    [Tooltip("バッファー")]
    private Buffer buffer;

    [SerializeField]
    [Tooltip("ヒーラー")]
    private Healer healer;


    [SerializeField]
    [Tooltip("アタッカーのHPのUI")]
    private TextMeshProUGUI attackerHPUGUI;

    [SerializeField]
    [Tooltip("バッファーのHPのUI")]
    private TextMeshProUGUI bufferHPUGUI;

    [SerializeField]
    [Tooltip("ヒーラーのHPのUI")]
    private TextMeshProUGUI healerHPUGUI;

    // Update is called once per frame
    void Update()
    {
        //プレイヤーキャラのHP数表示
        attackerHPUGUI.text = $"{attacker.PlayerCurrentHP}/ {attacker.PlayerMaxHP}";
        bufferHPUGUI.text = $"{buffer.PlayerCurrentHP}/ {buffer.PlayerMaxHP}";
        healerHPUGUI.text = $"{healer.PlayerCurrentHP}/  {healer.PlayerMaxHP}";


    }
}
