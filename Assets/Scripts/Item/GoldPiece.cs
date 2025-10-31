using Database;
using Object;
using Sound;
using UnityEngine;

namespace Item
{
    public class GoldPiece : ObtainableObject
    {
        public int value;

        protected override void Obtain()
        {
            Player.Player.Instance.playerItem.AddGold(value);
        }

        protected override void ObtainSound()
        {
            SoundManager.Instance.Play(AudioCategory.Obtain, "Coin");
        }
    }
}
