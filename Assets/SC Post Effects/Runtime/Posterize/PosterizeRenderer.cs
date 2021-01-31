#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class PosterizeRenderer : ScriptableRendererFeature
    {
        class PosterizeRenderPass : PostEffectRenderer<Posterize>
        {
            public PosterizeRenderPass()
            {
                shaderName = ShaderNames.Posterize;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Posterize>();
                
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

                Material.SetVector("_Params", new Vector4(volumeSettings.hue.value, volumeSettings.saturation.value, volumeSettings.value.value, volumeSettings.levels.value));

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, volumeSettings.hsvMode.value ? 1 : 0);
            }
        }

        PosterizeRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new PosterizeRenderPass();

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