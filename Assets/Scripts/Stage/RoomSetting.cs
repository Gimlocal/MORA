using System;
using Cam;
using UI;
using UnityEngine;

namespace Stage
{
    public class RoomSetting : MonoBehaviour
    {
        public bool setPlayer = true;
        public bool hasRoomLight = true;
        
        private ParticleSystem _particle;
        private CameraChange _cameraChange;
        private MiniMapController _minimapController;

        private void Awake()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
            _cameraChange = GetComponent<CameraChange>();
            _minimapController = FindAnyObjectByType<MiniMapController>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SetPlayerPosition(other.transform);
                Player.Player.Instance.playerItem.UseDefaultLantern(!hasRoomLight);
                
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

        protected virtual void OnTriggerExit2D(Collider2D other)
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
