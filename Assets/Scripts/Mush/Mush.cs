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
        [Header("Mush Setting")]
        [SerializeField] private MushDatabase mushDatabase;
        [SerializeField] private MushId mushId;
        [SerializeField] private GameObject explosionEffect;
        public MushRarity rarity;
        
        [Header("Drop Setting")]
        [SerializeField] private float dropInterval;
        [SerializeField] private Material pieceMaterial;
        [SerializeField] private Sprite goldSprite;
        [SerializeField] private float dropDamage = 1;
        [SerializeField] private float maxDropDistance = 1;
        [SerializeField] private float minDropDistance;
        
        private float _hitCount;
        private float _maxHp;

        protected override void InitialSetting()
        {
            _maxHp = hp;
            base.InitialSetting();
        }

        protected override void OnAttacked(float power, Tools tool)
        {
            if (hp > 0)
            {
                SoundManager.Instance.Play(AudioCategory.ToolHit, tool.hitAudioKey);

                if (power < dropDamage)
                {
                    return;
                }
                
                Split();
                Flick();
                
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
                if (hp <= 0)
                {
                    OnDead();
                }
            }
        }

        private void DropPiece()
        {
            Vector3 dropPos = transform.position + DropPosition();
            
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
            
            float jumpPower = (dropPos - transform.position).magnitude / 2f;
            
            dropPiece.transform.DOJump(dropPos, jumpPower, 1, 0.5f).
                OnComplete(() => { dropPiece.GetComponent<Collider2D>().enabled = true;});
        }

        private void DropGoldPiece()
        {
            Vector3 dropPos = transform.position + DropPosition();
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
            
            float jumpPower = (dropPos - transform.position).magnitude / 2f;
            
            Sequence seq = DOTween.Sequence();
            seq.Append(dropGoldPiece.transform.DOJump(midDropPos, jumpPower / 2, 1, 0.15f));
            seq.Append(dropGoldPiece.transform.DOJump(dropPos, jumpPower / 3, 1, 0.4f));
            seq.OnComplete(() => { dropGoldPiece.GetComponent<Collider2D>().enabled = true;});
        }

        private Vector3 DropPosition()
        {
            float x = Random.value < 0.5f ? Random.Range(-maxDropDistance, -minDropDistance) 
                : Random.Range(minDropDistance, maxDropDistance);
            float y = Random.value < 0.5f ? Random.Range(-maxDropDistance, -minDropDistance) 
                : Random.Range(minDropDistance, maxDropDistance);

            return new Vector3(x, y, 0);
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
