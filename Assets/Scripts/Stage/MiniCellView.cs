using Database;
using UnityEngine;
using UnityEngine.UI;

namespace Stage
{
    public class MiniCellView : MonoBehaviour
    {
        [Header("Refs")]
        public Image roomType;
        public GameObject highlight;
        public CanvasGroup fog;

        public DoorMask Doors { get; private set; }

        public void SetType(RoomType t) 
        {
            Color c = Color.gray;
            switch (t) {
                case RoomType.Start: c = Color.white; break;
                case RoomType.Normal: c = new Color(0.65f,0.65f,0.65f); break;
                case RoomType.Special: c = new Color(0.3f,0.6f,1f); break;
                case RoomType.Trap: c = new Color(1f,0.55f,0.2f); break;
                case RoomType.Boss: c = new Color(1f,0.2f,0.2f); break;
            }
            roomType.color = c;
        }

        public void SetDoors(DoorMask d) 
        {
            Doors = d;
        }

        public void SetHighlight(bool on) 
        {
            if (highlight)
            {
                highlight.SetActive(on);
            }
        }

        public void SetDiscovered(bool on) 
        {
            if (fog)
            {
                fog.alpha = on ? 0f : 1f;
            }
        }
    }
}