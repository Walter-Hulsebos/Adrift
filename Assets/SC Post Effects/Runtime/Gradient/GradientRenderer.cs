#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class GradientRenderer : ScriptableRendererFeature
    {
        class GradientRenderPass : PostEffectRenderer<Gradient>
        {
            public GradientRenderPass()
            {
                shaderName = ShaderNames.Gradient;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Gradient>();
                
                if(volumeSettings)  renderer.EnqueuePass(this);
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

                if (volumeSettings.gradientTex.value) Material.SetTexture("_Gradient", volumeSettings.gradientTex.value);
                Material.SetColor("_Color1", volumeSettings.color1.value);
                Material.SetColor("_Color2", volumeSettings.color2.value);
                Material.SetFloat("_Rotation", volumeSettings.rotation.value * 6);
                Material.SetFloat("_Intensity", volumeSettings.intensity.value);
                Material.SetFloat("_BlendMode", (int)volumeSettings.mode.value);

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, (int)volumeSettings.input.value);
            }
        }

        GradientRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new GradientRenderPass();

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