using System;
using Database;
using DG.Tweening;
using Stage;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI
{
    public class MiniMapManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup miniMapCanvas;
        private bool _isActive;
        private Tween _tween;
        private const float FadeDuration = 0.2f;

        private void Awake()
        {
            FadeCanvas(false);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (SceneDatabase.GetSceneType(SceneManager.GetActiveScene().name) == SceneType.Underground
                    && Player.Player.Instance.playerItem.HasItem(ItemId.Scanner))
                {
                    FadeCanvas(!_isActive);
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneDatabase.GetSceneType(scene.name) != SceneType.Underground)
            {
                FadeCanvas(false);
            }
        }

        private void FadeCanvas(bool flag)
        {
            if (_tween != null && _tween.IsActive())
            {
                return;
            }
            
            _isActive = flag;
            _tween = miniMapCanvas.DOFade(flag ? 1f : 0f, FadeDuration).SetEase(Ease.OutCubic);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
