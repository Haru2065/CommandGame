using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトルBGMを制御するスクリプト
/// ステージセレクト画面に移動してもBGMは鳴り続ける
/// </summary>
public class TitleBGM : MonoBehaviour
{
    //タイトルBGMを制御スクリプトインスタンス化用
    private static TitleBGM instance;

    /// <summary>
    /// タイトルBGM制御スクリプトをインスタンス化
    /// </summary>
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
            //インスタンスが破棄されないオブジェクトにする
            DontDestroyOnLoad(gameObject);
        }
        //既にインスタンスが存在する場合破棄する
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //ステージ1なら破棄
        if (SceneManager.GetActiveScene().name == "Stage1")
        {
            Destroy(gameObject);
        }

        //ステージ2なら破棄
        else if (SceneManager.GetActiveScene().name == "Stage2")
        {
            Destroy(gameObject);
        }

        //ステージ3なら破棄
        else if (SceneManager.GetActiveScene().name == "Stage3")
        {
            Destroy(gameObject);
        }

        //チュートリアル1なら破棄
        else if(SceneManager.GetActiveScene().name == "Tutorial1")
        {
            Destroy(gameObject);
        }
        
        //チュートリアル2なら破棄
        else if (SceneManager.GetActiveScene().name == "Tutorial2")
        {
            Destroy(gameObject);
        }
    }
}
