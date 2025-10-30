using DG.Tweening;
using UnityEngine;

namespace Object
{
    public class ObtainableObject : MonoBehaviour
    {
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                ObtainEffect();
            }
        }

        protected void ObtainEffect()
        {
            var seq = DOTween.Sequence();
            float scale = transform.localScale.x;
            
            var t1 = transform.DOScaleX(scale * 3, 0.1f).SetEase(Ease.OutQuad);
            var t2 = transform.DOScaleY(0, 0.1f).SetEase(Ease.OutQuad);
            
            seq.Append(t1);
            seq.Join(t2);

            seq.OnComplete(() => { Destroy(gameObject); });
        }
    }
}
