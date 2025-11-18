using System;
using System.Collections.Generic;
using System.Linq;
using Tool;
using UnityEngine;

namespace Database
{
    public enum ProductID
    {
        Spacesuit,
        HomeTicket,
        Lantern,
        Drill,
    }

    public enum ProductEffect
    {
        UpgradeSuit,
        CanGoHome,
        Item,
        Equipment,
    }

    [Serializable]
    public class ProductData
    {
        public ProductID productID;
        public Sprite sprite;
        public string name;
        [TextArea] public string description;
        public List<MushIngredient> ingredients;
        public int price;
        public ProductEffect effect;
    }
    
    [CreateAssetMenu(fileName = "Product Database", menuName = "Product Database")]
    public class ProductDatabase : ScriptableObject
    {
        public ProductData[] products;

        public ProductData GetProductById(ProductID id)
        {
            return products.FirstOrDefault(product => product.productID == id);
        }

        public void GetEffect(ProductID id)
        {
            switch (GetProductById(id).effect)
            {
                case ProductEffect.UpgradeSuit:
                    Player.Player.Instance.playerItem.suitLevel++;
                    break;
                case ProductEffect.CanGoHome:
                    Player.Player.Instance.playerItem.canGoHome = true;
                    break;
                case ProductEffect.Equipment:
                    Player.Player.Instance.playerMining.
                        AddTool(Player.Player.Instance.gameObject.transform.Find(id.ToString()).GetComponent<Tools>());
                    break;
                case ProductEffect.Item:
                    Player.Player.Instance.playerItem.AddMush((MushId)Enum.Parse(typeof(MushId), id.ToString()));
                    Player.Player.Instance.playerItem.hasLantern = true;
                    break;
            }
        }
    }
}
