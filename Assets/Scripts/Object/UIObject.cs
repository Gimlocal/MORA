using UI;
using UnityEngine;

namespace Object
{
    public class UIObject : InteractableObject
    {
        [SerializeField] private Canvas uI;
        [SerializeField] private Canvas text;
        private UIBase _uIBase;

        protected override void Start()
        {
            base.Start();
            _uIBase = uI.GetComponentInChildren<UIBase>(true);
        }

        protected override void ManageUI()
        {
            if (IsPlayerInRange && Input.GetKeyDown(KeyCode.Z) && !GameManager.UIManager.isOptionOpened)
            {
                bool isActive = uI.gameObject.activeSelf;
                uI.gameObject.SetActive(!isActive);
                if (!isActive)
                {
                    GameManager.UIManager.RegisterUI(_uIBase);
                    Player.playerMovement.canMove = false;
                    Player.playerMovement.StopPlayer();
                }
                else
                {
                    GameManager.UIManager.UnRegisterUI(_uIBase);
                    if (GameManager.UIManager.uIList.Count == 0)
                    {
                        Player.playerMovement.canMove = true;
                    }
                }
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
