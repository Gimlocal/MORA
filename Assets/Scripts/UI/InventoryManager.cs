using System;
using UnityEngine;

namespace UI
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }
        private bool _inventoryOpened;
        [SerializeField] private GameObject inventoryPanel;
        private UIBase _uIBase;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            _uIBase = inventoryPanel.GetComponentInChildren<UIBase>();
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
                if (_inventoryOpened)
                {
                    UIManager.Instance.RegisterUI(_uIBase);
                    Player.Player.Instance.playerMovement.StopPlayer();
                }
                else
                {
                    UIManager.Instance.UnRegisterUI(_uIBase);
                }
                
                if (_inventoryOpened || UIManager.Instance.uIList.Count == 0)
                    Player.Player.Instance.playerMovement.canMove = !_inventoryOpened;
            }
        }
    }
}
