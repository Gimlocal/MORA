using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioType = UnityEngine.AudioType;

namespace Object
{
    public class ShipObject : InteractableObject
    {
        [SerializeField] private Canvas text;
        protected override void ManageUI()
        {
            if (IsPlayerInRange && Input.GetKeyDown(KeyCode.Z) && Player.playerItem.canGoHome)
            {
                SceneManager.LoadScene(5);
            }
        }
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other.CompareTag("Player"))
            {
                text.gameObject.SetActive(true);
            }
        }
        
        protected override void OnTriggerExit2D(Collider2D other)
        {
            base.OnTriggerExit2D(other);
            if (other.CompareTag("Player"))
            {
                text.gameObject.SetActive(false);
            }
        }
    }
}
