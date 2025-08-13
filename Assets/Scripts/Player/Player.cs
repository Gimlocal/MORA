using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerAction playerAction;
        [SerializeField] private PlayerSorting playerSorting;

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