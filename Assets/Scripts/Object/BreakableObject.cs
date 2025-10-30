using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Object
{
    public class BreakableObject : MonoBehaviour
    {
        public ObjectName objectName;
        public float hp = 3;
        protected SpriteRenderer Sr;
        private Collider2D _collider;
        private Coroutine _flickCoroutine;
        private ParticleSystem _particle;
        private Color _particleColor;

        protected virtual void Awake()
        {
            InitialSetting();
        }

        protected virtual void InitialSetting()
        {
            Sr = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            
            _particle = GetComponentInChildren<ParticleSystem>();
            _particleColor = GameManager.ColorManager.GetSpriteColor(objectName);
            ParticleSystem.MainModule main = _particle.main;
            main.startColor = _particleColor;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Tool"))
            {
                hp--;
                Split();
                if (hp <= 0)
                {
                    Break();
                }
            }
        }
        
        protected void Flick()
        {
            if (_flickCoroutine != null)
            {
                StopCoroutine(_flickCoroutine);
                _flickCoroutine = null;
            }
            _flickCoroutine = StartCoroutine(FlickAlpha());
        }
        
        private IEnumerator FlickAlpha(float duration = 0.1f)
        {
            Color originalColor = Sr.color;
            originalColor.a = 1f;
            Color destColor = originalColor;
            destColor.a = 0.5f;
            Sr.color = destColor;
            yield return new WaitForSeconds(duration);
            Sr.color = originalColor; 
        }

        protected void Split()
        {
            _particle.Play();
        }

        protected void Break()
        {
            _collider.enabled = false;
            Sr.DOFade(0, 1f).OnComplete(() => { Destroy(gameObject); });
        }
    }
}
