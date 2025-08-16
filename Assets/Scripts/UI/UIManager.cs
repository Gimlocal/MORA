using System;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private bool _inventoryOpened;
        [SerializeField] private GameObject inventoryPanel;

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
            }
        }
    }
}
