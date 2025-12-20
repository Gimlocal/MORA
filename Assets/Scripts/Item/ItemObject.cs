using Database;
using UnityEngine;

namespace Item
{
    public class ItemObject : MonoBehaviour
    {
        public ItemId itemId;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player.Player.Instance.playerItem.AddItem(itemId);
                Destroy(gameObject);
            }
        }
    }
}
