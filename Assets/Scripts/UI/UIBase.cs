using UnityEngine;

namespace UI
{
    public class UIBase : MonoBehaviour
    {
        public bool top;
        public int layer = 0;
        public int topLayer = 1;
        [SerializeField] private GameObject canvasOrPanel;

        public void CloseUI()
        {
            canvasOrPanel.gameObject.SetActive(false);
        }
    }
}
