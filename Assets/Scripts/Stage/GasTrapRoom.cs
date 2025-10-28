using System;
using System.Collections;
using Player;
using UnityEngine;

namespace Stage
{
    public class GasTrapRoom : MonoBehaviour
    {
        [SerializeField] private float damageDelay;
        [SerializeField] private float damage;
        private Coroutine _gasTrapCoroutine;
        private PlayerStat _stat;

        private void Start()
        {
            _stat = Player.Player.Instance.playerStat;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _stat.isInGasTrap = true;
                _stat.ApplyPenalty();
                _gasTrapCoroutine = StartCoroutine(GasTrapCoroutine());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _stat.isInGasTrap = false;
                _stat.CheckCorruptionState();
                StopCoroutine(_gasTrapCoroutine);
            }
        }

        private IEnumerator GasTrapCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(damageDelay);
                _stat.Corrupt(damage);
            }
        }
    }
}
