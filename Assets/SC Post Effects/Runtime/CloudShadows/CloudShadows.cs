﻿#if URP
using UnityEngine.Rendering.Universal;
#endif

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
#if URP
    [Serializable, VolumeComponentMenu("SC Post Effects/Environment/Cloud Shadows")]
    public sealed class CloudShadows : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("The red channel of this texture is used to sample the clouds")]
        public TextureParameter texture = new TextureParameter(null);

        [Space]

        [Range(0f, 1f)]
        public ClampedFloatParameter size = new ClampedFloatParameter(0.5f, 0f,1f);

        [Range(0f, 1f)]
        public ClampedFloatParameter density = new ClampedFloatParameter(0f,0f,1f);

        [Range(0f, 1f)]
        public ClampedFloatParameter speed = new ClampedFloatParameter(0.5f, 0f,1f);

        [Tooltip("Set the X and Z world-space direction the clouds should move in")]
        public Vector2Parameter direction = new Vector2Parameter(new Vector2(0f, 1f));

        public static bool isOrtho = false;

        public bool IsActive() => density.value > 0f && texture.value && this.active;

        public bool IsTileCompatible() => false;
#endif
    }
}