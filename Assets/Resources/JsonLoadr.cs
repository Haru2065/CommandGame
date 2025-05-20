using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class JsonLoadr : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ロードしたいJsonファイルのアドレス")]
    private string jsonLoadAddress;

    public IEnumerator LoadJsonText(Action<string> onSuccess)
    {
        // 指定されたアドレスキーからTextAssetを非同期でロードする
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(jsonLoadAddress);

        //読み込みが完了するまでまつ
        yield return handle;

        //読み込みが成功したかチェック
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //読み込んだTextAssetの中身を取得する
            string json = handle.Result.text;

            //取得したTextAssetの内容をコンソールで確認
            Debug.Log(json);

            //少し待つ
            yield return null;

            //成功を表示
            Debug.Log("成功!");

            //成功時のコールバック（JsonUtilityでのパースや表示処理を呼び出す。
            onSuccess?.Invoke(json);
        }
        else
        {
            Debug.Log($"Jsonのロード失敗:{jsonLoadAddress}");
        }

        //読み込んだリソースを解放する
        Addressables.Release(handle);
    }
}
