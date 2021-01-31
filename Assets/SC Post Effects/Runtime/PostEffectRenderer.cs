// SC Post Effects
// Staggart Creations http://staggart.xyz
// Copyright protected under the Unity Asset Store EULA

//Required for stacked cameras with depth effects, copies the base camera depth onto that of the overlay.
//Without this, depth-based effects can only read the depth texture of the last overlay camera, since it's cleared after every camera is done rendering
//If it's not cleared, it works fine, but overlay rendered objects aren't sorted properly and render through objects
//#define DEPTH_COPY 

#if URP
using UnityEngine.Rendering.Universal;
#endif
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
#if URP
    /// <summary>
    /// Base class for screen-space post processing through a ScriptableRenderPass
    /// </summary>
    /// <typeparam name="T">Related settings class</typeparam>
    public class PostEffectRenderer<T> : ScriptableRenderPass
    {
        /// <summary>
        /// VolumeComponent settings instance
        /// </summary>
        public T volumeSettings;

        public bool requiresDepth = false;
        public bool requiresDepthNormals = false;
        private bool usingStackedCameras = false;
        private bool renderingAsOverlayCamera;
        private static bool msaaEnabled;

#if DEPTH_COPY
        private static Camera previousCamera;
        private static bool renderingNewCamera; //used to know when the base camera is done, and now at an overlay
#endif
        /// <summary>
        /// When required and using a camera stack, a copy of the base camera is passed over to the overlay camera
        /// </summary>
        private bool RequireDepthCopy => (requiresDepth || requiresDepthNormals) && usingStackedCameras;

        public string shaderName;
        [SerializeField]
        private Shader shader;
        public string ProfilerTag;
        public Material Material;
        
        public RenderTargetIdentifier cameraColorTarget;
        public RenderTargetIdentifier cameraDepthTarget;
        public RenderTextureDescriptor mainTexDesc;
        public RenderTargetHandle mainTexID;
        public RenderTextureDescriptor depthNormalDsc;
        private int depthNormalsID;
        
#if DEPTH_COPY
        public int depthCopyID;
        public int blendedDepthID;
        private RenderTextureDescriptor depthDsc;
        private RenderTextureDescriptor blendedDepthDsc;
#endif
        
        public RenderTargetIdentifier GetCameraTarget(ScriptableRenderer renderer)
        {
#if URP_9_0_OR_NEWER
            //Fetched in CopyTargets function, no longer allowed from a ScriptableRenderFeature setup function (target may be not be created yet, or was disposed)
            return cameraColorTarget;
#else
            return renderer.cameraColorTarget;
#endif
        }
        
        public RenderTargetIdentifier GetCameraDepthTarget(ScriptableRenderer renderer)
        {
#if URP_9_0_OR_NEWER
            //Fetched in CopyTargets function, no longer allowed from a ScriptableRenderFeature setup function (target may be not be created yet, or was disposed)
            return cameraDepthTarget;
#else
            return renderer.cameraDepth;
#endif
        }
        
        private void CreateMaterialIfNull(ref Material material, ref Shader m_shader, string m_shaderName)
        {
            if (material) return;

             if(!m_shader) m_shader = Shader.Find(m_shaderName);
             
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!m_shader)
            {
                Debug.LogError("[SC Post Effects] Shader with the name <i>" + m_shaderName + "</i> could not be found, ensure all effect files are imported");
                return;
            }
#endif
            
            material = CoreUtils.CreateEngineMaterial(m_shader);
            //Needs to be saved in editor, so a shader reference is serialized and included in builds
            material.hideFlags = HideFlags.DontSaveInBuild;
            material.name = m_shaderName;
        }
        

        /// <summary>
        /// Sets up MainTex RT and depth normals if needed. Check if settings are valid before calling this base implementation
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="cameraTextureDescriptor"></param>
#if URP_9_0_OR_NEWER
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
#else
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
#endif
        {
#if URP_9_0_OR_NEWER
            RenderTextureDescriptor cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
#endif
            ConfigurePass(cmd, cameraTextureDescriptor);
        }

        public virtual void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            CreateMaterialIfNull(ref Material, ref shader, shaderName);

            mainTexDesc = cameraTextureDescriptor;
            mainTexDesc.msaaSamples = 1;
            //Buffer needs to be 16 bit to support HDR
            cameraTextureDescriptor.colorFormat =
                SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR) &&
                UniversalRenderPipeline.asset.supportsHDR
                    ? RenderTextureFormat.DefaultHDR
                    : RenderTextureFormat.Default;
            
            mainTexID = new RenderTargetHandle();
            mainTexID.id = Shader.PropertyToID(TextureNames.Main);
            cmd.GetTemporaryRT(mainTexID.id, mainTexDesc);
            
            //TODO: Use integrated depth normals in URP 10+
            if (requiresDepthNormals)
            {
                //https://github.com/Unity-Technologies/Graphics/blob/c6eb37bbad8d85f5c6f9aa53648d2f4a49c33b59/com.unity.render-pipelines.universal/Runtime/Passes/DepthNormalOnlyPass.cs#L40
                depthNormalDsc = cameraTextureDescriptor;
                depthNormalDsc.depthBufferBits = 0;
                depthNormalDsc.colorFormat = RenderTextureFormat.RGHalf;
                depthNormalDsc.msaaSamples = 1;
                
                depthNormalsID = Shader.PropertyToID(TextureNames.DepthNormals);
                cmd.GetTemporaryRT(depthNormalsID, depthNormalDsc);
                
                cmd.SetGlobalTexture(depthNormalsID, depthNormalsID);
            }

