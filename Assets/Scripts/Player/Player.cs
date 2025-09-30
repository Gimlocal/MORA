using Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        public PlayerMovement playerMovement;
        public PlayerMining playerMining;
        public PlayerSorting playerSorting;
        public PlayerItem playerItem;
        public PlayerStat playerStat;
        public GameObject playerSprite;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneDatabase.GetSceneType(scene.name) == SceneType.Out)
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