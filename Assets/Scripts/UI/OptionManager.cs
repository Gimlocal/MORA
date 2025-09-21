using UnityEngine;

namespace UI
{
    public class OptionManager : MonoBehaviour
    {
        private bool _optionOpened; 
        [SerializeField] private GameObject optionPanel;
        private UIBase _uIBase;
        
        private void Awake()
        {
            _uIBase = optionPanel.GetComponentInChildren<UIBase>();
        }

        private void Update()
        {
            ManageOption();
        }

        private void ManageOption()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                bool opened = optionPanel.activeSelf;
                if (!opened && GameManager.UIManager.uIList.Count == 0)
                {
                    GameManager.UIManager.isOptionOpened = true;
                    optionPanel.SetActive(true);
                }
                else if (opened)
                {
                    GameManager.UIManager.isOptionOpened = false;
                    optionPanel.SetActive(false);
                }
            }
        }
    }
}
