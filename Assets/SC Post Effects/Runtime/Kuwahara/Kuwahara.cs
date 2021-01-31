#if URP
using UnityEngine.Rendering.Universal;
#endif

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
#if URP
    [Serializable, VolumeComponentMenu("SC Post Effects/Stylized/Kuwahara")]
    public sealed class Kuwahara : VolumeComponent, IPostProcessComponent
    {
        public enum KuwaharaMode
        {
            Regular = 0,
            DepthFade = 1
        }

        [Serializable]
        public sealed class KuwaharaModeParam : VolumeParameter<KuwaharaMode> { }

        [Tooltip("Choose to apply the effect to the entire screen, or fade in/out over a distance")]
        public KuwaharaModeParam mode = new KuwaharaModeParam { value = KuwaharaMode.Regular };

        //[Range(0, 8), DisplayName("Radius")]
        public ClampedIntParameter radius = new ClampedIntParameter(0, 0, 8);

        public BoolParameter invertFadeDistance = new BoolParameter(false);

        //[DisplayName("Fade distance")]
        public FloatParameter fadeDistance = new FloatParameter(1000f);

        public bool IsActive() => radius.value > 0 && this.active;

        public bool IsTileCompatible() => false;
#endif
    }
}