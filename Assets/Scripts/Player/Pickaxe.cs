using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Player
{
    public class Pickaxe : MonoBehaviour
    {
        private Coroutine _miningCoroutine;
        private Tween _miningTween;
        
        private Vector2 _startPos;
        private float _startAngle;
        private float _endAngle;
        
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        public void Mining(float duration)
        {
            StopMining();
            _miningCoroutine = StartCoroutine(MiningCoroutine(duration));
        }
        
        private IEnumerator MiningCoroutine(float duration)
        {
            if (Player.Instance.playerMovement.lastMovementX > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                _startPos = (Vector2)Player.Instance.transform.position + new Vector2(0.1f, 1.2f) * Player.Instance.transform.localScale;
                _startAngle = 70f;
                _endAngle = -30f;
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
                _startPos = (Vector2)Player.Instance.transform.position + new Vector2(-0.1f, 1.2f) * Player.Instance.transform.localScale;
                _startAngle = -70f;
                _endAngle = 30f;
            }
            transform.position = _startPos;
            transform.rotation = Quaternion.Euler(0, 0, _startAngle);
            _miningTween = transform.DORotate(new Vector3(0f, 0, _endAngle), duration).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(duration / 3 * 2);
            _collider.enabled = true;
            yield return _miningTween.WaitForCompletion();
            _collider.enabled = false;
            gameObject.SetActive(false);
        }

        private void StopMining()
        {
            _collider.enabled = false;
            
            if (_miningCoroutine != null)
            {
                StopCoroutine(_miningCoroutine);
                _miningCoroutine = null;
            }
            
            if (_miningTween != null && _miningTween.IsActive())
            {
                _miningTween.Kill();
                _miningTween = null;
            }
        }
    }
}
