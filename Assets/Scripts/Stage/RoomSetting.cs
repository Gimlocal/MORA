using System;
using Cam;
using UI;
using UnityEngine;

namespace Stage
{
    public class RoomSetting : MonoBehaviour
    {
        public bool setPlayer = true;
        private ParticleSystem _particle;
        private CameraChange _cameraChange;
        private MiniMapController _minimapController;

        private void Awake()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
            _cameraChange = GetComponent<CameraChange>();
            _minimapController = FindAnyObjectByType<MiniMapController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SetPlayerPosition(other.transform);
                _cameraChange.ChangeCamera();
                if (_particle != null)
                {
                    _particle?.Play();
                }
                if (!setPlayer)
                {
                    setPlayer = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (_particle != null)
                {
                    _particle?.Stop();
                }
            }
        }

        private void SetPlayerPosition(Transform player)
        {
            if (!setPlayer) return;
            
            Vector3 playerPos = player.position;
            Vector3 center = transform.position;
            Vector3 diff = center - playerPos;

            Vector2 dir = Mathf.Abs(diff.x) > Mathf.Abs(diff.y) ?
                new Vector3(Mathf.Sign(diff.x), 0) :
                new Vector3(0, Mathf.Sign(diff.y) / 2);

            player.position += (Vector3)dir;
            
            _minimapController.UpdateMiniMap();
        }
    }
}
