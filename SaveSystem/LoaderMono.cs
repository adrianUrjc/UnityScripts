using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderMono : MonoBehaviour
{
    [SerializeField] private ALoader loader=new();

   //Load
   [ContextMenu("Load Data")]
   public void LoadData()
   {
      loader.LoadValues();
    
   }
   //Save
    [ContextMenu("Save Data")]
    public void SaveData()
    {
        loader.SaveValues();
    }
   //Reset
    [ContextMenu("Reset Data")]
    public void ResetData()
    {
        loader.ResetToDefaults();
    }
    public void RemoveLoadedValues()
    {
        loader.RemoveLoadedValues();
    }
    public void ChangeAssetName(string newName)
    {
        loader.ChangeAssetName(newName);
    }
    public T GetValue<T>(string key)
    {
        return loader.GetValue<T>(key);
    }
    public void SetValue<T>(string key, T value)
    {
        loader.SetValue(key, value);
    }
}
