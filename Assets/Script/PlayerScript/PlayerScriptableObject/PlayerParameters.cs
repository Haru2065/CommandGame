using System;
using UnityEngine;

/// <summary>
/// �v���C���[�̃f�[�^�x�[�X�ŗ��p����p�����[�^
/// </summary>
[Serializable]
public class PlayerParameters
{
    [Tooltip("�v���C���[�̖��O�f�[�^")]
    public string PlayerNameData;

    [Tooltip("�v���C���[�̖��O�̍ő�̗͂̃f�[�^")]
    public int PlayerMaxHPData;

    [Tooltip("�v���C���[�U���̓f�[�^")]
    public int PlayerAttackPowerData;

    [Tooltip("�v���C���[�̃o�t�p���[�f�[�^")]
    public int BuffPowerData;

    [Tooltip("�v���C���[�̉񕜗̓f�[�^")]
    public int HealPowerData;
}
