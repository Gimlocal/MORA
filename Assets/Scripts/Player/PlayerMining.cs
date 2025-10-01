using System;
using System.Collections.Generic;
using Sound;
using Tool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMining : MonoBehaviour
    {
        public List<Tools> tools;
        public ToolName toolName;
        public bool canMine = true;

        private Tools _tool;
        public event Action OnToolsChanged;

        private void Update()
        {
            if (!canMine) return;
            Mining();
        }
        
        public void AddTool(Tools tool)
        {
            tools.Add(tool);
            OnToolsChanged?.Invoke();
        }

        private void Mining()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _tool = tools.Find(t => t.toolName == toolName);
                _tool.gameObject.SetActive(true);
                _tool.Mining();
            }
        }
    }
}
