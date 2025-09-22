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
            playerStat.OnCorruptionChanged += SetCapacity;
        }

        private void OnDisable()
        {
            playerStat.OnCorruptionChanged -= SetCapacity;
        }

        private void SetCapacity()
        {
            slider.value = playerStat.corruption /  playerStat.maxCorruption;
        }
    }
}
