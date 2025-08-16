using UnityEngine;
using UnityEngine.Serialization;

namespace Mush
{
    [System.Serializable]
    public class MushInfo
    {
        public MushId mushId;
        public string itemName;
        public int value;
        [TextArea] public string description;
        public Sprite sprite;
    }

    public enum MushId
    {
        GreenMush,
    }

    [CreateAssetMenu(fileName = "Mush Piece Database", menuName = "Mush Piece Database")]
    public class MushDatabase : ScriptableObject
    {
        public MushInfo[] pieces;

        public MushInfo GetPieceById(MushId id)
        {
            foreach (var piece in pieces)
            {
                if (piece.mushId == id)
                    return piece;
            }
            return null;
        }
    }
}