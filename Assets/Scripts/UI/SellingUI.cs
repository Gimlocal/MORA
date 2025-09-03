using System;
using System.Collections.Generic;
using System.Linq;
using Mush;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioType = UnityEngine.AudioType;

namespace UI
{
    public class SellingUI : UIBase
    {
        public GameObject itemButtonPrefab;
        public Transform itemListParent;
        public TextMeshProUGUI goldAmount;

        private Dictionary<MushId, int> _ownedItems;
        private List<GameObject> _itemButtons = new();
        private int _selectedIndex = 0;
        private List<MushId> _itemKeys = new();
        private Player.Player _player;
        
        [SerializeField] private MushDatabase mushDatabase;

        private void Start()
        {
            _player = Player.Player.Instance;
        }

        private void OnEnable()
        {
            Player.Player.Instance.playerItem.OnItemChanged += RefreshInventory;

            UpdateGoldAmount();
            LoadItemsFromPlayer();
            DisplayItemList();
        }

        private void OnDisable()
        {
            Player.Player.Instance.playerItem.OnItemChanged -= RefreshInventory;
        }
        
        private void RefreshInventory()
        {
            UpdateGoldAmount();
            LoadItemsFromPlayer();
            DisplayItemList();
        }

        private void Update()
        {
            if (top)
            {
                ManageMoveSelection();
                SellItem();
            }
        }

        private void ManageMoveSelection()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                MoveSelection(1);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                MoveSelection(-1);
        }

        private void SellItem()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_itemKeys.Count <= 0) return;
                if (_ownedItems[_itemKeys[_selectedIndex]] == 0) return;
                _player.playerItem.gold += mushDatabase.GetPieceById(_itemKeys[_selectedIndex]).value;
                _player.playerItem.UseItem(_itemKeys[_selectedIndex]);
                SoundManager.Instance.Play(Sound.AudioType.UI);
                RefreshInventory();
            }
        }

        private void UpdateGoldAmount()
        {
            goldAmount.text = Player.Player.Instance.playerItem.gold.ToString();
        }

        private void LoadItemsFromPlayer()
        {
            _ownedItems = Player.Player.Instance.playerItem.OwnedItems;
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
                _itemKeys.Add(id);
                GameObject buttonObj = Instantiate(itemButtonPrefab, itemListParent);
                MushInfo mushInfo = mushDatabase.GetPieceById(id);
                buttonObj.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != buttonObj)!.sprite = mushInfo.sprite;
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{mushInfo.itemName}  x{_ownedItems[id]}\n판매가격 : {mushInfo.value}";

                int capturedIndex = index;
                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _selectedIndex = capturedIndex;
                    HighlightSelectedItem();
                });

                index++;
                _itemButtons.Add(buttonObj);
            }
            
            _selectedIndex = _itemKeys.Count > 0 ? Math.Clamp(_selectedIndex, 0, _itemKeys.Count - 1) : 0;
            HighlightSelectedItem();
        }

        private void MoveSelection(int dir)
        {
            _selectedIndex += dir;
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _itemKeys.Count - 1);
            HighlightSelectedItem();
        }

        private void HighlightSelectedItem()
        {
            for (int i = 0; i < _itemButtons.Count; i++)
            {
                var image = _itemButtons[i].GetComponent<Image>();
                image.color = (i == _selectedIndex) ? Color.gray : Color.white;
            }
        }
    }
}
