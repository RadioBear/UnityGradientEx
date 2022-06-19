using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public sealed partial class InternalEditorGUILayout
{
    //internal static float kLabelFloatMinW => EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + EditorGUI.kSpacing;
    static System.Reflection.PropertyInfo s_Field_kLabelFloatMinW;
    static float kLabelFloatMinW
    {
        get
        {
            if (s_Field_kLabelFloatMinW == null)
            {
                s_Field_kLabelFloatMinW = typeof(EditorGUILayout).GetProperty("kLabelFloatMinW", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (s_Field_kLabelFloatMinW == null)
                {
                    UnityEngine.Debug.LogError("not found: \"internal static float kLabelFloatMinW\"");
                }
            }
            if (s_Field_kLabelFloatMinW != null)
            {
                return (float)s_Field_kLabelFloatMinW.GetValue(null);
            }
            return EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + EditorGUIUtility.standardVerticalSpacing;
        }
    }

    //internal static float kLabelFloatMaxW => EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + EditorGUI.kSpacing;
    static System.Reflection.PropertyInfo s_Field_kLabelFloatMaxW;
    static float kLabelFloatMaxW
    {
        get
        {
            if (s_Field_kLabelFloatMaxW == null)
            {
                s_Field_kLabelFloatMaxW = typeof(EditorGUILayout).GetProperty("kLabelFloatMaxW",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (s_Field_kLabelFloatMaxW == null)
                {
                    UnityEngine.Debug.LogError("not found: \"internal static float kLabelFloatMaxW\"");
                }
            }
            if (s_Field_kLabelFloatMaxW != null)
            {
                return (float)s_Field_kLabelFloatMaxW.GetValue(null);
            }
            return EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + EditorGUIUtility.standardVerticalSpacing;
        }
    }

    // internal static Rect s_LastRect;
    static System.Reflection.FieldInfo s_Field_LastRect;
    static Rect s_LastRect
    {
        get
        {
            if (s_Field_LastRect == null)
            {
                s_Field_LastRect = typeof(EditorGUILayout).GetField("s_LastRect", 
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (s_Field_LastRect == null)
                {
                    UnityEngine.Debug.LogError("not found: \"internal static Rect s_LastRect;\"");
                }
            }
            if (s_Field_LastRect != null)
            {
                return (Rect)s_Field_LastRect.GetValue(null);
            }
            return Rect.zero;
        }
        set
        {
            if (s_Field_LastRect == null)
            {
                s_Field_LastRect = typeof(EditorGUILayout).GetField("s_LastRect", 
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (s_Field_LastRect == null)
                {
                    UnityEngine.Debug.LogError("not found: \"internal static Rect s_LastRect;\"");
                }
            }
            if (s_Field_LastRect != null)
            {
                s_Field_LastRect.SetValue(null, value);
            }
        }
    }

    // Gradient versions
    public static GradientEx GradientExField(GradientEx value, params GUILayoutOption[] options)
    {
        Rect r = s_LastRect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, kLabelFloatMaxW, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.colorField, options);
        return InternalEditorGUI.GradientExField(r, value);
    }

    public static GradientEx GradientExField(string label, GradientEx value, params GUILayoutOption[] options)
    {
        Rect r = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.colorField, options);
        return InternalEditorGUI.GradientExField(r, label, value);
    }

    public static GradientEx GradientExField(GUIContent label, GradientEx value, params GUILayoutOption[] options)
    {
        Rect r = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.colorField, options);
        return InternalEditorGUI.GradientExField(r, label, value);
    }

    public static GradientEx GradientExField(GUIContent label, GradientEx value, bool hdr, params GUILayoutOption[] options)
    {
        Rect r = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.colorField, options);
        return InternalEditorGUI.GradientExField(r, label, value, hdr);
    }

    // SerializedProperty versions
    internal static GradientEx GradientExField(SerializedProperty value, params GUILayoutOption[] options)
    {
        Rect r = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.colorField, options);
        return InternalEditorGUI.GradientExField(r, value);
    }

    internal static GradientEx GradientExField(string label, SerializedProperty value, params GUILayoutOption[] options)
    {
        Rect r = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.colorField, options);
        return InternalEditorGUI.GradientExField(r, label, value);
    }

    internal static GradientEx GradientExField(GUIContent label, SerializedProperty value, params GUILayoutOption[] options)
    {
        Rect r = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorStyles.colorField, options);
        return InternalEditorGUI.GradientExField(r, label, value);
    }
}


public sealed partial class InternalEditorGUI
{
    static readonly int s_GradientExHash = "s_GradientExHash".GetHashCode();
    static int s_GradientExID;

    // Gradient versions
    public static GradientEx GradientExField(Rect position, GradientEx gradient)
    {
        int id = EditorGUIUtility.GetControlID(s_GradientExHash, FocusType.Keyboard, position);
        return DoGradientExField(position, id, gradient, null, false);
    }

