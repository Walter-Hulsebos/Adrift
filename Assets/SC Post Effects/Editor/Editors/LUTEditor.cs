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
    [VolumeComponentEditor(typeof(LUT))]
    sealed class LUTEditor : VolumeComponentEditor
    {
        LUT effect;
        SerializedDataParameter mode;
        SerializedDataParameter intensity;
        SerializedDataParameter lutNear;
        SerializedDataParameter lutFar;
        SerializedDataParameter distance;

        public override bool hasAdvancedMode => false;
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            effect = (LUT)target;
            var o = new PropertyFetcher<LUT>(serializedObject);

            isSetup = AutoSetup.ValidEffectSetup<LUTRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            intensity = Unpack(o.Find(x => x.intensity));
            lutNear = Unpack(o.Find(x => x.lutNear));
            lutFar = Unpack(o.Find(x => x.lutFar));
            distance = Unpack(o.Find(x => x.distance));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("lut");

            SCPE_GUI.DisplaySetupWarning<LUTRenderer>(ref isSetup);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                //GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Open LUT Extracter", EditorGUIUtility.IconContent("d_PreTextureRGB").image,
                    "Extract a LUT from the bottom-left corner of a screenshot"),
                    EditorStyles.miniButton, GUILayout.Height(20f), GUILayout.Width(150f)))
                {
                    LUTExtracterWindow.ShowWindow();
                }
            }

            EditorGUILayout.Space();

            CheckLUTImportSettings(lutNear);
            if (mode.value.intValue == 1) CheckLUTImportSettings(lutFar);

            PropertyField(mode);
            PropertyField(intensity);
            PropertyField(lutNear, new GUIContent(mode.value.intValue == 0 ? "Look up Texture" : "Near"));
            if (mode.value.intValue == 1)
            {
                PropertyField(lutFar);
                PropertyField(distance);
            }
        }

        // Checks import settings on the lut, offers to fix them if invalid
        void CheckLUTImportSettings(SerializedDataParameter tex)
        {
            if (tex != null)
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex.value.objectReferenceValue));

                if (importer != null) // Fails when using a non-persistent texture
                {
                    bool valid = importer.anisoLevel == 0
                        && importer.mipmapEnabled == false
                        && importer.sRGBTexture == false
                        && (importer.textureCompression == TextureImporterCompression.Uncompressed)
                        && importer.wrapMode == TextureWrapMode.Clamp;

                    if (!valid)
                    {
                        EditorGUILayout.HelpBox("\"" + tex.value.objectReferenceValue.name + "\" has invalid LUT import settings.", MessageType.Warning);

                        GUILayout.Space(-32);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Fix", GUILayout.Width(60)))
                            {
                                SetLUTImportSettings(importer);
                                AssetDatabase.Refresh();
                            }
                            GUILayout.Space(8);
                        }
                        GUILayout.Space(11);
                    }
                }
                else
                {
                    tex.value.objectReferenceValue = null;
                }
            }
        }

        void SetLUTImportSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Default;
            // importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.sRGBTexture = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.anisoLevel = 0;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

    }
#endif
}