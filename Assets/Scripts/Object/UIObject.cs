using UI;
using UnityEngine;

namespace Object
{
    public class UIObject : InteractableObject
    {
        [SerializeField] private Canvas uI;
        [SerializeField] private Canvas text;
        private UIBase _uiBase;

        protected override void Start()
        {
            base.Start();
            _uiBase = uI.GetComponentInChildren<UIBase>(true);
        }

        protected override void ManageUI()
        {
            if (IsPlayerInRange && Input.GetKeyDown(KeyCode.Z))
            {
                bool isActive = uI.gameObject.activeSelf;
                uI.gameObject.SetActive(!isActive);
                if (!isActive)
                {
                    GameManager.UIManager.RegisterUI(_uiBase);
                    Player.playerMovement.StopPlayer();
                }
                else
                {
                    GameManager.UIManager.UnRegisterUI(_uiBase);
                }
                if (!isActive || GameManager.UIManager.uIList.Count == 0)
                    Player.playerMovement.canMove = isActive;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other.CompareTag("Player"))
            {
                text.gameObject.SetActive(true);
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            base.OnTriggerExit2D(other);
            if (other.CompareTag("Player"))
            {
                text.gameObject.SetActive(false);
            }
            if (uI.gameObject.activeSelf)
            {
                uI.gameObject.SetActive(false);
                Player.playerMovement.canMove = true;
            }
        }
    }
}
