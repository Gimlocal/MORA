Shader "PostDistortion"
{
    Properties
    {
        _Intensity ("Intensity", Range(0,1)) = 0.5
        _Scale ("Scale", Float) = 20
        _Speed ("Speed", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _BlitTexture; // RenderGraph/Blitter가 바인드
            float _Intensity;
            float _Scale;
            float _Speed;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            // Full-screen triangle using SV_VertexID
            v2f vert(uint vertexID : SV_VertexID)
            {
                v2f o;
                // Full-screen triangle positions
                o.pos = float4((vertexID == 2) ? 3.0f : -1.0f, (vertexID == 1) ? -3.0f : 1.0f, 0.0f, 1.0f);
                o.uv  = o.pos.xy * 0.5f + 0.5f;
                #if UNITY_UV_STARTS_AT_TOP
                o.uv.y = 1.0 - o.uv.y;
                #endif
                return o;
            }

            float4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                // time (use _Time.y for seconds)
                float t = _Time.y * _Speed;

                // 물결/왜곡 오프셋
                float2 offset;
                offset.x = sin(uv.y * _Scale + t) * (_Intensity * 0.02);
                offset.y = cos(uv.x * _Scale - t) * (_Intensity * 0.02);

                uv = clamp(uv + offset, 0.0, 1.0);

                float4 col = tex2D(_BlitTexture, uv);

                return col;
            }
            ENDHLSL
        }
    }
    Fallback Off
}
