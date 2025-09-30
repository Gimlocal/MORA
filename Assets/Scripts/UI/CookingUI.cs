using System.Collections.Generic;
using Database;
using Mush;
using Object;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CookingUI : ItemUI
    {
        [SerializeField] private MushFoodDatabase mushFoodDatabase;
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private GameObject itemButtonPrefab;
        [SerializeField] private Transform itemListParent;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemIngredientsText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        
        private MushFoodInfo[] _mushFoodInfo;

        private void Start()
        {
            _mushFoodInfo = mushFoodDatabase.mushFoodInfo;
            DisplayItemList();
            UpdateItemInfoUI();
        }

        protected override void Act()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Dictionary<ItemId, int> ownedItems = Player.Player.Instance.playerItem.OwnedItems;
                var foodInfo = _mushFoodInfo[SelectedIndex];
                bool canCook = true;
                foreach (var info in foodInfo.ingredients)
                {
                    if (ownedItems.ContainsKey(info.mushId) && ownedItems[info.mushId] >= info.amount)
                        continue;
                    canCook = false;
                    break;
                }

                if (canCook)
                {
                    foreach (var info in foodInfo.ingredients)
                    {
                        Player.Player.Instance.playerItem.UseItem(info.mushId, info.amount);
                    }
                    mushFoodDatabase.EatMushFood(foodInfo.mushFoodId);
                    SoundManager.Instance.Play(AudioCategory.UI, "Success");
                }
                else
                {
                    SoundManager.Instance.Play(AudioCategory.UI, "Fail");
                }
            }
        }
        
        protected override void MoveSelection(int dir)
        {
            SelectedIndex += dir;
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _mushFoodInfo.Length - 1);
            UpdateItemInfoUI();
            HighlightSelectedItem();
        }

        private void DisplayItemList()
        {
            foreach (Transform child in itemListParent)
                Destroy(child.gameObject);

            int index = 0;
            foreach (var info in _mushFoodInfo)
            {
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
            
            HighlightSelectedItem();
        }
        
        
        
        private void UpdateItemInfoUI()
        {
            if (_mushFoodInfo.Length == 0) return;

            var itemData =  _mushFoodInfo[SelectedIndex];

            itemImage.sprite = itemData.sprite;
            itemNameText.text = itemData.name;
            itemIngredientsText.text = "";
            foreach (var text in itemData.ingredients)
            {
                itemIngredientsText.text += itemDatabase.GetItemById(text.mushId).itemName;
                itemIngredientsText.text += $" x{text.amount}\n";
            }
            itemDescriptionText.text = itemData.description;
        }
    }
}
