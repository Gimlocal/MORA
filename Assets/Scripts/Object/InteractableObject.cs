using System;
using UnityEngine;

namespace Object
{
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
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
                bool isActive = canvas.gameObject.activeSelf;
                canvas.gameObject.SetActive(!isActive);
                _player.playerMovement.canMove = isActive;
                if (!isActive)
                    _player.playerMovement.StopPlayer();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInRange = true;
                Player.Player.Instance.playerAction.canMine = false;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInRange = false;
                Player.Player.Instance.playerAction.canMine = true;
                if (canvas.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(false);
                    _player.playerMovement.canMove = true;
                }
            }
        }
    }
}
