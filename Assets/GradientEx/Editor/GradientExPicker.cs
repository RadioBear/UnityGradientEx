using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GradientExPicker : EditorWindow
{
    private const int k_DefaultNumSteps = 0;

    private static GradientExPicker s_GradientExPicker;

    private GradientExEditor m_GradientExEditor;
    private GradientEx m_GradientEx;
    private ScriptableObject m_DelegateView;
    private System.Action<GradientEx> m_Delegate;
    private bool m_HDR;

    // Static methods
    public static void Show(GradientEx newGradient, bool hdr)
    {
        var currentView = GradientExEditor.GUIView_current;
        PrepareShow(hdr);
        s_GradientExPicker.m_DelegateView = currentView as ScriptableObject;
        s_GradientExPicker.m_Delegate = null;
        s_GradientExPicker.Init(newGradient, hdr);

        GradientExPreviewCache.ClearCache();
    }

    // Static methods
    public static void Show(GradientEx newGradient, bool hdr, System.Action<GradientEx> onGradientExChanged)
    {
        PrepareShow(hdr);
        s_GradientExPicker.m_Delegate = onGradientExChanged;
        s_GradientExPicker.Init(newGradient, hdr);

        GradientExPreviewCache.ClearCache();
    }

    static void PrepareShow(bool hdr)
    {
        if (s_GradientExPicker == null)
        {
            string title = hdr ? "HDR GradientEx Editor" : "GradientEx Editor";
            s_GradientExPicker = (GradientExPicker)GetWindow(typeof(GradientExPicker), true, title, false);
            Vector2 minSize = new Vector2(360, 234);
            Vector2 maxSize = new Vector2(1900, 3000);
            s_GradientExPicker.minSize = minSize;
            s_GradientExPicker.maxSize = maxSize;
            s_GradientExPicker.wantsMouseMove = true;
            s_GradientExPicker.ShowAuxWindow(); // Use this if auto close on lost focus is wanted.
        }
        else
        {
            s_GradientExPicker.Repaint(); // Ensure we get a OnGUI so we refresh if new gradient
        }
    }

    //public static GradientExPicker instance
    //{
    //    get
    //    {
    //        if (!s_GradientExPicker)
    //        {
    //            Debug.LogError("GradientEx Picker not initalized, did you call Show first?");
    //        }
    //        return s_GradientExPicker;
    //    }
    //}

    private void Init(GradientEx newGradient, bool hdr)
    {
        m_GradientEx = newGradient;
        m_HDR = hdr;
        if (m_GradientExEditor != null)
        {
            m_GradientExEditor.Init(newGradient, k_DefaultNumSteps, m_HDR);
        }
        Repaint();
    }

    private void SetGradientData(GradientEx gradient)
    {
        m_GradientEx.colorKeys = gradient.colorKeys;
        m_GradientEx.alphaKeys = gradient.alphaKeys;
        m_GradientEx.mode = gradient.mode;
        Init(m_GradientEx, m_HDR);
    }

    public static bool visible
    {
        get { return s_GradientExPicker != null; }
    }

    public static GradientEx gradient
    {
        get
        {
            if (s_GradientExPicker != null)
            {
                return s_GradientExPicker.m_GradientEx;
            }
            return null;
        }
    }

    public void OnEnable()
    {
        hideFlags = HideFlags.DontSave;
        // Use these if window is not an aux window for auto closing on play/stop
        //EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
        wantsMouseMove = true;
    }

    public void OnDisable()
    {
        //EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;

        s_GradientExPicker = null;
    }

    void OnPlayModeStateChanged()
    {
        Close();
    }

    void InitIfNeeded()
    {
        // Init editor when needed
        if (m_GradientExEditor == null)
        {
            m_GradientExEditor = new GradientExEditor();
            m_GradientExEditor.Init(m_GradientEx, k_DefaultNumSteps, m_HDR);
        }
    }

    public void OnGUI()
    {
        // When we start play (using shortcut keys) we get two OnGui calls and m_Gradient is null: so early out.
        if (m_GradientEx == null)
        {
            return;
        }

        InitIfNeeded();

        float gradientEditorHeight = Mathf.Min(position.height, 500f);
        float distBetween = 15f;
        float presetLibraryHeight = position.height - gradientEditorHeight - distBetween;

        Rect gradientEditorRect = new Rect(10, 10, position.width - 20, gradientEditorHeight - 20);
        Rect gradientLibraryRect = new Rect(0, gradientEditorHeight + distBetween, position.width, presetLibraryHeight);

        // Separator
        EditorGUI.DrawRect(new Rect(gradientLibraryRect.x, gradientLibraryRect.y - 1, gradientLibraryRect.width, 1), new Color(0, 0, 0, 0.3f));
        EditorGUI.DrawRect(new Rect(gradientLibraryRect.x, gradientLibraryRect.y, gradientLibraryRect.width, 1), new Color(1, 1, 1, 0.1f));

        gradientLibraryRect.yMin += 5;
        // The meat
        EditorGUI.BeginChangeCheck();
        m_GradientExEditor.OnGUI(gradientEditorRect);
        //m_GradientExDataEditor.OnGUI(gradientLibraryRect);
        if (EditorGUI.EndChangeCheck())
        {
            SendEvent(true);
        }
    }

    void SendEvent(bool exitGUI)
    {
        if (m_DelegateView)
        {
            Event e = EditorGUIUtility.CommandEvent(GradientExEditor.EventCommandNames.GradientExPickerChanged);
            Repaint();
            GradientExEditor.GUIView_SendEvent(m_DelegateView, e);
            if (exitGUI)
            {
                GUIUtility.ExitGUI();
            }
        }
        if (m_Delegate != null)
        {
            m_Delegate(gradient);
        }
    }

    public static void SetCurrentGradient(GradientEx gradient)
    {
        if (s_GradientExPicker == null)
        {
            return;
        }

        s_GradientExPicker.SetGradientData(gradient);
        GUI.changed = true;
    }

    public static void CloseWindow()
    {
        if (s_GradientExPicker == null)
        {
            return;
        }

        s_GradientExPicker.Close();
        GUIUtility.ExitGUI();
    }

    public static void RepaintWindow()
    {
        if (s_GradientExPicker == null)
        {
            return;
        }
        s_GradientExPicker.Repaint();
    }
}
