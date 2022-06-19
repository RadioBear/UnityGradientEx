using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class GradientExEditor
{
    internal static class Styles
    {
        public const int k_SwatchWidht = 10;

        public const float fixedWindowWidth = 233;
        public const float channelSliderLabelWidth = 14f;
        public const float sliderTextFieldWidth = 45f;
        public const float extraVerticalSpacing = 6f;

        internal static GUIStyle GetStyle(string styleName)
        {
            GUIStyle s = GUI.skin.FindStyle(styleName) ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            if (s == null)
            {
                Debug.LogError("Missing built-in guistyle " + styleName);
                s = GUIStyle.none;
            }
            return s;

        }
        internal class SkinnedColor
        {
            Color normalColor;
            Color proColor;

            public SkinnedColor(Color color, Color proColor)
            {
                normalColor = color;
                this.proColor = proColor;
            }

            public SkinnedColor(Color color)
            {
                normalColor = color;
                proColor = color;
            }

            public Color color
            {
                get { return EditorGUIUtility.isProSkin ? proColor : normalColor; }

                set
                {
                    if (EditorGUIUtility.isProSkin)
                        proColor = value;
                    else
                        normalColor = value;
                }
            }

            public static implicit operator Color(SkinnedColor colorSkin)
            {
                return colorSkin.color;
            }
        }

        private static readonly GUIContent s_MixedValueContent = EditorGUIUtility.TrTextContent("\u2014", "Mixed Values");
        internal static GUIContent mixedValueContent => s_MixedValueContent;

        private static readonly GUIContent s_RemoveContent = EditorGUIUtility.IconContent("P4_DeletedLocal");
        internal static GUIContent removeContent => s_RemoveContent;

        public readonly static GUIStyle upSwatch = "Grad Up Swatch";
        public readonly static GUIStyle upSwatchOverlay = "Grad Up Swatch Overlay";
        public readonly static GUIStyle downSwatch = "Grad Down Swatch";
        public readonly static GUIStyle downSwatchOverlay = "Grad Down Swatch Overlay";

        public readonly static GUIContent modeText = EditorGUIUtility.TrTextContent("Mode");
        public readonly static GUIContent alphaText = EditorGUIUtility.TrTextContent("Alpha");
        public readonly static GUIContent colorText = EditorGUIUtility.TrTextContent("Color");
        public readonly static GUIContent locationText = EditorGUIUtility.TrTextContent("Location");
        public readonly static GUIContent percentText = new GUIContent("%");

        public static readonly GUIContent[] sliderModeLabels = new[]
        {
            EditorGUIUtility.TrTextContent("RGB 0-255"),
            EditorGUIUtility.TrTextContent("RGB 0-1.0"),
        };
        public static readonly int[] sliderModeValues = new[] { 0, 1};

        private static GUIStyle s_WhiteTextureStyle;
        internal static GUIStyle whiteTextureStyle => s_WhiteTextureStyle ??
            (s_WhiteTextureStyle = new GUIStyle { normal = { background = EditorGUIUtility.whiteTexture } });

        private static GUIStyle s_BasicTextureStyle;
        internal static GUIStyle GetBasicTextureStyle(Texture2D tex)
        {
            if (s_BasicTextureStyle == null)
                s_BasicTextureStyle = new GUIStyle();

            s_BasicTextureStyle.normal.background = tex;

            return s_BasicTextureStyle;
        }

        private static GUIStyle s_ColorPickerBox;
        internal static GUIStyle colorPickerBox => s_ColorPickerBox ??
            (s_ColorPickerBox = GetStyle("ColorPickerBox"));


        public readonly static GUIContent foldoutAlphaDataText = EditorGUIUtility.TrTextContent("Alpha Key Data");
        public readonly static GUIContent foldoutColorDataText = EditorGUIUtility.TrTextContent("Color Key Data");

        public readonly static SkinnedColor itemHoverColor = new SkinnedColor(Color.blue, Color.yellow);
    }

    internal static class EventCommandNames
    {
        public const string Delete = "Delete";
        public const string UndoRedoPerformed = "UndoRedoPerformed";

        public const string ColorPickerChanged = "ColorPickerChanged";
        public const string GradientExPickerChanged = "GradientExPickerChanged";

    }


    public abstract class SwatchBase
    {
        public float m_Time;

        public SwatchBase(float time)
        {
            m_Time = time;
        }

        public abstract Color GetBackgroundColor();
        public abstract GUIStyle GetSwatchBackgroundStyle();
        public abstract GUIStyle GetSwatchOverlayStyle();
        public abstract bool IsSwatchDirectionUpwards();
        public abstract bool CanShowColorPicker();
        public abstract void ShowColorPicker(bool hdr);
        public abstract void SetFromColorPicker(Color val);
        public abstract void DoDrawValue(Rect rect, bool hdr);


        public static T Create<T>(float time, Color val) where T : SwatchBase
        {
            if (typeof(T) == typeof(SwatchAlpha))
            {
                return new SwatchAlpha(time, val.a) as T;
            }
            else
            {
                return new SwatchColor(time, new Color(val.a, val.a, val.a, 1f)) as T;
            }
        }
    }

    public class SwatchColor : SwatchBase
    {
        public Color m_Value;

        public SwatchColor(float time, Color value) : base(time)
        {
            m_Value = value;
        }

        public override Color GetBackgroundColor()
        {
            return m_Value;
        }
        public override GUIStyle GetSwatchBackgroundStyle()
        {
            return Styles.upSwatch;
        }
        public override GUIStyle GetSwatchOverlayStyle()
        {
            return Styles.upSwatchOverlay;
        }
        public override bool IsSwatchDirectionUpwards()
        {
            return true;
        }

        public override bool CanShowColorPicker()
        {
            return true;
        }
        public override void ShowColorPicker(bool hdr)
        {
            ColorPicker_Show(m_Value, false, hdr);
        }
        public override void SetFromColorPicker(Color val)
        {
            m_Value = val;
        }


        public override void DoDrawValue(Rect rect, bool hdr)
        {
            m_Value = EditorGUI.ColorField(rect, Styles.colorText, m_Value, true, false, hdr);
        }
    }

    public class SwatchAlpha : SwatchBase
    {
        public float m_Value;

        public SwatchAlpha(float time, float value) : base(time)
        {
            m_Value = value;
        }

        public override Color GetBackgroundColor()
        {
            return Color.white;
        }
        public override GUIStyle GetSwatchBackgroundStyle()
        {
            return Styles.downSwatch;
        }
        public override GUIStyle GetSwatchOverlayStyle()
        {
            return Styles.downSwatchOverlay;
        }
        public override bool IsSwatchDirectionUpwards()
        {
            return false;
        }

        public override bool CanShowColorPicker()
        {
            return false;
        }
        public override void ShowColorPicker(bool hdr)
        {

        }
        public override void SetFromColorPicker(Color val)
        {

        }

        public override void DoDrawValue(Rect rect, bool hdr)
        {
            var old = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.fieldWidth = 30;
            float sliderValue = EditorGUI.IntSlider(rect, Styles.alphaText, (int)(m_Value * 255), 0, 255) / 255f;
            EditorGUIUtility.fieldWidth = old;
            if (sliderValue != m_Value)
            {
                sliderValue = Mathf.Clamp01(sliderValue);
                m_Value = sliderValue;
            }
        }
    }

    #region Background

    static Texture2D s_BackgroundTexture;
    public static Texture2D GetBackgroundTexture()
    {
        if (s_BackgroundTexture == null)
            s_BackgroundTexture = CreateCheckerTexture(32, 4, 4, Color.white, new Color(0.7f, 0.7f, 0.7f));
        return s_BackgroundTexture;
    }
    private static Texture2D CreateCheckerTexture(int numCols, int numRows, int cellPixelWidth, Color col1, Color col2)
    {
        int height = numRows * cellPixelWidth;
        int width = numCols * cellPixelWidth;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.hideFlags = HideFlags.HideAndDontSave;
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < numRows; i++)
            for (int j = 0; j < numCols; j++)
                for (int ci = 0; ci < cellPixelWidth; ci++)
                    for (int cj = 0; cj < cellPixelWidth; cj++)
                        pixels[(i * cellPixelWidth + ci) * width + j * cellPixelWidth + cj] = ((i + j) % 2 == 0) ? col1 : col2;

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    #endregion


    [System.NonSerialized]
    SwatchBase m_SelectedSwatch;

    List<SwatchColor> m_RGBSwatches;
    List<SwatchAlpha> m_AlphaSwatches;

    GradientEx m_GradientEx;
    int m_NumSteps;
    bool m_HDR;
    GradientEx.GradientMode m_GradientMode;

    enum SliderMode { RGB, RGBFloat }
    [SerializeField]
    SliderMode m_SliderMode = SliderMode.RGB;

    private static bool m_IsFoldoutAlpha;
    private static bool m_IsFoldoutColor;
    private Vector2 m_TotalScollPos;

    // Fixed steps are only used if numSteps > 1
    public void Init(GradientEx gradient, int numSteps, bool hdr)
    {
        m_GradientEx = gradient;
        m_NumSteps = numSteps;
        m_HDR = hdr;

        BuildArrays();

        if (m_RGBSwatches.Count > 0)
        {
            m_SelectedSwatch = m_RGBSwatches[0];
        }
    }

    public GradientEx target
    {
        get { return m_GradientEx; }
    }

    float GetTime(float actualTime)
    {
        actualTime = Mathf.Clamp01(actualTime);

        if (m_NumSteps > 1)
        {
            float stepSize = 1.0f / (m_NumSteps - 1);
            int step = Mathf.RoundToInt(actualTime / stepSize);
            return step / (float)(m_NumSteps - 1);
        }

        return actualTime;
    }


    void BuildArrays()
    {
        if (m_GradientEx == null)
        {
            return;
        }
        var colorKeys = m_GradientEx.colorKeys;
        m_RGBSwatches = new List<SwatchColor>(colorKeys.Length);
        for (int i = 0; i < colorKeys.Length; i++)
        {
            Color color = colorKeys[i].color;
            color.a = 1f;
            m_RGBSwatches.Add(new SwatchColor(colorKeys[i].NormalizedTime, color));
        }

        var alphaKeys = m_GradientEx.alphaKeys;
        m_AlphaSwatches = new List<SwatchAlpha>(alphaKeys.Length);
        for (int i = 0; i < alphaKeys.Length; i++)
        {
            float a = alphaKeys[i].alpha;
            m_AlphaSwatches.Add(new SwatchAlpha(alphaKeys[i].NormalizedTime, a));
        }
        m_GradientMode = m_GradientEx.mode;
    }

    private static void DrawGradient(Rect position, Color topColor, Color bottomColor)
    {
        var width = Mathf.RoundToInt(position.width);
        var height = Mathf.RoundToInt(position.height);
        position.height = 1;
        var old = GUI.backgroundColor;
        for (int i = 0; i < height; i++)
        {
            Color columnColor = Color.Lerp(topColor, bottomColor, i / (float)(height - 1));
            GUI.backgroundColor = columnColor;
            GUI.DrawTexture(position, EditorGUIUtility.whiteTexture);
            position.y = position.y + 1;
        }
        GUI.backgroundColor = old;
    }

    public static void DrawGradientWithBackground(Rect position, GradientEx gradient)
    {
        Texture2D gradientTexture = GradientExPreviewCache.GetGradientPreview(gradient);
        Rect r2 = new Rect(position.x + 1, position.y + 1, position.width - 2, position.height - 2);

        // Background checkers
        Texture2D backgroundTexture = GetBackgroundTexture();
        Rect texCoordsRect = new Rect(0, 0, r2.width / backgroundTexture.width, r2.height / backgroundTexture.height);
        GUI.DrawTextureWithTexCoords(r2, backgroundTexture, texCoordsRect, false);

        // Gradient texture
        if (gradientTexture != null)
        {
            GUI.DrawTexture(r2, gradientTexture, ScaleMode.StretchToFill, true);
        }

        // Frame over texture
        //GUI.Label(position, GUIContent.none, Styles.colorPickerBox);
        Styles.colorPickerBox.Draw(position, false, false, false, false);

        // HDR label
        float maxColorComponent = GetMaxColorComponent(gradient);
        if (maxColorComponent > 1.0f)
        {
            GUI.Label(new Rect(position.x, position.y, position.width - 3, position.height), "HDR", EditorStyles.centeredGreyMiniLabel);
        }
    }

    public void OnGUI(Rect position)
    {
        float separatorTotalHeight = 4;
        float separatorLineHeight = 1;
        position.height -= separatorTotalHeight;
        float swatchTotalHeight = Mathf.Min(position.height * 0.6f, 150f);
        float dataTotalHeight = position.height  - swatchTotalHeight;

        Rect rect = new Rect(position.x, position.y, position.width, swatchTotalHeight);
        OnGUI_Swatch(rect);

        // Separator
        rect.yMin = rect.yMax;
        rect.y += (separatorTotalHeight - (separatorLineHeight * 2.0f)) * 0.5f;
        rect.height = separatorTotalHeight;
        EditorGUI.DrawRect(new Rect(rect.x, rect.y - separatorLineHeight, rect.width, separatorLineHeight), new Color(0, 0, 0, 0.3f));
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, separatorLineHeight), new Color(1, 1, 1, 0.1f));

        // data
        rect.yMin = rect.yMax;
        rect.height = dataTotalHeight;
        OnGUI_Data(rect);
    }

    private void OnGUI_Swatch(Rect position)
    {
        Rect drawRect = position;
        float modeHeight = 28f;
        float spaceH_ModeToSwatch = 5f;
        float swatchHeight = 16f;
        float spaceH_SwatchToEditSection = 15f;
        float editSectionHeight = 26f;
        float gradientTextureHeight = position.height - spaceH_ModeToSwatch - spaceH_SwatchToEditSection - (2 * swatchHeight) - editSectionHeight - modeHeight;

        drawRect.height = modeHeight;
        m_GradientMode = (GradientEx.GradientMode)EditorGUI.EnumPopup(drawRect, Styles.modeText, m_GradientMode);
        if (m_GradientMode != m_GradientEx.mode)
        {
            AssignBack();
        }

        // Alpha swatches (no idea why they're top, but that's what Adobe & Apple seem to agree on)
        drawRect.yMin = drawRect.yMax;
        drawRect.y += spaceH_ModeToSwatch;
        drawRect.height = swatchHeight;
        {
            SwatchAlpha swatchAlpha = m_SelectedSwatch as SwatchAlpha;
            SwatchAlpha selected = swatchAlpha;
            ShowSwatchArray(drawRect, m_AlphaSwatches, ref selected);
            if (selected != swatchAlpha)
            {
                m_SelectedSwatch = selected;
            }
        }

        // Gradient texture
        drawRect.yMin = drawRect.yMax;
        drawRect.height = gradientTextureHeight;
        if (Event.current.type == EventType.Repaint)
        {
            DrawGradientWithBackground(drawRect, m_GradientEx);
        }

        // Color swatches (bottom)
        drawRect.yMin = drawRect.yMax;
        drawRect.height = swatchHeight;
        {
            SwatchColor swatchColor = m_SelectedSwatch as SwatchColor;
            SwatchColor selected = swatchColor;
            ShowSwatchArray(drawRect, m_RGBSwatches, ref selected);
            if (selected != swatchColor)
            {
                m_SelectedSwatch = selected;
            }
        }

        if (m_SelectedSwatch != null)
        {
            drawRect.yMin = drawRect.yMax;
            drawRect.y += spaceH_SwatchToEditSection;
            drawRect.height = editSectionHeight;

            float locationWidth = 65;
            float locationTextWidth = 60;
            float space = 20;
            float alphaOrColorTextWidth = 40;
            float totalLocationWidth = locationTextWidth + space + locationTextWidth + locationWidth;

            // Alpha or Color field
            Rect rect = drawRect;
            rect.height = 18;
            rect.x += 0;
            rect.width -= totalLocationWidth;
            EditorGUIUtility.labelWidth = alphaOrColorTextWidth;
            if (m_SelectedSwatch != null)
            {
                EditorGUI.BeginChangeCheck();
                m_SelectedSwatch.DoDrawValue(rect, m_HDR);
                if (EditorGUI.EndChangeCheck())
                {
                    AssignBack();
                    HandleUtility.Repaint();
                }
            }

            // Location of key
            rect.x += rect.width + space;
            rect.width = locationWidth + locationTextWidth;

            EditorGUIUtility.labelWidth = locationTextWidth;

            EditorGUI.BeginChangeCheck();
            float newLocation = EditorGUI.FloatField(rect, Styles.locationText, m_SelectedSwatch.m_Time * 100.0f) / 100.0f;
            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedSwatch.m_Time = Mathf.Clamp(newLocation, 0f, 1f);
                AssignBack();
            }

            rect.x += rect.width;
            rect.width = 20;
            GUI.Label(rect, Styles.percentText);
        }
    }

    private void OnGUI_Data(Rect position)
    {
        if (m_GradientEx == null)
        {
            return;
        }
        using (var areaScope = new GUILayout.AreaScope(position))
        {
            m_TotalScollPos = EditorGUILayout.BeginScrollView(m_TotalScollPos);
            {
                m_IsFoldoutAlpha = EditorGUILayout.Foldout(m_IsFoldoutAlpha, Styles.foldoutAlphaDataText, true);
                if (m_IsFoldoutAlpha)
                {
                    var list = GetGradientExAlphaKeyList(m_GradientEx);
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; ++i)
                        {
                            SwatchAlpha switchItem = null;
                            if (i >= 0 && i < m_AlphaSwatches.Count)
                            {
                                switchItem = m_AlphaSwatches[i];
                            }
                            var key = list[i];
                            var alpha = key.alpha;
                            bool isSelected = (m_SelectedSwatch != null) ? (m_SelectedSwatch == switchItem) : false;
                            var rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true));
                            System.Action<Rect, int, bool, bool> func = (rect, id, on, hover) =>
                            {
                                if (hover)
                                {
                                    var c = EditorStyles.label.normal.textColor;
                                    c.a = 0.5f;
                                    DrawOutline(rect, 1.0f, c);
                                }
                                else if (isSelected)
                                {
                                    DrawOutline(rect, 1.0f, Styles.itemHoverColor);
                                }

                                var colorRect = GetGUIDataItemColorRect(rect);
                                // Background checkers
                                Texture2D backgroundTexture = GetBackgroundTexture();
                                Rect texCoordsRect = new Rect(0, 0, colorRect.width / backgroundTexture.width, colorRect.height / backgroundTexture.height);
                                GUI.DrawTextureWithTexCoords(colorRect, backgroundTexture, texCoordsRect, false);

                                var newColor = EditorStyles.label.normal.textColor;
                                newColor.a = alpha;
                                EditorGUI.DrawRect(colorRect, newColor);

                                var textRect = GetGUIDataItemTextRect(rect);
                                GUI.Label(textRect, GUIAlphaValueToString(alpha), EditorStyles.label);
                            };

                            if (GradientDataItem(rect, func, GUIStyle.none))
                            {
                                if (i >= 0 && i < m_AlphaSwatches.Count)
                                {
                                    m_SelectedSwatch = m_AlphaSwatches[i];
                                }
                            }
                        }
                    }
                }

                m_IsFoldoutColor = EditorGUILayout.Foldout(m_IsFoldoutColor, Styles.foldoutColorDataText, true);
                if (m_IsFoldoutColor)
                {
                    var list = GetGradientExColorKeyList(m_GradientEx);
                    if (list != null)
                    {
                        m_SliderMode = (SliderMode)EditorGUILayout.IntPopup(GUIContent.none, (int)m_SliderMode, Styles.sliderModeLabels, Styles.sliderModeValues);
                        GUILayout.Space(Styles.extraVerticalSpacing);

                        for (int i = 0; i < list.Count; ++i)
                        {
                            SwatchColor switchItem = null;
                            if (i >= 0 && i < m_RGBSwatches.Count)
                            {
                                switchItem = m_RGBSwatches[i];
                            }
                            var key = list[i];
                            var color = key.color;
                            color.a = 1.0f;
                            var inv_color = new Color(1.0f - color.r, 1.0f - color.g, 1.0f - color.b, 1.0f);
                            bool isSelected = (m_SelectedSwatch != null) ? (m_SelectedSwatch == switchItem) : false;
                            var rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true));
                            System.Action<Rect, int, bool, bool> func = (rect, id, on, hover) =>
                            {
                                if (hover)
                                {
                                    var c = EditorStyles.label.normal.textColor;
                                    c.a = 0.5f;
                                    DrawOutline(rect, 1.0f, c);
                                }
                                else if (isSelected)
                                {
                                    DrawOutline(rect, 1.0f, Styles.itemHoverColor);
                                }

                                var colorRect = GetGUIDataItemColorRect(rect);
                                EditorGUI.DrawRect(colorRect, color);

                                var textRect = GetGUIDataItemTextRect(rect);
                                GUI.Label(textRect, GUIColorRGBToString(color, m_SliderMode), EditorStyles.label);
                            };

                            if (GradientDataItem(rect, func, GUIStyle.none))
                            {
                                if (i >= 0 && i < m_RGBSwatches.Count)
                                {
                                    m_SelectedSwatch = m_RGBSwatches[i];
                                }
                            }


                            

                            
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private class Reflection
    {

        public class GUI
        {
            static System.Reflection.MethodInfo s_Method_GrabMouseControl;
            // internal static extern void GrabMouseControl(int id);
            internal static void GrabMouseControl(int id)
            {
                if (s_Method_GrabMouseControl == null)
                {
                    s_Method_GrabMouseControl = typeof(UnityEngine.GUI).GetMethod("GrabMouseControl",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    if (s_Method_GrabMouseControl == null)
                    {
                        UnityEngine.Debug.LogError("not found: \"internal static extern void GrabMouseControl(int id);\"");
                    }
                }
                if (s_Method_GrabMouseControl != null)
                {
                    s_Method_GrabMouseControl.Invoke(null, new object[]{ id });
                }
            }

            static System.Reflection.MethodInfo s_Method_HasMouseControl;
            // internal static extern bool HasMouseControl(int id);
            internal static bool HasMouseControl(int id)
            {
                if (s_Method_HasMouseControl == null)
                {
                    s_Method_HasMouseControl = typeof(UnityEngine.GUI).GetMethod("HasMouseControl",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    if (s_Method_HasMouseControl == null)
                    {
                        UnityEngine.Debug.LogError("not found: \"internal static extern bool HasMouseControl(int id);\"");
                    }
                }
                if (s_Method_HasMouseControl != null)
                {
                    return (bool)s_Method_HasMouseControl.Invoke(null, new object[] { id });
                }
                return false;
            }

            static System.Reflection.MethodInfo s_Method_ReleaseMouseControl;
            // internal static extern void ReleaseMouseControl();
            internal static void ReleaseMouseControl()
            {
                if (s_Method_ReleaseMouseControl == null)
                {
                    s_Method_ReleaseMouseControl = typeof(UnityEngine.GUI).GetMethod("ReleaseMouseControl",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    if (s_Method_ReleaseMouseControl == null)
                    {
                        UnityEngine.Debug.LogError("not found: \"internal static extern void ReleaseMouseControl();\"");
                    }
                }
                if (s_Method_ReleaseMouseControl != null)
                {
                    s_Method_ReleaseMouseControl.Invoke(null, null);
                }
            }

        }
    }

    internal static void DrawOutline(Rect rect, float size, Color color)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        Color orgColor = GUI.color;
        GUI.color = GUI.color * color;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, size), EditorGUIUtility.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - size, rect.width, size), EditorGUIUtility.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - size, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);

        GUI.color = orgColor;
    }

    internal static bool HitTest(Rect rect, Vector2 point, int offset)
    {
        return (point.x >= rect.xMin - offset) && (point.x < rect.xMax + offset) && (point.y >= rect.yMin - offset) && (point.y < rect.yMax + offset);
    }
    internal static bool HitTest(Rect rect, Vector2 point, bool isDirectManipulationDevice)
    {
        int offset = isDirectManipulationDevice ? 3 : 0;
        return HitTest(rect, point, offset);
    }
    internal static bool HitTest(Rect rect, Event evt)
    {
        bool isDirectManipulationDevice = evt.pointerType == PointerType.Pen || evt.pointerType == PointerType.Touch;
        return HitTest(rect, evt.mousePosition, isDirectManipulationDevice);
    }

    private static readonly int s_GradientDataItemHash = "GradientDataItem".GetHashCode();

    // Make a single press button. The user clicks them and something happens immediately.
    private static bool GradientDataItem(Rect position, System.Action<Rect, int, bool, bool> drawContent, GUIStyle style)
    {
        int id = GUIUtility.GetControlID(s_GradientDataItemHash, FocusType.Passive, position);
        return GradientDataItem(position, id, drawContent, style);
    }

    private static bool GradientDataItem(Rect position, int id, System.Action<Rect, int, bool, bool> drawContent, GUIStyle style)
    {
        return DoGradientDataItem(position, id, drawContent, style);
    }

    private static bool DoGradientDataItem(Rect position, int id, System.Action<Rect, int, bool, bool> drawContent, GUIStyle style)
    {
        return DoControl(position, id, false, position.Contains(Event.current.mousePosition), drawContent, style);
    }

    private static bool DoControl(Rect position, int id, bool on, bool hover, System.Action<Rect, int, bool, bool> drawContent, GUIStyle style)
    {
        var evt = Event.current;
        switch (evt.type)
        {
            case EventType.Repaint:
                drawContent.Invoke(position, id, on, hover);
                break;
            case EventType.MouseDown:
                if (HitTest(position, evt))
                {
                    Reflection.GUI.GrabMouseControl(id);
                    evt.Use();
                }
                break;
            case EventType.KeyDown:
                bool anyModifiers = (evt.alt || evt.shift || evt.command || evt.control);
                if ((evt.keyCode == KeyCode.Space || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter) && !anyModifiers && GUIUtility.keyboardControl == id)
                {
                    evt.Use();
                    GUI.changed = true;
                    return !on;
                }
                break;
            case EventType.MouseUp:
                if (Reflection.GUI.HasMouseControl(id))
                {
                    Reflection.GUI.ReleaseMouseControl();
                    evt.Use();
                    if (HitTest(position, evt))
                    {
                        GUI.changed = true;
                        return !on;
                    }
                }
                break;
            case EventType.MouseDrag:
                if (Reflection.GUI.HasMouseControl(id))
                    evt.Use();
                break;
        }
        return on;
    }

    Rect GetGUIDataItemColorRect(Rect rect)
    {
        rect.width = 150f;
        var newHeight = rect.height * 0.8f;
        var diff = (rect.height - newHeight) * 0.5f;
        rect.xMin += diff;
        rect.xMax -= diff;
        rect.yMin += diff;
        rect.yMax -= diff;
        return rect;
    }

    Rect GetGUIDataItemTextRect(Rect rect)
    {
        rect.xMin += 150f;
        // space
        rect.xMin += 10f;

        var newHeight = rect.height * 0.8f;
        var diff = (rect.height - newHeight) * 0.5f;
        rect.xMin += diff;
        rect.xMax -= diff;
        rect.yMin += diff;
        rect.yMax -= diff;
        return rect;
    }

    string GUIAlphaValueToString(float alpha)
    {
        var value = Mathf.RoundToInt(alpha * 100f);
        var valueByte = Mathf.RoundToInt(alpha * 255f);
        return string.Format("{0}% ({1})", value, valueByte);
    }

    string GUIColorRGBToString(Color col, SliderMode mode)
    {
        if (mode == SliderMode.RGB)
        {
            var format = "#######0";
            var r = Mathf.RoundToInt(col.r * 255f);
            var g = Mathf.RoundToInt(col.g * 255f);
            var b = Mathf.RoundToInt(col.b * 255f);
            return string.Format("({0}, {1}, {2})", r.ToString(format), g.ToString(format), b.ToString(format));
        }
        else
        {
            var format = "F3";
            return string.Format("({0}, {1}, {2})", col.r.ToString(format), col.g.ToString(format), col.b.ToString(format));
        }
    }


    void ShowSwatchArray<T>(Rect position, List<T> swatches, ref T selected) where T : SwatchBase
    {
        int id = GUIUtility.GetControlID(652347689, FocusType.Passive);
        Event evt = Event.current;

        float mouseSwatchTime = GetTime((Event.current.mousePosition.x - position.x) / position.width);
        Vector2 fixedStepMousePosition = new Vector3(position.x + mouseSwatchTime * position.width, Event.current.mousePosition.y);

        switch (evt.GetTypeForControl(id))
        {
            case EventType.MouseMove:
                {
                    HandleUtility.Repaint();
                }
                break;
            case EventType.Repaint:
                {
                    //if(position.Contains(evt.mousePosition))
                    //{
                    //    DrawGradient(position, Color.white, Color.black);
                    //}

                    bool hasSelection = false;
                    foreach (T s in swatches)
                    {
                        if (selected == s)
                        {
                            hasSelection = true;
                            continue;
                        }
                        DrawSwatch(position, s, false);
                    }
                    // selected swatch drawn last
                    if (selected != null)
                    {
                        var isRemove = !hasSelection;
                        DrawSwatch(position, selected, isRemove);
                    }
                    break;
                }
            case EventType.MouseDown:
                {
                    Rect clickRect = position;

                    // Swatches have some thickness thus we enlarge the clickable area
                    clickRect.xMin -= 10;
                    clickRect.xMax += 10;
                    if (clickRect.Contains(evt.mousePosition))
                    {
                        GUIUtility.hotControl = id;
                        evt.Use();

                        // Make sure selected is topmost for the click
                        if (swatches.Contains(selected) && selected.CanShowColorPicker() && CalcSwatchRect(position, selected).Contains(evt.mousePosition))
                        {
                            if (evt.clickCount == 2)
                            {
                                GUIUtility.keyboardControl = id;
                                selected.ShowColorPicker(m_HDR);
                                GUIUtility.ExitGUI();
                            }
                            break;
                        }

                        bool found = false;
                        foreach (T s in swatches)
                        {
                            if (CalcSwatchRect(position, s).Contains(fixedStepMousePosition))
                            {
                                found = true;
                                selected = s;
                                EditorGUI_EndEditingActiveTextField();
                                break;
                            }
                        }

                        if (!found)
                        {
                            Color currentColor = m_GradientEx.Evaluate(mouseSwatchTime);
                            var newT = SwatchBase.Create<T>(mouseSwatchTime, currentColor);
                            selected = newT;
                            swatches.Add(newT);
                            AssignBack();
                        }
                    }
                    break;
                }
            case EventType.MouseDrag:

                if (GUIUtility.hotControl == id && selected != null)
                {
                    evt.Use();

                    // If user drags swatch outside in vertical direction, we'll remove the swatch
                    if ((evt.mousePosition.y + 5 < position.y || evt.mousePosition.y - 5 > position.yMax))
                    {
                        if (swatches.Count > 1)
                        {
                            swatches.Remove(selected);
                            // do not clear and keep to restore
                            //selected = null;
                            AssignBack();
                            break;
                        }
                    }
                    else if (!swatches.Contains(selected))
                    {
                        swatches.Add(selected);
                    }

                    selected.m_Time = mouseSwatchTime;
                    AssignBack();
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == id)
                {
                    GUIUtility.hotControl = 0;
                    evt.Use();

                    // If the dragged swatch is NOT in the timeline, it means it was dragged outside.
                    // We just forget about it and let GC get it later.
                    if (!swatches.Contains(selected))
                    {
                        selected = null;
                    }

                    // Remove duplicate keys on mouse up so that we do not kill any keys during the drag
                    RemoveDuplicateOverlappingSwatches();
                }
                break;

            case EventType.KeyDown:
                if (evt.keyCode == KeyCode.Delete)
                {
                    if (selected != null)
                    {
                        List<T> listToDeleteFrom = swatches;
                        if (listToDeleteFrom.Count > 1)
                        {
                            bool del = listToDeleteFrom.Remove(selected);
                            if (del)
                            {
                                selected = null;
                            }
                            AssignBack();
                            HandleUtility.Repaint();
                        }
                        evt.Use();
                    }
                }
                break;

            case EventType.ValidateCommand:
                if (evt.commandName == EventCommandNames.Delete)
                {
                    Event.current.Use();
                }
                break;

            case EventType.ExecuteCommand:
                if (evt.commandName == EventCommandNames.ColorPickerChanged)
                {
                    if (selected != null)
                    {
                        GUI.changed = true;
                        selected.SetFromColorPicker(ColorPicker_color);
                        AssignBack();
                        HandleUtility.Repaint();
                    }
                }
                else if (evt.commandName == EventCommandNames.Delete)
                {
                    if (selected != null)
                    {
                        if (swatches.Count > 1)
                        {
                            bool del = swatches.Remove(selected);
                            if (del)
                            {
                                selected = null;
                            }
                            AssignBack();
                            HandleUtility.Repaint();
                        }
                    }
                }
                break;
        }
    }

    void DrawSwatch<T>(Rect totalPos, T s, bool isRemove) where T : SwatchBase
    {
        Color temp = GUI.backgroundColor;
        Rect r = CalcSwatchRect(totalPos, s);
        GUI.backgroundColor = s.GetBackgroundColor();
        GUIStyle back = s.GetSwatchBackgroundStyle();
        GUIStyle overlay = s.GetSwatchOverlayStyle();
        back.Draw(r, false, false, m_SelectedSwatch == s, false);
        GUI.backgroundColor = temp;
        overlay.Draw(r, false, false, m_SelectedSwatch == s, false);

        if (isRemove)
        {
            Rect removeRect = r;
            if (removeRect.height != removeRect.width)
            {
                removeRect.height = removeRect.width;
            }
            // extend
            removeRect.min -= Vector2.one * 3;
            removeRect.max += Vector2.one * 3;
            // move
            removeRect.y = s.IsSwatchDirectionUpwards() ? r.yMax : (r.yMin - r.height);
            //removeRect.x = (r.x + (r.width * 0.5f)) - (removeRect.width * 0.5f);

            if (Styles.removeContent != null)
            {
                //GUI.DrawTexture(removeRect, EditorGUIUtility.whiteTexture, ScaleMode.ScaleToFit);
                GUI.DrawTexture(removeRect, Styles.removeContent.image, ScaleMode.ScaleAndCrop);
            }
            else
            {
                DrawCross(removeRect, new Color(1.0f, 0.5f, 0.5f, 1.0f), Color.black);
            }

        }
    }

    private void DrawCross(Rect totalPos, Color color, Color broderColor)
    {
        var space = Mathf.Round(totalPos.width * 0.15f);
        Handles.BeginGUI();
        var old = Handles.color;
        totalPos.min += Vector2.one * space * 0.5f;
        totalPos.max -= Vector2.one * space * 0.5f;
        Handles.color = broderColor;
        Handles.DrawAAPolyLine(5, new Vector2(totalPos.xMin, totalPos.yMin), new Vector2(totalPos.xMax, totalPos.yMax));
        Handles.DrawAAPolyLine(5, new Vector2(totalPos.xMax, totalPos.yMin), new Vector2(totalPos.xMin, totalPos.yMax));
        totalPos.min += Vector2.one * space;
        totalPos.max -= Vector2.one * space;
        Handles.color = color;
        Handles.DrawAAPolyLine(3, new Vector2(totalPos.xMin, totalPos.yMin), new Vector2(totalPos.xMax, totalPos.yMax));
        Handles.DrawAAPolyLine(3, new Vector2(totalPos.xMax, totalPos.yMin), new Vector2(totalPos.xMin, totalPos.yMax));
        Handles.DrawLine(new Vector2(totalPos.xMin, totalPos.yMin), new Vector2(totalPos.xMax, totalPos.yMax), 1);
        Handles.DrawLine(new Vector2(totalPos.xMax, totalPos.yMin), new Vector2(totalPos.xMin, totalPos.yMax), 1);
        Handles.color = old;
        Handles.EndGUI();
    }

    Rect CalcSwatchRect(Rect totalRect, SwatchBase s)
    {
        float time = s.m_Time;
        return new Rect(totalRect.x + Mathf.Round(totalRect.width * time) - Mathf.Round(Styles.k_SwatchWidht * 0.5f), totalRect.y, Styles.k_SwatchWidht, totalRect.height);
    }

    int SwatchSort(SwatchBase lhs, SwatchBase rhs)
    {
        if (lhs.m_Time == rhs.m_Time && lhs == m_SelectedSwatch)
            return -1;
        if (lhs.m_Time == rhs.m_Time && rhs == m_SelectedSwatch)
            return 1;

        return lhs.m_Time.CompareTo(rhs.m_Time);
    }

    // Assign back all swatches, to target gradient.
    void AssignBack()
    {
        m_RGBSwatches.Sort((a, b) => SwatchSort(a, b));
        GradientEx.ColorKey[] colorKeys = new GradientEx.ColorKey[m_RGBSwatches.Count];
        for (int i = 0; i < m_RGBSwatches.Count; i++)
        {
            colorKeys[i].color = m_RGBSwatches[i].m_Value;
            colorKeys[i].NormalizedTime = m_RGBSwatches[i].m_Time;
        }

        m_AlphaSwatches.Sort((a, b) => SwatchSort(a, b));
        GradientEx.AlphaKey[] alphaKeys = new GradientEx.AlphaKey[m_AlphaSwatches.Count];
        for (int i = 0; i < m_AlphaSwatches.Count; i++)
        {
            alphaKeys[i].alpha = m_AlphaSwatches[i].m_Value; // we use the red channel (see BuildArrays)
            alphaKeys[i].NormalizedTime = m_AlphaSwatches[i].m_Time;
        }

        m_GradientEx.colorKeys = colorKeys;
        m_GradientEx.alphaKeys = alphaKeys;
        m_GradientEx.mode = m_GradientMode;

        GUI.changed = true;
    }

    // Kill any swatches that are at the same time (For example as the result of dragging a swatch on top of another)
    void RemoveDuplicateOverlappingSwatches()
    {
        bool didRemoveAny = false;
        for (int i = 1; i < m_RGBSwatches.Count; i++)
        {
            if (Mathf.Approximately(m_RGBSwatches[i - 1].m_Time, m_RGBSwatches[i].m_Time))
            {
                m_RGBSwatches.RemoveAt(i);
                i--;
                didRemoveAny = true;
            }
        }

        for (int i = 1; i < m_AlphaSwatches.Count; i++)
        {
            if (Mathf.Approximately(m_AlphaSwatches[i - 1].m_Time, m_AlphaSwatches[i].m_Time))
            {
                m_AlphaSwatches.RemoveAt(i);
                i--;
                didRemoveAny = true;
            }
        }

        if (didRemoveAny)
            AssignBack();
    }

    #region GUI

    static System.Type s_typeof_GUIView;
    internal static System.Type typeof_GUIView
    {
        get
        {
            if (s_typeof_GUIView == null)
            {
                s_typeof_GUIView = typeof(EditorWindow).Assembly.GetType("UnityEditor.GUIView", false);
                if (s_typeof_GUIView == null)
                {
                    UnityEngine.Debug.LogError("not found: \"UnityEditor.GUIView\"");
                }
            }
            return s_typeof_GUIView;
        }
    }

    static System.Reflection.PropertyInfo s_GUIView_current;
    // public static extern GUIView current {[NativeMethod("GetCurrentGUIView")] get; }
    internal static object GUIView_current
    {
        get
        {
            if (s_GUIView_current == null)
            {
                var type = typeof_GUIView;
                if (type != null)
                {
                    s_GUIView_current = type.GetProperty("current", type);
                    if (s_GUIView_current == null)
                    {
                        UnityEngine.Debug.LogError("not found: \"public static extern GUIView current\"");
                    }
                }
            }
            if (s_GUIView_current != null)
            {
                return s_GUIView_current.GetValue(null);
            }
            return null;
        }
    }

    static System.Reflection.MethodInfo s_GUIView_SendEvent;
    // internal bool SendEvent(Event e)
    internal static bool GUIView_SendEvent(object GUIViewObj, Event e)
    {
        if (s_GUIView_SendEvent == null)
        {
            var type = typeof_GUIView;
            if (type != null)
            {
                s_GUIView_SendEvent = type.GetMethod("SendEvent",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    new System.Type[] { typeof(Event) }, null);
                if (s_GUIView_SendEvent == null)
                {
                    UnityEngine.Debug.LogError("not found: \"internal bool SendEvent(Event e)\"");
                }
            }
        }
        if (s_GUIView_SendEvent != null)
        {
            return (bool)s_GUIView_SendEvent.Invoke(GUIViewObj, new object[] { e });
        }
        return false;
    }

    static System.Type s_typeof_ColorPicker;
    internal static System.Type typeof_ColorPicker
    {
        get
        {
            if (s_typeof_ColorPicker == null)
            {
                s_typeof_ColorPicker = typeof(EditorWindow).Assembly.GetType("UnityEditor.ColorPicker", false);
                if (s_typeof_ColorPicker == null)
                {
                    UnityEngine.Debug.LogError("not found: \"UnityEditor.ColorPicker\"");
                }
            }
            return s_typeof_ColorPicker;
        }
    }

    static System.Reflection.MethodInfo s_ColorPicker_Show;
    // public static void Show(GUIView viewToUpdate, Color col, bool showAlpha = true, bool hdr = false)
    internal static void ColorPicker_Show(Color col, bool showAlpha = true, bool hdr = false)
    {
        if (s_ColorPicker_Show == null)
        {
            var type = typeof_ColorPicker;
            if (type != null)
            {
                s_ColorPicker_Show = type.GetMethod("Show", new System.Type[] { typeof_GUIView, typeof(Color), typeof(bool), typeof(bool) }, null);
                if (s_ColorPicker_Show == null)
                {
                    UnityEngine.Debug.LogError("not found: \"public static void Show(GUIView viewToUpdate, Color col, bool showAlpha = true, bool hdr = false)\"");
                }
            }
        }
        if (s_ColorPicker_Show != null)
        {
            s_ColorPicker_Show.Invoke(null, new object[] { GUIView_current, col, showAlpha, hdr });
        }
    }

    static System.Reflection.PropertyInfo s_ColorPicker_color;
    // public static Color color
    internal static Color ColorPicker_color
    {
        get
        {
            if (s_ColorPicker_color == null)
            {
                var type = typeof_ColorPicker;
                if (type != null)
                {
                    s_ColorPicker_color = type.GetProperty("color", typeof(Color));
                    if (s_ColorPicker_color == null)
                    {
                        UnityEngine.Debug.LogError("not found: \"public static Color color\"");
                    }
                }
            }
            if (s_ColorPicker_color != null)
            {
                return (Color)s_ColorPicker_color.GetValue(null);
            }
            return Color.white;
        }
    }


    static System.Reflection.MethodInfo s_EditorGUI_EndEditingActiveTextField;
    // internal static void EndEditingActiveTextField()
    internal static void EditorGUI_EndEditingActiveTextField()
    {
        if (s_EditorGUI_EndEditingActiveTextField == null)
        {
            s_EditorGUI_EndEditingActiveTextField = typeof(EditorGUI).GetMethod("EndEditingActiveTextField",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
                null,
                System.Array.Empty<System.Type>(),
                null);
            if (s_EditorGUI_EndEditingActiveTextField == null)
            {
                UnityEngine.Debug.LogError("not found: \"internal static void EndEditingActiveTextField()\"");
            }
        }
        if (s_EditorGUI_EndEditingActiveTextField != null)
        {
            s_EditorGUI_EndEditingActiveTextField.Invoke(null, null);
        }
    }

    // GUI Helpers
    private static Color s_MixedValueContentColorTemp = Color.white;
    internal static void BeginHandleMixedValueContentColor()
    {
        s_MixedValueContentColorTemp = GUI.contentColor;
        GUI.contentColor = EditorGUI.showMixedValue ? (GUI.contentColor * new Color(1, 1, 1, 0.5f)) : GUI.contentColor;
    }

    internal static void EndHandleMixedValueContentColor()
    {
        GUI.contentColor = s_MixedValueContentColorTemp;
    }

    public static void DrawGradientSwatch(Rect position, GradientEx gradient, Color bgColor)
    {
        DrawGradientSwatchInternal(position, gradient, null, bgColor);
    }

    public static void DrawGradientSwatch(Rect position, SerializedProperty property, Color bgColor)
    {
        DrawGradientSwatchInternal(position, null, property, bgColor);
    }

    private static void DrawGradientSwatchInternal(Rect position, GradientEx gradient, SerializedProperty property, Color bgColor)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        if (EditorGUI.showMixedValue)
        {
            Color oldColor = GUI.color;
            float a = GUI.enabled ? 1 : 2;

            GUI.color = new Color(0.82f, 0.82f, 0.82f, a) * bgColor;
            GUIStyle mgs = Styles.whiteTextureStyle;
            mgs.Draw(position, false, false, false, false);

            BeginHandleMixedValueContentColor();
            mgs.Draw(position, Styles.mixedValueContent, false, false, false, false);
            EndHandleMixedValueContentColor();

            GUI.color = oldColor;
            return;
        }

        // Draw Background
        Texture2D backgroundTexture = GetBackgroundTexture();
        if (backgroundTexture != null)
        {
            Color oldColor = GUI.color;
            GUI.color = bgColor;

            GUIStyle backgroundStyle = Styles.GetBasicTextureStyle(backgroundTexture);
            backgroundStyle.Draw(position, false, false, false, false);

            GUI.color = oldColor;
        }

        // DrawTexture
        Texture2D preview = null;
        float maxColorComponent = 0.0f;
        if (property != null)
        {
            preview = GradientExPreviewCache.GetPropertyPreview(property);
            var gradientValue = SerializedPropertyExtensions.GetSerializedValue<GradientEx>(property);
            if (gradientValue != null)
            {
                maxColorComponent = GetMaxColorComponent(gradientValue);
            }
        }
        else
        {
            preview = GradientExPreviewCache.GetGradientPreview(gradient);
            maxColorComponent = GetMaxColorComponent(gradient);
        }

        if (preview == null)
        {
            Debug.Log("Warning: Could not create preview for gradient");
            return;
        }

        GUIStyle gs = Styles.GetBasicTextureStyle(preview);
        gs.Draw(position, false, false, false, false);

        // HDR label
        if (maxColorComponent > 1.0f)
        {
            GUI.Label(new Rect(position.x, position.y - 1, position.width - 3, position.height + 2), "HDR", EditorStyles.centeredGreyMiniLabel);
        }
    }
    private static float GetMaxColorComponent(GradientEx gradient)
    {
        return gradient.maxColorComponent;
    }

    #endregion

    #region Reflection GradientEx

    private static System.Reflection.FieldInfo s_FieldInfo_GradientEx_m_ColorKeys;
    private static List<GradientEx.ColorKey> GetGradientExColorKeyList(GradientEx target)
    {
        if (s_FieldInfo_GradientEx_m_ColorKeys == null)
        {
            s_FieldInfo_GradientEx_m_ColorKeys = typeof(GradientEx).GetField("m_ColorKeys", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }
        if (s_FieldInfo_GradientEx_m_ColorKeys != null)
        {
            return s_FieldInfo_GradientEx_m_ColorKeys.GetValue(target) as List<GradientEx.ColorKey>;
        }
        return null;
    }

    private static System.Reflection.FieldInfo s_FieldInfo_GradientEx_m_AlphaKeys;
    private static List<GradientEx.AlphaKey> GetGradientExAlphaKeyList(GradientEx target)
    {
        if (s_FieldInfo_GradientEx_m_AlphaKeys == null)
        {
            s_FieldInfo_GradientEx_m_AlphaKeys = typeof(GradientEx).GetField("m_AlphaKeys", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }
        if (s_FieldInfo_GradientEx_m_AlphaKeys != null)
        {
            return s_FieldInfo_GradientEx_m_AlphaKeys.GetValue(target) as List<GradientEx.AlphaKey>;
        }
        return null;
    }

    #endregion

    // From: https://forum.unity.com/threads/get-a-general-object-value-from-serializedproperty.327098/
    internal static class SerializedPropertyExtensions
    {
        const string k_ArrayData = @"\.Array\.data\[([0-9]+)\]";


        static System.Type s_typeof_ScriptAttributeUtility;
        // internal class ScriptAttributeUtility
        internal static System.Type typeof_ScriptAttributeUtility
        {
            get
            {
                if (s_typeof_ScriptAttributeUtility == null)
                {
                    s_typeof_ScriptAttributeUtility = typeof(PropertyDrawer).Assembly.GetType("UnityEditor.ScriptAttributeUtility", false);
                    if (s_typeof_ScriptAttributeUtility == null)
                    {
                        UnityEngine.Debug.LogError("not found: \"UnityEditor.ScriptAttributeUtility\"");
                    }
                }
                return s_typeof_ScriptAttributeUtility;
            }
        }

        static System.Reflection.MethodInfo s_ScriptAttributeUtility_GetFieldInfoFromProperty;
        // internal static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
        internal static System.Reflection.FieldInfo ScriptAttributeUtility_GetFieldInfoFromProperty(SerializedProperty property, out System.Type out_type)
        {
            out_type = null;
            if (s_ScriptAttributeUtility_GetFieldInfoFromProperty == null)
            {
                var type = s_typeof_ScriptAttributeUtility;
                if (type != null)
                {
                    s_ScriptAttributeUtility_GetFieldInfoFromProperty = type.GetMethod("GetFieldInfoFromProperty",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
                        null,
                        new System.Type[] { typeof(SerializedProperty), typeof(System.Type) },
                        null);
                    if (s_ScriptAttributeUtility_GetFieldInfoFromProperty == null)
                    {
                        UnityEngine.Debug.LogError("not found: \"internal static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)\"");
                    }
                }
            }
            if (s_ScriptAttributeUtility_GetFieldInfoFromProperty != null)
            {
                return (System.Reflection.FieldInfo)s_ScriptAttributeUtility_GetFieldInfoFromProperty.Invoke(null, new object[] { property, out_type });
            }
            return null;
        }

        static bool IsArrayOrList(System.Type listType)
        {
            if (listType.IsArray)
            {
                return true;
            }
            else if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
            {
                return true;
            }
            return false;
        }

        static System.Type GetArrayOrListElementType(System.Type listType)
        {
            if (listType.IsArray)
            {
                return listType.GetElementType();
            }
            else if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
            {
                return listType.GetGenericArguments()[0];
            }
            return null;
        }

        public static T GetSerializedValue<T>(SerializedProperty property)
        {
            var tValue = default(T);
            if (property == null)
            {
                return tValue;
            }

            var propertyPath = property.propertyPath;
            // we are looking for array element only when the path ends with Array.data[x]
            //var lookingForArrayElement = System.Text.RegularExpressions.Regex.IsMatch(propertyPath, k_ArrayData + "$");
            // remove any Array.data[x] from the path because it is prevents cache searching.
            propertyPath = System.Text.RegularExpressions.Regex.Replace(propertyPath, k_ArrayData, ".___ArrayElement___.$1");


            object @object = property.serializedObject.targetObject;
            var type = @object.GetType();
            string[] parts = propertyPath.Split('.');
            // Get the last object of the property path.
            for (int i = 0; i < parts.Length; i++)
            {
                string member = parts[i];
                // GetField on class A will not find private fields in base classes to A,
                // so we have to iterate through the base classes and look there too.
                // Private fields are relevant because they can still be shown in the Inspector,
                // and that applies to private fields in base classes too.
                System.Reflection.FieldInfo foundField = null;
                for (System.Type currentType = type; foundField == null && currentType != null; currentType = currentType.BaseType)
                    foundField = currentType.GetField(member, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                if (foundField == null)
                {
                    return tValue;
                }
                type = foundField.FieldType;
                @object = foundField.GetValue(@object);
                if (@object == null)
                {
                    return tValue;
                }
                // we want to get the element type if we are looking for Array.data[x]
                if (i < parts.Length - 1 && parts[i + 1] == "___ArrayElement___" && IsArrayOrList(type))
                {
                    var arrayObject = @object as IList<T>;
                    if (arrayObject == null)
                    {
                        return tValue;
                    }
                    type = GetArrayOrListElementType(type);
                    if (type == null)
                    {
                        return tValue;
                    }
                    i += 2; // skip the "___ArrayElement___" part

                    if (i <= parts.Length - 1)
                    {
                        var propertyIndex = int.Parse(parts[i]);
                        if (propertyIndex < 0 || propertyIndex >= arrayObject.Count)
                        {
                            return tValue;
                        }
                        @object = arrayObject[propertyIndex];
                    }
                }
            }

            if (@object == null)
            {
                return tValue;
            }
            return (T)@object;
        }


        public static bool SetSerializedValue<T>(SerializedProperty property, T val)
        {
            if (property == null)
            {
                return false;
            }

            var propertyPath = property.propertyPath;
            // we are looking for array element only when the path ends with Array.data[x]
            //var lookingForArrayElement = System.Text.RegularExpressions.Regex.IsMatch(propertyPath, k_ArrayData + "$");
            // remove any Array.data[x] from the path because it is prevents cache searching.
            propertyPath = System.Text.RegularExpressions.Regex.Replace(propertyPath, k_ArrayData, ".___ArrayElement___.$1");

            object @object = property.serializedObject.targetObject;
            var type = @object.GetType();
            string[] parts = propertyPath.Split('.');
            // Get the last object of the property path.
            for (int i = 0; i < parts.Length; i++)
            {
                string member = parts[i];
                // GetField on class A will not find private fields in base classes to A,
                // so we have to iterate through the base classes and look there too.
                // Private fields are relevant because they can still be shown in the Inspector,
                // and that applies to private fields in base classes too.
                System.Reflection.FieldInfo foundField = null;
                for (System.Type currentType = type; foundField == null && currentType != null; currentType = currentType.BaseType)
                    foundField = currentType.GetField(member, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                if (foundField == null)
                {
                    return false;
                }
                type = foundField.FieldType;
                if (i == parts.Length - 1)
                {
                    // 最后一个
                    foundField.SetValue(@object, val);
                    return true;
                }
                if ((i == parts.Length - 3) && (parts[i + 1] == "___ArrayElement___") && IsArrayOrList(type))
                {
                    // 最后一个
                    var arrayObject = @object as IList<T>;
                    if (arrayObject == null)
                    {
                        return false;
                    }
                    var propertyIndex = int.Parse(parts[i + 2]);
                    if (propertyIndex < 0 || propertyIndex >= arrayObject.Count)
                    {
                        return false;
                    }
                    arrayObject[propertyIndex] = val;
                    return true;
                }

                @object = foundField.GetValue(@object);
                if (@object == null)
                {
                    return false;
                }
                // we want to get the element type if we are looking for Array.data[x]
                if (i < parts.Length - 1 && parts[i + 1] == "___ArrayElement___" && IsArrayOrList(type))
                {
                    var arrayObject = @object as IList<T>;
                    if (arrayObject == null)
                    {
                        return false;
                    }
                    type = GetArrayOrListElementType(type);
                    if (type == null)
                    {
                        return false;
                    }
                    i += 2; // skip the "___ArrayElement___" part

                    if (i < parts.Length - 1)
                    {
                        var propertyIndex = int.Parse(parts[i]);
                        if (propertyIndex < 0 || propertyIndex >= arrayObject.Count)
                        {
                            return false;
                        }
                        @object = arrayObject[propertyIndex];
                        i++; // skip the array index part
                    }
                }
            }
            return false;
        }

    }

}
