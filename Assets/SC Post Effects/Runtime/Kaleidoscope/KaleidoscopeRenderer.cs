#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class KaleidoscopeRenderer : ScriptableRendererFeature
    {
        class KaleidoscopeRenderPass : PostEffectRenderer<Kaleidoscope>
        {
            public KaleidoscopeRenderPass()
            {
                shaderName = ShaderNames.Kaleidoscope;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Kaleidoscope>();
                
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

                //Do not render for scene view camera
                //if (renderingData.cameraData.isSceneViewCamera) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                Material.SetFloat("_Splits", Mathf.PI * 2 / Mathf.Max(1, volumeSettings.splits.value));

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, 0);
            }
        }

        KaleidoscopeRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new KaleidoscopeRenderPass();

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