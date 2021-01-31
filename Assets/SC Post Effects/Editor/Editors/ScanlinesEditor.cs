﻿#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEditor.Rendering;

namespace SCPE
{
#if URP
    [VolumeComponentEditor(typeof(Scanlines))]
    sealed class ScanlinesEditor : VolumeComponentEditor
    {
        SerializedDataParameter intensity;
        SerializedDataParameter amount;
        SerializedDataParameter speed;

        public override bool hasAdvancedMode => false;
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Scanlines>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<ScanlinesRenderer>();

            intensity = Unpack(o.Find(x => x.intensity));
            amount = Unpack(o.Find(x => x.amount));
            speed = Unpack(o.Find(x => x.speed));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("scanlines");

            SCPE_GUI.DisplaySetupWarning<ScanlinesRenderer>(ref isSetup);

            PropertyField(intensity);
            PropertyField(amount);
            PropertyField(speed);
        }
    }
#endif
}