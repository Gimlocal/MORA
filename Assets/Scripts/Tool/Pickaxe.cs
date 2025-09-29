using System.Collections;
using DG.Tweening;
using Sound;
using UnityEngine;

namespace Tool
{
    public class Pickaxe : Tools
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

        public override void Mining()
        {
            if (_miningCoroutine != null) return;
            SoundManager.Instance.Play(AudioCategory.Tool, audioKey);
            _miningCoroutine = StartCoroutine(MiningCoroutine(Player.Player.Instance.playerStat.axeSpeed));
        }
        
        private IEnumerator MiningCoroutine(float duration)
        {
            if (Player.Player.Instance.playerMovement.lastMovementX > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                _startPos = (Vector2)Player.Player.Instance.transform.position + new Vector2(0.1f, 1.2f) * Player.Player.Instance.transform.localScale;
                _startAngle = 70f;
                _endAngle = -30f;
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
                _startPos = (Vector2)Player.Player.Instance.transform.position + new Vector2(-0.1f, 1.2f) * Player.Player.Instance.transform.localScale;
                _startAngle = -70f;
                _endAngle = 30f;
            }
            transform.position = _startPos;
            transform.rotation = Quaternion.Euler(0, 0, _startAngle);
            _miningTween = transform.DORotate(new Vector3(0f, 0, _endAngle), duration).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(duration / 5 * 4);
            _collider.enabled = true;
            yield return _miningTween.WaitForCompletion();
            _collider.enabled = false;
            gameObject.SetActive(false);
            
            _miningCoroutine = null;
        }
    }
}