#if DEPTH_COPY
            if (RequireDepthCopy)
            {
                depthDsc = cameraTextureDescriptor;
                depthDsc.colorFormat = RenderTextureFormat.Depth;
                depthDsc.depthBufferBits = 32;
                //depthDsc.msaaSamples = 1; //MSAA not used in depth pass anyway
                //depthDsc.bindMS = cameraTextureDescriptor.msaaSamples > 1 && !SystemInfo.supportsMultisampleAutoResolve && (SystemInfo.supportsMultisampledTextures != 0);
                
                depthCopyID = Shader.PropertyToID(TextureNames.DepthCopy);
                cmd.GetTemporaryRT(depthCopyID, depthDsc);
                //cmd.SetGlobalTexture(depthCopyID, depthCopyID);

                blendedDepthDsc = cameraTextureDescriptor;
                blendedDepthDsc.colorFormat = RenderTextureFormat.Depth;
                blendedDepthDsc.depthBufferBits = 32;
                blendedDepthID = Shader.PropertyToID(TextureNames.BlendedDepth);
                cmd.GetTemporaryRT(blendedDepthID, blendedDepthDsc);

            }
#endif
            
            ToggleStackedCamerasKeyword(Material, RequireDepthCopy);
        }

        /// <summary>
        /// Compose and execute command buffer. No need to call base implementation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderingData"></param>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if(UniversalRenderPipeline.asset) msaaEnabled = UniversalRenderPipeline.asset.msaaSampleCount > 1;
            
#if DEPTH_COPY
            if (usingStackedCameras)
            {
                if (previousCamera != renderingData.cameraData.camera)
                {
                    renderingNewCamera = true;
                    previousCamera = renderingData.cameraData.camera;
                }
                else
                {
                    renderingNewCamera = false;
                }
            }
#endif
        }


        /// <summary>
        /// Do not override!
        /// </summary>
        /// <param name="cmd"></param>
#if URP_9_0_OR_NEWER
        public override void OnCameraCleanup(CommandBuffer cmd)
#else
        public override void FrameCleanup(CommandBuffer cmd)
