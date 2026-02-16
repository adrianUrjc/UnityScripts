using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[RequireComponent(typeof(Volume))]
public class CameraViewPortSettings : MonoBehaviour , ILoaderUser
{
    [SerializeField] float baseValueBrightness=-1;
    [SerializeField] float maxValueBrightness=0.8f;
    [SerializeField] float brightness;
    [SerializeField] float baseValueContrast=0;
    [SerializeField] float maxValueContrast=100;
    [SerializeField] float contrast;
    
    Volume cameraVolume;
    ColorAdjustments colorAdjustments;
    void Start()
    {
        cameraVolume = GetComponent<Volume>();
       

        if (cameraVolume.profile.TryGet(out colorAdjustments))
        {
           
            OnCameraViewportSettingsChange();
        }
        else
        {
            Debug.LogWarning("ColorAdjustments no encontrado en el Volume Profile.");
        }
    }
     public void OnValuesChange()
    {
       OnCameraViewportSettingsChange();
    }

    void OnCameraViewportSettingsChange() {
        

        float bt = SettingsManager.Instance.GetValue<float>("Brightness");
        brightness =Mathf.Lerp(baseValueBrightness, maxValueBrightness, bt);
        colorAdjustments.postExposure.value = brightness;

        float ct = SettingsManager.Instance.GetValue<float>("Contrast");

        contrast = Mathf.Lerp(baseValueContrast, maxValueContrast,ct );
        colorAdjustments.contrast.value = contrast;
        
    }

    public void SubscribeToValuesChange()
    {
       var container = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISettingsContainer>()
            .FirstOrDefault();
        container.SubscribeToSettingsChange(OnValuesChange);
      
    }
}
