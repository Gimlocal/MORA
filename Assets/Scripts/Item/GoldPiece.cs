using Object;
using UnityEngine;

namespace Item
{
    public class GoldPiece : ObtainableObject
    {
        public int value;

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player.Player.Instance.playerItem.AddGold(value);
                ObtainEffect();
            }
        }
    }
}
