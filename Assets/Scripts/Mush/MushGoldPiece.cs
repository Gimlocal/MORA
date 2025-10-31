using Database;
using Object;
using Sound;
using UnityEngine;

namespace Mush
{
    public class MushGoldPiece : ObtainableObject
    {
        public ItemInfo mushInfo;

        protected override void Obtain()
        {
            Player.Player.Instance.playerItem.AddGold(mushInfo.value * 2);
        }
        
        protected override void ObtainSound()
        {
            SoundManager.Instance.Play(AudioCategory.Obtain, "Coin");
        }
    }
}
