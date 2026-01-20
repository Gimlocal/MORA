using System.Collections;
using Cam;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Stage
{
    public class CliffTrapRoom : RoomSetting
    {
        [Header("Ground Setting")]
        [SerializeField] private Tilemap groundTilemap;
        
        [Header("Trap Value")]
        [SerializeField] private float fallingDuration;
        [SerializeField] private float fallingSpeed;
        [SerializeField] private float fallingAmountY;
        [SerializeField] private float fallingAngle;
        [SerializeField] private float corruptAmount;
        
        private bool _isFalling;
        private Player.Player _player;
        private SpriteRenderer _playerSprite;
        private Collider2D _playerCollider;
        private Vector3 _safePos;
        private int _groundOrder;
        
        private void Start()
        {
            _player = Player.Player.Instance;
            _playerSprite = _player.playerSprite.GetComponent<SpriteRenderer>();
            _playerCollider = _player.GetComponent<Collider2D>();
            _safePos = _player.transform.position;
            _groundOrder = groundTilemap.GetComponent<TilemapRenderer>().sortingOrder;
        }
        
        private void Update()
        {
            if (!IsInRoom || _isFalling) return;

            Vector3 playerPos = _player.transform.position;
            
            if (CheckOnGround(playerPos))
            {
                _safePos = GetSafeTileCenter(playerPos);
            }
            else
            {
                StartCoroutine(Falling());
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other.CompareTag("Player"))
            {
                Player.Player.Instance.playerItem.UseDefaultLantern(true);
            }
        }

        protected override void OnPlayerLeftRoom()
        {
            base.OnPlayerLeftRoom();
            Player.Player.Instance.playerItem.UseDefaultLantern(false);
        }
        
        private bool CheckOnGround(Vector3 pos)
        {
            Vector3Int cell = groundTilemap.WorldToCell(pos);
            return groundTilemap.HasTile(cell);
        }
        
        private Vector3 GetSafeTileCenter(Vector2 pos)
        {
            Vector3Int cell = groundTilemap.WorldToCell(pos);
            return groundTilemap.GetCellCenterWorld(cell);
        }

        private IEnumerator Falling()
        {
            _isFalling = true;
            _player.playerMovement.canMove = false;
            _player.playerMining.canMine = false;
            _playerCollider.enabled = false;

            _playerSprite.sortingOrder = _groundOrder - 1;
            float playerDirection = _playerSprite.flipX ? 1 : -1;
            
            Sequence seq = DOTween.Sequence();

            Tween t1 = _player.transform.DOMove(_player.transform.position - new Vector3(0, fallingAmountY, 0), fallingDuration);
            Tween t2 = _player.transform.DOScale(0.01f, fallingDuration);
            Tween t3 = _player.transform.DORotate(new Vector3(0, 0, playerDirection * fallingAngle), fallingDuration);
            
            seq.Append(t1);
            seq.Join(t2);
            seq.Join(t3);
            
            yield return seq.WaitForCompletion();
            _playerSprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            
            // todo : Effect & Sound

            _player.playerStat.Corrupt(corruptAmount);
            _player.transform.localRotation = Quaternion.identity;
            _player.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            _playerSprite.sortingOrder =  _groundOrder + 1;
            _player.playerMovement.StopPlayer();
            _player.transform.position = _safePos;
            _playerSprite.enabled = true;

            yield return new WaitForSeconds(0.1f);
            
            _player.playerMining.canMine = true;
            _player.playerMovement.canMove = true; 
            _playerCollider.enabled = true;
            _isFalling = false;
        }
    }
}
