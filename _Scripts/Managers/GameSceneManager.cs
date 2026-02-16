using Patterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameSceneManager : ASingleton<GameSceneManager>, IManager
    {
        public enum SceneIds { BOOTSTRAP, MAINMENUSCENE, GAMESCENE, PRUEBAENEMIGOS, PRUEBATIENDA }
        public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
        [Header("Scene to start")]
        [SerializeField] public SceneIds StartingScene = SceneIds.MAINMENUSCENE;
        [SerializeField] public GameObject fadeToBlackScreen;
        private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;


        public void StartManager()
        {
            Debug.Log($"[{name}]:Iniciando...");
            DontDestroyOnLoad(fadeToBlackScreen);
            fadeToBlackScreen.SetActive(true);
            canvasGroup = fadeToBlackScreen.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;
            LoadMenuScene();
        }
        public void LoadMenuScene()
        {

            SceneManager.LoadScene((int)StartingScene, LoadSceneMode.Single);
            StartCoroutine(FadeOut());
            //LoadSceneAsyncID((int)StartingScene);
        }
        public IEnumerator FadeOut()
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        public IEnumerator FadeIn()
        {
            canvasGroup.blocksRaycasts = true;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }
       

        public void LoadSceneById(int id)
        {
            StartCoroutine(FadeIn());
            SceneManager.LoadScene(id, LoadSceneMode.Single);
            StartCoroutine(FadeOut());

        }

        public void LoadSceneAsyncID(int id)
        {
            StartCoroutine(LoadSceneAsyncIDRoutine(id));
        }
        private IEnumerator LoadSceneAsyncIDRoutine(int id)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(id, LoadSceneMode.Single);
            while (!op.isDone)
            {
                //Se puede mostrar barra de carga por aqui
                yield return null;
            }
        }
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName));
        }

        private IEnumerator LoadSceneAsyncRoutine(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!op.isDone)
            {
                //Se puede mostrar barra de carga por aqui
                yield return null;
            }
        }
        public void LoadData()
        {
            throw new System.NotImplementedException();
        }

        public void OnEnd()
        {
            Debug.Log($"[{name} cerrando...]");
        }

        public void OnEndGame()
        {
        }

        public void SaveData()
        {
            throw new System.NotImplementedException();
        }

        public void OnStartGame()
        {
        }
    }
}