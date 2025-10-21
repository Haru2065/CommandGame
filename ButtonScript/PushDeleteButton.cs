using UnityEngine;

/// <summary>
/// ボタンが押されたらセーブデータを消去するスクリプト
/// </summary>
public class PushDeleteButton : MonoBehaviour
{
    /// <summary>
    /// 消去ボタンが押されたときの処理
    /// </summary>
   public void PushDeleteSavedata()
   {
        //各セーブデータを消去
        SaveManager.DeleteSaveData("StageSaveData.json");
        SaveManager.DeleteSaveData("Attacker_save.json");
        SaveManager.DeleteSaveData("Buffer_save.json");
        SaveManager.DeleteSaveData("Healer_save.json");

        //セーブデータ消去ボタンを非表示
        SaveUIControl.Instance.HideSaveDeleteButton();
   }
}
