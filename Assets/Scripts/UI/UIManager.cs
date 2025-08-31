using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public List<UIBase> uIList = new();

        public void RegisterUI(UIBase uI)
        {
            if (uIList.Count > 0)
            {
                uIList[^1].top = false;
                foreach (var uIBase in uIList)
                {
                    uIBase.GetComponentInParent<Canvas>().sortingOrder = uI.layer;
                }
            }

            if (uIList.Contains(uI))
            {
                uIList.Remove(uI);
            }

            uIList.Add(uI);
            uI.GetComponentInParent<Canvas>().sortingOrder = uI.topLayer;
            uI.top = true;
        }

        public void UnRegisterUI(UIBase ui)
        {
            if (uIList.Contains(ui))
            {
                uIList[^1].top = false;
                uIList.Remove(ui);
                if (uIList.Count > 0) uIList[^1].top = true;
            }
        }
    }
}