#endif
        {
            Cleanup(cmd);
        }

        /// <summary>
        /// Releases the basic resources used by any effect. Cleanup be effect specific resources before calling the base implementation!
        /// Wrapper function, called by different functions between URP 8- and 9+
        /// </summary>
        public virtual void Cleanup(CommandBuffer cmd)
        {
#if SCPE_DEV
            //Debug.Log(ProfilerTag + " cleaned up");
#endif
            
            cmd.ReleaseTemporaryRT(mainTexID.id);
            cmd.ReleaseTemporaryRT(depthNormalsID);
#if DEPTH_COPY
            cmd.ReleaseTemporaryRT(depthCopyID);
            cmd.ReleaseTemporaryRT(blendedDepthID);
#endif
        }

        /// <summary>
        /// Copies the color, depth and depth normals if required
        /// </summary>
        /// <param name="cmd"></param>
        public void CopyTargets(CommandBuffer cmd, RenderingData renderingData)
        {
            #if URP_9_0_OR_NEWER
            //Color target can now only be fetched inside a ScriptableRenderPass
            this.cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
            #endif
            
            Blit(cmd, cameraColorTarget, mainTexID.id);
            
            CopyDepth(this, cmd);
            
            GenerateDepthNormals(this, cmd);
        }

        [SerializeField]
        private Material DepthNormalsMat;
        private static Shader DepthNormalsShader;
        
        /// <summary>
        /// Reconstructs view-space normals from depth texture
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="cmd"></param>
        /// <param name="dest"></param>
        public void GenerateDepthNormals(ScriptableRenderPass pass, CommandBuffer cmd)
        {
            if (!requiresDepthNormals) return;
            
            CreateMaterialIfNull(ref DepthNormalsMat, ref DepthNormalsShader, ShaderNames.DepthNormals);
            ToggleStackedCamerasKeyword(DepthNormalsMat, RequireDepthCopy);
            
            //Needs depth from current camera. For edge detection, black depth pixels don't result in edges, so the effect blends well
            if(usingStackedCameras) cmd.SetGlobalTexture(TextureNames.DepthTexture, cameraDepthTarget);
            
            Blit(pass, cmd, pass.depthAttachment /* not actually used */, depthNormalsID, DepthNormalsMat, 0);
        }
        
        [SerializeField]
        private Material CopyDepthMat;
        private static Shader CopyDepthShader;
        private Material BlendDepthMat;
        private static Shader BlendDepthShader;

        /// <summary>
        /// Copied the current camera depth attachment into a copy texture.
        /// </summary>
        /// <param name="cmd"></param>
        public void CopyDepth(ScriptableRenderPass pass, CommandBuffer cmd)
        {
#if DEPTH_COPY
            if (!RequireDepthCopy) return;

            //Only need to copy the depth once per camera 
            //if (renderingNewCamera == false) return;
            
            //Note breaks with MSAA enabled in 7.2.1
            
            
            if (!renderingAsOverlayCamera)
            {
                //Built-in copy that also handles MSAA
                CreateMaterialIfNull(ref CopyDepthMat, ref CopyDepthShader, ShaderNames.CopyDepth);
                Blit(pass, cmd, cameraDepthTarget, depthCopyID, CopyDepthMat, 0);
                cmd.SetGlobalTexture(depthCopyID, depthCopyID);
                
                //Base camera uses regular depth target
               // cmd.SetGlobalTexture(TextureNames.DepthTexture, depthCopyID);

            }
            //Overlay camera gets depth from base camera
            else
            {
                CreateMaterialIfNull(ref BlendDepthMat, ref BlendDepthShader, ShaderNames.BlendDepth);
                cmd.SetGlobalTexture("_SourceDepth", depthCopyID);
                cmd.SetGlobalTexture("_DestDepth", cameraDepthTarget);
                
                cmd.Blit(depthCopyID, blendedDepthID, BlendDepthMat, 0);
                cmd.SetGlobalTexture(depthCopyID, blendedDepthID);
                
                //cmd.SetGlobalTexture("_CameraDepthTexture", blendedDepthID);
            }

#endif
        }

        /// <summary>
        /// Wrapper for ScriptableRenderPass.Blit but allows shaders to keep using _MainTex across render pipelines
        /// </summary>
        /// <param name="cmd">Command buffer to record command for execution.</param>
        /// <param name="source">Source texture or target identifier to blit from.</param>
        /// <param name="destination">Destination texture or target identifier to blit into. This becomes the renderer active render target.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="passIndex">Shader pass to use. Default is 0.</param>
        public static void Blit(ScriptableRenderPass pass, CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int passIndex)
        {
            //TODO: Simply use a shader macro?
            cmd.SetGlobalTexture(TextureNames.Main, source);
            pass.Blit(cmd, source, dest, mat, passIndex);
        }

        /// <summary>
        /// Blits and executes the command buffer
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="context"></param>
        /// <param name="cmd"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="mat"></param>
        /// <param name="passIndex"></param>
        public void FinalBlit(ScriptableRenderPass pass, ScriptableRenderContext context, CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int passIndex)
        {
            Blit(pass, cmd, source, dest, mat, passIndex);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        /// <summary>
        /// Toggles the "multi_compile_local STACKED_CAMERA" keyword on the material. When enabled, the depth copy is sampled
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="state"></param>
        public void ToggleStackedCamerasKeyword(Material mat, bool state)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!mat)
            {
                Debug.LogError("ToggleStackedCamerasKeyword, input material was null");
                return;
            }
