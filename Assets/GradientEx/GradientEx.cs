using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class GradientEx
{
    #region Serializable Classes

    public enum GradientMode
    {
        Blend = 0,  // Keys will blend smoothly when the gradient is evaluated. (Default)
        Fixed = 1   // An exact key color will be returned when the gradient is evaluated.
    }

    [System.Serializable]
    public struct ColorKey
    {
        public Color color;
        public System.UInt16 time;

        public float NormalizedTime
        {
            set
            {
                time = NormalizedToUInt16(value);
            }
            get
            {
                return UInt16ToNormalized(time);
            }
        }

        public ColorKey(in Color c, System.UInt16 t)
        {
            color = c;
            time = t;
        }
        public ColorKey(in GradientColorKey ck)
        {
            color = ck.color;
            time = NormalizedToUInt16(ck.time);
        }

        public static GradientColorKey[] GetKeys(ColorKey[] serialized)
        {
            GradientColorKey[] result = new GradientColorKey[serialized.Length];
            for (int i = 0; i < serialized.Length; ++i)
            {
                result[i] = new GradientColorKey(serialized[i].color, UInt16ToNormalized(serialized[i].time));
            }
            return result;
        }
    }
    [System.Serializable]
    public struct AlphaKey
    {
        public float alpha;
        public ushort time;

        public float NormalizedTime
        {
            set
            {
                time = NormalizedToUInt16(value);
            }
            get
            {
                return UInt16ToNormalized(time);
            }
        }

        public AlphaKey(float a, ushort t)
        {
            alpha = a;
            time = t;
        }
        public AlphaKey(in GradientAlphaKey ak)
        {
            alpha = NormalizedToByte(ak.alpha);
            time = NormalizedToUInt16(ak.time);
        }

        public static GradientAlphaKey[] GetKeys(AlphaKey[] serialized)
        {
            GradientAlphaKey[] result = new GradientAlphaKey[serialized.Length];
            for (int i = 0; i < serialized.Length; ++i)
            {
                result[i] = new GradientAlphaKey(serialized[i].alpha, UInt16ToNormalized(serialized[i].time));
            }
            return result;
        }
    }
    #endregion

    [SerializeField]
    List<ColorKey> m_ColorKeys;
    [SerializeField]
    List<AlphaKey> m_AlphaKeys;
    [SerializeField]
    GradientMode m_Mode;

    public GradientMode mode 
    {
        get { return m_Mode; }
        set { m_Mode = value; }
    }

    public GradientEx()
    {

    }

    public GradientEx(GradientEx other)
    {
        SetKeys(other.m_ColorKeys, other.m_AlphaKeys);
        m_Mode = other.m_Mode;
    }

    public GradientEx Clone()
    {
        return new GradientEx(this);
    }

    public void CopyTo(GradientEx other)
    {
        other.SetKeys(m_ColorKeys, m_AlphaKeys);
        other.m_Mode = m_Mode;
    }


    public int ColorKeyCount
    {
        get { return m_ColorKeys != null ? m_ColorKeys.Count : 0; }
    }
    public int AlphaKeyCount
    {
        get { return m_AlphaKeys != null ? m_AlphaKeys.Count : 0; }
    }
    public ColorKey[] colorKeys
    {
        get
        {
            return m_ColorKeys != null ? m_ColorKeys.ToArray() : System.Array.Empty<ColorKey>();
        }
        set
        {
            SetColorKeys(value);
        }
    }
    public AlphaKey[] alphaKeys
    {
        get
        {
            return m_AlphaKeys != null ? m_AlphaKeys.ToArray() : System.Array.Empty<AlphaKey>();
        }
        set
        {
            SetAlphaKeys(value);
        }
    }

    public float maxColorComponent
    {
        get
        {
            float maxColorComponent = 0.0f;
            var keyCount = m_ColorKeys != null ? m_ColorKeys.Count : 0; ;
            for (int i = 0; i < keyCount; ++i)
            {
                maxColorComponent = Mathf.Max(maxColorComponent, m_ColorKeys[i].color.maxColorComponent);
            }
            return maxColorComponent;
        }
    }

    public Color Evaluate(float normalizedTime)
    {
        UnityEngine.Assertions.Assert.IsTrue((normalizedTime >= 0.0f) && (normalizedTime <= 1.0f));
        var time = NormalizedToUInt16(normalizedTime);
        return Evaluate(time);
    }
    public Color Evaluate(System.UInt16 time)
    {
        Color color = Color.white;

        // Color blend
        var numColorKeys = m_ColorKeys != null ? m_ColorKeys.Count : 0;
        if (numColorKeys > 1)
        {
            var timeColor = System.Math.Min(System.Math.Max(m_ColorKeys[0].time, time), m_ColorKeys[numColorKeys - 1].time);
            for (int i = 1; i < numColorKeys; ++i)
            {
                var currTime = m_ColorKeys[i].time;
                if (timeColor <= currTime)
                {
                    switch (m_Mode)
                    {
                        case GradientMode.Blend:
                            {
                                var prevTime = m_ColorKeys[i - 1].time;
                                color = Color.LerpUnclamped(m_ColorKeys[i - 1].color, m_ColorKeys[i].color, LerpWord(prevTime, currTime, timeColor));
                            }
                            break;
                        case GradientMode.Fixed:
                            {
                                color = m_ColorKeys[i].color;
                            }
                            break;
                    }
                    break;
                }
            }
        }
        else if(numColorKeys > 0)
        {
            color = m_ColorKeys[0].color;
        }

        // Alpha blend
        var numAlphaKeys = m_AlphaKeys != null ? m_AlphaKeys.Count : 0;
        if (numAlphaKeys > 1)
        {
            var timeAlpha = System.Math.Min(System.Math.Max(m_AlphaKeys[0].time, time), m_AlphaKeys[numAlphaKeys - 1].time);
            for (int i = 1; i < numAlphaKeys; ++i)
            {
                var currTime = m_AlphaKeys[i].time;
                if (timeAlpha <= currTime)
                {
                    switch(m_Mode)
                    {
                        case GradientMode.Blend:
                            {
                                var prevTime = m_AlphaKeys[i - 1].time;
                                color.a = Mathf.Lerp(m_AlphaKeys[i - 1].alpha, m_AlphaKeys[i].alpha, LerpWord(prevTime, currTime, timeAlpha));
                            }
                            break;
                        case GradientMode.Fixed:
                            {
                                color.a = m_AlphaKeys[i].alpha;
                            }
                            break;
                    }
                    
                    break;
                }
            }
        }
        else if (numAlphaKeys > 0)
        {
            color.a = m_AlphaKeys[0].alpha;
        }

        return color;
    }

    void SwapColorKeys(int i, int j)
    {
        var tmp = m_ColorKeys[i];
        m_ColorKeys[i] = m_ColorKeys[j];
        m_ColorKeys[j] = tmp;
    }

    void SwapAlphaKeys(int i, int j)
    {
        var tmp = m_AlphaKeys[i];
        m_AlphaKeys[i] = m_AlphaKeys[j];
        m_AlphaKeys[j] = tmp;
    }

    public void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys)
    {
        SetColorKeys(colorKeys);
        SetAlphaKeys(alphaKeys);
    }

    public void SetKeys(IList<ColorKey> colorKeys, IList<AlphaKey> alphaKeys)
    {
        SetColorKeys(colorKeys);
        SetAlphaKeys(alphaKeys);
    }

    void SetColorKeys(GradientColorKey[] colorKeys)
    {
        var numKeys = colorKeys != null ? colorKeys.Length : 0;
        if (numKeys > 0)
        {
            if(m_ColorKeys == null)
            {
                m_ColorKeys = new List<ColorKey>(numKeys);
            }
            m_ColorKeys.Clear();
            for (int i = 0; i < numKeys; ++i)
            {
                m_ColorKeys.Add(new ColorKey(colorKeys[i]));
            }
        }
        else
        {
            if (m_ColorKeys != null)
            {
                m_ColorKeys.Clear();
            }
        }

        // Ensure sorted!
        {
            int i = 0;
            var keyCount = m_ColorKeys != null ? m_ColorKeys.Count : 0;
            while ((i + 1) < keyCount)
            {
                if (m_ColorKeys[i].time > m_ColorKeys[i + 1].time)
                {
                    SwapColorKeys(i, i + 1);
                    if (i > 0)
                    {
                        i -= 2;
                    }
                }
                i++;
            }
        }
    }
    void SetColorKeys(IList<ColorKey> colorKeys)
    {
        var numKeys = colorKeys != null ? colorKeys.Count : 0;
        if (numKeys > 0)
        {
            if (m_ColorKeys == null)
            {
                m_ColorKeys = new List<ColorKey>(numKeys);
            }
            m_ColorKeys.Clear();
            for (int i = 0; i < numKeys; ++i)
            {
                m_ColorKeys.Add(colorKeys[i]);
            }
        }
        else
        {
            if (m_ColorKeys != null)
            {
                m_ColorKeys.Clear();
            }
        }

        // Ensure sorted!
        {
            int i = 0;
            var keyCount = m_ColorKeys != null ? m_ColorKeys.Count : 0;
            while ((i + 1) < keyCount)
            {
                if (m_ColorKeys[i].time > m_ColorKeys[i + 1].time)
                {
                    SwapColorKeys(i, i + 1);
                    if (i > 0)
                    {
                        i -= 2;
                    }
                }
                i++;
            }
        }
    }

    void SetAlphaKeys(GradientAlphaKey[] alphaKeys)
    {
        var numKeys = alphaKeys != null ? alphaKeys.Length : 0;
        if (numKeys > 0)
        {
            if (m_AlphaKeys == null)
            {
                m_AlphaKeys = new List<AlphaKey>(numKeys);
            }
            m_AlphaKeys.Clear();
            for (int i = 0; i < numKeys; ++i)
            {
                m_AlphaKeys.Add(new AlphaKey(alphaKeys[i]));
            }
        }
        else
        {
            if (m_AlphaKeys != null)
            {
                m_AlphaKeys.Clear();
            }
        }

        // Ensure sorted!
        {
            int i = 0;
            var keyCount = m_AlphaKeys != null ? m_AlphaKeys.Count : 0; ;
            while ((i + 1) < keyCount)
            {
                if (m_AlphaKeys[i].time > m_AlphaKeys[i + 1].time)
                {
                    SwapAlphaKeys(i, i + 1);
                    if (i > 0)
                    {
                        i -= 2;
                    }
                }
                i++;
            }
        }
    }

    void SetAlphaKeys(IList<AlphaKey> alphaKeys)
    {
        var numKeys = alphaKeys != null ? alphaKeys.Count : 0;
        if (numKeys > 0)
        {
            if (m_AlphaKeys == null)
            {
                m_AlphaKeys = new List<AlphaKey>(numKeys);
            }
            m_AlphaKeys.Clear();
            for (int i = 0; i < numKeys; ++i)
            {
                m_AlphaKeys.Add(alphaKeys[i]);
            }
        }
        else
        {
            if (m_AlphaKeys != null)
            {
                m_AlphaKeys.Clear();
            }
        }

        // Ensure sorted!
        {
            int i = 0;
            var keyCount = m_AlphaKeys != null ? m_AlphaKeys.Count : 0; ;
            while ((i + 1) < keyCount)
            {
                if (m_AlphaKeys[i].time > m_AlphaKeys[i + 1].time)
                {
                    SwapAlphaKeys(i, i + 1);
                    if (i > 0)
                    {
                        i -= 2;
                    }
                }
                i++;
            }
        }
    }

    public Color GetConstantColor()
    {
        return Evaluate(0.0f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float LerpWord(System.UInt16 from, System.UInt16 to, System.UInt16 v)
    {
        if(from == to)
        {
            return 0.0f;
        }
        if(to == v)
        {
            return 1.0f;
        }
        return (v - from) / (float)(to - from);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static byte InverseLerpWord(System.UInt32 from, System.UInt32 to, System.UInt32 v)
    {
        UnityEngine.Assertions.Assert.IsTrue((from & 0xffff) == from);
        UnityEngine.Assertions.Assert.IsTrue((to & 0xffff) == to);
        UnityEngine.Assertions.Assert.IsTrue((v & 0xffff) == v);
        UnityEngine.Assertions.Assert.IsTrue(from <= to);
        unchecked
        {
            System.UInt32 nom = (v - from) << 16;
            System.UInt32 den = System.Math.Max(to - from, 1);
            System.UInt32 res = nom / den;
            return (byte)(res >> 8);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static byte LerpByte(System.UInt32 u0, System.UInt32 u1, System.UInt32 scale)
    {
        UnityEngine.Assertions.Assert.IsTrue((u0 & 0xff) == u0);
        UnityEngine.Assertions.Assert.IsTrue((u1 & 0xff) == u1);
        //DebugAssert((scale & 0xff) == scale);
        unchecked
        {
            return (byte)(u0 + (((u1 - u0) * scale) >> 8) & 0xff);
        }
    }


    ///  Fast conversion of float [0...1] to 0 ... 65535
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static System.UInt16 NormalizedToUInt16(float f)
    {
        unchecked
        {
            f = Mathf.Max(f, 0.0F);
            f = Mathf.Min(f, 1.0F);
            return (System.UInt16)(f * 65535.0f);
        }
    }

    ///  Fast conversion of float [0...1] to 0 ... 65535
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float UInt16ToNormalized(int p)
    {
        UnityEngine.Assertions.Assert.IsFalse(p < 0 || p > 65535);
        unchecked
        {
            return (float)p / 65535.0F;
        }
    }

    ///  Fast conversion of float [0...1] to 0 ... 255
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static byte NormalizedToByte(float f)
    {
        f = Mathf.Max(f, 0.0F);
        f = Mathf.Min(f, 1.0F);
        unchecked
        {
            return (byte)(f * 255.0f);
        }
    }

    ///  Fast conversion of float [0...1] to 0 ... 255
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float ByteToNormalized(int p)
    {
        UnityEngine.Assertions.Assert.IsFalse(p < 0 || p > 255);
        unchecked
        {
            return (float)p / 255.0F;
        }
    }


}
