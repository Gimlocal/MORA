using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D _playerRb;
        private SpriteRenderer _playerSr;
        private Animator _playerAnim;
        private Vector2 _movement;
        public bool canMove = true;
        public float lastMovementX = 1;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");

        private void Awake()
        {
            _playerRb = GetComponent<Rigidbody2D>();
            _playerSr = GetComponent<SpriteRenderer>();
            _playerAnim = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            if (!canMove) return;
            Move();
            FacingDirection();
        }

        private void Move()
        {
            _movement.x = 0; _movement.y = 0;
            
            if (Input.GetKey(KeyCode.LeftArrow)) _movement.x = lastMovementX = -1;
            if (Input.GetKey(KeyCode.RightArrow)) _movement.x = lastMovementX = 1;
            if (Input.GetKey(KeyCode.UpArrow)) _movement.y = 1;
            if (Input.GetKey(KeyCode.DownArrow)) _movement.y = -1;

            _playerAnim.SetBool(IsWalking, _movement != Vector2.zero);
            _movement.Normalize();

            Vector2 velocity = _playerRb.linearVelocity;
            velocity = Vector2.Lerp(velocity, _movement * Player.Instance.playerStat.moveSpeed, Player.Instance.playerStat.moveAcceleration * Time.fixedDeltaTime);
            _playerRb.linearVelocity = velocity;
        }

        public void StopPlayer()
        {
            _playerAnim.SetBool(IsWalking, false);
            _playerRb.linearVelocity = Vector2.zero;
        }

        private void FacingDirection()
        {
            _playerSr.flipX = lastMovementX < 0;
        }
    }
}
