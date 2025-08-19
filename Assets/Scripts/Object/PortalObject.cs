using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Object
{
    public class PortalObject : MonoBehaviour
    {
        [SerializeField] private PortalID portalID;
        [SerializeField] private PortalDatabase portalDatabase;
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
                Player.Player.Instance.playerAction.canMine = false;
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(_portalData.targetScene);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _isPlayerInRange = true;
            Player.Player.Instance.playerAction.canMine = false;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _isPlayerInRange = false;
            Player.Player.Instance.playerAction.canMine = true;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Player.Player.Instance.playerAction.canMine = true;
            Player.Player.Instance.transform.position = _portalData.targetPos;
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
