using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[���G�ɍU������Ώۂ�I������X�N���v�g
/// </summary>
public class PlayerTargetSelect : MonoBehaviour
{
    //���̃X�N���v�g���C���X�^���X��
    private static PlayerTargetSelect instance;

    /// <summary>
    /// �C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static PlayerTargetSelect Instance
    {
        get => instance;
    }

    //�G�̍U������^�[�Q�b�g
    private BaseEnemyStatus attackTarget;

    
    [Tooltip("�J�n���̂ݎg�p�o�g���J�n���ɐݒ肷��^�[�Q�b�g")]
    public List<BaseEnemyStatus> StartSetTargets;

    /// <summary>
    /// �V���O���g���p�^�[���ŃC���X�^���X��
    /// </summary>
    private void Awake()
    {
        //�C���X�^���X���Ȃ���΃C���X�^���X��
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(Instance);
        }
        
    }

    /// <summary>
    /// �o�g���J�n���ɍU���Ώۂ�ݒ肷�郁�\�b�h
    /// </summary>
    public void SetStartBattleTarget()
    {
        //�����ݒ肷��^�[�Q�b�g���X�g�ɂ���΃��X�g�̗v�f�P�ڂ̓G���^�[�Q�b�g�Ƃ��Đݒ�
        if (StartSetTargets.Count > 0)
        {
            SetTarget(StartSetTargets[0]);
        }
    }

    /// <summary>
    /// �v���C���[���G�ɍU������Ώۂ�ݒ肷�郁�\�b�h
    /// </summary>
    /// <param name="newTarget">�V�����ݒ肷��U���Ώ�</param>
    public void SetTarget(BaseEnemyStatus newTarget)
    {
        //�G�̍U���Ώۂ����ɐݒ肳��Ă�����UI���\��
        if(attackTarget != null)
        {
            attackTarget.ShowTargetUI(false);
        }

        //�U���Ώۂ�V�����^�[�Q�b�g�ɐݒ�
        attackTarget = newTarget;

        //�^�[�Q�b�gUI��\��
        attackTarget.ShowTargetUI(true);
        Debug.Log("�^�[�Q�b�g��ύX���܂���");
    }

    /// <summary>
    /// �w�肵���G���|�ꂽ�ꍇ�Ƀ^�[�Q�b�g���đI�����郁�\�b�h
    /// </summary>
    /// <param name="deadEnemy">�|���ꂽ�G</param>
    public void RemoveSetTarget(BaseEnemyStatus deadEnemy)
    {
        // ���X�g�ɑ��݂���ꍇ�A�|�ꂽ�G���^�[�Q�b�g���X�g����폜
        if (StartSetTargets.Contains(deadEnemy))
        {
            StartSetTargets.Remove(deadEnemy);
        }

        // ���݂̍U���Ώۂ��|���ꂽ�G�������ꍇ�A���̃^�[�Q�b�g�����肷��
        if (attackTarget == deadEnemy)
        {
            if (StartSetTargets.Count > 0)
            {
                // ���X�g�̐擪��V�����^�[�Q�b�g�Ƃ��Đݒ�
                SetTarget(StartSetTargets[0]);
            }
            else
            {
                // ���ׂĂ̓G���|���ꂽ�ꍇ�A�^�[�Q�b�gUI���\���ɂ���
                attackTarget.ShowTargetUI(false);
            }
        }

    }

    /// <summary>
    /// �ݒ肵���^�[�Q�b�g���擾���郁�\�b�h
    /// </summary>
    /// <returns>�擾�����^�[�Q�b�g�̒l���e�v���C���[�L�����̃X�e�[�^�X�ɓn��</returns>
    public BaseEnemyStatus GetAttackTargetEnemy()
    {
        return attackTarget;
    }
}
