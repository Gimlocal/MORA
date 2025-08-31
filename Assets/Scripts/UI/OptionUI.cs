using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI
{
    public class OptionUI : UIBase
    {
        public void OnButtonClick()
        {
            SceneManager.LoadScene(0);
        }
    }
}
