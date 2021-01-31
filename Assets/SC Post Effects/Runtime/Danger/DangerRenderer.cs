#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class DangerRenderer : ScriptableRendererFeature
    {
        class DangerRenderPass : PostEffectRenderer<Danger>
        {
            public DangerRenderPass()
            {
                shaderName = ShaderNames.Danger;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Danger>();
                
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

                Material.SetVector("_Params", new Vector4(volumeSettings.intensity.value, volumeSettings.size.value, 0, 0));
                Material.SetColor("_Color", volumeSettings.color.value);
                var overlayTexture = volumeSettings.overlayTex.value == null ? Texture2D.blackTexture : volumeSettings.overlayTex.value;
                Material.SetTexture("_Overlay", overlayTexture);

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, 0);
            }
        }

        DangerRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new DangerRenderPass();

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