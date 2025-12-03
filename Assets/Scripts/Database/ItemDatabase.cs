using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sound;
using UnityEngine;

namespace Database
{
    public enum ItemId
    {
        PlanetInfo,
        Lantern,
        Record1,
        Spacesuit,
        HomeTicket,
    }

    public enum ItemEffect
    {
        None,
        Lantern,
    }

    [System.Serializable]
    public class ItemInfo
    {
        public ItemId itemId;
        public string itemName;
        [TextArea] public string description;
        public bool oneTime;
        public ItemEffect effect;
        public Sprite sprite;
    }
    
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public ItemInfo[] items;
        private Dictionary<ItemEffect, Action> _itemEffects;

        private void OnEnable()
        {
            _itemEffects = new Dictionary<ItemEffect, Action>
            {
                { ItemEffect.None, () => { } },
                { ItemEffect.Lantern, UseLantern },
            };
        }
        
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
        
        public void UseItem(ItemId id)
        {
            var item = GetItemById(id);
            _itemEffects[item.effect]?.Invoke();
        }

        private void UseLantern()
        {
            GameObject lantern = Player.Player.Instance.transform.Find("Lantern").gameObject;
            lantern.SetActive(!lantern.activeSelf);
            SoundManager.Instance.Play(AudioCategory.UI, "Success");
        }
    }
}
