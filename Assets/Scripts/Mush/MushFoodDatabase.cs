using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mush
{
    [System.Serializable]
    public class MushFoodInfo
    {
        public MushFoodId mushFoodId;
        public Sprite sprite;
        public string name;
        public List<MushIngredient> ingredients;
        [TextArea] public string description;
        public MushFoodEffect mushFoodEffect;
    }
    
    [System.Serializable]
    public class MushIngredient
    {
        public MushId mushId;
        public int amount;
    }

    public enum MushFoodId
    {
        GreenSoup,
        RedMeat,
        WhiteBread,
    }

    public enum MushFoodEffect
    {
        IncreaseSpeed,
        IncreaseCapacity,
        IncreasePower,
    }
    
    [CreateAssetMenu(fileName = "Mush Food Database", menuName = "Mush Food Database")]
    public class MushFoodDatabase : ScriptableObject
    {
        public MushFoodInfo[] mushFoodInfo;

        public MushFoodInfo GetMushFoodInfo(MushFoodId id)
        {
            foreach (var foodInfo in mushFoodInfo)
            {
                if (foodInfo.mushFoodId == id)
                    return  foodInfo;
            }
            return null;
        }

        public void EatMushFood(MushFoodId id)
        {
            switch (GetMushFoodInfo(id).mushFoodEffect)
            {
                case MushFoodEffect.IncreaseSpeed:
                    Player.Player.Instance.playerStat.moveSpeed += 0.1f;
                    break;
                case MushFoodEffect.IncreaseCapacity:
                    Player.Player.Instance.playerStat.maxCapacity += 5f;
                    break;
                case MushFoodEffect.IncreasePower:
                    Player.Player.Instance.playerStat.power += 0.2f;
                    break;
            }
        }
    }
}
