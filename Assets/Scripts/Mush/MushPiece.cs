using UnityEngine;

namespace Mush
{
    public class MushPiece : MonoBehaviour
    {
        public MushInfo mushInfo;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = Player.Player.Instance;
                if (player.playerStat.capacity + mushInfo.value <= player.playerStat.maxCapacity)
                {
                    player.playerItem.AddItem(mushInfo.mushId);
                    player.playerStat.Store(mushInfo.value);
                    Destroy(gameObject);
                }
            }
        }
    }
}
