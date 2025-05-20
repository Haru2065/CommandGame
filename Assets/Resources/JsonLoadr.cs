using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class JsonLoadr : MonoBehaviour
{
    [SerializeField]
    [Tooltip("���[�h������Json�t�@�C���̃A�h���X")]
    private string jsonLoadAddress;

    public IEnumerator LoadJsonText(Action<string> onSuccess)
    {
        // �w�肳�ꂽ�A�h���X�L�[����TextAsset��񓯊��Ń��[�h����
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(jsonLoadAddress);

        //�ǂݍ��݂���������܂ł܂�
        yield return handle;

        //�ǂݍ��݂������������`�F�b�N
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //�ǂݍ���TextAsset�̒��g���擾����
            string json = handle.Result.text;

            //�擾����TextAsset�̓��e���R���\�[���Ŋm�F
            Debug.Log(json);

            //�����҂�
            yield return null;

            //������\��
            Debug.Log("����!");

            //�������̃R�[���o�b�N�iJsonUtility�ł̃p�[�X��\���������Ăяo���B
            onSuccess?.Invoke(json);
        }
        else
        {
            Debug.Log($"Json�̃��[�h���s:{jsonLoadAddress}");
        }

        //�ǂݍ��񂾃��\�[�X���������
        Addressables.Release(handle);
    }
}
