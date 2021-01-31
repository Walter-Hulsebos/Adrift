namespace CommonGames.Utilities.CGTK
{
    #if UNITY_EDITOR

    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using UnityEditor;
    using UnityEditor.VersionControl;

    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector.Editor;
    
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;

    //using PropertyDrawer = OdinValueDrawer<SceneReference>;
    #endif
    
    /// <summary>
    /// Display a Scene Reference object in the editor.
    /// If scene is valid, provides basic buttons to interact with the scene's role in Build Settings.
    /// </summary>
    /// <inheritdoc />
    [CustomPropertyDrawer(typeof(SceneReference))]
    public sealed class SceneReferencePropertyDrawer : PropertyDrawer //OdinValueDrawer<SceneReference>
    {
        #region Variables
        
        // The exact name of the asset Object variable in the SceneReference object
        private const string _SCENE_ASSET_PROPERTY_STRING = "sceneAsset";

        /// <summary> The exact name of  the scene Path variable in the SceneReference object. </summary>
        private const string _SCENE_PATH_PROPERTY_STRING = "scenePath";

        private static readonly RectOffset BoxPadding = EditorStyles.helpBox.padding;

        private const float
            _PAD_SIZE = 2f,
            _FOOTER_HEIGHT = 10f;

        private static readonly float
            LineHeight = EditorGUIUtility.singleLineHeight,
            PaddedLine = LineHeight + _PAD_SIZE;
        
        #endregion

        #region Methods
        
        /// <inheritdoc />
        /// <summary> Drawing the 'SceneReference' property </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty __sceneAssetProperty = GetSceneAssetProperty(property: property);

            // Draw the Box Background
            position.height -= _FOOTER_HEIGHT;
            
            
            GUI.Box(
                //position: EditorGUI.IndentedRect(source: position), 
                position: EditorGUI.IndentedRect(new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label))), 
                content: GUIContent.none,
                style: EditorStyles.helpBox);
            
            position = BoxPadding.Remove(rect: position);
            position.height = LineHeight;

            // Draw the main Object field
            label.tooltip = "The actual Scene Asset reference.\nOn serialize this is also stored as the asset's path.";

            EditorGUI.BeginProperty(totalPosition: position, label: GUIContent.none, property: property);
            EditorGUI.BeginChangeCheck();
            int __sceneControlId = GUIUtility.GetControlID(focus: FocusType.Passive);

            Object __selectedObject = EditorGUI.ObjectField(position: position, label: label,
                obj: __sceneAssetProperty.objectReferenceValue, objType: typeof(SceneAsset), false);

            BuildSceneUtilities.BuildScene __buildScene =
                BuildSceneUtilities.GetBuildScene(sceneObject: __selectedObject);

            if(EditorGUI.EndChangeCheck())
            {
                __sceneAssetProperty.objectReferenceValue = __selectedObject;

                // If no valid scene asset was selected, reset the stored path accordingly
                if(__buildScene.Scene == null)
                {
                    GetScenePathProperty(property: property).stringValue = string.Empty;
                }
            }

            position.y += PaddedLine;

            if(__buildScene.AssetGuid.Empty() == false)
            {
                // Draw the Build Settings Info of the selected Scene
                DrawSceneInfoGui(position: position, buildScene: __buildScene, sceneControlId: __sceneControlId + 1);
            }

            EditorGUI.EndProperty();
        }

        /// <summary> Ensure that what we draw in OnGUI always has the room it needs </summary>
        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int __lines = 2;
            SerializedProperty __sceneAssetProperty = GetSceneAssetProperty(property: property);

            if(__sceneAssetProperty.objectReferenceValue == null) __lines = 1;

            return BoxPadding.vertical + (LineHeight * __lines) + (_PAD_SIZE * (__lines - 1)); //+ _FOOTER_HEIGHT/2;
        }

        /// <summary> Draws info box of the provided scene </summary>
        private static void DrawSceneInfoGui(
            in Rect position,
            in BuildSceneUtilities.BuildScene buildScene,
            in int sceneControlId)
        {
            bool __readOnly = BuildSceneUtilities.IsReadOnly;

            string __readOnlyWarning = BuildSceneUtilities.IsReadOnly
                ? "\n\n WARNING: Build Settings is not checked out and so cannot be modified."
                : string.Empty;

            // Label Prefix
            GUIContent __iconContent;
            GUIContent __labelContent = new GUIContent();

            #region Draw SceneAsset BuildState Information

            // Missing from build scenes
            if(buildScene.BuildIndex == -1)
            {
                __iconContent = EditorGUIUtility.IconContent(name: "d_winbtn_mac_close");
                __labelContent.text = "NOT In Build";
                __labelContent.tooltip = "This scene is NOT in build settings.\nIt will be NOT included in builds.";
            }
            // In build scenes and enabled
            else if(buildScene.Scene.enabled)
            {
                __iconContent = EditorGUIUtility.IconContent(name: "d_winbtn_mac_max");
                __labelContent.text = "BuildIndex: " + buildScene.BuildIndex;
                __labelContent.tooltip =
                    "This scene is in build settings and ENABLED.\nIt will be included in builds." +
                    __readOnlyWarning;
            }
            // In build scenes and disabled
            else
            {
                __iconContent = EditorGUIUtility.IconContent(name: "d_winbtn_mac_min");
                __labelContent.text = "BuildIndex: " + buildScene.BuildIndex;
                __labelContent.tooltip =
                    "This scene is in build settings and DISABLED.\nIt will be NOT included in builds.";
            }

            #endregion

            #region Draw Left Status Label

            using(new EditorGUI.DisabledScope(disabled: __readOnly))
            {
                Rect __labelRect = CustomEditorHelpers.GetLabelRect(position: position);
                Rect __iconRect = __labelRect;
                __iconRect.width = __iconContent.image.width + _PAD_SIZE;
                __labelRect.width -= __iconRect.width;
                __labelRect.x += __iconRect.width;
                EditorGUI.PrefixLabel(totalPosition: __iconRect, id: sceneControlId, label: __iconContent);
                EditorGUI.PrefixLabel(totalPosition: __labelRect, id: sceneControlId, label: __labelContent);
            }

            #endregion

            #region Draw Context Buttons

            Rect __buttonRect = CustomEditorHelpers.GetFieldRect(position: position);
            __buttonRect.width /= 3;

            string __tooltipMsg;

            using(new EditorGUI.DisabledScope(disabled: __readOnly))
            {
                // NOT in build settings
                if(buildScene.BuildIndex == -1)
                {
                    __buttonRect.width *= 2;
                    int __addIndex = EditorBuildSettings.scenes.Length;
                    __tooltipMsg =
                        $"Add this scene to build settings. It will be appended to the end of the build scenes as buildIndex: {__addIndex}.{__readOnlyWarning}";

                    if(CustomEditorHelpers.ButtonHelper(
                        position: __buttonRect,
                        shortMessage: "Add...",
                        longMessage: $"Add (buildIndex {__addIndex})",
                        style: EditorStyles.miniButtonLeft, tooltip: __tooltipMsg))
                    {
                        BuildSceneUtilities.AddBuildScene(buildScene: buildScene);
                    }

                    __buttonRect.width /= 2;
                    __buttonRect.x += __buttonRect.width;
                }
                // In build settings
                else
                {
                    bool __isEnabled = buildScene.Scene.enabled;
                    string __stateString = __isEnabled ? "Disable" : "Enable";

                    __tooltipMsg = __stateString + " this scene in build settings.\n" +
                                   (__isEnabled
                                       ? "It will no longer be included in builds"
                                       : "It will be included in builds") + $".{__readOnlyWarning}";

                    if(CustomEditorHelpers.ButtonHelper(
                        position: __buttonRect,
                        shortMessage: __stateString,
                        longMessage: __stateString + " In Build",
                        style: EditorStyles.miniButtonLeft,
                        tooltip: __tooltipMsg))
                    {
                        BuildSceneUtilities.SetBuildSceneState(buildScene: buildScene, enabled: !__isEnabled);
                    }

                    __buttonRect.x += __buttonRect.width;

                    __tooltipMsg = "Completely remove this scene from build settings." +
                                   $"\nYou will need to add it again for it to be included in builds! {__readOnlyWarning}";

                    if(CustomEditorHelpers.ButtonHelper(
                        position: __buttonRect,
                        shortMessage: "Remove...",
                        longMessage: "Remove from Build",
                        style: EditorStyles.miniButtonMid,
                        tooltip: __tooltipMsg))
                    {
                        BuildSceneUtilities.RemoveBuildScene(buildScene: buildScene);
                    }
                }
            }

            __buttonRect.x += __buttonRect.width;

            __tooltipMsg = $"Open the 'Build Settings' Window for managing scenes.{__readOnlyWarning}";
            if(CustomEditorHelpers.ButtonHelper(
                position: __buttonRect,
                shortMessage: "Settings",
                longMessage: "Build Settings",
                style: EditorStyles.miniButtonRight,
                tooltip: __tooltipMsg))
            {
                BuildSceneUtilities.OpenBuildSettings();
            }

            #endregion
        }

        #region GetSceneAssetProperties

        private static SerializedProperty GetSceneAssetProperty(in SerializedProperty property)
            => property.FindPropertyRelative(relativePropertyPath: _SCENE_ASSET_PROPERTY_STRING);
        private static SerializedProperty GetScenePathProperty(in SerializedProperty property)
            => property.FindPropertyRelative(relativePropertyPath: _SCENE_PATH_PROPERTY_STRING);
        
        #endregion
        
        #endregion
    }

    /*public sealed class SceneReferenceDrawer : OdinValueDrawer<SceneReference>
    {
        #region Variables

        /// <summary> The exact name of the asset Object variable in the SceneReference object. </summary>
        private const string _SCENE_ASSET_PROPERTY_STRING = "sceneAsset";

        /// <summary> The exact name of  the scene Path variable in the SceneReference object. </summary>
        private const string _SCENE_PATH_PROPERTY_STRING = "scenePath";

        private static readonly RectOffset BoxPadding = EditorStyles.helpBox.padding;

        private const float
            _PAD_SIZE = 2f,
            _FOOTER_HEIGHT = 10f;

        private static readonly float
            LineHeight = EditorGUIUtility.singleLineHeight,
            PaddedLine = LineHeight + _PAD_SIZE;
        
        #endregion

        #region Methods

        protected override void DrawPropertyLayout(IPropertyValueEntry<SceneReference> entry, GUIContent label)
        {
            SceneReference __sceneReference = entry.SmartValue;
            
            Rect __rect = EditorGUILayout.GetControlRect();
            if(label != null)
            {
                __rect = EditorGUI.PrefixLabel(__rect, label);
            }

            GUIHelper.PushLabelWidth(40);
            __sceneReference.Scene = EditorGUI.ObjectField(
                position: __rect.AlignRight(__rect.width * 0.5f), 
                label: "Scene",
                obj: __sceneReference.Scene,
                objType: typeof(SceneAsset),
                allowSceneObjects: false);
            GUIHelper.PopLabelWidth();
        }

        private static SerializedProperty GetSceneAssetProperty(in SerializedProperty property)
            => property.FindPropertyRelative(relativePropertyPath: _SCENE_ASSET_PROPERTY_STRING);
        private static SerializedProperty GetScenePathProperty(in SerializedProperty property)
            => property.FindPropertyRelative(relativePropertyPath: _SCENE_PATH_PROPERTY_STRING);

        #endregion
    }*/
    
    #endif
}