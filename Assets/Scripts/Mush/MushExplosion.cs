using UnityEngine;

namespace Mush
{
    public class MushExplosion : MonoBehaviour
    {
        public float amount;
        private bool _corrupted;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (!_corrupted)
                {
                    Player.Player.Instance.playerStat.Corrupt(amount);
                    _corrupted = true;
                }
            }
        }

        public void OnExplosionEnd()
        {
            Destroy(gameObject);
        }
    }
}
