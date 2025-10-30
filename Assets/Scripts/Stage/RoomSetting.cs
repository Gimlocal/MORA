using System;
using Cam;
using UnityEngine;

namespace Stage
{
    public class RoomSetting : MonoBehaviour
    {
        private ParticleSystem _particle;
        private CameraChange _cameraChange;

        private void Awake()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
            _cameraChange = GetComponent<CameraChange>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _cameraChange.ChangeCamera();
                _particle.Play();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _particle.Stop();
            }
        }
    }
}
