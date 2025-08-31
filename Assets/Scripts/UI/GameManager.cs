using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private static UIManager _uIManager;
        public static UIManager UIManager
        {
            get
            {
                if (_uIManager == null)
                {
                    _uIManager = FindFirstObjectByType<UIManager>();
                }
                return _uIManager;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0)
            {
                Destroy(gameObject);
                Instance = null;
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
