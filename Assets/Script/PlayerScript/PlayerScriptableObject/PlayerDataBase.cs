using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�̃X�N���^�u���I�u�W�F�N�g���쐬
/// </summary>
[CreateAssetMenu(fileName ="PlayerDataBase",menuName = "ScriptableObject/PlayerDataBase")]
public class PlayerDataBase : ScriptableObject
{
    //�v���C���[�̃p�����[�^�̃��X�g
    public List<PlayerParameters> PlayerParameters = new List<PlayerParameters>();
}
