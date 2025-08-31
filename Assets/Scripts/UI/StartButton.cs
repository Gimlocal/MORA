using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class StartButton : MonoBehaviour
    {
        public void OnButtonClick()
        {
            SceneManager.LoadScene(1);
        }
    }
}
