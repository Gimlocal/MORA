using System.Collections.Generic;
using Mush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryUI : UIBase
    {
        public GameObject itemButtonPrefab;
        public Transform itemListParent;
        public Image itemImage;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemDescriptionText;
        public TextMeshProUGUI goldAmount;

        private Dictionary<MushId, int> _ownedItems;
        private List<GameObject> _itemButtons = new();
        private int _selectedIndex = 0;
        private List<MushId> _itemKeys = new();
        
        [SerializeField] private MushDatabase mushDatabase;

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

        private void Update()
        {
            if (top)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    MoveSelection(1);
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                    MoveSelection(-1);
            }
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
            
            _itemButtons.Clear();
            _itemKeys.Clear();

            int index = 0;
            foreach (var id in _ownedItems.Keys)
            {
                if (_ownedItems[id] == 0) continue;
                
                _itemKeys.Add(id); // 키 저장
                GameObject buttonObj = Instantiate(itemButtonPrefab, itemListParent);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{mushDatabase.GetPieceById(id).itemName}  x{_ownedItems[id]}";

                int capturedIndex = index; // 캡처한 인덱스
                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _selectedIndex = capturedIndex;
                    HighlightSelectedItem();
                    UpdateItemInfoUI();
                });

                index++;
                _itemButtons.Add(buttonObj);
            }


            HighlightSelectedItem();
        }

        private void MoveSelection(int dir)
        {
            _selectedIndex += dir;
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _itemKeys.Count - 1);
            HighlightSelectedItem();
            UpdateItemInfoUI();
        }

        private void HighlightSelectedItem()
        {
            for (int i = 0; i < _itemButtons.Count; i++)
            {
                var image = _itemButtons[i].GetComponent<Image>();
                image.color = (i == _selectedIndex) ? Color.gray : Color.white; // 하이라이트 색상
            }
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
            
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _itemKeys.Count - 1);
            HighlightSelectedItem();

            MushId id = _itemKeys[_selectedIndex];
            var itemData = mushDatabase.GetPieceById(id);

            itemImage.gameObject.SetActive(true);
            itemImage.sprite = itemData.sprite;
            itemNameText.text = itemData.itemName;
            itemDescriptionText.text = itemData.description;
        }
    }
}
