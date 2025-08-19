using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Object
{
    public class CameraSetting : MonoBehaviour
    {
        private CinemachineCamera _camera;
        private CinemachineConfiner2D _confiner;

        private void Awake()
        {
            _camera = GetComponent<CinemachineCamera>();
            _confiner = GetComponent<CinemachineConfiner2D>();
        }

        private void Start()
        {
            _camera.Follow = Player.Player.Instance.transform;
            _confiner.BoundingShape2D = GetComponentInParent<PolygonCollider2D>();
        }
    }
}
