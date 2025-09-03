using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class EndingUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            StartCoroutine(Ending());  
        }

        private IEnumerator Ending()
        {
            yield return text.DOFade(1, 2f).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(0);
        }
    }
}
