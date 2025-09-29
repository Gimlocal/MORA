using System.Collections;
using Sound;
using UnityEngine;

namespace Tool
{
    public class Drill : Tools
    {
        public override void Mining()
        {
            StartCoroutine(DrillCoroutine());
        }

        private IEnumerator DrillCoroutine()
        {
            while (Input.GetKey(KeyCode.Z))
            {
                SoundManager.Instance.Play(AudioCategory.Tool, audioKey, true);
                transform.localScale = Player.Player.Instance.playerMovement.lastMovementX > 0 ?
                    new Vector3(0.6f + Random.Range(-0.1f, 0.1f) * Player.Player.Instance.transform.localScale.x, 0.6f + Random.Range(-0.1f, 0.1f) * Player.Player.Instance.transform.localScale.x, 1) :
                    new Vector3(-0.6f + Random.Range(-0.1f, 0.1f) * Player.Player.Instance.transform.localScale.x, 0.6f + Random.Range(-0.1f, 0.1f) * Player.Player.Instance.transform.localScale.x, 1);

                yield return null;
            }

            SoundManager.Instance.Stop(AudioCategory.Tool);
            gameObject.SetActive(false);
        }
    }
}
