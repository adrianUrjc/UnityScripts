
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettingsApplier : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // Mixer general del proyecto

    public void Init()
    {
        ApplySoundSettings();
        SettingsManager.Instance.onSettingsChange.AddListener(ApplySoundSettings);
        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(string.Empty);
       
    }

    void ApplySoundSettings()
    {
        if (SettingsManager.Instance.GetValue<bool>("Mute"))//si esta en silencio poner a 0
        {
            audioMixer.SetFloat("MasterVolume", -80f);
            audioMixer.SetFloat("MusicVolume", -80f);
            audioMixer.SetFloat("SFXVolume", -80f);
            audioMixer.SetFloat("InterfaceVolume", -80f);
            return;
        }
        float masterVol = SettingsManager.Instance.GetValue<float>("MasterVolume");
        float musicVol = SettingsManager.Instance.GetValue<float>("MusicVolume");
        float sfxVol = SettingsManager.Instance.GetValue<float>("SFXVolume");
        float interfaceVolume = SettingsManager.Instance.GetValue<float>("InterfaceVolume");

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