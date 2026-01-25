using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Mush;
using Object;
using Tool;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerItem : MonoBehaviour
    {
        [SerializeField] private MushDatabase mushDatabase;
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private GameObject lantern;
        [SerializeField] private GameObject defaultLantern;
        public Dictionary<MushId, int> OwnedMushes = new();
        public Dictionary<ItemId, int> OwnedItems = new();
        public int gold = 0;
        public int suitLevel = 0;
        public bool canGoHome;
        public bool hasLantern;
        public event Action OnMushChanged;
        public event Action OnItemChanged; 
        public event Action OnGoldChanged;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            AddItem(ItemId.PlanetInfo);
            AddItem(ItemId.Lantern);
        }

        public void AddMush(MushId id)
        {
            if (!OwnedMushes.TryAdd(id, 1))
            {
                OwnedMushes[id]++;
            }
            OnMushChanged?.Invoke();
        }

        public void AddItem(ItemId id)
        {
            if (!OwnedItems.TryAdd(id, 1))
            {
                OwnedItems[id]++;
            }
            OnItemChanged?.Invoke();
        }

        public List<MushInfo> GetAllMushesInfo()
        {
            List<MushInfo> mushInfos = new();
            foreach (var id in OwnedMushes.Keys)
            {
                var info = mushDatabase.GetItemById(id);
                mushInfos.Add(info);
            }
            return mushInfos;
        }

        public List<ItemInfo> GetAllItemInfos()
        {
            List<ItemInfo> itemInfos = new();
            foreach (var id in OwnedItems.Keys)
            {
                var info = itemDatabase.GetItemById(id);
                itemInfos.Add(info);
            }
            return itemInfos;
        }

        public bool HasMush(MushId id)
        {
            return OwnedMushes.ContainsKey(id);
        }

        public bool HasMush(MushId id, int amount)
        {
            return OwnedMushes.ContainsKey(id) && OwnedMushes[id] >= amount;
        }

        public bool HasItem(ItemId id)
        {
            return OwnedItems.ContainsKey(id);
        }

        public bool HasItem(ItemId id, int amount)
        {
            return OwnedItems.ContainsKey(id) && OwnedItems[id] >= amount;
        }

        public void UseMush(MushId id, int amount = 1)
        {
            if (OwnedMushes.ContainsKey(id) && OwnedMushes[id] >= amount)
            {
                OwnedMushes[id] -= amount;
            }
            OnMushChanged?.Invoke();
        }

        public void UseItem(ItemId id, int amount = 1)
        {
            if (OwnedItems.ContainsKey(id) && OwnedItems[id] >= amount)
            {
                OwnedItems[id] -= amount;
            }
            OnItemChanged?.Invoke();
        }

        public void AddGold(int amount)
        {
            gold += amount;
            OnGoldChanged?.Invoke();
        }

        public void UseGold(int amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneDatabase.GetSceneType(scene.name) == SceneType.Underground)
            {
                // if (OwnedItems.Any(t => t.Key == MushId.Lantern))
                // {
                //     lantern.SetActive(true);
                // }
            }
            else if (SceneDatabase.GetSceneType(scene.name) == SceneType.Normal)
            {
                lantern.SetActive(false);
            }
        }

        public void UseDefaultLantern(bool on)
        {
            defaultLantern.SetActive(on);
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
