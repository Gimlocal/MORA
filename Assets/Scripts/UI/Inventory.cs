using System.Collections.Generic;
using Mush;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class Inventory : MonoBehaviour
    {
        public GameObject itemButtonPrefab;  // List에 들어갈 버튼 프리팹
        public Transform itemListParent;     // List 오브젝트
        public Image itemImage;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemDescriptionText;

        private Dictionary<MushId, int> _ownedItems;
        private List<GameObject> _itemButtons = new();
        private int _selectedIndex = 0;
        private List<MushId> _itemKeys = new();
        
        [SerializeField] private MushDatabase mushDatabase;

        private void OnEnable()
        {
            LoadItemsFromPlayer();
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
            if (_itemKeys.Count == 0) return;

            MushId id = _itemKeys[_selectedIndex];
            var itemData = mushDatabase.GetPieceById(id);

            itemImage.sprite = itemData.sprite;
            itemNameText.text = itemData.itemName;
            itemDescriptionText.text = itemData.description;
        }

    }
}
