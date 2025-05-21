using TMPro;
using UnityEngine;

/// <summary>
/// HP�}�l�[�W���[
/// </summary>
public class HPManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�A�^�b�J�[")]
    private Attacker attacker;

    [SerializeField]
    [Tooltip("�o�b�t�@�[")]
    private Buffer buffer;

    [SerializeField]
    [Tooltip("�q�[���[")]
    private Healer healer;


    [SerializeField]
    [Tooltip("�A�^�b�J�[��HP��UI")]
    private TextMeshProUGUI attackerHPUGUI;

    [SerializeField]
    [Tooltip("�o�b�t�@�[��HP��UI")]
    private TextMeshProUGUI bufferHPUGUI;

    [SerializeField]
    [Tooltip("�q�[���[��HP��UI")]
    private TextMeshProUGUI healerHPUGUI;

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�L������HP���\��
        attackerHPUGUI.text = $"{attacker.PlayerCurrentHP}/ {attacker.PlayerMaxHP}";
        bufferHPUGUI.text = $"{buffer.PlayerCurrentHP}/ {buffer.PlayerMaxHP}";
        healerHPUGUI.text = $"{healer.PlayerCurrentHP}/  {healer.PlayerMaxHP}";


    }
}
