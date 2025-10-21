using System;
using System.Collections;
using Database;
using UnityEngine;
using DG.Tweening;
using Object;
using Sound;
using Tool;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Mush
{
    public class Mush : MonoBehaviour
    {
        [SerializeField] private ItemDatabase mushDatabase;
        [SerializeField] private ItemId mushId;
        [SerializeField] private float hp;
        [SerializeField] private float dropInterval;
        [SerializeField] private Material pieceMaterial;
        private float _hitCount;
        private float _maxHp;
        private Collider2D _col;
        private Coroutine _flickCoroutine;
        private float _lastMiningTime;
        
        public SpriteRenderer sR;
        
        [HideInInspector]
        public MushRarity rarity;

        private void Awake()
        {
            sR = GetComponent<SpriteRenderer>();
            _col = GetComponent<Collider2D>();
            _maxHp = hp;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Tool"))
            {
                Tools tool = other.gameObject.GetComponent<Tools>();
                if (tool.toolType == ToolType.OneTime)
                {
                    OnMined(Player.Player.Instance.playerStat.power, tool);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Tool"))
            {
                Tools tool = other.gameObject.GetComponent<Tools>();
                if (tool.toolType == ToolType.Continuous)
                {
                    if (Time.time >= _lastMiningTime + tool.mineInterval)
                    {
                        OnMined(Player.Player.Instance.playerStat.power / 3 * 2, tool);
                        _lastMiningTime = Time.time;
                    }
                }
            }
        }

        private void OnMined(float power, Tools tool)
        {
            if (hp > 0)
            {
                Flick();
                
                SoundManager.Instance.Play(AudioCategory.ToolHit, tool.hitAudioKey);
                
                float prevHitCount = _hitCount;
                _hitCount += power;
                _hitCount = Math.Clamp(_hitCount, 0, _maxHp);
                // 이전 hitCount ~ 현재 hitCount 사이에 dropInterval 배수가 몇 개 있었는지 체크
                int prevDrop = Mathf.FloorToInt(prevHitCount / dropInterval);
                int newDrop = Mathf.FloorToInt(_hitCount / dropInterval);
                int dropCount = newDrop - prevDrop;
                for (int i = 0; i < dropCount; i++)
                {
                    DropPiece();
                }

                hp -= power;
                if (hp <= 0) OnDead();
            }
        }

        private void Flick()
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
            Color originalColor = sR.color;
            originalColor.a = 1f;
            Color destColor = originalColor;
            destColor.a = 0.5f;
            sR.color = destColor;
            yield return new WaitForSeconds(duration);
            sR.color = originalColor; 
        }

        private void DropPiece()
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 dropPos = transform.position + (Vector3)randomDir;
            
            // piece 생성
            GameObject dropPiece = new GameObject("Piece");
            dropPiece.transform.position = transform.position;
            SpriteRenderer sR = dropPiece.AddComponent<SpriteRenderer>();
            sR.sprite = this.sR.sprite;
            sR.sortingOrder = this.sR.sortingOrder;
            sR.material = pieceMaterial;
            CircleCollider2D cd = sR.gameObject.AddComponent<CircleCollider2D>();
            cd.radius = 0.12f;
            cd.isTrigger = true;
            cd.enabled = false;
            MushPiece mushPiece = dropPiece.AddComponent<MushPiece>();
            mushPiece.mushInfo = mushDatabase.GetItemById(mushId);
            
            dropPiece.transform.DOJump(dropPos, 0.5f, 1, 0.5f).
                OnComplete(() => { dropPiece.GetComponent<Collider2D>().enabled = true;});
        }

        private void OnDead()
        {
            _col.enabled = false;
            var component = GetComponent<ShadowCaster2D>();
            if (component != null) component.castsShadows = false;
            sR.DOFade(0, 1f).OnComplete(() => { Destroy(gameObject); });
        }
    }
}
