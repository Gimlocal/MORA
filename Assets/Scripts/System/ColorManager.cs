using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace System
{
    public enum ObjectName
    {
        GreenMush = 0,
        BlueMush = 1,
        RedMush = 2,
        LightBlueMush = 3,
        WhiteMush = 4,
        GoldMush = 5,
        MetalMush = 6,
        BombMush = 7,
        Stone = 100,
    }

    [Serializable]
    public class ObjectSprite
    {
        public ObjectName objectName;
        public Sprite sprite;
    }
    
    public class ColorManager : MonoBehaviour
    {
        public List<ObjectSprite> Sprites = new();

        public Color GetSpriteColor(ObjectName objectName)
        {
            return GetAverageColor(Sprites.FirstOrDefault(c => c.objectName == objectName)?.sprite);
        }

        /// <summary>
        /// Get Average Color of Sprite
        /// </summary>
        private Color GetAverageColor(Sprite sprite)
        {
            Texture2D tex = sprite.texture;
            Rect r = sprite.textureRect;

            int x = Mathf.RoundToInt(r.x);
            int y = Mathf.RoundToInt(r.y);
            int w = Mathf.RoundToInt(r.width);
            int h = Mathf.RoundToInt(r.height);
            
            Color accum = Color.black;
            int count = 0;

            // Sprite의 Read/Write Enabled가 켜져 있어야 됨.
            Color[] pixels = tex.GetPixels(x, y, w, h);

            for (int i = 0; i < pixels.Length; i++)
            {
                accum += pixels[i];
            }

            count = pixels.Length;
            if (count > 0)
                return accum / count;
            else
                return Color.gray;
        }
    }
    
    
}
