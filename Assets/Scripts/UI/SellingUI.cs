using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Mush;
using Object;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class SellingUI : InventoryUI
    {
        [SerializeField] private MushDatabase mushDatabase;
        
        public GameObject itemButtonPrefab;
        public Transform itemListParent;
        public TextMeshProUGUI goldAmount;

        private Dictionary<MushId, int> _ownedItems;
        private List<MushId> _itemKeys = new();
        private Player.Player _player;

        private void Start()
        {
            _player = Player.Player.Instance;
        }

        private void OnEnable()
        {
            Player.Player.Instance.playerItem.OnMushChanged += RefreshInventory;

            UpdateGoldAmount();
            LoadItemsFromPlayer();
            DisplayItemList();
        }

        private void OnDisable()
        {
            Player.Player.Instance.playerItem.OnMushChanged -= RefreshInventory;
        }
        
        private void RefreshInventory()
        {
            UpdateGoldAmount();
            LoadItemsFromPlayer();
            DisplayItemList();
        }

        protected override void Act()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_itemKeys.Count <= 0) return;
                if (_ownedItems[_itemKeys[SelectedIndex]] == 0) return;
                _player.playerItem.gold += mushDatabase.GetItemById(_itemKeys[SelectedIndex]).value;
                // if (_itemKeys[SelectedIndex] == ItemId.Lantern)
                // {
                //     Player.Player.Instance.playerItem.hasLantern = false;
                // }
                _player.playerItem.UseMush(_itemKeys[SelectedIndex]);
                SoundManager.Instance.Play(AudioCategory.UI, "Success");
                RefreshInventory();
            }
        }

        private void UpdateGoldAmount()
        {
            goldAmount.text = Player.Player.Instance.playerItem.gold.ToString();
        }

        private void LoadItemsFromPlayer()
        {
            _ownedItems = Player.Player.Instance.playerItem.OwnedMushes;
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
                _itemKeys.Add(id);
                GameObject buttonObj = Instantiate(itemButtonPrefab, itemListParent);
                MushInfo mushInfo = mushDatabase.GetItemById(id);
                buttonObj.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != buttonObj)!.sprite = mushInfo.sprite;
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = 
                    $"{mushInfo.mushName}  x{_ownedItems[id]}\n가공 : {mushInfo.value}";

                int capturedIndex = index;
                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SelectedIndex = capturedIndex;
                    HighlightSelectedItem();
                });

                index++;
                ItemButtons.Add(buttonObj);
            }
            
            SelectedIndex = _itemKeys.Count > 0 ? Math.Clamp(SelectedIndex, 0, _itemKeys.Count - 1) : 0;
            HighlightSelectedItem();
        }

        protected override void MoveSelection(int dir)
        {
            SelectedIndex += dir;
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _itemKeys.Count - 1);
            HighlightSelectedItem();
        }
    }
}
