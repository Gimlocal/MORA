using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D _playerRb;
        private SpriteRenderer _playerSr;
        private Animator _playerAnim;
        private Vector2 _movement;
        private float _lastMovementX = 1;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float moveAcceleration;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");

        private void Awake()
        {
            _playerRb = GetComponent<Rigidbody2D>();
            _playerSr = GetComponent<SpriteRenderer>();
            _playerAnim = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            Move();
            FacingDirection();
        }

        private void Move()
        {
            _movement.x = 0; _movement.y = 0;
            
            if (Input.GetKey(KeyCode.LeftArrow)) _movement.x = _lastMovementX = -1;
            if (Input.GetKey(KeyCode.RightArrow)) _movement.x = _lastMovementX = 1;
            if (Input.GetKey(KeyCode.UpArrow)) _movement.y = 1;
            if (Input.GetKey(KeyCode.DownArrow)) _movement.y = -1;

            _playerAnim.SetBool(IsWalking, _movement != Vector2.zero);
            _movement.Normalize();

            Vector2 velocity = _playerRb.linearVelocity;
            velocity = Vector2.Lerp(velocity, _movement * moveSpeed, moveAcceleration * Time.fixedDeltaTime);
            _playerRb.linearVelocity = velocity;
        }

        private void FacingDirection()
        {
            _playerSr.flipX = _lastMovementX < 0;
        }
    }
}
