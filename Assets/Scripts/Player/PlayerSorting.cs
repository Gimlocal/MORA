using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Player
{
    public class PlayerSorting : MonoBehaviour
    {
        private SpriteRenderer _playerSR;
        private CapsuleCollider2D _capsuleCol;
        private int _defaultOrder;
        private RaycastHit2D _hit;
        private void Awake()
        {
            _playerSR = GetComponent<SpriteRenderer>();
            _capsuleCol = GetComponent<CapsuleCollider2D>();
            _defaultOrder = _playerSR.sortingOrder;
        }

        private void Update()
        {
            DynamicSort();
        }

        private void DynamicSort()
        {
            Vector2 playerPos = (Vector2)transform.position + _capsuleCol.offset * transform.localScale;
            RaycastHit2D hitUp = Physics2D.Raycast(playerPos, Vector2.up, 1f, LayerMask.GetMask("Wall"));
            RaycastHit2D hitDown = Physics2D.Raycast(playerPos, Vector2.down, 1f, LayerMask.GetMask("Wall"));

            if (!hitUp.collider && !hitDown.collider)
            {
                // 둘 다 충돌 없을 때 처리 (예: 아무것도 하지 않음)
                if (_hit.collider) _hit.transform.GetComponent<TilemapRenderer>().sortingOrder = _defaultOrder;
                return;
            }

            RaycastHit2D hit;

            if (hitUp.collider && hitDown.collider)
            {
                // 둘 다 있을 때 가까운 쪽 선택
                hit = (hitUp.point.y - playerPos.y) > (playerPos.y - hitDown.point.y) ? hitDown : hitUp;
            }
            else if (hitUp.collider)
            {
                hit = hitUp;
            }
            else // hitDown.collider만 있을 때
            {
                hit = hitDown;
            }

            _hit = hit;

            var tilemapRenderer = _hit.transform.GetComponent<TilemapRenderer>();
            if (tilemapRenderer != null)
            {
                tilemapRenderer.sortingOrder = _defaultOrder + (playerPos.y - hit.point.y > 0 ? 1 : -1);
            }

        }
    }
}
