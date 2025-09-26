using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public List<UIBase> uIList = new();
        [SerializeField] private GameObject optionPanel;
        public bool isOptionOpened;

        private void Update()
        {
            ManageUI();
        }

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

        private void ManageUI()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (uIList.Count > 0 && !isOptionOpened)
                {
                    UIBase uI = uIList[^1];
                    UnRegisterUI(uI);
                    uI.CloseUI();
                    if (uIList.Count == 0)
                    {
                        Player.Player.Instance.playerMovement.canMove = true;
                    }
                }
                else if (uIList.Count == 0)
                {
                    bool opened = optionPanel.activeSelf;
                    if (!opened)
                    {
                        isOptionOpened = true;
                        Player.Player.Instance.playerMovement.canMove = false;
                        Player.Player.Instance.playerMovement.StopPlayer();
                        optionPanel.SetActive(true);
                    }
                    else
                    {
                        isOptionOpened = false;
                        Player.Player.Instance.playerMovement.canMove = true;
                        optionPanel.SetActive(false);
                    }
                }
            }
        }
    }
}
