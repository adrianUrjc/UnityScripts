

using UnityEngine;

namespace Patterns.Singleton
{
    public abstract class ASingleton<T> : MonoBehaviour where T : UnityEngine.Component
    {
        public static T Instance { get; private set; }

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
            
            DontDestroyOnLoad(gameObject);
        }
    }
}