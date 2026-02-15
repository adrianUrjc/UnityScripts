using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SimpleScene scene;
    public void LoadScene()
    {
        SceneManager.LoadScene(scene.Index);
    }
}
