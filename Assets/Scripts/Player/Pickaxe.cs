using System.Collections;
using UnityEngine;

namespace Player
{
    public class Pickaxe : MonoBehaviour
    {
        private Coroutine _miningCoroutine;
        private Vector2 _startPos;
        private float _startAngle;
        private float _endAngle;
        
        public void Mining()
        {
            _miningCoroutine = StartCoroutine(MiningCoroutine());
        }
        
        private IEnumerator MiningCoroutine()
        {
            if (Player.Instance.playerMovement.lastMovementX > 0)
            {
                _startPos = new Vector2(0.6f, 1.1f);
                _startAngle = 70f;
                _endAngle = -30f;
            }
            else
            {
                _startPos = new Vector2(-0.6f, 1.1f);
                _startAngle = -70f;
                _endAngle = 30f;
            }
            yield return null;
        }

        public void StopMining()
        {
            if (_miningCoroutine == null) return;
            StopCoroutine(_miningCoroutine);
            _miningCoroutine = null;
        }
    }
}
