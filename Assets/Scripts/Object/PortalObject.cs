using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Object
{
    public class PortalObject : MonoBehaviour
    {
        [SerializeField] private PortalID portalID;
        [SerializeField] private PortalDatabase portalDatabase;
        [SerializeField] private Canvas canvas;
        private PortalData _portalData;
        private bool _isPlayerInRange;

        private void Awake()
        {
            _portalData = portalDatabase.GetPortal(portalID);
        }

        private void Update()
        {
            if (_isPlayerInRange && Input.GetKeyDown(KeyCode.Z))
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(_portalData.targetScene);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInRange = true;
                Player.Player.Instance.playerAction.canMine = false;
                canvas.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInRange = false;
                Player.Player.Instance.playerAction.canMine = true;
                canvas.gameObject.SetActive(false);
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Player.Player.Instance.playerAction.canMine = true;
            Player.Player.Instance.transform.position = _portalData.targetPos;
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
