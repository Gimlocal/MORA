using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CapacityUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Player.PlayerStat playerStat;

        private void OnEnable()
        {
            playerStat.OnCapacityChanged += SetCapacity;
        }

        private void OnDisable()
        {
            playerStat.OnCapacityChanged -= SetCapacity;
        }

        private void SetCapacity()
        {
            slider.value = playerStat.capacity /  playerStat.maxCapacity;
        }
    }
}
