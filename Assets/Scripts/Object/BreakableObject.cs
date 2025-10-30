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
        protected SpriteRenderer SR;
        protected Collider2D Collider;
        protected Coroutine FlickCoroutine;
        private ParticleSystem _particle;
        private Color _particleColor;

        protected virtual void Awake()
        {
            InitialSetting();
        }

        protected virtual void InitialSetting()
        {
            SR = GetComponent<SpriteRenderer>();
            Collider = GetComponent<Collider2D>();
            
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
            if (FlickCoroutine != null)
            {
                StopCoroutine(FlickCoroutine);
                FlickCoroutine = null;
            }
            FlickCoroutine = StartCoroutine(FlickAlpha());
        }
        
        private IEnumerator FlickAlpha(float duration = 0.1f)
        {
            Color originalColor = SR.color;
            originalColor.a = 1f;
            Color destColor = originalColor;
            destColor.a = 0.5f;
            SR.color = destColor;
            yield return new WaitForSeconds(duration);
            SR.color = originalColor; 
        }

        protected void Split()
        {
            _particle.Play();
        }

        protected void Break()
        {
            Collider.enabled = false;
            SR.DOFade(0, 1f).OnComplete(() => { Destroy(gameObject); });
        }
    }
}
