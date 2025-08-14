using UnityEngine;

namespace Mush
{
    public class MushPiece : MonoBehaviour
    {
        public MushInfo mushInfo;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        }
    }
}
