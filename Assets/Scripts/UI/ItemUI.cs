using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class ItemUI : UIBase
    {
        protected List<GameObject> ItemButtons = new();
        protected int SelectedIndex = 0;
        [SerializeField] private ScrollRect scrollRect;
        
        private void Update()
        {
            if (top)
            {
                ManageMoveSelection();
                Act();
            }
        }

        protected abstract void Act();
        
        private void ManageMoveSelection()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                MoveSelection(1);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                MoveSelection(-1);
        }
        
        protected  abstract void MoveSelection(int direction);
        
        protected void HighlightSelectedItem()
        {
            for (int i = 0; i < ItemButtons.Count; i++)
            {
                var image = ItemButtons[i].GetComponent<Image>();
                image.color = (i == SelectedIndex) ? Color.gray : Color.white;
            }
            ScrollToSelected();
        }
        
        private void ScrollToSelected()
        {
            if (SelectedIndex < 0 || SelectedIndex >= ItemButtons.Count) return;

            var selected = ItemButtons[SelectedIndex].GetComponent<RectTransform>();
            var viewport = scrollRect.viewport;
            var content = scrollRect.content;

// 아이템 높이와 전체 content 높이
            float itemHeight = selected.rect.height;
            float contentHeight = content.rect.height;
            float viewportHeight = viewport.rect.height;

// 스크롤 이동 비율 (아이템 하나 크기만큼)
            float step = itemHeight / (contentHeight - viewportHeight);

// 아이템 위치를 viewport 기준 좌표로 변환
            Vector3 itemWorldPos = selected.position;
            Vector3 itemLocalPos = viewport.InverseTransformPoint(itemWorldPos);

// 위쪽 벗어남 → 스크롤 한 칸 위로
            if (itemLocalPos.y >= viewport.rect.height * 0.5f)
            {
                scrollRect.verticalScrollbar.value = Mathf.Clamp01(scrollRect.verticalScrollbar.value + step);
            }
// 아래쪽 벗어남 → 스크롤 한 칸 아래로
            else if (itemLocalPos.y <= -viewport.rect.height * 0.5f)
            {
                scrollRect.verticalScrollbar.value = Mathf.Clamp01(scrollRect.verticalScrollbar.value - step);
            }

        }
    }
}
