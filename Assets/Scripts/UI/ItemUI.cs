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
        
        protected virtual void ManageMoveSelection()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                MoveSelection(1);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                MoveSelection(-1);
        }
        
        protected abstract void MoveSelection(int direction);
        
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
            
            float itemHeight = selected.rect.height;
            float contentHeight = content.rect.height;
            float viewportHeight = viewport.rect.height;
            
            float step = itemHeight / (contentHeight - viewportHeight);
            
            Vector3 itemWorldPos = selected.position;
            Vector3 itemLocalPos = viewport.InverseTransformPoint(itemWorldPos);
            
            if (itemLocalPos.y >= viewport.rect.height * 0.5f)
            {
                scrollRect.verticalScrollbar.value = Mathf.Clamp01(scrollRect.verticalScrollbar.value + step);
            }
            else if (itemLocalPos.y <= -viewport.rect.height * 0.5f)
            {
                scrollRect.verticalScrollbar.value = Mathf.Clamp01(scrollRect.verticalScrollbar.value - step);
            }

        }
    }
}
