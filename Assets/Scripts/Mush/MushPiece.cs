using Database;
using Object;
using UnityEngine;

namespace Mush
{
    public class MushPiece : ObtainableObject
    {
        public MushInfo mushInfo;

        protected override bool ObtainCondition()
        {
            return Player.Player.Instance.playerStat.corruption + mushInfo.value
                   <= Player.Player.Instance.playerStat.maxCorruption;
        }

        protected override void Obtain()
        {
            Player.Player.Instance.playerItem.AddMush(mushInfo.mushId);
            Player.Player.Instance.playerStat.Corrupt(mushInfo.value);
        }
    }
}
