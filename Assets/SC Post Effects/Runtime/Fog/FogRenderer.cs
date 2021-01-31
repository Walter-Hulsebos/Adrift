#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class FogRenderer : ScriptableRendererFeature
    {
        public static Material FogMaterial;
        public static bool enableSkyboxCapture;

        class FogRenderPass : PostEffectRenderer<Fog>
        {
            public FogRenderPass()
            {
                shaderName = ShaderNames.Fog;
                requiresDepth = true;
                ProfilerTag = this.ToString();
            }
            enum Pass
            {
                Prefilter,
                Downsample,
                Upsample,
                Blend,
                BlendScattering
            }
            
            public void Setup(ScriptableRenderer renderer)
            {
                FogMaterial = this.Material;
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Fog>();
                
                if(volumeSettings) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings)
                {
                    enableSkyboxCapture = false;
                    return;
                }

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!volumeSettings) return;
                if (!volumeSettings.IsActive()) return;

                base.Execute(context, ref renderingData);

                var cmd = CommandBufferPool.Get(ProfilerTag);
                
                CopyTargets(cmd, renderingData);
                
                enableSkyboxCapture = volumeSettings.colorSource == Fog.FogColorSource.SkyboxColor;

                #region Property value composition
                var p = GL.GetGPUProjectionMatrix(renderingData.cameraData.camera.projectionMatrix, false);
                p[2, 3] = p[3, 2] = 0.0f;
                p[3, 3] = 1.0f;
                var clipToWorld = Matrix4x4.Inverse(p * renderingData.cameraData.camera.worldToCameraMatrix) * Matrix4x4.TRS(new Vector3(0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);
                Material.SetMatrix("clipToWorld", clipToWorld);

                float FdotC = renderingData.cameraData.camera.transform.position.y - volumeSettings.height.value;
                float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
                //Always exclude skybox for skybox color mode
                //Always include when using light scattering to avoid depth discontinuity
                float skyboxInfluence = (volumeSettings.lightScattering.value) ? 1.0f : volumeSettings.skyboxInfluence.value;
                float distanceFog = (volumeSettings.distanceFog.value) ? 1.0f : 0.0f;
                float heightFog = (volumeSettings.heightFog.value) ? 1.0f : 0.0f;

                int colorSource = (volumeSettings.useSceneSettings.value) ? 0 : (int)volumeSettings.colorSource.value;
                var sceneMode = (volumeSettings.useSceneSettings.value) ? RenderSettings.fogMode : volumeSettings.fogMode.value;
                var sceneDensity = (volumeSettings.useSceneSettings.value) ? RenderSettings.fogDensity : volumeSettings.globalDensity.value / 100;
                var sceneStart = (volumeSettings.useSceneSettings.value) ? RenderSettings.fogStartDistance : volumeSettings.fogStartDistance.value;
                var sceneEnd = (volumeSettings.useSceneSettings.value) ? RenderSettings.fogEndDistance : volumeSettings.fogEndDistance.value;

                bool linear = (sceneMode == FogMode.Linear);
                float diff = linear ? sceneEnd - sceneStart : 0.0f;
                float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;

                Vector4 sceneParams;
                sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
                sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
                sceneParams.z = linear ? -invDiff : 0.0f;
                sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;

                float gradientDistance = (volumeSettings.gradientUseFarClipPlane.value) ? volumeSettings.gradientDistance.value : renderingData.cameraData.camera.farClipPlane;
                #endregion

                #region Property assignment
                if (volumeSettings.heightNoiseTex.value) Material.SetTexture("_NoiseTex", volumeSettings.heightNoiseTex.value);
                if (volumeSettings.fogColorGradient.value) Material.SetTexture("_ColorGradient", volumeSettings.fogColorGradient.value);
                Material.SetFloat("_FarClippingPlane", gradientDistance);
                Material.SetVector("_SceneFogParams", sceneParams);
                Material.SetVector("_SceneFogMode", new Vector4((int)sceneMode, volumeSettings.useRadialDistance.value ? 1 : 0, colorSource, volumeSettings.heightFogNoise.value ? 1 : 0));
                Material.SetVector("_NoiseParams", new Vector4(volumeSettings.heightNoiseSize.value * 0.01f, volumeSettings.heightNoiseSpeed.value * 0.01f, volumeSettings.heightNoiseStrength.value, 0));
                Material.SetVector("_DensityParams", new Vector4(volumeSettings.distanceDensity.value, volumeSettings.heightNoiseStrength.value, volumeSettings.skyboxMipLevel.value, 0));
                Material.SetVector("_HeightParams", new Vector4(volumeSettings.height.value, FdotC, paramK, volumeSettings.heightDensity.value * 0.5f));
                Material.SetVector("_DistanceParams", new Vector4(-sceneStart, 0f, distanceFog, heightFog));
                Material.SetColor("_FogColor", (volumeSettings.useSceneSettings.value) ? RenderSettings.fogColor : volumeSettings.fogColor.value);
                Material.SetVector("_SkyboxParams", new Vector4(skyboxInfluence, volumeSettings.skyboxMipLevel.value, 0, 0));

                Vector3 sunDir = (volumeSettings.useLightDirection.value) ? FogLightSource.sunDirection : volumeSettings.lightDirection.value.normalized;
                float sunIntensity = (volumeSettings.useLightIntensity.value) ? FogLightSource.intensity : volumeSettings.lightIntensity.value;
                sunIntensity = (volumeSettings.enableDirectionalLight.value) ? sunIntensity : 0f;
                Material.SetVector("_DirLightParams", new Vector4(sunDir.x, sunDir.y, sunDir.z, sunIntensity));

                Color sunColor = (volumeSettings.useLightColor.value) ? FogLightSource.color : volumeSettings.lightColor.value;
                Material.SetVector("_DirLightColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, 0));
                #endregion

                bool enableScattering = false;

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, enableScattering ? (int)Pass.BlendScattering : (int)Pass.Blend);
            }
        }


        public class SkyboxTextureRenderPass : ScriptableRenderPass
        {
            public RenderTexture skyboxTex;
            RenderTargetIdentifier skyboxTexID;

            public string ProfilerTag = "Skybox to texture";

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!enableSkyboxCapture) return;

                RenderTextureDescriptor dsc = cameraTextureDescriptor;
                dsc.width /= 2;
                dsc.height /= 2;

                skyboxTex = RenderTexture.GetTemporary(dsc.width, dsc.height, 0);
                skyboxTex.filterMode = FilterMode.Trilinear;
                skyboxTex.useMipMap = true;

                skyboxTexID = new RenderTargetIdentifier(skyboxTex);

                ConfigureTarget(skyboxTexID);
                ConfigureClear(ClearFlag.All, Color.black);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!enableSkyboxCapture) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                context.DrawSkybox(renderingData.cameraData.camera);

                //Doesn't work in this scope, cmd's execute at different points
                //cmd.SetGlobalTexture("_SkyboxTex", source);

                //Works
                if(FogMaterial) FogMaterial.SetTexture("_SkyboxTex", skyboxTex);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            
#if URP_9_0_OR_NEWER
            public override void OnCameraCleanup(CommandBuffer cmd)
#else
            public override void FrameCleanup(CommandBuffer cmd)
#endif
            {
                if (enableSkyboxCapture) RenderTexture.ReleaseTemporary(skyboxTex);
            }
        }

        FogRenderPass fogRenderPass;
        SkyboxTextureRenderPass skyboxRenderPass;

        public override void Create()
        {
            fogRenderPass = new FogRenderPass();
            skyboxRenderPass = new SkyboxTextureRenderPass();

            // Configures where the render pass should be injected.
            fogRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            if (enableSkyboxCapture) skyboxRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            fogRenderPass.CheckForStackedRendering(renderer, renderingData.cameraData);
            fogRenderPass.Setup(renderer);

            if (enableSkyboxCapture) renderer.EnqueuePass(skyboxRenderPass);
        }
    }
#endif
}