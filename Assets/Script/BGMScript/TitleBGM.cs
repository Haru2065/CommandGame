using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �^�C�g��BGM�𐧌䂷��X�N���v�g
/// �X�e�[�W�Z���N�g��ʂɈړ����Ă�BGM�͖葱����
/// </summary>
public class TitleBGM : MonoBehaviour
{
    //�^�C�g��BGM�𐧌�X�N���v�g�C���X�^���X���p
    private static TitleBGM instance;

    /// <summary>
    /// �^�C�g��BGM����X�N���v�g���C���X�^���X��
    /// </summary>
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
            //�C���X�^���X���j������Ȃ��I�u�W�F�N�g�ɂ���
            DontDestroyOnLoad(gameObject);
        }
        //���ɃC���X�^���X�����݂���ꍇ�j������
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�X�e�[�W1�Ȃ�j��
        if (SceneManager.GetActiveScene().name == "Stage1")
        {
            Destroy(gameObject);
        }

        //�X�e�[�W2�Ȃ�j��
        else if (SceneManager.GetActiveScene().name == "Stage2")
        {
            Destroy(gameObject);
        }

        //�X�e�[�W3�Ȃ�j��
        else if (SceneManager.GetActiveScene().name == "Stage3")
        {
            Destroy(gameObject);
        }

        //�`���[�g���A��1�Ȃ�j��
        else if(SceneManager.GetActiveScene().name == "Tutorial1")
        {
            Destroy(gameObject);
        }
        
        //�`���[�g���A��2�Ȃ�j��
        else if (SceneManager.GetActiveScene().name == "Tutorial2")
        {
            Destroy(gameObject);
        }
    }
}
