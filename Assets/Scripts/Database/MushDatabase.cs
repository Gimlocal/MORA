using UnityEngine;
using UnityEngine.Serialization;

namespace Database
{
    public enum MushId
    {
        GreenMush,
        BlueMush,
        RedMush,
        LightBlueMush,
        WhiteMush,
        GoldMush,
        MetalMush,
        BombMush,
    }
    
    [System.Serializable]
    public class MushInfo
    {
        public MushId mushId;
        public string mushName;
        public int value;
        [TextArea] public string description;
        public Sprite sprite;
    }
    
    [CreateAssetMenu(fileName = "MushDatabase", menuName = "MushDatabase")]
    public class MushDatabase : ScriptableObject
    { 
        public MushInfo[] mushes;

        public MushInfo GetItemById(MushId id)
        {
            foreach (var mush in mushes)
            {
                if (mush.mushId == id)
                {
                    return mush;
                }
            }
            return null;
        }
    }
}