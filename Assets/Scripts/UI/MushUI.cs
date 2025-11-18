using System;
using System.Collections.Generic;
using Database;
using Mush;
using Object;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class MushUI : InventoryUI
    {
        [SerializeField] private MushDatabase mushDatabase;
        
        public GameObject mushButtonPrefab;
        public Transform mushListParent;
        public Image mushImage;
        public TextMeshProUGUI mushNameText;
        public TextMeshProUGUI mushDescriptionText;
        public TextMeshProUGUI goldAmount;

        private Dictionary<MushId, int> _ownedMushes;
        private List<MushId> _mushKeys = new();
        private const int UIIndex = 0;
        private UIBase[] _uiBases;
        
        private void Start()
        {
            _uiBases = GetComponentInParent<InventoryManager>().uIBases;
        }

        private void OnEnable()
        {
            Player.Player.Instance.playerItem.OnMushChanged += RefreshInventory;
            RefreshInventory();
        }

        private void OnDisable()
        {
            Player.Player.Instance.playerItem.OnMushChanged -= RefreshInventory;
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
            _ownedMushes = Player.Player.Instance.playerItem.OwnedMushes;
        }
        
        private void UpdateGoldAmount()
        {
            goldAmount.text = Player.Player.Instance.playerItem.gold.ToString();
        }

        private void DisplayItemList()
        {
            foreach (Transform child in mushListParent)
                Destroy(child.gameObject);
            
            ItemButtons.Clear();
            _mushKeys.Clear();

            int index = 0;
            foreach (var id in _ownedMushes.Keys)
            {
                if (_ownedMushes[id] == 0) continue;
                
                _mushKeys.Add(id); // 키 저장
                GameObject buttonObj = Instantiate(mushButtonPrefab, mushListParent);
                var itemData = mushDatabase.GetItemById(id);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{mushDatabase.GetItemById(id).mushName}  x{_ownedMushes[id]}";

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
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _mushKeys.Count - 1);
            HighlightSelectedItem();
            UpdateItemInfoUI();
        }

        private void UpdateItemInfoUI()
        {
            if (_mushKeys.Count == 0)
            {
                mushImage.sprite = null;
                mushImage.gameObject.SetActive(false);
                mushNameText.text = "";
                mushDescriptionText.text = "";
                return;
            }
            
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _mushKeys.Count - 1);
            HighlightSelectedItem();

            MushId id = _mushKeys[SelectedIndex];
            var itemData = mushDatabase.GetItemById(id);

            mushImage.gameObject.SetActive(true);
            mushImage.sprite = itemData.sprite;
            mushNameText.text = itemData.mushName;
            mushDescriptionText.text = itemData.description;
        }
    }
}
