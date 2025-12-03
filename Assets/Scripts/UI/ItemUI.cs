using System;
using System.Collections.Generic;
using Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ItemUI : InventoryUI
    {
        [SerializeField] private ItemDatabase itemDatabase;

        public GameObject itemButtonPrefab;
        public Transform itemListParent;
        public Image itemImage;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDescription;

        private Dictionary<ItemId, int> _ownedItems;
        private List<ItemId> _itemKeys = new();
        private const int UIIndex = 1;
        private UIBase[] _uiBases;
        
        private void Start()
        {
            _uiBases = GetComponentInParent<InventoryManager>().uIBases;
        }

        private void OnEnable()
        {
            Player.Player.Instance.playerItem.OnItemChanged += RefreshInventory;
            RefreshInventory();
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
        }

        protected override void Act()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var info = itemDatabase.GetItemById(_itemKeys[SelectedIndex]);
                itemDatabase.UseItem(info.itemId);
                if (info.oneTime)
                {
                    _ownedItems[info.itemId]--;
                }
            }
        }
        
        protected override void ManageMoveSelection()
        {
            base.ManageMoveSelection();

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                int idx = Mod(UIIndex - 1, _uiBases.Length);
                GameManager.UIManager.RegisterUI(_uiBases[idx]);
                GameManager.UIManager.UnRegisterUI(this);
                _uiBases[idx].gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                int idx = Mod(UIIndex + 1, _uiBases.Length);
                GameManager.UIManager.RegisterUI(_uiBases[idx]);
                GameManager.UIManager.UnRegisterUI(this);
                _uiBases[idx].gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
        
        private int Mod(int a, int b)
        {
            return  (a % b + b) % b;
        }

        private void LoadItemsFromPlayer()
        {
            _ownedItems = Player.Player.Instance.playerItem.OwnedItems;
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
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{itemDatabase.GetItemById(id).itemName}";

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
                itemName.text = "";
                itemDescription.text = "";
                return;
            }
            
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _itemKeys.Count - 1);
            HighlightSelectedItem();

            ItemId id = _itemKeys[SelectedIndex];
            var itemData = itemDatabase.GetItemById(id);

            itemImage.gameObject.SetActive(true);
            itemImage.sprite = itemData.sprite;
            itemName.text = itemData.itemName;
            itemDescription.text = itemData.description;
        }
    }
}
