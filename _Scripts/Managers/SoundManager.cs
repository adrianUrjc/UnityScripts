using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.Singleton;
namespace Managers
{
    public class SoundManager : ASingleton<SoundManager>, IManager
    {
        public enum SoundTrack { MENU, INTRO, DEATH, CREDITS }
        [SerializeField]
        private SoundTrack currentTrack;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private List<AudioClip> audioClips;

        public IManager.GameStartMode StartMode => IManager.GameStartMode.LATE;

        public void PlaySFX(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void StartManager()
        {
            Debug.Log($"[{name}]:Iniciando...");
            PlayMusic(audioClips[(int)SoundTrack.MENU]);//voy a poner uno para que el recolector de basura no lo borre por no hacer nada
            currentTrack = SoundTrack.MENU;
            LoadData();
        }
        public void PlayTrack(SoundTrack track, bool loop = true)
        {
            PlayMusic(audioClips[(int)track], loop);
            currentTrack = track;
        }

        public void LoadData()
        {
            GetComponent<SoundSettingsApplier>().Init();
        }

        public void SaveData()
        {

        }

        public void OnEndGame()
        {
           
        }

        public void OnEnd()
        {
            SaveData();
            Debug.Log($"[{name} cerrando...]");
        }

        public void OnStartGame()
        {
            PlayMusic(audioClips[(int)SoundTrack.INTRO], false);
            currentTrack = SoundTrack.INTRO;
        }
        public void OnPlayerDeath()
        {
            PlayMusic(audioClips[(int)SoundTrack.DEATH]);
            currentTrack = SoundTrack.DEATH;
            
        }
        private void OnDestroy()
        {
        }

    }
}