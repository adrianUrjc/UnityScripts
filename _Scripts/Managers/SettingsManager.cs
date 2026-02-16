using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Patterns.Singleton;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LoaderMono))]
public class SettingsManager : ASingleton<SettingsManager>, IManager, ISettingsContainer
{
    public IManager.GameStartMode StartMode => IManager.GameStartMode.FIRST;
    [SerializeField, ExposedScriptableObject]
    GroupValues settingsValues;
    public UnityEvent onSettingsChange;
    #region MANAGERLOGIC
    public void OnValuesChange()
    {
        Debug.Log($"[{name}] Han habido cambios");
        onSettingsChange.Invoke();
    }
    public void  SetValue<T>(string key,T value)//cambia de valor y aplica(pero no se guarda)
    {
        if (settingsValues == null) return;
        settingsValues.SetValue<T>(key, value);
        OnValuesChange();
    }
    public T GetValue<T>(string key)
    {
        return settingsValues.GetValue<T>(key);
    }
   public void StartManager()
    {
        Debug.Log($"[{name}]Inciando...");
        LoadData();
    }
    public void SetSettingsAppliers()
    {
       var settingsAppliers=FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ILoaderUser>();
       foreach(var applier in settingsAppliers)
        {
            onSettingsChange.AddListener(applier.OnValuesChange);
        }
    }
    public void OnStartGame()
    {
        
    }
    [ContextMenu("Cargar archivos")]
    public void LoadData()
    {
        settingsValues = GetComponent<LoaderMono>().LoadValues();
        OnValuesChange();
    }
    [ContextMenu("Guardar archivos")]
    public void SaveData()
    {
        GetComponent<LoaderMono>().SaveData(settingsValues);
    }

    
     public void OnEnd()
    {
        onSettingsChange.RemoveAllListeners();
        SaveData();

    }

    public void OnEndGame()
    {
        SaveData();   
    }

    public void SubscribeToSettingsChange(Action onChange)
    {
      onSettingsChange.AddListener(onChange.Invoke);
    }

    #endregion
}
