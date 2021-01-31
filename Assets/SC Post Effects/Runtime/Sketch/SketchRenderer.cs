#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class SketchRenderer : ScriptableRendererFeature
    {
        class SketchRenderPass : PostEffectRenderer<Sketch>
        {
            public SketchRenderPass()
            {
                shaderName = ShaderNames.Sketch;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                this.cameraDepthTarget = GetCameraDepthTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Sketch>();
                
                if(volumeSettings) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                requiresDepth = volumeSettings.projectionMode == Sketch.SketchProjectionMode.WorldSpace;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!volumeSettings) return;
                if (volumeSettings.IsActive() == false) return;
                //if (!renderingData.postProcessingEnabled) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);
                
                CopyTargets(cmd, renderingData);
                
                var p = GL.GetGPUProjectionMatrix(renderingData.cameraData.camera.projectionMatrix, false);
                p[2, 3] = p[3, 2] = 0.0f;
                p[3, 3] = 1.0f;
                var clipToWorld = Matrix4x4.Inverse(p * renderingData.cameraData.camera.worldToCameraMatrix) *
                                  Matrix4x4.TRS(new Vector3(0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);
                Material.SetMatrix("clipToWorld", clipToWorld);

                if (volumeSettings.strokeTex.value) Material.SetTexture("_Strokes", volumeSettings.strokeTex.value);

                Material.SetVector("_Params", new Vector4(0, (int)volumeSettings.blendMode.value, volumeSettings.intensity.value, ((int)volumeSettings.projectionMode.value == 1) ? volumeSettings.tiling.value * 0.1f : volumeSettings.tiling.value));
                Material.SetVector("_Brightness", volumeSettings.brightness.value);

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, (int)volumeSettings.projectionMode.value);
            }
        }

        SketchRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new SketchRenderPass();

            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.CheckForStackedRendering(renderer, renderingData.cameraData);
            m_ScriptablePass.Setup(renderer);
        }
    }
#endif
}
