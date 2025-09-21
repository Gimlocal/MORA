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
            if (Input.GetKeyDown(KeyCode.I) && !GameManager.UIManager.isOptionOpened)
            {
                bool isActive = inventoryPanel.activeSelf;
                inventoryPanel.SetActive(!isActive);
                if (!isActive)
                {
                    GameManager.UIManager.RegisterUI(_uIBase);
                    Player.Player.Instance.playerMovement.canMove = false;
                    Player.Player.Instance.playerMovement.StopPlayer();
                }
                else
                {
                    GameManager.UIManager.UnRegisterUI(_uIBase);
                    if (GameManager.UIManager.uIList.Count == 0)
                    {
                        Player.Player.Instance.playerMovement.canMove = true;
                    }
                }
            }
        }
    }
}
