using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�̃X�N���v�^�u���I�u�W�F�N�g���쐬
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemySatusData")]
public class EnemyDataBase : ScriptableObject
{
    //�G�̃p�����[�^���X�g
    public List<EnemyParameters> EnemyParameters = new List<EnemyParameters>();
}
