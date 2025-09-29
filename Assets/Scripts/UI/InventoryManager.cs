using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class InventoryManager : MonoBehaviour
    {
        private bool _inventoryOpened;
        [SerializeField] private GameObject inventoryPanel;
        public UIBase[] uIBases;

        private void Awake()
        {
            uIBases = inventoryPanel.GetComponentsInChildren<UIBase>(true);
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
                    foreach (var uI in uIBases)
                    {
                        if (uI.gameObject.activeSelf) GameManager.UIManager.RegisterUI(uI);
                    }
                    Player.Player.Instance.playerMovement.canMove = false;
                    Player.Player.Instance.playerMovement.StopPlayer();
                }
                else
                {
                    foreach (var uI in uIBases)
                    {
                        if (uI.gameObject.activeSelf) GameManager.UIManager.UnRegisterUI(uI);
                    }
                    if (GameManager.UIManager.uIList.Count == 0)
                    {
                        Player.Player.Instance.playerMovement.canMove = true;
                    }
                }
            }
        }
    }
}
