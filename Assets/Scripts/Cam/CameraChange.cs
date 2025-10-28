using System.Collections;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Cam
{
    public class CameraChange : MonoBehaviour
    {
        private CinemachineBrain _brain;

        private void Awake()
        {
            if (Camera.main != null)
            {
                _brain = Camera.main.GetComponent<CinemachineBrain>();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(StopPlayer());
                if (CameraManager.Instance.cinemachineCamera != null)
                {
                    CameraManager.Instance.cinemachineCamera.Priority = 0;
                }
                CameraManager.Instance.cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
                CameraManager.Instance.cinemachineCamera.Priority = 1;
            }
        }
        
        private IEnumerator StopPlayer()
        {
            PlayerMovement movement = Player.Player.Instance.playerMovement;
            movement.canMove = false;
            movement.StopPlayer();
            yield return new WaitForSeconds(_brain.DefaultBlend.Time * 1.5f);
            movement.canMove = true;
        }
    }
}
