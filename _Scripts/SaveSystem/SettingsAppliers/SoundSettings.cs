
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettingsApplier : MonoBehaviour, ILoaderUser
{
    [SerializeField] private AudioMixer audioMixer; // Mixer general del proyecto
    ISettingsContainer container;
    public void Init()
    {
        SubscribeToValuesChange();

        ApplySoundSettings();
    }




    public void SubscribeToValuesChange()
    {
        container = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISettingsContainer>()
            .FirstOrDefault();
        container.SubscribeToSettingsChange(ApplySoundSettings);

    }

    void ApplySoundSettings()
    {
        if (container.GetValue<bool>("Mute"))//si esta en silencio poner a 0
        {
            audioMixer.SetFloat("MasterVolume", -80f);
            audioMixer.SetFloat("MusicVolume", -80f);
            audioMixer.SetFloat("SFXVolume", -80f);
            audioMixer.SetFloat("InterfaceVolume", -80f);
            return;
        }
        float masterVol = container.GetValue<float>("MasterVolume");
        float musicVol = container.GetValue<float>("MusicVolume");
        float sfxVol = container.GetValue<float>("SFXVolume");
        float interfaceVolume = container.GetValue<float>("InterfaceVolume");

        // Convierte de [0,1] lineal a dB (logar�tmico)
        audioMixer.SetFloat("MasterVolume", LinearToDecibel(masterVol));
        audioMixer.SetFloat("MusicVolume", LinearToDecibel(musicVol));
        audioMixer.SetFloat("SFXVolume", LinearToDecibel(sfxVol));
        audioMixer.SetFloat("InterfaceVolume", LinearToDecibel(interfaceVolume));
    }

    float LinearToDecibel(float value)
    {
        if (value <= 0.0001f)
            return -80f; // Silencio
        return Mathf.Log10(value) * 20f;
    }
}