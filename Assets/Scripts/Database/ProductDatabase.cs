using System;
using System.Collections.Generic;
using System.Linq;
using Tool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Database
{
    public enum ProductID
    {
        Spacesuit,
        HomeTicket,
        Lantern,
        Drill,
    }

    public enum ProductType
    {
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
        public ProductType type;
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
            switch (GetProductById(id).type)
            {
                case ProductType.Equipment:
                    Player.Player.Instance.playerMining.
                        AddTool(Player.Player.Instance.gameObject.transform.Find(id.ToString()).GetComponent<Tools>());
                    break;
                case ProductType.Item:
                    Player.Player.Instance.playerItem.AddItem((ItemId)Enum.Parse(typeof(ItemId), id.ToString()));
                    break;
            }
        }
    }
}
