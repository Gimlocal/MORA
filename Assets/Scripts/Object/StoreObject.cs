using Sound;
using UnityEngine;
using AudioType = Sound.AudioType;

namespace Object
{
    public class StoreObject : InteractableObject
    {
        [SerializeField] private Canvas text;
        
        protected override void ManageUI()
        {
            if (IsPlayerInRange && Input.GetKeyDown(KeyCode.Z))
            {
                Player.playerStat.ResetCapacity();
                SoundManager.Instance.Play(AudioType.UI);
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
