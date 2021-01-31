using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(ProtoTexture))]
[CanEditMultipleObjects()]
public class ProtoTextureEditor : Editor
{
    private const string MATERIAL_ID = "_material";
    private const string TEXTURE_ID = "_texture";
    private const string NAME_ID = "name._value";
    private const string SUBTITLE_ID = "subTitle._value";
    private const string TEXTURE_ELEMENT_PATTERN = "{0} : {1}";

    private static readonly GUIContent OrphanContent = new GUIContent("Orphaned Textures");
    private static readonly GUIContent DeleteContent = new GUIContent("Delete");
    private static readonly GUIContent TexturesContent = new GUIContent("Textures");

    ProtoTexture instance = null;
    SerializedProperty _textureDefinitions;

    ReorderableList _textureList;
    int _selected;

    static Material _protoPreviewMaterial;
    public static Material ProtoPreviewMaterial
    {
        get
        {
            if (!_protoPreviewMaterial)
            {
                _protoPreviewMaterial = new Material(Shader.Find("Hidden/ProtoTexturePreview-Colored"))
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            return _protoPreviewMaterial;
        }
    }

    static GUIStyle _boxStyle;
    public static GUIStyle BoxStyle { get { return _boxStyle ?? (_boxStyle = "Box"); } }

    void OnEnable()
    {
        instance = target as ProtoTexture;

        _textureDefinitions = serializedObject.FindProperty("_textureDefinitions");

        _textureList = new ReorderableList(serializedObject, _textureDefinitions);
        _textureList.drawElementCallback += DrawTextureElement;
        _textureList.elementHeightCallback += TextureElementHeight;
        _textureList.drawHeaderCallback += DrawTextureHeader;
        _textureList.onSelectCallback += SelectTexture;
        _textureList.onAddCallback += AddTexture;
        _textureList.onRemoveCallback += RemoveTexture;
        _textureList.onReorderCallbackWithDetails += ReorderLayers;
        _textureList.index = _selected;
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();

        _textureList.DoLayoutList();

        _selected = Mathf.Clamp(_selected, 0, _textureDefinitions.arraySize - 1);

        if ((_selected >= 0) && (_selected < _textureDefinitions.arraySize))
        {
            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                var property = _textureDefinitions.GetArrayElementAtIndex(_selected);
                var path = property.propertyPath;
                var enterChildren = true;

                while (property.NextVisible(enterChildren) && property.propertyPath.StartsWith(path))
                {
                    using (new EditorGUI.DisabledScope(property.name == TEXTURE_ID || property.name == MATERIAL_ID))
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }

                    if ((property.name == MATERIAL_ID) && (property.objectReferenceValue != null))
                    {
                        var materialSerializedObject = new SerializedObject(property.objectReferenceValue);
                        materialSerializedObject.Update();

                        using (var check = new EditorGUI.ChangeCheckScope())
                        using (new EditorGUI.IndentLevelScope(1))
                        {
                            var shaderProperty = materialSerializedObject.FindProperty("m_Shader");
                            EditorGUILayout.PropertyField(shaderProperty, true);

                            if (check.changed)
                                materialSerializedObject.ApplyModifiedProperties();
                        }
                    }

                    enterChildren = false;
                }

                if (changeCheck.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    instance.Build(_selected);
                    serializedObject.Update();
                }
            }
        }

        serializedObject.ApplyModifiedProperties();

        var textures = new List<Object>(AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target)));
        textures.RemoveAll(t => (t == null) || (t.GetType() != typeof(Texture2D)) || instance.HasTexture(t as Texture2D));

        if (textures.Count > 0)
        {
            EditorGUILayout.Separator();
            using (new EditorGUILayout.VerticalScope(BoxStyle))
            {
                GUILayout.Label(OrphanContent, EditorStyles.boldLabel);
                EditorGUILayout.Separator();

                int count = 0;

                foreach (var target in textures)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.ObjectField(target, target.GetType(), false);
                        if (GUILayout.Button(DeleteContent))
                        {
                            ++count;
                            DestroyImmediate(target, true);
                        }
                    }
                }

                if (count > 0)
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(instance)); // destroying orphans
            }
        }
    }

    private void ReorderLayers(ReorderableList list, int oldIndex, int newIndex)
    {
        serializedObject.ApplyModifiedProperties();
        _textureDefinitions.MoveArrayElement(oldIndex, newIndex);
        serializedObject.Update();
    }

    private void AddTexture(ReorderableList list)
    {
        serializedObject.ApplyModifiedProperties();
        instance.AddTexture();
        serializedObject.Update();
        _textureList.index = _selected = _textureDefinitions.arraySize - 1;
    }

    private void RemoveTexture(ReorderableList list)
    {
        instance.RemoveTexture(list.index);
        --_selected;
    }

    private void SelectTexture(ReorderableList list)
    {
        _selected = list.index;
    }

    private void DrawTextureHeader(Rect rect)
    {
        GUI.Label(rect, TexturesContent);
    }

    private float TextureElementHeight(int index)
    {
        return EditorGUIUtility.singleLineHeight + 2;
    }

    private void DrawTextureElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var property = _textureDefinitions.GetArrayElementAtIndex(index);
        var texRect = new Rect(rect.x, rect.y + 1, rect.width, rect.height - 2);

        DrawTexture(texRect, instance.GetTextureDefinition(index).Texture, ProtoPreviewMaterial, ScaleMode.ScaleAndCrop);

        GUI.Label(rect, string.Format(TEXTURE_ELEMENT_PATTERN, property.FindPropertyRelative(NAME_ID).stringValue, property.FindPropertyRelative(SUBTITLE_ID).stringValue));
    }

    public override void DrawPreview(Rect previewArea)
    {
        if (HasPreviewGUI())
        {
            var prop = instance.GetTextureDefinition(_selected);
            if ((prop != null) && (prop.Texture != null))
                DrawTexture(previewArea, prop.Texture, null, ScaleMode.ScaleToFit);
        }
    }

    public override bool HasPreviewGUI()
    {
        return (_selected >= 0) && (_selected < _textureDefinitions.arraySize);
    }

    static void DrawTexture(Rect rect, Texture texture, Material material, ScaleMode scaleMode)
    {
        GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
        EditorGUI.DrawPreviewTexture(rect, texture, material, scaleMode);
        GL.sRGBWrite = false;
    }
}