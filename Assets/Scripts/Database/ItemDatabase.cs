using UnityEngine;
using UnityEngine.Serialization;

namespace Database
{
    [System.Serializable]
    public class ItemInfo
    {
        [FormerlySerializedAs("mushId")] 
        public ItemId itemId;
        public bool isMush;
        public string itemName;
        public int value;
        [TextArea] public string description;
        public Sprite sprite;
    }

    public enum ItemId
    {
        PlanetInfo,
        WorkInfo,
        GreenMush,
        BlueMush,
        RedMush,
        LightBlueMush,
        WhiteMush,
        GoldMush,
        MetalMush,
        BombMush,
        Lantern,
    }

    [CreateAssetMenu(fileName = "Item Database", menuName = "Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [FormerlySerializedAs("pieces")] 
        public ItemInfo[] items;

        public ItemInfo GetItemById(ItemId id)
        {
            foreach (var item in items)
            {
                if (item.itemId == id)
                    return item;
            }
            return null;
        }
    }
}