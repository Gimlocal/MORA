using System;
using System.Collections.Generic;
using Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuyingUI : UIBase
    {
        [SerializeField] private ProductDatabase productDatabase;
        [SerializeField] private GameObject itemButtonPrefab;
        [SerializeField] private Transform itemListParent;
        [SerializeField] private TextMeshProUGUI goldAmount;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        
        private int _selectedIndex = 0;
        private List<GameObject> _itemButtons = new();
        private ProductData[] _productData;
        private Player.PlayerItem _playerItem;
        private List<ProductID> _productIDs = new();

        private void Awake()
        {
            _productData = productDatabase.products;
            _playerItem = Player.Player.Instance.playerItem;
            DisplayItemList();
            UpdateItemInfoUI();
            UpdateGoldAmount();
        }

        private void Update()
        {
            if (top)
            {
                ManageMoveSelection();
                BuyItem();
            }
        }

        private void OnEnable()
        {
            _playerItem.OnGoldChanged += UpdateGoldAmount;
            UpdateGoldAmount();
        }

        private void OnDisable()
        {
            _playerItem.OnGoldChanged -= UpdateGoldAmount;
        }
        
        private void UpdateGoldAmount()
        {
            goldAmount.text = _playerItem.gold.ToString();
        }

        private void ManageMoveSelection()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                MoveSelection(1);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                MoveSelection(-1);
        }

        private void BuyItem()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var productInfo = productDatabase.GetProductById(_productIDs[_selectedIndex]);
                
                if (productInfo.productID == ProductID.Spacesuit)
                {
                    if (_playerItem.gold >= productInfo.price * (_playerItem.suitLevel + 1) && _playerItem.suitLevel < 2)
                    {
                        _playerItem.UseGold(productInfo.price * (_playerItem.suitLevel + 1));
                        productDatabase.GetEffect(productInfo.productID);
                    }
                }
                else
                {
                    if (_playerItem.gold >= productInfo.price)
                    {
                        _playerItem.UseGold(productInfo.price);
                        productDatabase.GetEffect(productInfo.productID);
                    }
                }
                
                DisplayItemList();
                UpdateItemInfoUI();
            }
        }
        
        private void MoveSelection(int dir)
        {
            _selectedIndex += dir;
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _itemButtons.Count - 1);
            UpdateItemInfoUI();
            HighlightSelectedItem();
        }

        private void DisplayItemList()
        {
            foreach (Transform child in itemListParent)
                Destroy(child.gameObject);
            
            _itemButtons.Clear();
            _productIDs.Clear();

            int index = 0;
            foreach (var info in _productData)
            {
                if (info.productID == ProductID.Spacesuit && _playerItem.suitLevel == 2)
                    continue;
                if (info.productID == ProductID.HomeTicket && _playerItem.canGoHome)
                    continue;
                
                _productIDs.Add(info.productID);
                GameObject buttonObj = Instantiate(itemButtonPrefab, itemListParent);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = info.name;
                
                int capturedIndex = index;
                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _selectedIndex = capturedIndex;
                    HighlightSelectedItem();
                });
                
                index++;
                _itemButtons.Add(buttonObj);
            }
            
            _selectedIndex = _productIDs.Count > 0 ? Math.Clamp(_selectedIndex, 0, _productIDs.Count - 1) : 0;
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
        
        private void UpdateItemInfoUI()
        {
            if (_productData.Length == 0) return;

            if (_productIDs.Count == 0)
            {
                itemImage.gameObject.SetActive(false);
                itemNameText.text = "";
                itemDescriptionText.text = "";
                return;
            }

            var itemData =  productDatabase.GetProductById(_productIDs[_selectedIndex]);
            
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = itemData.sprite;
            itemNameText.text = itemData.name;
            itemDescriptionText.text = itemData.description;
            if (itemData.productID == ProductID.Spacesuit)
            {
                itemNameText.text += $" : {(itemData.price *  (_playerItem.suitLevel + 1)).ToString()}원";
            }
            else
            {
                itemNameText.text += $" : {(itemData.price).ToString()}원";
            }
        }
    }
}
