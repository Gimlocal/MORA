using System;
using Database;
using DG.Tweening;
using Stage;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MiniMapManager : MonoBehaviour
    {
        private CanvasGroup _canvas;
        private bool _isActive;
        private Tween _tween;
        private const float FadeDuration = 0.2f;

        private void Awake()
        {
            SceneManager.sceneLoaded += SetCanvas;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab)
                && SceneDatabase.GetSceneType(SceneManager.GetActiveScene().name) == SceneType.Underground)
            {
                FadeCanvas(!_isActive);
            }
        }

        private void SetCanvas(Scene scene, LoadSceneMode mode)
        {
            if (SceneDatabase.GetSceneType(scene.name) == SceneType.Underground)
            {
                _canvas = FindAnyObjectByType<MiniMapController>().GetComponentInParent<CanvasGroup>();
                _canvas.alpha = 0;
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= SetCanvas;
        }

        private void FadeCanvas(bool flag)
        {
            if (_tween != null && _tween.IsActive())
            {
                return;
            }
            
            _isActive = flag;
            _tween = _canvas.DOFade(flag ? 1f : 0f, FadeDuration).SetEase(Ease.OutCubic);
        }
    }
}
