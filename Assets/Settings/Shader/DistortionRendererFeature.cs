using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Settings.Shader
{
    [Serializable]
    public class DistortionRendererFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class DistortionSettings
        {
            public Material blitMaterial = null; // 할당할 머티리얼 (아래 셰이더 사용)
            public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public DistortionSettings settings = new DistortionSettings();

        DistortionPass m_Pass;

        public override void Create()
        {
            // 패스 생성 (material은 Inspector에서 할당)
            if (settings.blitMaterial != null)
            {
                m_Pass = new DistortionPass(settings.blitMaterial);
                m_Pass.renderPassEvent = settings.injectionPoint;
            }
            else
            {
                m_Pass = null;
            }
        }

        // 카메라 타겟이 준비되었을 때(렌더타겟 디스크립터 전달)
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (m_Pass != null)
                m_Pass.Setup(renderingData.cameraData.cameraTargetDescriptor);
        }

        // 매 프레임(카메라 당) 호출 — 볼륨값을 읽어서 패스에 전달하고 큐에 넣음
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_Pass == null || settings.blitMaterial == null) return;

            // Volume 값 읽기
            var stack = VolumeManager.instance.stack;
            var post = stack.GetComponent<PostDistortion>();
            if (post == null || !post.IsActive()) return;

            m_Pass.SetParams(post.intensity.value, post.speed.value, post.scale.value);

            // 게임 카메라에만 넣고 싶으면 카메라 타입 체크 가능
            renderer.EnqueuePass(m_Pass);
        }

        protected override void Dispose(bool disposing)
        {
            // Material은 외부에서 관리(Inspector에 할당)한다고 가정 -> 삭제하지 않음.
        }

        // =========================
        // Distortion Pass (RenderGraph 방식)
        // =========================
        class DistortionPass : ScriptableRenderPass
        {
            Material m_Material;
            float m_Intensity = 0f;
            float m_Speed = 1f;
            float m_Scale = 20f;
            RenderTextureDescriptor m_CameraDesc;

            public DistortionPass(Material mat)
            {
                m_Material = mat;
            }

            public void SetParams(float intensity, float speed, float scale)
            {
                m_Intensity = intensity;
                m_Speed = speed;
                m_Scale = scale;
            }

            public void Setup(RenderTextureDescriptor cameraDescriptor)
            {
                m_CameraDesc = cameraDescriptor;
            }

            // RenderGraph에 패스(블릿)를 등록하는 지점
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (m_Material == null) return;
                if (m_Intensity <= 0f) return;

                // 프레임 리소스 읽기
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

                // 활성 컬러 텍스처(현재 프레임의 source)
                TextureHandle source = resourceData.activeColorTexture;
                if (!source.IsValid()) return;

                // destination 임시 텍스처 생성 (카메라 타겟에 맞춤)
                var desc = cameraData.cameraTargetDescriptor;
                desc.msaaSamples = 1;     // blit용으론 일반적으로 resolve된 텍스처 사용
                desc.depthBufferBits = 0; // 색상 전용
                desc.colorFormat = cameraData.cameraTargetDescriptor.colorFormat; // HDR / sRGB 일치

                TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "Distortion_Dest", false);

                // MaterialPropertyBlock을 이용해 per-pass 파라미터 안전하게 지정
                var mpb = new MaterialPropertyBlock();
                mpb.SetFloat("_Intensity", m_Intensity);
                mpb.SetFloat("_Speed", m_Speed);
                mpb.SetFloat("_Scale", m_Scale);

                // Blit with material (RenderGraph helper)
                // 생성자 인자(많음)를 안전하게 제공: (src, dst, scale, offset, material, pass, mpb, dstSlice, dstMip, numSlices, numMips, srcSlice, srcMip)
                var blitParams = new RenderGraphUtils.BlitMaterialParameters(
                    source,
                    destination,
                    Vector2.one,
                    Vector2.zero,
                    m_Material,
                    0,      // shader pass
                    mpb,
                    0, 0,  // destinationSlice, destinationMip
                    1, 1,    // numSlices, numMips
                    0, 0   // sourceSlice, sourceMip
                );

                renderGraph.AddBlitPass(blitParams, "PostDistortion_Blit");

                // downstream 패스들이 수정된 카메라 컬러를 사용하게 함 (double-blit 피하기)
                resourceData.cameraColor = destination;
            }
        }
    }
}
