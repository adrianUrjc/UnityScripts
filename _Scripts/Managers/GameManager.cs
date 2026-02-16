using Patterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Managers.GameSceneManager;

namespace Managers
{
    public enum GameState { STARTING, INMAINMENU, INCREDITS, INGAME, INPAUSE, ENDGAME }
    public enum GamePlatform
    {
        Standalone,
        WebGL_PC,
        WebGL_Mobile
    }


    public class GameManager : ASingleton<GameManager>, IManager
    {
        public List<IManager> managersList;
        private GameState gameState = GameState.STARTING;
        public static Action<bool> onPause;
        public GameState CurrentState { get { return gameState; } }

        public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
        #region DEBUGGING
        [Header("DebugGame")]
        public bool DebugGame = true;

        #endregion

        void Start()
        {

            if (managersList == null)
            {
                managersList = new List<IManager>();
                var allManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                             .OfType<IManager>()
                             .Where(m => !(m is GameManager) && (m != null)); // excluir GameManager

                managersList.AddRange(allManagers);
                StartManager();
            }


        }
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnChangeScene;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnChangeScene;
        }
        private void OnChangeScene(Scene scene, LoadSceneMode mode)
        {
            switch (scene.buildIndex)
            {

            }
        }
        public void RestartGame()
        {
            OnEndGame();
            GameSceneManager.Instance.LoadSceneById((int)SceneIds.GAMESCENE);
        }
        public void PauseGame()
        {
            Debug.Log("Pausar juego");
            gameState = GameState.INPAUSE;
            onPause?.Invoke(true);
        }
        public void UnPauseGame()
        {
            gameState = GameState.INGAME;
            onPause?.Invoke(false);
        }
        public void InCredits()
        {
            gameState = GameState.INCREDITS;
        }
        public void OutCredits()
        {
            gameState = GameState.INMAINMENU;

        }
        public void SetValue<T>(string key, T value)
        {

            GetComponent<ALoader>().SetValue<T>(key, value);
        }
        public T GetValue<T>(string key)
        {

            return GetComponent<ALoader>().GetValue<T>(key);
        }
        [ContextMenu("Reset to default values")]
        public void ResetToDefaultValues()
        {
            GetComponent<ALoader>().ResetDefaultValues();
        }
        public void SaveData()
        {
            GetComponent<ALoader>().SaveValues();
        }
        public void LoadData()
        {
            GetComponent<ALoader>().LoadValues();
        }

        public void OnEnd()
        {
            foreach (var manager in managersList)
            {
                manager.OnEnd();
            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        public void OnEndGame()//necesitamos resetear todo y volver al estado inicial de juego con este metodo
        {
            foreach (var manager in managersList)
            {
                manager.OnEndGame();
            }
            SaveData();

        }
        public void StartManager()
        {
            Debug.Log($"[{name}]:Iniciando...");
            managersList = managersList.Where(m => m != null).ToList();
            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.FIRST))
            {
                manager.StartManager();
            }
            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.EARLY))
            {
                manager.StartManager();
            }
            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.NORMAL))
            {
                manager.StartManager();
            }
            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.LATE))
            {
                manager.StartManager();
            }
            LoadData();
        }
        public void OnStartGame()
        {
            Debug.Log($"[{name}]Empezando juego");
            //hay varios tipos de arranque de manager(unos dependen de otros) por defecto empiezan en normal
            //ArgumentNullException: Value cannot be null.
            //No entiendo como arreglar este error debajo de esta linea, no se que está tomando para que sea null
            managersList = managersList.Where(m => m != null).ToList();


            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.FIRST))
            {
                manager.OnStartGame();
            }
            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.EARLY))
            {
                manager.OnStartGame();
            }
            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.NORMAL))
            {
                manager.OnStartGame();
            }
            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.LATE))
            {
                manager.OnStartGame();
            }
        }
        public void OnDestroy()
        {
            managersList = managersList.Where(m => m != null).ToList();

            managersList.Clear();
        }
        public void GoBackToMainMenu()//cuando se llame a esta funcion es porque se ha salido a traves del menu de pausa
        {
            GameSceneManager.Instance.LoadSceneById((int)SceneIds.MAINMENUSCENE);
            OnEndGame();
        }
    }
}