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
}
