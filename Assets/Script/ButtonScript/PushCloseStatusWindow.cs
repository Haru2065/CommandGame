using UnityEngine;

/// <summary>
/// �X�e�[�^�X�E�B���h�E�����{�^��
/// </summary>
public class PushCloseStatusWindow : MonoBehaviour
{
    /// <summary>
    /// �{�^���������ƃX�e�[�^�X�E�B���h�E�����
    /// </summary>
    public void OnButton()
    { 
        //UI�}�l�[�W���[����X�e�[�^�X���\���ɂ���
        UIManager.Instance.CloseStatusWindow();
    }
}
