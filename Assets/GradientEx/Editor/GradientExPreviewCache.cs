using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

internal class GradientExPreviewCache
{
    struct CacheEntry : IEqualityComparer<CacheEntry>, System.IEquatable<CacheEntry>
    {
        public object target;
        public string propertyName;


        public CacheEntry(object obj, string propertyName)
        {
            target = obj;
            this.propertyName = propertyName;
        }

        public CacheEntry(GradientEx gradient)
        {
            target = gradient;
            propertyName = string.Empty;
        }

        public bool Equals(CacheEntry other)
        {
            return Equals(this, other);
        }

        public bool Equals(CacheEntry x, CacheEntry y)
        {
            return object.Equals(x.target, y.target) && string.Equals(x.propertyName, y.propertyName, System.StringComparison.Ordinal);
        }

        public int GetHashCode(CacheEntry obj)
        {
            return target.GetHashCode() ^ propertyName.GetHashCode();
        }
    }

    private readonly Dictionary<CacheEntry, Texture2D> m_Entries = new Dictionary<CacheEntry, Texture2D>();

    ~GradientExPreviewCache()
    {
        InernalClearCache();
    }

    Texture2D LookupCache(GradientEx gradient)
    {
        var entry = new CacheEntry(gradient);
        Texture2D tex;
        m_Entries.TryGetValue(entry, out tex);
        return tex;
    }
    Texture2D LookupCache(Object obj, string propertyName)
    {
        var entry = new CacheEntry(obj, propertyName);
        Texture2D tex;
        m_Entries.TryGetValue(entry, out tex);
        return tex;
    }

    void InernalClearCache()
    {
        var iter = m_Entries.GetEnumerator();
        while (iter.MoveNext())
        {
            var texture = iter.Current.Value;
            if (texture != null)
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        m_Entries.Clear();
    }

    Texture2D GetPreview(GradientEx gradient)
    {
        return GetPreviewInternal(null, null, gradient);
    }

    Texture2D GetPreview(SerializedProperty property)
    {
        string propertyPath = property.propertyPath;
        var target = property.serializedObject.targetObject;

        Texture2D tex = LookupCache(target, propertyPath);
        if (tex != null)
        {
            return tex;
        }

        GradientEx gradient = GradientExEditor.SerializedPropertyExtensions.GetSerializedValue<GradientEx>(property);
        if (gradient == null)
        {
            return null;
        }

        tex = GetPreviewInternal(target, propertyPath, gradient);

        return tex;
    }

    Texture2D GetPreviewInternal(Object obj, string propertyName, GradientEx gradient)
    {
        if (gradient == null)
        {
            return null;
        }

        // Find cached preview texture
        Texture2D preview = null;
        if (obj != null)
        {
            preview = LookupCache(obj, propertyName);
        }
        else
        {
            preview = LookupCache(gradient);
        }

        if (preview != null)
        {
            return preview;
        }

        // Clear cache whenever we have too many previews. It's very unlikely there are 50 preview textures on screen.
        if (m_Entries.Count > 50)
        {
            ClearCache();
        }

        // Generate preview			
        preview = GeneratePreviewTexture(gradient);
        if (preview == null)
        {
            return null;
        }

        // Inject preview into cache
        if (obj != null)
        {
            var entry = new CacheEntry(obj, propertyName);
            m_Entries[entry] = preview;
        }
        else if (gradient != null)
        {
            var entry = new CacheEntry(gradient);
            m_Entries[entry] = preview;
        }

        return preview;
    }

    static GradientExPreviewCache g_GradientExPreviewCache = null;
    static GradientExPreviewCache Get()
    {
        if (g_GradientExPreviewCache == null)
        {
            g_GradientExPreviewCache = new GradientExPreviewCache();
        }
        return g_GradientExPreviewCache;
    }
    public static Texture2D GenerateGradientPreview(GradientEx gradient, Texture2D existingTexture)
    {
        return null;
    }

    public static void ClearCache()
    {
        Get().InernalClearCache();
    }

    public static Texture2D GetPropertyPreview(SerializedProperty property)
    {
        return Get().GetPreview(property);
    }

    public static Texture2D GetGradientPreview(GradientEx curve)
    {
        return Get().GetPreview(curve);
    }


    Texture2D GeneratePreviewTexture(GradientEx gradient)
    {
        // Fixed size previews
        const int width = 256;
        const int height = 2;

        Texture2D texture = new Texture2D(width, height, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        var pixel = texture.GetPixelData<Color32>(0);
        for (int x = 0; x < width; ++x)
        {
            Color32 color = gradient.Evaluate(x / (float)width);
            pixel[x] = color;
            pixel[x + width] = color;
        }
        texture.Apply(false);
        return texture;
    }
}