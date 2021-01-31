#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEditor.Rendering;
using UnityEngine;
using UnityEditor;

namespace SCPE
{
#if URP
    [VolumeComponentEditor(typeof(Kuwahara))]
    sealed class KuwaharaEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter radius;
        SerializedDataParameter invertFadeDistance;
        SerializedDataParameter fadeDistance;

        private bool isOrthographic = false;
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Kuwahara>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<KuwaharaRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            radius = Unpack(o.Find(x => x.radius));
            fadeDistance = Unpack(o.Find(x => x.fadeDistance));
            invertFadeDistance = Unpack(o.Find(x => x.invertFadeDistance));

            if (Camera.current) isOrthographic = Camera.current.orthographic;
        }

        public override string GetDisplayTitle()
        {
            return "Kuwahara" + ((mode.value.intValue == 0) ? "" : " (Depth Fade)");
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("kuwahara");

            SCPE_GUI.DisplaySetupWarning<KuwaharaRenderer>(ref isSetup);

            SCPE_GUI.ShowDepthTextureWarning(mode.value.intValue == 1);

            invertFadeDistance.overrideState.boolValue = fadeDistance.overrideState.boolValue;

            EditorGUI.BeginDisabledGroup(isOrthographic);
            PropertyField(mode);
            EditorGUI.EndDisabledGroup();

            if (isOrthographic)
            {
                mode.value.intValue = 0;
                EditorGUILayout.HelpBox("Depth fade is disabled for orthographic cameras", MessageType.Info);
            }
            PropertyField(radius);
            if (mode.value.intValue != 0)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    // Override checkbox
                    var overrideRect = GUILayoutUtility.GetRect(17f, 17f, GUILayout.ExpandWidth(false));
                    overrideRect.yMin += 4f;
                    DrawOverrideCheckbox(fadeDistance);

                    EditorGUILayout.PrefixLabel(fadeDistance.displayName);

                    GUILayout.FlexibleSpace();

                    fadeDistance.value.floatValue = EditorGUILayout.FloatField(fadeDistance.value.floatValue);

                    bool enabled = invertFadeDistance.value.boolValue;
                    enabled = GUILayout.Toggle(enabled, "Start", EditorStyles.miniButtonLeft, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
                    enabled = !GUILayout.Toggle(!enabled, "End", EditorStyles.miniButtonRight, GUILayout.Width(50f), GUILayout.ExpandWidth(false));

                    invertFadeDistance.value.boolValue = enabled;
                }
            }
        }
    }
}
#endif
