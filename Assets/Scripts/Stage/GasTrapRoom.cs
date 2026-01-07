using System;
using System.Collections;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Stage
{
    public class GasTrapRoom : MonoBehaviour
    {
        [SerializeField] private float damageDelay;
        [SerializeField] private float damage;
        private Coroutine _gasTrapCoroutine;
        private PlayerStat _stat;
        
        private Volume _volume;
        private DepthOfField _depthOfField;

        private void Start()
        {
            _stat = Player.Player.Instance.playerStat;

            if (Camera.main != null)
            {
                _volume = Camera.main.GetComponent<Volume>();
                _volume.profile.TryGet(out _depthOfField);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _stat.isInGasTrap = true;
                // BlurEffect();
                // _stat.ApplyPenalty();
                _gasTrapCoroutine = StartCoroutine(GasTrapCoroutine());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _stat.isInGasTrap = false;
                // RemoveEffect();
                // _stat.CheckCorruptionState();
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

        private void BlurEffect()
        {
            _depthOfField.mode.value = DepthOfFieldMode.Gaussian;
            _depthOfField.mode.overrideState = true;
        }

        private void RemoveEffect()
        {
            _depthOfField.mode.value = DepthOfFieldMode.Off;
            _depthOfField.mode.overrideState = true;
        }
    }
}
