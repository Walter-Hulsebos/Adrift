#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class RipplesRenderer : ScriptableRendererFeature
    {
        class RipplesRenderPass : PostEffectRenderer<Ripples>
        {
            public RipplesRenderPass()
            {
                shaderName = ShaderNames.Ripples;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Ripples>();
                
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

                Material.SetFloat("_Strength", (volumeSettings.strength.value * 0.01f));
                Material.SetFloat("_Distance", (volumeSettings.distance.value * 0.01f));
                Material.SetFloat("_Speed", volumeSettings.speed.value);
                Material.SetVector("_Size", new Vector2(volumeSettings.width.value, volumeSettings.height.value));

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, (int)volumeSettings.mode.value);
            }
        }

        RipplesRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new RipplesRenderPass();

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