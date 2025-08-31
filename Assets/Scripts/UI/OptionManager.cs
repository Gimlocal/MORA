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
                optionPanel.SetActive(!_optionOpened);
                _optionOpened = !_optionOpened;
                if (_optionOpened)
                {
                    GameManager.UIManager.RegisterUI(_uIBase);
                    Player.Player.Instance.playerMovement.StopPlayer();
                }
                else
                {
                    GameManager.UIManager.UnRegisterUI(_uIBase);
                }
                
                if (_optionOpened || GameManager.UIManager.uIList.Count == 0)
                    Player.Player.Instance.playerMovement.canMove = !_optionOpened;
            }
        }
    }
}
