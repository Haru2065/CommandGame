using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushDeleteButton : MonoBehaviour
{
   public void PushDeleteSavedata()
   {
        SaveManager.DeleteSaveData("StageSaveData.json");
        SaveManager.DeleteSaveData("Attacker_save.json");
        SaveManager.DeleteSaveData("Buffer_save.json");
        SaveManager.DeleteSaveData("Healer_save.json");
   }
}
