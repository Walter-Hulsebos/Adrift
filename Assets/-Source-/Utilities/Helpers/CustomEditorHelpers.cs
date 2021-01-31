#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public static class CustomEditorHelpers
{
    private static readonly RectOffset BoxPadding = EditorStyles.helpBox.padding;
        
    private const float 
        _PAD_SIZE = 2f, 
        _FOOTER_HEIGHT = 10f;
        
    private static readonly float 
        LineHeight = EditorGUIUtility.singleLineHeight,
        PaddedLine = LineHeight + _PAD_SIZE;
    
    /// <summary> Draw a GUI button, choosing between a short and a long button text based on if it fits. </summary>
    public static bool ButtonHelper(
        in Rect position, 
        in string longMessage, 
        in string shortMessage,
        in GUIStyle style,
        in string tooltip = null)
    {
        GUIContent __content = new GUIContent(longMessage)
        {
            tooltip = tooltip
        };

        float __longMessageWidth = style.CalcSize(content: __content).x;
        if(__longMessageWidth > position.width)
        {
            __content.text = shortMessage;
        }

        return GUI.Button(position, __content, style);
    }

    /// <summary> Given a position rect, get its field portion </summary>
    public static Rect GetFieldRect(Rect position)
    {
        position.width -= EditorGUIUtility.labelWidth;
        position.x += EditorGUIUtility.labelWidth;
        return position;
    }

    /// <summary> Given a position rect, get its label portion </summary>
    public static Rect GetLabelRect(Rect position)
    {
        position.width = EditorGUIUtility.labelWidth - _PAD_SIZE;
        return position;
    }
}

#endif