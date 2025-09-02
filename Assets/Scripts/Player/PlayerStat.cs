using UnityEngine;

namespace Player
{
    public class PlayerStat : MonoBehaviour
    {
        public float moveSpeed;
        public float moveAcceleration;
        public float capacity;
        public float maxCapacity;
        public float power;
        public event System.Action OnCapacityChanged; 

        public void ResetCapacity()
        {
            capacity = 0;
            OnCapacityChanged?.Invoke();
        }

        public void Store(float value)
        {
            capacity += value;
            OnCapacityChanged?.Invoke();
        }

        public void IncreaseMaxCapacity(float value)
        {
            maxCapacity += value;
            OnCapacityChanged?.Invoke();
        }

        public void IncreasePower(float value)
        {
            power += value;
        }
    }
}
