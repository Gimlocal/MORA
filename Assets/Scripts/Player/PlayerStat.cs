using System;
using System.Collections;
using Database;
using Settings.Shader;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerStat : MonoBehaviour
    {
        [Header("Player Stat")]
        public float moveSpeed;
        public float moveAcceleration;
        public float axeSpeed;
        public float corruption;
        public float maxCorruption;
        public float power;

        [Header("Penalty Value")] 
        [SerializeField] private float moveSpeedPenaltyRatio;
        [SerializeField] private float axeSpeedPenaltyRatio;
        [SerializeField] private float firstPenaltyRatio;
        [SerializeField] private float secondPenaltyRatio;
        [SerializeField] private float firstVignetteIntensity;
        [SerializeField] private float secondVignetteIntensity;
        [SerializeField] private float chromaticAberrationIntensity;
        [SerializeField] private float postDistortionIntensity;
        public float deathCount;

        public event Action OnCorruptionChanged; 

        private Coroutine _deathCoroutine;
        private bool _isPenaltyApplied;
        private Volume _mainCameraVolume;
        private Vignette _vignette;
        private ChromaticAberration _chromaticAberration;
        private Bloom _bloom;
        private DepthOfField _depthOfField;
        private PostDistortion _postDistortion;
        private ColorAdjustments _colorAdjustments;
        
        private PlayerData _playerData;

        [HideInInspector] public bool isInGasTrap;

        private void Start()
        {
            if (Camera.main != null && Camera.main.TryGetComponent(out _mainCameraVolume))
            {
                _mainCameraVolume.profile.TryGet(out _vignette);
                _mainCameraVolume.profile.TryGet(out _chromaticAberration);
                _mainCameraVolume.profile.TryGet(out _bloom);
                _bloom.intensity.value = 1f;
                _mainCameraVolume.profile.TryGet(out _depthOfField);
                _mainCameraVolume.profile.TryGet(out _postDistortion);
                _mainCameraVolume.profile.TryGet(out _colorAdjustments);
            }
        }

        public void Cleanse()
        {
            corruption = 0;
            OnCorruptionChanged?.Invoke();
            CheckCorruptionState();
        }

        public void Corrupt(float value)
        {
            corruption += value;
            OnCorruptionChanged?.Invoke();
            CheckCorruptionState();
        }

        public void IncreaseMaxCorruption(float value)
        {
            maxCorruption += value;
            OnCorruptionChanged?.Invoke();
            CheckCorruptionState();
        }

        public void IncreasePower(float value)
        {
            power += value;
        }

        public void CheckCorruptionState()
        {
            float ratio = corruption / maxCorruption;
            
            if (ratio >= firstPenaltyRatio && !_isPenaltyApplied)
            {
                ApplyPenalty();
            }
            else if (ratio < firstPenaltyRatio && _isPenaltyApplied && !isInGasTrap)
            {
                RemovePenalty();
            }
            
            if (ratio >= secondPenaltyRatio && _deathCoroutine == null)
            {
                _deathCoroutine = StartCoroutine(DeathCountdown(deathCount));
            }
            else if (ratio < secondPenaltyRatio && _deathCoroutine != null)
            {
                StopCoroutine(_deathCoroutine);
                _vignette.intensity.value = 0;
                _chromaticAberration.intensity.value = 0;
                _postDistortion.intensity.value = 0;
                _deathCoroutine = null;
            }
        }

        public void ApplyPenalty()
        {
            _isPenaltyApplied = true;
            moveSpeed *= moveSpeedPenaltyRatio;
            axeSpeed *= axeSpeedPenaltyRatio;
            _vignette.intensity.value = firstVignetteIntensity;
        }

        public void RemovePenalty()
        {
            _isPenaltyApplied = false;
            moveSpeed /= moveSpeedPenaltyRatio;
            axeSpeed /= axeSpeedPenaltyRatio;
            _vignette.intensity.value = 0f;
        }

        private IEnumerator DeathCountdown(float duration)
        {
            yield return ApplyStrongPenalty(duration);

            if (corruption / maxCorruption >= secondPenaltyRatio)
            {
                Die();
            }

            _deathCoroutine = null;
        }

        private IEnumerator ApplyStrongPenalty(float duration)
        {
            float dt = 0;
            while (dt < duration)
            {
                dt += Time.deltaTime;
                float ratio = dt / duration;
                _vignette.intensity.value = firstVignetteIntensity + (secondVignetteIntensity - firstVignetteIntensity) * ratio;
                _chromaticAberration.intensity.value = chromaticAberrationIntensity * ratio;
                _postDistortion.intensity.value = postDistortionIntensity * ratio;
                yield return null;
            }
        }

        private void Die()
        {
            _vignette.intensity.value = 0;
            _chromaticAberration.intensity.value = 0;
            _postDistortion.intensity.value = 0;
            _colorAdjustments.saturation.value = -100;
            PlayerMovement movement = Player.Instance.playerMovement;
            movement.canMove = false;
            movement.StopPlayer();
            int dir = movement.lastMovementX > 0 ? -1 : 1;
            float angle = 90 * dir;
            Player.Instance.playerSprite.transform.DORotate(new Vector3(0, 0, angle), 0.3f).SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    _playerData = DataManager.LoadData();
                    SceneManager.sceneLoaded += DeadSceneLoaded;
                    _colorAdjustments.postExposure.value = -10;
                    SceneManager.LoadScene(_playerData.sceneName);
                });
        }

        private void DeadSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            corruption = 0;
            DataManager.SetData(_playerData);
            OnCorruptionChanged?.Invoke();
            Player.Instance.playerSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
            _colorAdjustments.saturation.value = 0;
            DOTween.To(
                () => _colorAdjustments.postExposure.value,
                x => _colorAdjustments.postExposure.value = x,
                0f,
                3f
            ).From(-10f).SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                Player.Instance.playerMovement.canMove = true;
            });
            SceneManager.sceneLoaded -= DeadSceneLoaded;
        }
    }
}
