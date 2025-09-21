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
                if (player.playerStat.corruption + mushInfo.value <= player.playerStat.maxCorruption)
                {
                    player.playerItem.AddItem(mushInfo.mushId);
                    player.playerStat.Corrupt(mushInfo.value);
                    Destroy(gameObject);
                }
            }
        }
    }
}
