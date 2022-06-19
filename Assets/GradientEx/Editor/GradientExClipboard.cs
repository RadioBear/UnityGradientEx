using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

internal static class GradientExClipboard
{
    internal static class ClipboardParser
    {
        public static bool ParseCustom<T>(string text, out T res) where T : new()
        {
            res = new T();
            if (string.IsNullOrEmpty(text))
                return false;
            var prefix = CustomPrefix<T>();
            if (!text.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
                return false;
            try
            {
                EditorJsonUtility.FromJsonOverwrite(text.Substring(prefix.Length), res);
            }
            catch (System.ArgumentException)
            {
                return false;
            }
            return true;
        }

        public static string WriteCustom<T>(T val)
        {
            return CustomPrefix<T>() + EditorJsonUtility.ToJson(val);
        }

        static string CustomPrefix<T>()
        {
            return typeof(T).FullName + "JSON:";
        }
    }

    internal class ClipboardState
    {
        internal string m_RawContents;

        internal bool? m_HasGradientEx;
        internal GradientEx m_ValueGradientEx;
        internal void FetchGradientEx()
        {
            if (!m_HasGradientEx.HasValue)
            {
                m_HasGradientEx = ClipboardParser.ParseCustom<GradientEx>(m_RawContents, out var wrapper);
                m_ValueGradientEx = wrapper;
            }
        }
    }

    static ClipboardState m_State = new ClipboardState();

    public static bool hasString
    {
        get
        {
            FetchState();
            return !string.IsNullOrEmpty(m_State.m_RawContents);
        }
    }

    public static string stringValue
    {
        get
        {
            FetchState();
            return m_State.m_RawContents;
        }
        set => EditorGUIUtility.systemCopyBuffer = value;
    }

    public static bool hasGradientEx
    {
        get
        {
            FetchState();
            m_State.FetchGradientEx();
            return m_State.m_HasGradientEx.Value;
        }
    }

    public static GradientEx gradientExValue
    {
        get
        {
            FetchState();
            m_State.FetchGradientEx();
            return m_State.m_ValueGradientEx;
        }
        set => EditorGUIUtility.systemCopyBuffer = ClipboardParser.WriteCustom(value);
    }

    static void FetchState()
    {
        var systemClipboard = EditorGUIUtility.systemCopyBuffer;
        if (systemClipboard == m_State.m_RawContents)
            return;
        m_State = new ClipboardState { m_RawContents = systemClipboard };
    }
}