using System;
using Cam;
using UI;
using UnityEngine;

namespace Stage
{
    public class RoomSetting : MonoBehaviour
    {
        public static RoomSetting CurrentRoom { get; private set; }
        
        public bool setPlayer = true;
        
        protected bool IsInRoom;
        
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
                if (IsInRoom)
                {
                    return;
                }
                IsInRoom = true;
                
                if (CurrentRoom != null && CurrentRoom != this)
                {
                    CurrentRoom.OnPlayerLeftRoom();
                }
                CurrentRoom = this;
                
                SetPlayerPosition(other.transform);
                
                _cameraChange.ChangeCamera();
                
                if (!setPlayer)
                {
                    setPlayer = true;
                }
                
                if (_particle != null)
                {
                    _particle?.Play();
                }
            }
        }
        
        protected virtual void OnPlayerLeftRoom()
        {
            IsInRoom = false;
            if (_particle != null)
            {
                _particle.Stop();
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
