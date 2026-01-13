using System.Collections;
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
        
        [Header("Falling Value")]
        [SerializeField] private float fallingDuration;
        [SerializeField] private float fallingSpeed;
        [SerializeField] private float fallingAmountY;
        [SerializeField] private float fallingAngle;

        private bool _isInRoom;
        private bool _isFalling;
        private Player.Player _player;
        private SpriteRenderer _playerSprite;
        private Vector3 _safePos;
        private int _groundOrder;

        private float _corruptAmount;

        private void Start()
        {
            _player = Player.Player.Instance;
            _playerSprite = _player.playerSprite.GetComponent<SpriteRenderer>();
            _safePos = _player.transform.position;
            _groundOrder = groundTilemap.GetComponent<TilemapRenderer>().sortingOrder;
            _corruptAmount = 30f;
        }
        
        private void Update()
        {
            if (!_isInRoom || _isFalling) return;

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
                _isInRoom = true;
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            base.OnTriggerExit2D(other);
            if (other.CompareTag("Player"))
            {
                _isInRoom = false;
            }
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

            _player.playerStat.Corrupt(_corruptAmount);
            _player.transform.localRotation = Quaternion.identity;
            _player.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            _playerSprite.sortingOrder =  _groundOrder + 1;
            _player.playerMovement.StopPlayer();
            _player.transform.position = _safePos;
            _playerSprite.enabled = true;

            yield return new WaitForSeconds(0.1f);
            
            _player.playerMining.canMine = true;
            _player.playerMovement.canMove = true;            
            _isFalling = false;
        }
    }
}
