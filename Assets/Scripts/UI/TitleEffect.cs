using UnityEngine;

namespace UI
{
    public class TitleEffect : MonoBehaviour
    {
        [Header("회전(Wobble) 설정")]
        public float wobbleSpeed = 2f;     // 흔들리는 속도
        public float wobbleAngle = 5f;     // 최대 회전 각도

        [Header("스케일(Pulse) 설정")]
        public float scaleSpeed = 3f;      // 크기 변화 속도
        public float scaleAmount = 0.05f;  // 크기 변화 정도 (0.05 = ±5%)

        private Quaternion _baseRot;
        private Vector3 _baseScale;

        private void Start()
        {
            _baseRot = transform.rotation;
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            // 흔들림 (좌우 회전)
            float step = Mathf.Sign(Mathf.Sin(Time.time * wobbleSpeed));
            transform.rotation = _baseRot * Quaternion.Euler(0, 0, step * wobbleAngle);

            // 크기 변화 (말랑거림)
            float scale = 1f + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
            transform.localScale = _baseScale * scale;
        }
    }
}
