using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerStat : MonoBehaviour
    {
        public float moveSpeed;
        public float moveAcceleration;
        public float axeSpeed;
        public float corruption;
        public float maxCorruption;
        public float power;
        public float deathCount;

        public event System.Action OnCapacityChanged; 

        private Coroutine _deathCoroutine;
        private bool _isPenaltyApplied;

        public void Cleanse()
        {
            corruption = 0;
            OnCapacityChanged?.Invoke();
            CheckCorruptionState();
        }

        public void Corrupt(float value)
        {
            corruption += value;
            OnCapacityChanged?.Invoke();
            CheckCorruptionState();
        }

        public void IncreaseMaxCorruption(float value)
        {
            maxCorruption += value;
            OnCapacityChanged?.Invoke();
            CheckCorruptionState();
        }

        public void IncreasePower(float value)
        {
            power += value;
        }

        private void CheckCorruptionState()
        {
            float ratio = corruption / maxCorruption;
            
            if (ratio >= 0.8f && !_isPenaltyApplied)
            {
                _isPenaltyApplied = true;
                ApplyPenalty();
            }
            else if (ratio < 0.8f && _isPenaltyApplied)
            {
                _isPenaltyApplied = false;
                RemovePenalty();
            }
            
            if (ratio >= 0.9f && _deathCoroutine == null)
            {
                _deathCoroutine = StartCoroutine(DeathCountdown(deathCount));
            }
            else if (ratio < 0.9f && _deathCoroutine != null)
            {
                StopCoroutine(_deathCoroutine);
                _deathCoroutine = null;
            }
        }

        private void ApplyPenalty()
        {
            moveSpeed *= 0.8f;
            axeSpeed *= 1.5f;
        }

        private void RemovePenalty()
        {
            moveSpeed /= 0.8f;
            axeSpeed /= 1.5f;
        }

        private IEnumerator DeathCountdown(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (corruption / maxCorruption >= 0.9f)
            {
                Die();
            }

            _deathCoroutine = null;
        }

        private void Die()
        {
            
        }
    }
}
