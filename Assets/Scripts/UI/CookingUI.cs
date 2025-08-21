using System;
using System.Collections.Generic;
using Mush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CookingUI : MonoBehaviour
    {
        [SerializeField] private MushFoodDatabase mushFoodDatabase;
        [SerializeField] private MushDatabase mushDatabase;
        [SerializeField] private GameObject itemButtonPrefab;
        [SerializeField] private Transform itemListParent;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemIngredientsText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        
        private int _selectedIndex = 0;
        private MushFoodInfo[] _mushFoodInfo;
        private List<GameObject> _itemButtons = new();

        private void Start()
        {
            _mushFoodInfo = mushFoodDatabase.mushFoodInfo;
            DisplayItemList();
            UpdateItemInfoUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                MoveSelection(1);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                MoveSelection(-1);
        }
        
        private void MoveSelection(int dir)
        {
            _selectedIndex += dir;
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _mushFoodInfo.Length - 1);
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
                    _selectedIndex = capturedIndex;
                    HighlightSelectedItem();
                });
                
                index++;
                _itemButtons.Add(buttonObj);
            }
            
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
            if (_mushFoodInfo.Length == 0) return;

            var itemData =  _mushFoodInfo[_selectedIndex];

            itemImage.sprite = itemData.sprite;
            itemNameText.text = itemData.name;
            foreach (var text in itemData.ingredients)
            {
                itemIngredientsText.text += mushDatabase.GetPieceById(text.mushId).itemName;
                itemIngredientsText.text += $" x{text.amount}\n";
            }
            itemDescriptionText.text = itemData.description;
        }
    }
}
