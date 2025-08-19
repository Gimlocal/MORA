using System.Collections;
using System.Data;
using UnityEngine;
using DG.Tweening;

namespace Mush
{
    public class Mush : MonoBehaviour
    {
        [SerializeField] private MushDatabase mushDatabase;
        [SerializeField] private MushId mushId;
        [SerializeField] private float hp;
        [SerializeField] private int dropInterval;
        private int _hitCount;
        private SpriteRenderer _sR;
        private Collider2D _col;
        private Coroutine _flickCoroutine;

        private void Awake()
        {
            _sR = GetComponent<SpriteRenderer>();
            _col = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Tool"))
            {
                OnMined();
            }
        }

        private void OnMined()
        {
            if (hp > 0)
            {
                Flick();
                _hitCount++;
                if (_hitCount % dropInterval == 0) DropPiece();
                hp--;
                if (hp == 0) OnDead();
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
            Color originalColor = _sR.color;
            Color destColor = originalColor;
            destColor.a = 0.5f;
            _sR.color = destColor;
            yield return new WaitForSeconds(duration);
            _sR.color = originalColor; 
        }

        private void DropPiece()
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 dropPos = transform.position + (Vector3)randomDir;
            
            // piece 생성
            GameObject dropPiece = new GameObject("Piece");
            dropPiece.transform.position = transform.position;
            SpriteRenderer sR = dropPiece.AddComponent<SpriteRenderer>();
            sR.sprite = _sR.sprite;
            sR.sortingOrder = _sR.sortingOrder;
            CircleCollider2D cd = sR.gameObject.AddComponent<CircleCollider2D>();
            cd.radius = 0.12f;
            cd.isTrigger = true;
            MushPiece mushPiece = dropPiece.AddComponent<MushPiece>();
            mushPiece.mushInfo = mushDatabase.GetPieceById(mushId);
            
            dropPiece.transform.DOJump(dropPos, 0.5f, 1, 0.5f).
                OnComplete(() => { dropPiece.GetComponent<Collider2D>().enabled = true;});
        }

        private void OnDead()
        {
            _col.enabled = false;
            _sR.DOFade(0, 1f).OnComplete(() => { Destroy(gameObject); });
        }
    }
}
