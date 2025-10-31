using Database;
using Object;
using UnityEngine;

namespace Mush
{
    public class MushPiece : ObtainableObject
    {
        public ItemInfo mushInfo;

        protected override bool ObtainCondition()
        {
            return Player.Player.Instance.playerStat.corruption + mushInfo.value
                   <= Player.Player.Instance.playerStat.maxCorruption;
        }

        protected override void Obtain()
        {
            Player.Player.Instance.playerItem.AddItem(mushInfo.itemId);
            Player.Player.Instance.playerStat.Corrupt(mushInfo.value);
        }
    }
}
