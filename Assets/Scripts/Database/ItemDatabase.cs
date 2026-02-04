using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Database
{
    public enum ItemId
    {
        PlanetInfo,
        Lantern,
        Record1,
        Spacesuit,
        HomeTicket,
        DisposableCleanser,
        DisposableReturner,
        Scanner,
    }

    public enum ItemEffect
    {
        None,
        Lantern,
        Cleanse,
        Return,
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
                { ItemEffect.Cleanse, UseCleanser },
                { ItemEffect.Return, UseReturner },
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

        #region ItemEffects
        private void UseLantern()
        {
            GameObject lantern = Player.Player.Instance.transform.Find("Lantern").gameObject;
            lantern.SetActive(!lantern.activeSelf);
            SoundManager.Instance.Play(AudioCategory.UI, "Success");
        }

        private void UseCleanser()
        {
            Player.Player.Instance.playerStat.Cleanse();
        }

        private void UseReturner()
        {
            SceneManager.sceneLoaded += SetPosition;
            SceneManager.LoadScene("MORA-0");
        }

        private void SetPosition(Scene scene, LoadSceneMode mode)
        {
            Player.Player.Instance.transform.position = new Vector3(-19.7f, 3.42f, 0);
            SceneManager.sceneLoaded -= SetPosition;
        }
        #endregion
    }
}
