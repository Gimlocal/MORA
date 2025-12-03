using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
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
        IncreaseMaxCorruption,
        IncreasePower,
    }
    
    [CreateAssetMenu(fileName = "Mush Food Database", menuName = "Mush Food Database")]
    public class MushFoodDatabase : ScriptableObject
    {
        public MushFoodInfo[] mushFoodInfo;
        private Dictionary<MushFoodEffect, Action> _mushFoodEffects;

        private void OnEnable()
        {
            _mushFoodEffects = new Dictionary<MushFoodEffect, Action>
            {
                { MushFoodEffect.IncreaseSpeed, IncreaseSpeed },
                { MushFoodEffect.IncreaseMaxCorruption, IncreaseMaxCorruption },
                { MushFoodEffect.IncreasePower, IncreasePower },
            };
        }

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
            _mushFoodEffects[GetMushFoodInfo(id).mushFoodEffect]?.Invoke();
        }

        private void IncreaseSpeed()
        {
            Player.Player.Instance.playerStat.moveSpeed =
                Math.Clamp(Player.Player.Instance.playerStat.moveSpeed + 0.3f, 3, 6);
        }

        private void IncreaseMaxCorruption()
        {
            Player.Player.Instance.playerStat.maxCorruption += 20f;
        }

        private void IncreasePower()
        {
            Player.Player.Instance.playerStat.power += 0.5f;
        }
    }
}
