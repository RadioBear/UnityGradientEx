using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

internal class GradientExContextMenu
{
    readonly SerializedProperty m_Property;
    readonly GradientEx m_Value;

    internal static void Show(SerializedProperty prop)
    {
        GUIContent copy = EditorGUIUtility.TrTextContent("Copy");
        GUIContent paste = EditorGUIUtility.TrTextContent("Paste");

        GenericMenu menu = new GenericMenu();
        var gradientMenu = new GradientExContextMenu(prop);
        menu.AddItem(copy, false, gradientMenu.Copy);
        if (GradientExClipboard.hasGradientEx)
        {
            menu.AddItem(paste, false, gradientMenu.Paste);
        }
        else
        {
            menu.AddDisabledItem(paste);
        }
        menu.ShowAsContext();
        Event.current.Use();
    }

    internal static void Show(GradientEx prop)
    {
        GUIContent copy = EditorGUIUtility.TrTextContent("Copy");
        GUIContent paste = EditorGUIUtility.TrTextContent("Paste");

        GenericMenu menu = new GenericMenu();
        var gradientMenu = new GradientExContextMenu(prop);
        menu.AddItem(copy, false, gradientMenu.Copy);
        if (GradientExClipboard.hasGradientEx)
        {
            menu.AddItem(paste, false, gradientMenu.Paste);
        }
        else
        {
            menu.AddDisabledItem(paste);
        }
        menu.ShowAsContext();
        Event.current.Use();
    }

    GradientExContextMenu(SerializedProperty prop)
    {
        m_Property = prop;
    }

    GradientExContextMenu(GradientEx val)
    {
        m_Value = val;
    }

    void Copy()
    {
        if (m_Property != null)
        {
            GradientExClipboard.gradientExValue = GradientExEditor.SerializedPropertyExtensions.GetSerializedValue<GradientEx>(m_Property);
        }
        else if (m_Value != null)
        {
            GradientExClipboard.gradientExValue = m_Value;
        }
    }

    void Paste()
    {
        var grad = GradientExClipboard.gradientExValue;
        if (grad == null)
        {
            return;
        }

        if (m_Property != null)
        {
            GradientExEditor.SerializedPropertyExtensions.SetSerializedValue(m_Property, grad);
            m_Property.serializedObject.ApplyModifiedProperties();
            GradientExPreviewCache.ClearCache();
        }
        else if(m_Value != null)
        {
            grad.CopyTo(m_Value);
        }
    }
}