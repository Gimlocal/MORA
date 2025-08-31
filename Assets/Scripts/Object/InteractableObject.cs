using System;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Object
{
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField] private Canvas uI;
        [SerializeField] private Canvas text;
        private Player.Player _player;
        private bool _isPlayerInRange;
        private UIBase _uiBase;

        private void Start()
        {
            _player = Player.Player.Instance;
            _uiBase = uI.GetComponentInChildren<UIBase>(true);
        }

        private void Update()
        {
            ManageUI();
        }

        private void ManageUI()
        {
            if (_isPlayerInRange && Input.GetKeyDown(KeyCode.Z))
            {
                bool isActive = uI.gameObject.activeSelf;
                uI.gameObject.SetActive(!isActive);
                if (!isActive)
                {
                    GameManager.UIManager.RegisterUI(_uiBase);
                    _player.playerMovement.StopPlayer();
                }
                else
                {
                    GameManager.UIManager.UnRegisterUI(_uiBase);
                }
                if (!isActive || GameManager.UIManager.uIList.Count == 0)
                    _player.playerMovement.canMove = isActive;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                text.gameObject.SetActive(true);
                _isPlayerInRange = true;
                Player.Player.Instance.playerAction.canMine = false;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                text.gameObject.SetActive(false);
                _isPlayerInRange = false;
                Player.Player.Instance.playerAction.canMine = true;
                if (uI.gameObject.activeSelf)
                {
                    uI.gameObject.SetActive(false);
                    _player.playerMovement.canMove = true;
                }
            }
        }
    }
}
