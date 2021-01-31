#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEditor.Rendering;
using UnityEditor;

namespace SCPE
{
#if URP
    [VolumeComponentEditor(typeof(Overlay))]
    sealed class OverlayEditor : VolumeComponentEditor
    {
        SerializedDataParameter intensity;
        SerializedDataParameter overlayTex;
        SerializedDataParameter autoAspect;
        SerializedDataParameter blendMode;
        SerializedDataParameter tiling;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Overlay>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<OverlayRenderer>();

            intensity = Unpack(o.Find(x => x.intensity));
            overlayTex = Unpack(o.Find(x => x.overlayTex));
            autoAspect = Unpack(o.Find(x => x.autoAspect));
            blendMode = Unpack(o.Find(x => x.blendMode));
            tiling = Unpack(o.Find(x => x.tiling));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("overlay");

            SCPE_GUI.DisplaySetupWarning<OverlayRenderer>(ref isSetup);
            PropertyField(overlayTex);

            if (overlayTex.overrideState.boolValue && overlayTex.value.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a texture", MessageType.Info);
            }

            EditorGUILayout.Space();

            PropertyField(intensity);
            PropertyField(autoAspect);
            PropertyField(blendMode);
            PropertyField(tiling);
        }
    }
}
#endif