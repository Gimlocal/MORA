using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class TitleUI : MonoBehaviour
    {
        private GameObject _firstButton;

        private void Start()
        {
            SetStartButton();
        }

        private void SetStartButton()
        {
            var buttons = GetComponentsInChildren<Button>();
            _firstButton = buttons.FirstOrDefault(b => b.gameObject.activeInHierarchy)?.gameObject;
            
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_firstButton);
        }
    }
}
