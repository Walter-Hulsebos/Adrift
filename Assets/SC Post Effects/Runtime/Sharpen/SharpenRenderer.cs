#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class SharpenRenderer : ScriptableRendererFeature
    {
        class SharpenRenderPass : PostEffectRenderer<Sharpen>
        {
            public SharpenRenderPass()
            {
                shaderName = ShaderNames.Sharpen;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Sharpen>();
                
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

                Material.SetVector("_Params", new Vector2(volumeSettings.amount.value, volumeSettings.radius.value));

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, 0);
            }
        }

        SharpenRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new SharpenRenderPass();

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
