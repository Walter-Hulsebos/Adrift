#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEditor.Rendering;
using UnityEditor;
using UnityEngine;

namespace SCPE
{
#if URP
    [VolumeComponentEditor(typeof(TiltShift))]
    sealed class TiltShiftEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter quality;
        SerializedDataParameter areaSize;
        SerializedDataParameter areaFalloff;
        SerializedDataParameter amount;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<TiltShift>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<TiltShiftRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            quality = Unpack(o.Find(x => x.quality));
            areaSize = Unpack(o.Find(x => x.areaSize));
            areaFalloff = Unpack(o.Find(x => x.areaFalloff));
            amount = Unpack(o.Find(x => x.amount));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("tilt-shift");

            SCPE_GUI.DisplaySetupWarning<TiltShiftRenderer>(ref isSetup);

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawOverrideCheckbox(mode);
                using (new EditorGUI.DisabledGroupScope(mode.overrideState.boolValue == false))
                {
                    EditorGUILayout.PrefixLabel(mode.displayName);
                    mode.value.intValue = GUILayout.Toolbar(mode.value.intValue, mode.value.enumDisplayNames, GUILayout.Height(17f));
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawOverrideCheckbox(quality);
                using (new EditorGUI.DisabledGroupScope(quality.overrideState.boolValue == false))
                {
                    EditorGUILayout.PrefixLabel(quality.displayName);
                    quality.value.intValue = GUILayout.Toolbar(quality.value.intValue, quality.value.enumDisplayNames, GUILayout.Height(17f));
                }
            }
            EditorGUILayout.LabelField("Screen area", EditorStyles.boldLabel);
            PropertyField(areaSize, new GUIContent("Size"));
            PropertyField(areaFalloff, new GUIContent("Falloff"));

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(" ");
                TiltShift.debug = EditorGUILayout.ToggleLeft(" Visualize area", TiltShift.debug);
            }

            PropertyField(amount, new GUIContent("Blur amount"));
        }
    }
}
#endif