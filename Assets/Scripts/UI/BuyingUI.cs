using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Object;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuyingUI : ItemUI
    {
        [SerializeField] private ProductDatabase productDatabase;
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private GameObject itemButtonPrefab;
        [SerializeField] private Transform itemListParent;
        [SerializeField] private TextMeshProUGUI goldAmount;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        
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

        private void OnEnable()
        {
            _playerItem.OnGoldChanged += UpdateGoldAmount;
            DisplayItemList();
            UpdateItemInfoUI();
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

        protected override void Act()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var productInfo = productDatabase.GetProductById(_productIDs[SelectedIndex]);
                Dictionary<ItemId, int> ownedItems =  _playerItem.OwnedItems;
                
                if (productInfo.productID == ProductID.Spacesuit)
                {
                    if (CheckIngredients(productInfo,ownedItems, productInfo.price * (_playerItem.suitLevel + 1)) 
                        && _playerItem.suitLevel < 2)
                    {
                        _playerItem.UseGold(productInfo.price * (_playerItem.suitLevel + 1));
                        foreach (var ingredient in productInfo.ingredients)
                        {
                            _playerItem.UseItem(ingredient.mushId, ingredient.amount);
                        }
                        productDatabase.GetEffect(productInfo.productID);
                        SoundManager.Instance.Play(AudioCategory.UI, "Success");
                    }
                }
                else
                {
                    if (CheckIngredients(productInfo, ownedItems))
                    {
                        _playerItem.UseGold(productInfo.price);
                        foreach (var ingredient in productInfo.ingredients)
                        {
                            _playerItem.UseItem(ingredient.mushId, ingredient.amount);
                        }
                        productDatabase.GetEffect(productInfo.productID);
                        SoundManager.Instance.Play(AudioCategory.UI, "Success");
                    }
                }
                
                DisplayItemList();
                UpdateItemInfoUI();
            }
        }

        private bool CheckIngredients(ProductData data, Dictionary<ItemId, int> ownedItems, int price = 0)
        {
            bool flag = true;
            
            // 재료 확인
            foreach (var ingredient in data.ingredients)
            {
                if (!ownedItems.ContainsKey(ingredient.mushId) ||
                    !_playerItem.HasItem(ingredient.mushId, ingredient.amount))
                {
                    flag = false;
                }
            }
            // 가격 확인
            if (price == 0)
            {
                price = data.price;
            }
            if (price > _playerItem.gold)
            {
                flag = false;
            }

            return flag;
        }
        
        protected override void MoveSelection(int dir)
        {
            SelectedIndex += dir;
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, ItemButtons.Count - 1);
            UpdateItemInfoUI();
            HighlightSelectedItem();
        }

        private void DisplayItemList()
        {
            foreach (Transform child in itemListParent)
                Destroy(child.gameObject);
            
            ItemButtons.Clear();
            _productIDs.Clear();

            int index = 0;
            foreach (var info in _productData)
            {
                if (info.effect == ProductEffect.UpgradeSuit && _playerItem.suitLevel == 2)
                    continue;
                if (info.effect == ProductEffect.CanGoHome && _playerItem.canGoHome)
                    continue;
                if (info.effect == ProductEffect.Equipment &&
                    Player.Player.Instance.playerMining.tools.Any(t => t.name == info.productID.ToString()))
                    continue;
                if (info.effect == ProductEffect.Item && 
                    Player.Player.Instance.playerItem.OwnedItems.
                        Any(t => t.Key.ToString() == info.productID.ToString()))
                    continue;
                
                
                _productIDs.Add(info.productID);
                GameObject buttonObj = Instantiate(itemButtonPrefab, itemListParent);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = info.name;
                
                int capturedIndex = index;
                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SelectedIndex = capturedIndex;
                    HighlightSelectedItem();
                });
                
                index++;
                ItemButtons.Add(buttonObj);
            }
            
            SelectedIndex = _productIDs.Count > 0 ? Math.Clamp(SelectedIndex, 0, _productIDs.Count - 1) : 0;
            HighlightSelectedItem();
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

            var itemData =  productDatabase.GetProductById(_productIDs[SelectedIndex]);
            
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = itemData.sprite;
            itemNameText.text = "";
            itemDescriptionText.text = itemData.description;
            if (itemData.productID == ProductID.Spacesuit)
            {
                for (int i = 0; i < itemData.ingredients.Count; i++)
                {
                    itemNameText.text += 
                        $"{itemDatabase.GetItemById(itemData.ingredients[i].mushId).itemName}x{itemData.ingredients[i].amount}";
                    if (i % 3 == 2)
                    {
                        itemNameText.text += "\n";
                    }
                    else
                    {
                        if (i < itemData.ingredients.Count - 1)
                        {
                            itemNameText.text += "   ";
                        }
                        else
                        {
                            itemNameText.text += "\n";
                        }
                    }
                }
                itemNameText.text += $"{(itemData.price *  (_playerItem.suitLevel + 1)).ToString()}원";
            }
            else
            {
                for (int i = 0; i < itemData.ingredients.Count; i++)
                {
                    itemNameText.text += 
                        $"{itemDatabase.GetItemById(itemData.ingredients[i].mushId).itemName}x{itemData.ingredients[i].amount}";
                    if (i % 3 == 2)
                    {
                        itemNameText.text += "\n";
                    }
                    else
                    {
                        if (i < itemData.ingredients.Count - 1)
                        {
                            itemNameText.text += "   ";
                        }
                        else
                        {
                            itemNameText.text += "\n";
                        }
                    }
                }
                itemNameText.text += $"{(itemData.price).ToString()}원";
            }
        }
    }
}
