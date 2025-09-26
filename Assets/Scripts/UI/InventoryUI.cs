using System.Collections.Generic;
using Mush;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class InventoryUI : ItemUI
    {
        public GameObject itemButtonPrefab;
        public Transform itemListParent;
        public Image itemImage;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemDescriptionText;
        public TextMeshProUGUI goldAmount;

        private Dictionary<ItemId, int> _ownedItems;
        private List<ItemId> _itemKeys = new();
        
        [SerializeField] private ItemDatabase itemDatabase;

        private void OnEnable()
        {
            Player.Player.Instance.playerItem.OnItemChanged += RefreshInventory;

            LoadItemsFromPlayer();
            DisplayItemList();
            UpdateItemInfoUI();
            UpdateGoldAmount();
        }

        private void OnDisable()
        {
            Player.Player.Instance.playerItem.OnItemChanged -= RefreshInventory;
        }
        
        private void RefreshInventory()
        {
            LoadItemsFromPlayer();
            DisplayItemList();
            UpdateItemInfoUI();
            UpdateGoldAmount();
        }

        protected override void Act()
        {
            
        }

        private void LoadItemsFromPlayer()
        {
            _ownedItems = Player.Player.Instance.playerItem.OwnedItems;
        }
        
        private void UpdateGoldAmount()
        {
            goldAmount.text = Player.Player.Instance.playerItem.gold.ToString();
        }

        private void DisplayItemList()
        {
            foreach (Transform child in itemListParent)
                Destroy(child.gameObject);
            
            ItemButtons.Clear();
            _itemKeys.Clear();

            int index = 0;
            foreach (var id in _ownedItems.Keys)
            {
                if (_ownedItems[id] == 0) continue;
                
                _itemKeys.Add(id); // 키 저장
                GameObject buttonObj = Instantiate(itemButtonPrefab, itemListParent);
                var itemData = itemDatabase.GetItemById(id);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = itemData.isMush ? 
                    $"{itemDatabase.GetItemById(id).itemName}  x{_ownedItems[id]}" :
                    $"{itemDatabase.GetItemById(id).itemName}";

                int capturedIndex = index; // 캡처한 인덱스
                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SelectedIndex = capturedIndex;
                    HighlightSelectedItem();
                    UpdateItemInfoUI();
                });

                index++;
                ItemButtons.Add(buttonObj);
            }


            HighlightSelectedItem();
        }

        protected override void MoveSelection(int dir)
        {
            SelectedIndex += dir;
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _itemKeys.Count - 1);
            HighlightSelectedItem();
            UpdateItemInfoUI();
        }

        private void UpdateItemInfoUI()
        {
            if (_itemKeys.Count == 0)
            {
                itemImage.sprite = null;
                itemImage.gameObject.SetActive(false);
                itemNameText.text = "";
                itemDescriptionText.text = "";
                return;
            }
            
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _itemKeys.Count - 1);
            HighlightSelectedItem();

            ItemId id = _itemKeys[SelectedIndex];
            var itemData = itemDatabase.GetItemById(id);

            itemImage.gameObject.SetActive(true);
            itemImage.sprite = itemData.sprite;
            itemNameText.text = itemData.itemName;
            itemDescriptionText.text = itemData.description;
        }
    }
}
