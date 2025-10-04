using System.Collections.Generic;
using Tool;
using UnityEngine;

namespace Database
{
    [System.Serializable]
    public class PlayerData
    {
        public List<ItemData> ownedItems = new();
        public List<string> tools = new List<string> {"Pickaxe"};
        public int gold = 0;
        public int suitLevel = 0;
        public bool canGoHome;
        public bool hasLantern;
        
        public float moveSpeed = 3;
        public float maxCorruption = 20;
        public float power = 1;

        public string sceneName = "MORA-0";
        public Vector3 playerPosition = new Vector3(-19.7f, 3.42f, 0);
        public float lastMovementX = 1;
    }

    [System.Serializable]
    public class ItemData
    {
        public string itemId;
        public int amount;
    }
}
