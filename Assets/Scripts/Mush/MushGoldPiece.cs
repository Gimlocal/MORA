using Database;
using UnityEngine;

namespace Mush
{
    public class MushGoldPiece : MonoBehaviour
    {
        public ItemInfo mushInfo;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = Player.Player.Instance;
                player.playerItem.AddGold(mushInfo.value * 2);
                Destroy(gameObject);
            }
        }
    }
}
