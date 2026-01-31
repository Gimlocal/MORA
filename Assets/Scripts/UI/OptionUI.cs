using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class OptionUI : UIBase
    {
        private void Start()
        {
            SetFirstButton();
        }

        private void SetFirstButton()
        {
            var button = GetComponentInChildren<Button>().gameObject;
            
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button);
        }
        
        public void OnButtonClick()
        {
            SceneManager.LoadScene(0);
        }
    }
}
