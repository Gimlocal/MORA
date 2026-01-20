using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cam
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }
        public CinemachineCamera cinemachineCamera;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            cinemachineCamera ??= FindAnyObjectByType<CinemachineCamera>();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            cinemachineCamera ??= FindAnyObjectByType<CinemachineCamera>();
            
            if (scene.buildIndex == 0)
            {
                Destroy(gameObject);
                Instance = null;
            }
        }

        public void CameraShake()
        {
            cinemachineCamera.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
