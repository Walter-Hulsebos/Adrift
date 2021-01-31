#if URP
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
    [VolumeComponentEditor(typeof(EdgeDetection))]
    sealed class EdgeDetectionEditor : VolumeComponentEditor
    {
        EdgeDetection effect;
        SerializedDataParameter mode;
        SerializedDataParameter debug;

        SerializedDataParameter sensitivityDepth;
        SerializedDataParameter sensitivityNormals;
        SerializedDataParameter lumThreshold;

        SerializedDataParameter edgeExp;
        SerializedDataParameter edgeSize;

        SerializedDataParameter edgeColor;
        SerializedDataParameter edgeOpacity;

        SerializedDataParameter invertFadeDistance;
        SerializedDataParameter fadeDistance;
        SerializedDataParameter sobelThin;

        public override bool hasAdvancedMode => false;
        private bool isSetup;
        public override void OnEnable()
        {
            base.OnEnable();

            effect = (EdgeDetection)target;
            var o = new PropertyFetcher<EdgeDetection>(serializedObject);

            isSetup = AutoSetup.ValidEffectSetup<EdgeDetectionRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            debug = Unpack(o.Find(x => x.debug));

            sensitivityDepth = Unpack(o.Find(x => x.sensitivityDepth));
            sensitivityNormals = Unpack(o.Find(x => x.sensitivityNormals));
            lumThreshold = Unpack(o.Find(x => x.lumThreshold));

            edgeExp = Unpack(o.Find(x => x.edgeExp));
            edgeSize = Unpack(o.Find(x => x.edgeSize));

            edgeColor = Unpack(o.Find(x => x.edgeColor));
            edgeOpacity = Unpack(o.Find(x => x.edgeOpacity));

            invertFadeDistance = Unpack(o.Find(x => x.invertFadeDistance));
            fadeDistance = Unpack(o.Find(x => x.fadeDistance));
            sobelThin = Unpack(o.Find(x => x.sobelThin));

        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("edge-detection");

            SCPE_GUI.DisplaySetupWarning<EdgeDetectionRenderer>(ref isSetup);

            SCPE_GUI.ShowDepthTextureWarning();

            //Link override states
            edgeOpacity.overrideState.boolValue = (edgeColor.overrideState.boolValue == true) ? true : false;
            invertFadeDistance.overrideState.boolValue = fadeDistance.overrideState.boolValue;

            PropertyField(debug);

            PropertyField(mode);

            if (mode.overrideState.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                switch (mode.value.intValue)
                {
                    case 0:
                        EditorGUILayout.HelpBox("Checks pixels for differences between geometry normals and their distance from the camera", MessageType.None);
                        break;
                    case 1:
                        EditorGUILayout.HelpBox("Same as Depth Normals but also uses vertical sampling for improved accuracy", MessageType.None);
                        break;
                    case 2:
                        EditorGUILayout.HelpBox("Draws edges only where neighboring pixels greatly differ in their depth value.", MessageType.None);
                        break;
                    case 3:
                        EditorGUILayout.HelpBox("Creates an edge where the luminance value of a pixel differs from its neighbors, past the threshold", MessageType.None);
                        break;
                }
                EditorGUILayout.EndHorizontal();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                // Override checkbox
                var overrideRect = GUILayoutUtility.GetRect(17f, 17f, GUILayout.ExpandWidth(false));
                overrideRect.yMin += 4f;
                DrawOverrideCheckbox(fadeDistance);
                using (new EditorGUI.DisabledGroupScope(!fadeDistance.overrideState.boolValue))
                {

                    EditorGUILayout.PrefixLabel(fadeDistance.displayName);

                    GUILayout.FlexibleSpace();

                    fadeDistance.value.floatValue = EditorGUILayout.FloatField(fadeDistance.value.floatValue);

                    bool enabled = invertFadeDistance.value.boolValue;
                    enabled = GUILayout.Toggle(enabled, "Start", EditorStyles.miniButtonLeft, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
                    enabled = !GUILayout.Toggle(!enabled, "End", EditorStyles.miniButtonRight, GUILayout.Width(50f), GUILayout.ExpandWidth(false));

                    invertFadeDistance.value.boolValue = enabled;
                }

            }

            if (mode.value.intValue < 2)
            {
                PropertyField(sensitivityDepth);
                PropertyField(sensitivityNormals);
            }
            else if (mode.value.intValue == 2)
            {
                PropertyField(edgeExp);
            }
            else
            {
                // lum based mode
                PropertyField(lumThreshold);
            }

            //Edges
            PropertyField(edgeColor);
            PropertyField(edgeOpacity);
            PropertyField(edgeSize);
            if (mode.value.intValue == 2)
            {
                PropertyField(sobelThin);
            }

            //Store edge opacity value in the color's alpha channel
            edgeColor.value.colorValue = new Color(edgeColor.value.colorValue.r, edgeColor.value.colorValue.g, edgeColor.value.colorValue.b, edgeOpacity.value.floatValue);
        }
    }
#endif
}
