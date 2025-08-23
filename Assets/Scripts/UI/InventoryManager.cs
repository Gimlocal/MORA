using System;
using UnityEngine;

namespace UI
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }
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
            ManageInventory();
        }

        private void ManageInventory()
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
