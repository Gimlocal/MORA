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
            Rect r = sprite.textureRect; // or sprite.rect depending Unity version
            // textureRect는 텍스처 상에서 이 스프라이트가 차지하는 영역(픽셀 단위)

            int x = Mathf.RoundToInt(r.x);
            int y = Mathf.RoundToInt(r.y);
            int w = Mathf.RoundToInt(r.width);
            int h = Mathf.RoundToInt(r.height);

            // 성능 때문에 전체 픽셀 다 읽는 게 부담이면 샘플링 간격을 늘릴 수도 있음
            Color accum = Color.black;
            int count = 0;

            // NOTE: texture.GetPixel() / GetPixels() 는 Read/Write Enabled가 켜져 있어야 함!
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
