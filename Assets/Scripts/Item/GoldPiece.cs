using UnityEngine;

namespace Item
{
    public class GoldPiece : MonoBehaviour
    {
        public int value;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player.Player.Instance.playerItem.AddGold(value);
                Destroy(gameObject);
            }
        }
    }
}
