using System;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Object
{
    public abstract class InteractableObject : MonoBehaviour
    {
        protected Player.Player Player;
        protected bool IsPlayerInRange;
        
        protected virtual void Start()
        {
            Player = global::Player.Player.Instance;
        }

        private void Update()
        {
            ManageUI();
        }

        protected abstract void ManageUI();

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                IsPlayerInRange = true;
                Player.playerAction.canMine = false;
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                IsPlayerInRange = false;
                Player.playerAction.canMine = true;
            }
        }
    }
}
