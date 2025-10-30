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
    public class Mush : BreakableObject
    {
        [SerializeField] private ItemDatabase mushDatabase;
        [SerializeField] private ItemId mushId;
        [SerializeField] private float dropInterval;
        [SerializeField] private Material pieceMaterial;
        [SerializeField] private Sprite goldSprite;
        [SerializeField] private GameObject explosionEffect;
        
        public MushRarity rarity;
        
        private float _hitCount;
        private float _maxHp;
        private float _lastMiningTime;

        protected override void Awake()
        {
            _maxHp = hp;
            InitialSetting();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
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
                Split();
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
                    if (rarity == MushRarity.Gold)
                    {
                        DropGoldPiece();
                    }
                }
                
                hp -= power;
                if (hp <= 0) OnDead();
            }
        }

        private void DropPiece()
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 dropPos = transform.position + (Vector3)randomDir;
            
            // piece 생성
            GameObject dropPiece = new GameObject("Piece");
            dropPiece.transform.position = transform.position;
            
            SpriteRenderer sR = dropPiece.AddComponent<SpriteRenderer>();
            sR.sprite = Sr.sprite;
            sR.sortingOrder = Sr.sortingOrder;
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

        private void DropGoldPiece()
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 dropPos = transform.position + (Vector3)randomDir;
            Vector3 midDropPos = (transform.position + dropPos * 2) / 3;
            
            GameObject dropGoldPiece = new GameObject("GoldPiece");
            dropGoldPiece.transform.position = transform.position;
            dropGoldPiece.transform.localScale *= 0.2f;
            
            SpriteRenderer sR = dropGoldPiece.AddComponent<SpriteRenderer>();
            sR.sprite = goldSprite;
            sR.sortingOrder = Sr.sortingOrder;
            sR.material = pieceMaterial;
            
            CircleCollider2D cd = sR.gameObject.AddComponent<CircleCollider2D>();
            cd.radius = 0.12f;
            cd.isTrigger = true;
            cd.enabled = false;
            
            MushGoldPiece mushGoldPiece = dropGoldPiece.AddComponent<MushGoldPiece>();
            mushGoldPiece.mushInfo = mushDatabase.GetItemById(mushId);
            
            Sequence seq = DOTween.Sequence();
            seq.Append(dropGoldPiece.transform.DOJump(midDropPos, 0.15f, 1, 0.15f));
            seq.Append(dropGoldPiece.transform.DOJump(dropPos, 0.4f, 1, 0.4f));
            seq.OnComplete(() => { dropGoldPiece.GetComponent<Collider2D>().enabled = true;});
        }

        private void OnDead()
        {
            if (rarity == MushRarity.Boom)
            {
                GameObject mushExplosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                mushExplosion.GetComponent<MushExplosion>().amount = mushDatabase.GetItemById(mushId).value * 10;
                SoundManager.Instance.Play(AudioCategory.Effect, "MushExplosion");
                mushExplosion.GetComponent<Collider2D>().enabled = true;
            }
            var component = GetComponent<ShadowCaster2D>();
            if (component != null) component.castsShadows = false;

            Break();
        }
    }
}
