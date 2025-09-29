using System;
using System.Collections.Generic;
using Mush;
using Object;
using Sound;
using TMPro;
using Tool;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EquipmentUI : ItemUI
    {
        public GameObject itemButtonPrefab;
        public Transform itemListParent;
        public Image itemImage;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemDescriptionText;

        private List<Tools> _ownedTools;
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
                Player.Player.Instance.playerMining.toolName = _ownedTools[SelectedIndex].toolName;
                SoundManager.Instance.Play(AudioCategory.UI, "Success");
            }
        }

        protected override void ManageMoveSelection()
        {
            base.ManageMoveSelection();
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (UIIndex - 1 >= 0)
                {
                    GameManager.UIManager.RegisterUI(_uiBases[UIIndex - 1]);
                    GameManager.UIManager.UnRegisterUI(this);
                    _uiBases[UIIndex - 1].gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (UIIndex + 1 <= _uiBases.Length - 1)
                {
                    GameManager.UIManager.RegisterUI(_uiBases[UIIndex + 1]);
                    GameManager.UIManager.UnRegisterUI(this);
                    _uiBases[UIIndex + 1].gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }

        private void LoadItemsFromPlayer()
        {
            _ownedTools = Player.Player.Instance.playerMining.tools;
        }

        private void DisplayItemList()
        {
            foreach (Transform child in itemListParent)
                Destroy(child.gameObject);
            
            ItemButtons.Clear();

            int index = 0;
            foreach (var tool in _ownedTools)
            {
                GameObject buttonObj = Instantiate(itemButtonPrefab, itemListParent);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{tool.toolKName}";

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
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _ownedTools.Count - 1);
            HighlightSelectedItem();
            UpdateItemInfoUI();
        }

        private void UpdateItemInfoUI()
        {
            if (_ownedTools.Count == 0)
            {
                itemImage.sprite = null;
                itemImage.gameObject.SetActive(false);
                itemNameText.text = "";
                itemDescriptionText.text = "";
                return;
            }
            
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _ownedTools.Count - 1);
            HighlightSelectedItem();

            var tool = _ownedTools[SelectedIndex];
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = tool.GetComponent<SpriteRenderer>().sprite;
            itemNameText.text = tool.toolKName;
            itemDescriptionText.text = tool.toolDescription;
        }
    }
}
