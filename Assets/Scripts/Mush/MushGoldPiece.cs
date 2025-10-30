using Database;
using Object;
using UnityEngine;

namespace Mush
{
    public class MushGoldPiece : ObtainableObject
    {
        public ItemInfo mushInfo;
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player.Player.Instance.playerItem.AddGold(mushInfo.value * 2);
                ObtainEffect();
            }
        }
    }
}
