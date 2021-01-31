﻿#if URP
using UnityEngine.Rendering.Universal;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEditor.Rendering;

namespace SCPE
{
#if URP
    [VolumeComponentEditor(typeof(Blur))]
    sealed class BlurEditor : VolumeComponentEditor
    {
        Blur effect;

        SerializedDataParameter mode;
        SerializedDataParameter highQuality;
        SerializedDataParameter amount;
        SerializedDataParameter iterations;
        SerializedDataParameter downscaling;


        public override bool hasAdvancedMode => false;
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            effect = (Blur)target;
            var o = new PropertyFetcher<Blur>(serializedObject);

            isSetup = AutoSetup.ValidEffectSetup<BlurRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            highQuality = Unpack(o.Find(x => x.highQuality));
            amount = Unpack(o.Find(x => x.amount));
            iterations = Unpack(o.Find(x => x.iterations));
            downscaling = Unpack(o.Find(x => x.downscaling));
        }


        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("blur");

            SCPE_GUI.DisplaySetupWarning<BlurRenderer>(ref isSetup);

            PropertyField(mode);
            PropertyField(highQuality);
            PropertyField(amount);
            PropertyField(iterations);
            PropertyField(downscaling);
        }
    }
#endif
}