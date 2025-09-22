using System;
using UnityEngine.Rendering;

namespace Settings.Shader
{
    [Serializable, VolumeComponentMenu("Custom/PostDistortion")]
    public class PostDistortion : VolumeComponent, IPostProcessComponent
    {
        // 화면 흔들림 강도 (0 = off)
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        // 흔들림 속도
        public FloatParameter speed = new FloatParameter(1f);

        // 흔들림 파장 (빈도/스케일)
        public FloatParameter scale = new FloatParameter(20f);

        public bool IsActive() => intensity.value > 0f;
        public bool IsTileCompatible() => false;
    }
}