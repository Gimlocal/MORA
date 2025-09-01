using System.Linq;
using UnityEngine;

namespace Object
{
    public enum ProductID
    {
        Spacesuit,
        HomeTicket,
    }

    public enum ProductEffect
    {
        UpgradeSuit,
        CanGoHome
    }

    [System.Serializable]
    public class ProductData
    {
        public ProductID productID;
        public Sprite sprite;
        public string name;
        [TextArea] public string description;
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
            }
        }
    }
}
