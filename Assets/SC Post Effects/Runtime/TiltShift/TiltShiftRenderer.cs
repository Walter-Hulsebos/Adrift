﻿#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class TiltShiftRenderer : ScriptableRendererFeature
    {
        class TiltShiftRenderPass : PostEffectRenderer<TiltShift>
        {
            int screenCopyID;
            private RenderTextureDescriptor blurredBuffDsc;           

            public TiltShiftRenderPass()
            {
                shaderName = ShaderNames.TiltShift;
                ProfilerTag = this.ToString();
                screenCopyID = Shader.PropertyToID("_BlurredTex");
            }

            enum Pass
            {
                FragHorizontal,
                FragHorizontalHQ,
                FragRadial,
                FragRadialHQ,
                FragBlend,
                FragDebug
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<TiltShift>();
                
                if(volumeSettings) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.ConfigurePass(cmd, cameraTextureDescriptor);

                blurredBuffDsc = cameraTextureDescriptor;
                //Require a high-precision alpha channel
                blurredBuffDsc.colorFormat = RenderTextureFormat.ARGBHalf;
                blurredBuffDsc.msaaSamples = 1; //No need to resolve AA for a blurred RT
                cmd.GetTemporaryRT(screenCopyID, blurredBuffDsc);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!volumeSettings) return;
                if (volumeSettings.IsActive() == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                Material.SetVector("_Params", new Vector4(volumeSettings.areaSize.value, volumeSettings.areaFalloff.value, volumeSettings.amount.value, (int)volumeSettings.mode.value));

                int pass = (int)volumeSettings.mode.value + (int)volumeSettings.quality.value;
                switch ((int)volumeSettings.mode.value)
                {
                    case 0:
                        pass = 0 + (int)volumeSettings.quality.value;
                        break;
                    case 1:
                        pass = 2 + (int)volumeSettings.quality.value;
                        break;
                }
                Blit(this, cmd, cameraColorTarget, screenCopyID, Material, pass);
                cmd.SetGlobalTexture("_BlurredTex", screenCopyID);

                FinalBlit(this, context, cmd, mainTexID.id, cameraColorTarget, Material, TiltShift.debug ? (int)Pass.FragDebug : (int)Pass.FragBlend);
            }

            public override void Cleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(screenCopyID);
                base.Cleanup(cmd);
            }
        }

        TiltShiftRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new TiltShiftRenderPass();

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