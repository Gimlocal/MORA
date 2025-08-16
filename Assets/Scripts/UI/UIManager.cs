using System;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        private bool _inventoryOpened;
        [SerializeField] private GameObject inventoryPanel;

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

        private void Update()
        {
            InventoryManager();
        }

        private void InventoryManager()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryPanel.SetActive(!_inventoryOpened);
                _inventoryOpened = !_inventoryOpened;
                Player.Player.Instance.playerMovement.canMove = !_inventoryOpened;
                if (_inventoryOpened) Player.Player.Instance.playerMovement.StopPlayer();
            }
        }
    }
}
