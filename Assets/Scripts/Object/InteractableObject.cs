using System;
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

        private void Start()
        {
            _player = Player.Player.Instance;
        }

        private void Update()
        {
            if (_isPlayerInRange && Input.GetKeyDown(KeyCode.Z))
            {
                bool isActive = uI.gameObject.activeSelf;
                uI.gameObject.SetActive(!isActive);
                _player.playerMovement.canMove = isActive;
                if (!isActive)
                    _player.playerMovement.StopPlayer();
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
