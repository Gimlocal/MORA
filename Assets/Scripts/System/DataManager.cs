using Database;
using Player;
using Tool;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace System
{
    public class DataManager
    {
        public static void SaveData()
        {
            PlayerData playerData = new PlayerData();
            
            PlayerItem playerItem = Player.Player.Instance.playerItem;
            PlayerMining playerMining = Player.Player.Instance.playerMining;
            PlayerStat playerStat = Player.Player.Instance.playerStat;
            PlayerMovement playerMovement = Player.Player.Instance.playerMovement;
            
            foreach (var item in playerItem.OwnedItems)
            {
                ItemData itemData = new ItemData
                {
                    itemId = item.Key.ToString(),
                    amount = item.Value
                };

                playerData.ownedItems.Add(itemData); 
            }
            foreach (var tool in playerMining.tools)
            {
                playerData.tools.Add(tool.toolName.ToString());
            }
            playerData.gold = playerItem.gold;
            playerData.suitLevel = playerItem.suitLevel;
            playerData.hasLantern = playerItem.hasLantern;
            playerData.canGoHome = playerItem.canGoHome;

            playerData.moveSpeed = playerStat.moveSpeed;
            playerData.maxCorruption = playerStat.maxCorruption;
            playerData.power = playerStat.power;

            playerData.sceneName = SceneManager.GetActiveScene().name;
            playerData.playerPosition = playerMovement.transform.position;
            playerData.lastMovementX = playerMovement.lastMovementX;
            
            string json = JsonUtility.ToJson(playerData);
            PlayerPrefs.SetString("PlayerData", json);
            PlayerPrefs.Save();
        }

        public static PlayerData LoadData()
        {
            if (!PlayerPrefs.HasKey("PlayerData"))
            {
                return new PlayerData();
            }
            string json = PlayerPrefs.GetString("PlayerData");
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data;
        }

        public static void SetData(PlayerData data)
        {
            PlayerItem playerItem = Player.Player.Instance.playerItem;
            PlayerMining playerMining = Player.Player.Instance.playerMining;
            PlayerStat playerStat = Player.Player.Instance.playerStat;
            PlayerMovement playerMovement = Player.Player.Instance.playerMovement;
            
            playerItem.OwnedItems.Clear();
            foreach (var item in data.ownedItems)
            {
                playerItem.OwnedItems[(ItemId)Enum.Parse(typeof(ItemId), item.itemId)] = item.amount;
            }
            playerMining.tools.Clear();
            foreach (var tool in data.tools)
            {
                var t = Player.Player.Instance.transform.Find(tool).GetComponent<Tools>();
                if (!playerMining.tools.Contains(t))
                {
                    playerMining.tools.Add(t);
                }
            }
            playerItem.gold = data.gold;
            playerItem.suitLevel = data.suitLevel;
            playerItem.hasLantern = data.hasLantern;
            playerItem.canGoHome = data.canGoHome;
            
            playerStat.moveSpeed = data.moveSpeed;
            playerStat.maxCorruption = data.maxCorruption;
            playerStat.power = data.power;
            
            Player.Player.Instance.transform.position = data.playerPosition;
            playerMovement.lastMovementX = data.lastMovementX;
        }
    }
}
