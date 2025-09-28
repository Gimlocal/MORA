using System.Collections;
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
                transform.localScale = Player.Player.Instance.playerMovement.lastMovementX > 0 ?
                    new Vector3(0.6f, 0.6f, 1) : new Vector3(-0.6f, 0.6f, 1);

                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
