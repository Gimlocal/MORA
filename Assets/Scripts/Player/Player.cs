using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        public PlayerMovement playerMovement;
        public PlayerAction playerAction;
        public PlayerSorting playerSorting;
        public PlayerItem playerItem;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}