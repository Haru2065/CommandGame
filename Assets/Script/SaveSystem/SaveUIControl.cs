using UnityEngine;

/// <summary>
/// �X�^�[�g�{�^���������ꂽ��Z�[�u�f�[�^������΃Z�[�u�f�[�^��I������X�N���v�g
/// �Z�[�u�f�[�^������΃Z�[�u�f�[�^����������{�^�����\��
/// </summary>
public class SaveUIControl : MonoBehaviour
{
    //SaveUIControl�̃C���X�^���X
    private static SaveUIControl instance;

    /// <summary>
    /// �C���X�^���X�̃Q�b�^�[
    /// </summary>
    public static SaveUIControl Instance
    {
        get => instance;
    }

    [SerializeField]
    [Tooltip("�Z�[�u�f�[�^�����{�^��")]
    private GameObject deleteButton;

    /// <summary>
    /// �C���X�^���X���C���X�^���X���Ȃ���΃I�u�W�F�N�g������
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // �Z�[�u�f�[�^�����鎞�̓Z�[�u�f�[�^�I���{�^����\��
        if (SaveManager.HasAnySaveData())
        {
            deleteButton.SetActive(true);
        }
        //�Ȃ���΃Z�[�u�f�[�^�I���{�^�����\��
        else
        {
            deleteButton.SetActive(false);
        }
    }

    /// <summary>
    /// �����{�^���������ꂽ������{�^�����\���ɂ��郁�\�b�h
    /// </summary>
    public void HideSaveDeleteButton()
    {
        deleteButton.SetActive(false);
    }
}
