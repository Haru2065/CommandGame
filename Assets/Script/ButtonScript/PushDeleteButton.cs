using UnityEngine;

/// <summary>
/// �{�^���������ꂽ��Z�[�u�f�[�^����������X�N���v�g
/// </summary>
public class PushDeleteButton : MonoBehaviour
{
    /// <summary>
    /// �����{�^���������ꂽ�Ƃ��̏���
    /// </summary>
   public void PushDeleteSavedata()
   {
        //�e�Z�[�u�f�[�^������
        SaveManager.DeleteSaveData("StageSaveData.json");
        SaveManager.DeleteSaveData("Attacker_save.json");
        SaveManager.DeleteSaveData("Buffer_save.json");
        SaveManager.DeleteSaveData("Healer_save.json");

        //�Z�[�u�f�[�^�����{�^�����\��
        SaveUIControl.Instance.HideSaveDeleteButton();
   }
}
