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
        [SerializeField] private ItemDatabase mushDatabase;
        [SerializeField] private GameObject lantern;
        public Dictionary<ItemId, int> OwnedItems = new();
        public int gold = 0;
        public int suitLevel = 0;
        public bool canGoHome;
        public bool hasLantern;
        public event System.Action OnItemChanged;
        public event System.Action OnGoldChanged;

        private void Start()
        {
            AddItem(ItemId.PlanetInfo);
            AddItem(ItemId.WorkInfo);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void AddItem(ItemId id)
        {
            if (!OwnedItems.TryAdd(id, 1))
            {
                OwnedItems[id]++;
            }
            OnItemChanged?.Invoke();
        }

        private List<ItemInfo> GetAllItemsInfo()
        {
            List<ItemInfo> mushInfos = new();
            foreach (var id in OwnedItems.Keys)
            {
                var info = mushDatabase.GetItemById(id);
                mushInfos.Add(info);
            }
            return  mushInfos;
        }

        public bool HasItem(ItemId id)
        {
            return OwnedItems.ContainsKey(id);
        }

        public bool HasItem(ItemId id, int amount)
        {
            return OwnedItems.ContainsKey(id) && OwnedItems[id] >= amount;
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
                if (OwnedItems.Any(t => t.Key == ItemId.Lantern))
                {
                    lantern.SetActive(true);
                }
            }
            else if (SceneDatabase.GetSceneType(scene.name) == SceneType.Normal)
            {
                lantern.SetActive(false);
            }
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
