using System;
using UnityEngine;

/// <summary>
/// �G�̃f�[�^�x�[�X�ŗ��p����p�����[�^
/// </summary>
[Serializable]
public class EnemyParameters
{
    [Tooltip("�GID")]
    public string EnemyNameData;

    [Tooltip("�G�̍ő�̗�")]
    public int EnemyMaxHPData;

    [SerializeField]
    [Tooltip("�G�̍U����")]
    public int EnemyAttackPowerData;

    [Tooltip("�f�o�t��")]
    public int DebuffPowerData;
}
