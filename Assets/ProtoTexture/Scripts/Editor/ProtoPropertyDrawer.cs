using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ProtoProperty), true)]
public class ProtoPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            var labelRect = new Rect(position) { width = EditorGUIUtility.labelWidth };
            var fieldRect = new Rect(position);
            fieldRect.xMin += EditorGUIUtility.labelWidth;

            var enabled = property.FindPropertyRelative("enabled");
            enabled.boolValue = EditorGUI.ToggleLeft(labelRect, label, enabled.boolValue);
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("_value"), GUIContent.none);
        }
    }
}