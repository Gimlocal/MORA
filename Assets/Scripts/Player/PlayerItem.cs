using System.Collections.Generic;
using Mush;
using UnityEngine;

namespace Player
{
    public class PlayerItem : MonoBehaviour
    {
        [SerializeField] private MushDatabase mushDatabase;
        public Dictionary<MushId, int> OwnedItems = new();
        public int gold = 0;
        public event System.Action OnItemChanged;

        public void AddItem(MushId id)
        {
            if (!OwnedItems.TryAdd(id, 1))
            {
                OwnedItems[id]++;
            }
            OnItemChanged?.Invoke();
        }

        private List<MushInfo> GetAllItemsInfo()
        {
            List<MushInfo> mushInfos = new();
            foreach (var id in OwnedItems.Keys)
            {
                var info = mushDatabase.GetPieceById(id);
                mushInfos.Add(info);
            }
            return  mushInfos;
        }

        public bool HasItem(MushId id)
        {
            return OwnedItems.ContainsKey(id);
        }

        public bool HasItem(MushId id, int amount)
        {
            return OwnedItems.ContainsKey(id) && OwnedItems[id] >= amount;
        }

        public void UseItem(MushId id)
        {
            if (OwnedItems.ContainsKey(id) &&  OwnedItems[id] > 0)
            {
                OwnedItems[id]--;
            }
            OnItemChanged?.Invoke();
        }
    }
}