#endif


#if !DEPTH_COPY
            mat.DisableKeyword(ShaderKeywords.StackedCamera);
            
#if SCPE_DEV && UNITY_EDITOR
            if(mat.IsKeywordEnabled(ShaderKeywords.StackedCamera)) Debug.Log("<color=orange>DISABLED</color> camera stacking keyword on <b>" + mat.name + "</b>");
#endif
#else
#if SCPE_DEV && UNITY_EDITOR
            if (!usingStackedCameras) state = false;

            //Temp! See if built in depth tex works
            //state = false;
            
            if(state && !mat.IsKeywordEnabled(ShaderKeywords.StackedCamera)) Debug.Log("<color=green>ENABLED</color> camera stacking keyword on <b>" + mat.name + "</b>");
            if(!state && mat.IsKeywordEnabled(ShaderKeywords.StackedCamera)) Debug.Log("<color=orange>DISABLED</color> camera stacking keyword on <b>" + mat.name + "</b>");
            #endif
            
            if(state) mat.EnableKeyword(ShaderKeywords.StackedCamera);
            if(!state) mat.DisableKeyword(ShaderKeywords.StackedCamera);
            
#endif
        }
        
        /// <summary>
        /// Checks if the camera has any layers in the stack. If so, a stacked camera setup is in use
        /// </summary>
        public void CheckForStackedRendering(ScriptableRenderer renderer, CameraData cameraData)
        {

#if !DEPTH_COPY
            usingStackedCameras = false;
            return;
#else
            
            //2D and Deferred renderers
            if (!renderer.supportedRenderingFeatures.cameraStacking)
            {
                usingStackedCameras = false;
                return;
            }

            //Skip if enabled, depth coping breaks down
            if (cameraData.camera.allowMSAA)
            {
                //usingStackedCameras = false;
                //return;
            }
            
#if UNITY_EDITOR
            if (cameraData.isSceneViewCamera) 
            {
                usingStackedCameras = false;
                return;
            }
#endif
            
            if (cameraData.renderType == CameraRenderType.Base)
            {
                renderingAsOverlayCamera = false;
                
                UniversalAdditionalCameraData additionalCameraData = cameraData.camera.GetComponent<UniversalAdditionalCameraData>();
                
                usingStackedCameras = additionalCameraData && additionalCameraData.cameraStack.Count > 0;

                //Need the depth clearing after each camera :(
                if (cameraData.antialiasing != AntialiasingMode.SubpixelMorphologicalAntiAliasing) cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                
                return;
            }
            
            if (cameraData.renderType == CameraRenderType.Overlay)
            {
                renderingAsOverlayCamera = true;
                
                //When using depth texture, force it to clear it from previous camera's (SMAA already does this)
                if (!cameraData.clearDepth && requiresDepth && cameraData.antialiasing != AntialiasingMode.SubpixelMorphologicalAntiAliasing)
                {
                    cameraData.clearDepth = true;
                    
                    //Copy to CameraData component as well (doesn't immediately apply though)
                    UniversalAdditionalCameraData overlayCameraData = cameraData.camera.gameObject.GetComponent<UniversalAdditionalCameraData>();
                    typeof(UniversalAdditionalCameraData).GetField("m_ClearDepth", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(overlayCameraData, cameraData.clearDepth);
#if UNITY_EDITOR
                    EditorUtility.SetDirty(overlayCameraData);
#endif //DEPTH_COPY

                }
                
                return;
            }
#endif
        }
    }
#endif //URP
}