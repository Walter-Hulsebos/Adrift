#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class OverlayRenderer : ScriptableRendererFeature
    {
        class OverlayRenderPass : PostEffectRenderer<Overlay>
        {
            public OverlayRenderPass()
            {
                shaderName = ShaderNames.Overlay;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Overlay>();
                
                if(volumeSettings) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!volumeSettings) return;
                if (volumeSettings.IsActive() == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                if (volumeSettings.overlayTex.value) Material.SetTexture("_OverlayTex", volumeSettings.overlayTex.value);
                Material.SetVector("_Params", new Vector4(volumeSettings.intensity.value, Mathf.Pow(volumeSettings.tiling.value + 1, 2), volumeSettings.autoAspect.value ? 1f : 0f, (int)volumeSettings.blendMode.value));

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, 0);
            }
        }

        OverlayRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new OverlayRenderPass();

            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
#endif
}