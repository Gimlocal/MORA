using System.Collections.Generic;
using UnityEngine;

namespace Database
{
    public enum ItemId
    {
        PlanetInfo,
        Lantern,
        Record1,
    }

    [System.Serializable]
    public class ItemInfo
    {
        public ItemId itemId;
        public string itemName;
        [TextArea] public string description;
        public Sprite sprite;
    }
    
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public ItemInfo[] items;

        public ItemInfo GetItemById(ItemId id)
        {
            foreach (var item in items)
            {
                if (item.itemId == id)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
