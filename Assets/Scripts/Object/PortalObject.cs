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
        [SerializeField] private TextMeshProUGUI tmPro;
        [SerializeField] private String text;
        private PortalData _portalData;
        private bool _isPlayerInRange;

        private void Awake()
        {
            _portalData = portalDatabase.GetPortal(portalID);
            tmPro.text = text;
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
            canvas.gameObject.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _isPlayerInRange = false;
            Player.Player.Instance.playerAction.canMine = true;
            canvas.gameObject.SetActive(false);
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Player.Player.Instance.playerAction.canMine = true;
            Player.Player.Instance.transform.position = _portalData.targetPos;
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
