using Player;
using UnityEngine;

namespace Tool
{
    public enum ToolName
    {
        Pickaxe,
        Drill,
    }

    public enum ToolType
    {
        OneTime,
        Continuous,
    }
    
    public abstract class Tools : MonoBehaviour
    {
        public ToolName toolName;
        public ToolType toolType;
        public float mineInterval;
        public string audioKey;
        public string hitAudioKey;
        public string toolKName;
        [TextArea] public string toolDescription;
        
        public abstract void Mining();
    }
}
