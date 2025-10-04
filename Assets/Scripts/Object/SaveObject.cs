using System;
using Database;
using Sound;
using UnityEngine;

namespace Object
{
    public class SaveObject : InteractableObject
    {
        [SerializeField] private Canvas text;
        
        protected override void ManageUI()
        {
            if (IsPlayerInRange && Input.GetKeyDown(KeyCode.Z))
            {
                DataManager.SaveData();
                SoundManager.Instance.Play(AudioCategory.UI, "Success");
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
