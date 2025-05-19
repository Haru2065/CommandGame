using UnityEngine;
using UnityEngine.SceneManagement;

public class PushStageSelectButton : MonoBehaviour
{
    public void StageSelectButton()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
