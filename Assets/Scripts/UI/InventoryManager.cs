using System;
using UnityEngine;

namespace UI
{
    public class InventoryManager : MonoBehaviour
    {
        private bool _inventoryOpened;
        [SerializeField] private GameObject inventoryPanel;
        private UIBase _uIBase;

        private void Awake()
        {
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
                    GameManager.UIManager.RegisterUI(_uIBase);
                    Player.Player.Instance.playerMovement.StopPlayer();
                }
                else
                {
                    GameManager.UIManager.UnRegisterUI(_uIBase);
                }
                
                if (_inventoryOpened || GameManager.UIManager.uIList.Count == 0)
                    Player.Player.Instance.playerMovement.canMove = !_inventoryOpened;
            }
        }
    }
}
