using UnityEngine;

namespace UI
{
    public class QuitButton : MonoBehaviour
    {
        public void OnButtonClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
