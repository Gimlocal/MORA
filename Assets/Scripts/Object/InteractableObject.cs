using System;
using UnityEngine;

namespace Object
{
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        private Player.Player _player;

        private void Start()
        {
            _player = Player.Player.Instance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _player.playerAction.canMine = false;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (Input.GetKey(KeyCode.Z) && !canvas.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(true);
                    _player.playerMovement.canMove = false;
                    _player.playerMovement.StopPlayer();
                }

                if (canvas.gameObject.activeSelf && Input.GetKey(KeyCode.Z))
                {
                    canvas.gameObject.SetActive(false);
                    _player.playerMovement.canMove = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (canvas.gameObject.activeSelf) canvas.gameObject.SetActive(false);
                _player.playerAction.canMine = true;
                _player.playerMovement.canMove = true;
            }
        }
    }
}
