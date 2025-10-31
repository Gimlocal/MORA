using Database;
using DG.Tweening;
using Sound;
using UnityEngine;

namespace Object
{
    public abstract class ObtainableObject : MonoBehaviour
    {
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (ObtainCondition())
                {
                    Obtain();
                    ObtainSound();
                    ObtainEffect();
                }
            }
        }

        protected abstract void Obtain();

        protected virtual bool ObtainCondition()
        {
            return true;
        }

        protected virtual void ObtainEffect()
        {
            var seq = DOTween.Sequence();
            float scale = transform.localScale.x;
            
            var t1 = transform.DOScaleX(scale * 3, 0.1f).SetEase(Ease.OutQuad);
            var t2 = transform.DOScaleY(0, 0.1f).SetEase(Ease.OutQuad);
            
            seq.Append(t1);
            seq.Join(t2);

            seq.OnComplete(() => { Destroy(gameObject); });
        }

        protected virtual void ObtainSound()
        {
            SoundManager.Instance.Play(AudioCategory.Obtain, "Default");
        }
    }
}