    public static GradientEx GradientExField(Rect position, string label, GradientEx gradient)
    {
        return GradientExField(position, EditorGUIUtility.TrTempContent(label), gradient);
    }

    public static GradientEx GradientExField(Rect position, GUIContent label, GradientEx gradient)
    {
        return GradientExField(position, label, gradient, false);
    }

    public static GradientEx GradientExField(Rect position, GUIContent label, GradientEx gradient, bool hdr)
    {
        int id = EditorGUIUtility.GetControlID(s_GradientExHash, FocusType.Keyboard, position);
        return DoGradientExField(EditorGUI.PrefixLabel(position, id, label), id, gradient, null, hdr);
    }

    // SerializedProperty versions
    internal static GradientEx GradientExField(Rect position, SerializedProperty property)
    {
        return GradientExField(position, property, false);
    }

    internal static GradientEx GradientExField(Rect position, SerializedProperty property, bool hdr)
    {
        int id = EditorGUIUtility.GetControlID(s_GradientExHash, FocusType.Keyboard, position);
        return DoGradientExField(position, id, null, property, hdr);
    }

    internal static GradientEx GradientExField(Rect position, string label, SerializedProperty property)
    {
        return GradientExField(position, EditorGUIUtility.TrTempContent(label), property);
    }

    internal static GradientEx GradientExField(Rect position, GUIContent label, SerializedProperty property)
    {
        int id = EditorGUIUtility.GetControlID(s_GradientExHash, FocusType.Keyboard, position);
        return DoGradientExField(EditorGUI.PrefixLabel(position, id, label), id, null, property, false);
    }

    internal static GradientEx DoGradientExField(Rect position, int id, GradientEx value, SerializedProperty property, bool hdr)
    {
        Event evt = Event.current;

        switch (evt.GetTypeForControl(id))
        {
            case EventType.MouseDown:
                if (position.Contains(evt.mousePosition))
                {
                    if (evt.button == 0)
                    {
                        s_GradientExID = id;
                        GUIUtility.keyboardControl = id;
                        var propertyValue = property != null ? GradientExEditor.SerializedPropertyExtensions.GetSerializedValue<GradientEx>(property) : null;
                        GradientEx gradient = propertyValue != null ? propertyValue : value;
                        GradientExPicker.Show(gradient, hdr);
                        GUIUtility.ExitGUI();
                    }
                    else if (evt.button == 1)
                    {
                        if (property != null)
                        {
                            GradientExContextMenu.Show(property.Copy());
                        }
                        else if(value != null)
                        {
                            GradientExContextMenu.Show(value);
                        }
                    }
                }
                break;
            case EventType.Repaint:
                {
                    Rect r2 = new Rect(position.x + 1, position.y + 1, position.width - 2, position.height - 4);    // Adjust for box drawn on top
                    if (property != null)
                    {
                        GradientExEditor.DrawGradientSwatch(r2, property, Color.white);
                    }
                    else
                    {
                        GradientExEditor.DrawGradientSwatch(r2, value, Color.white);
                    }
                    GradientExEditor.Styles.colorPickerBox.Draw(position, GUIContent.none, id);
                    break;
                }
            case EventType.ExecuteCommand:
                if (s_GradientExID == id && evt.commandName == GradientExEditor.EventCommandNames.GradientExPickerChanged)
                {
                    GUI.changed = true;
                    GradientExPreviewCache.ClearCache();
                    HandleUtility.Repaint();
                    if (property != null)
                    {
                        GradientExEditor.SerializedPropertyExtensions.SetSerializedValue(property, GradientExPicker.gradient);
                    }

                    return GradientExPicker.gradient;
                }
                break;
            case EventType.ValidateCommand:
                if (s_GradientExID == id && evt.commandName == GradientExEditor.EventCommandNames.UndoRedoPerformed)
                {
                    if (property != null)
                    {
                        var propertyValue = property != null ? GradientExEditor.SerializedPropertyExtensions.GetSerializedValue<GradientEx>(property) : null;
                        GradientEx gradient = propertyValue != null ? propertyValue : value;
                        GradientExPicker.SetCurrentGradient(gradient);
                    }
                    GradientExPreviewCache.ClearCache();
                    return value;
                }
                break;
            case EventType.KeyDown:
                if (GUIUtility.keyboardControl == id && (evt.keyCode == KeyCode.Space || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter))
                {
                    Event.current.Use();
                    var propertyValue = property != null ? GradientExEditor.SerializedPropertyExtensions.GetSerializedValue<GradientEx>(property) : null;
                    GradientEx gradient = propertyValue != null ? propertyValue : value;
                    GradientExPicker.Show(gradient, hdr);
                    GUIUtility.ExitGUI();
                }
                break;
        }
        return value;
    }
}
