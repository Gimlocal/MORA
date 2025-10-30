using System;
using DG.Tweening;
using UnityEngine;

namespace Object
{
    public class BreakableObject : MonoBehaviour
    {
        public int hp = 3;
        private SpriteRenderer _sR;
        private Collider2D _collider;
        private ParticleSystem _particle;
        private Color _particleColor;

        private void Awake()
        {
            InitialSetting();
        }

        private void InitialSetting()
        {
            _sR = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            
            _particle = GetComponentInChildren<ParticleSystem>();
            _particleColor = GetAverageColorFromSprite(_sR.sprite);
            ParticleSystem.MainModule main = _particle.main;
            main.startColor = _particleColor;
        }

        private void OnTriggerEnter2D(Collider2D other)
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

        private void Split()
        {
            _particle.Play();
        }

        private void Break()
        {
            _collider.enabled = false;
            _sR.DOFade(0, 1f).OnComplete(() => { Destroy(gameObject); });
        }
        
        /// <summary>
        /// Get Average Color of Sprite
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        Color GetAverageColorFromSprite(Sprite sprite)
        {
            Texture2D tex = sprite.texture;
            Rect r = sprite.textureRect; // or sprite.rect depending Unity version
            // textureRect는 텍스처 상에서 이 스프라이트가 차지하는 영역(픽셀 단위)

            int x = Mathf.RoundToInt(r.x);
            int y = Mathf.RoundToInt(r.y);
            int w = Mathf.RoundToInt(r.width);
            int h = Mathf.RoundToInt(r.height);

            // 성능 때문에 전체 픽셀 다 읽는 게 부담이면 샘플링 간격을 늘릴 수도 있음
            Color accum = Color.black;
            int count = 0;

            // NOTE: texture.GetPixel() / GetPixels() 는 Read/Write Enabled가 켜져 있어야 함!
            Color[] pixels = tex.GetPixels(x, y, w, h);

            for (int i = 0; i < pixels.Length; i++)
            {
                accum += pixels[i];
            }

            count = pixels.Length;
            if (count > 0)
                return accum / count;
            else
                return Color.gray;
        }

    }
}
