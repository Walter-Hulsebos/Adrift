#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class CloudShadowsRenderer : ScriptableRendererFeature
    {
        class CloudShadowsRenderPass : PostEffectRenderer<CloudShadows>
        {
            public CloudShadowsRenderPass()
            {
                shaderName = ShaderNames.CloudShadows;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<CloudShadows>();
                
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
   
                CloudShadows.isOrtho = renderingData.cameraData.camera.orthographic;

                var noiseTexture = volumeSettings.texture.value == null ? Texture2D.whiteTexture : volumeSettings.texture.value;
                Material.SetTexture("_NoiseTex", noiseTexture);

                var p = GL.GetGPUProjectionMatrix(renderingData.cameraData.camera.projectionMatrix, false);
                p[2, 3] = p[3, 2] = 0.0f;
                p[3, 3] = 1.0f;
                var clipToWorld = Matrix4x4.Inverse(p * renderingData.cameraData.camera.worldToCameraMatrix) * Matrix4x4.TRS(new Vector3(0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);
                Material.SetMatrix("clipToWorld", clipToWorld);

                float cloudsSpeed = volumeSettings.speed.value * 0.1f;
                Material.SetVector("_CloudParams", new Vector4(volumeSettings.size.value * 0.01f, volumeSettings.direction.value.x * cloudsSpeed, volumeSettings.direction.value.y * cloudsSpeed, volumeSettings.density.value));             

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, 0);
            }

            public override void Cleanup(CommandBuffer cmd)
            {
                base.Cleanup(cmd);
            }
        }

        CloudShadowsRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new CloudShadowsRenderPass();

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