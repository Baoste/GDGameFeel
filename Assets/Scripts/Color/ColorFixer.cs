using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFixer
{
    /// <summary>
    /// 将两种颜色进行模拟混合
    /// </summary>
    /// <param name="color1">颜色1</param>
    /// <param name="color2">颜色2</param>
    /// <returns>混合后的颜色</returns>
    public Color Fix(Color color1, Color color2, float t)
    {
        Color fixedColor = ToColor(Mixbox.Lerp(ToArgbVal(color1), ToArgbVal(color2), t));
        return fixedColor;
        
    }
    /// <summary>
    /// 将Unity的Color值转为32位int值
    /// </summary>
    /// <param name="color">传入的Color值</param>
    /// <returns>Color对应的int值，其中高8位为alpha，接下来每8位储存一种RGB值</returns>
    private int ToArgbVal(Color color)
    {
        return (255 << 24) | (ToIntColor(color.r) << 16) | (ToIntColor(color.g) << 8) | ToIntColor(color.b);
    }
    /// <summary>
    /// 将32位int值转为Unity的Color值
    /// </summary>
    /// <param name="argb">传入的int值</param>
    /// <returns>Unity的Color值</returns>
    private Color ToColor(int argb)
    {
        return new Color(((argb >> 16) & 0xFF) / 255f, ((argb >> 8) & 0xFF) / 255f, (argb & 0xFF) / 255f);
    }
    /// <summary>
    /// 将(0,1)的float值转为(0,255)的int值
    /// </summary>
    /// <param name="x">float</param>
    /// <returns>int</returns>
    private int ToIntColor(float x)
    {
        return (int)(x * 255f);
    }
}
