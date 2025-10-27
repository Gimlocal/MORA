using Unity.Cinemachine;
using UnityEngine;

namespace Cam
{
    public class CameraChange : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (CameraManager.Instance.cinemachineCamera != null) CameraManager.Instance.cinemachineCamera.Priority = 0;
                CameraManager.Instance.cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
                CameraManager.Instance.cinemachineCamera.Priority = 1;
            }
        }
    }
}
