using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �^�C�g���{�^��
/// </summary>
public class PushTitleButton : MonoBehaviour
{
    /// <summary>
    /// �{�^���������ꂽ��^�C�g���V�[���Ɉړ�
    /// </summary>
    public void OnButton()
    {
        SceneManager.LoadScene("Title");
    }
}
