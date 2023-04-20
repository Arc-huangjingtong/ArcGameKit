using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaiPackage
{
    /// <summary>
    /// 此工具组方便了在编辑新UI编辑系统而设计
    /// 参考文档如下
    /// https://docs.unity3d.com/2021.2/Documentation/ScriptReference/UIElements.IStyle.html
    /// </summary>
    public static class StyleTool
    {
        /// <summary>
        /// 设置倒角
        /// </summary>
        /// <param name="style"></param>
        /// <param name="upperLeft">左上的半径</param>
        /// <param name="upperRight">右上的半径</param>
        /// <param name="lowerLeft">左下的半径</param>
        /// <param name="lowerRight">右下的半径</param>
        public static void SetRadius(this IStyle style, float upperLeft = 0, float upperRight = 0, float lowerLeft = 0, float lowerRight = 0)
        {
            style.borderTopLeftRadius = upperLeft;
            style.borderTopRightRadius = upperRight;
            style.borderBottomLeftRadius = lowerLeft;
            style.borderBottomRightRadius = lowerRight;
        }
        /// <summary>
        /// 设置统一倒角
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value">在元素的框中绘制圆角矩形时的半径</param>
        public static void SetRadiusAll(this IStyle style, float value)
        {
            style.borderBottomRightRadius = style.borderBottomLeftRadius =
                style.borderTopRightRadius = style.borderTopLeftRadius = value;
        }
        /// <summary>
        /// 设置边框颜色
        /// </summary>
        /// <param name="style"></param>
        /// <param name="color"></param>
        public static void SetBorderColorAll(this IStyle style, Color color)
        {
            style.borderRightColor = style.borderLeftColor = style.borderBottomColor = style.borderTopColor = color;
        }
        /// <summary>
        /// 设置统一边框宽度
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value"></param>
        public static void SetBorderWidthAll(this IStyle style, float value)
        {
            style.borderRightWidth = style.borderLeftWidth = style.borderBottomWidth = style.borderTopWidth = value;
        }
        /// <summary>
        /// 设置边框宽度
        /// </summary>
        /// <param name="style"></param>
        /// <param name="top">顶部</param>
        /// <param name="bottom">底部</param>
        /// <param name="left">左边</param>
        /// <param name="right">右边</param>
        public static void SetBorderWidth(this IStyle style, float top = 0, float bottom = 0, float left = 0, float right = 0)
        {
            style.borderTopWidth = top;
            style.borderBottomWidth = bottom;
            style.borderLeftWidth = left;
            style.borderRightWidth = right;
        }
        /// <summary>
        /// 元素的最大高度和宽度，当它是灵活的或测量其自身的大小时
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value"></param>
        public static void SetMaxHeightAndWidth(this IStyle style, float value)
        {
            style.maxHeight = style.maxWidth = value;
        }
        /// <summary>
        /// 元素的最小高度和宽度，当它是灵活的或测量其自身的大小时
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value"></param>
        public static void SetMinHeightAndWidth(this IStyle style, float value)
        {
            style.minHeight = style.minWidth = value;
        }
        /// <summary>
        /// 设置在布局阶段为边距保留的空间
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value"></param>
        public static void SetMarginAll(this IStyle style, float value)
        {
            style.marginRight = style.marginLeft = style.marginBottom = style.marginTop = value;
        }
        /// <summary>
        /// 设置在布局阶段为边距保留的空间
        /// </summary>
        /// <param name="style"></param>
        /// <param name="top">顶部</param>
        /// <param name="bottom">底部</param>
        /// <param name="left">左边</param>
        /// <param name="right">右边</param>
        public static void SetMargin(this IStyle style, float top = 0, float bottom = 0, float left = 0, float right = 0)
        {
            style.marginTop = top;
            style.marginBottom = bottom;
            style.marginLeft = left;
            style.marginRight = right;
        }
        /// <summary>
        /// 设置在布局阶段为填充边缘保留的空间
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value"></param>
        public static void SetPaddingAll(this IStyle style, float value)
        {
            style.paddingRight = style.paddingLeft = style.paddingBottom = style.paddingTop = value;
        }
        /// <summary>
        /// 设置在布局阶段为填充边缘保留的空间
        /// </summary>
        /// <param name="style"></param>
        /// <param name="top">顶部</param>
        /// <param name="bottom">底部</param>
        /// <param name="left">左边</param>
        /// <param name="right">右边</param>
        public static void SetPadding(this IStyle style, float top = 0, float bottom = 0, float left = 0, float right = 0)
        {
            style.paddingTop = top;
            style.paddingBottom = bottom;
            style.paddingLeft = left;
            style.paddingRight = right;
        }
        /// <summary>
        /// 设置在绘制元素的背景图像时 9 切片的大小。
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value"></param>
        public static void SetSliceAll(this IStyle style, int value)
        {
            var t = style.unitySliceTop;
            var b = style.unitySliceBottom;
            var l = style.unitySliceLeft;
            var r = style.unitySliceRight;
            t.value = b.value = l.value = r.value = value;
            style.unitySliceTop = t;
            style.unitySliceBottom = b;
            style.unitySliceLeft = l;
            style.unitySliceRight = r;
        }
        /// <summary>
        /// 设置在绘制元素的背景图像时 9 切片的大小。
        /// </summary>
        /// <param name="style"></param>
        /// <param name="top">顶部</param>
        /// <param name="bottom">底部</param>
        /// <param name="left">左边</param>
        /// <param name="right">右边</param>
        public static void SetSlice(this IStyle style, int top = 0, int bottom = 0, int left = 0, int right = 0)
        {
            var t = style.unitySliceTop;
            var b = style.unitySliceBottom;
            var l = style.unitySliceLeft;
            var r = style.unitySliceRight;
            t.value = top;
            b.value = bottom;
            l.value = left;
            r.value = right;
            style.unitySliceTop = t;
            style.unitySliceBottom = b;
            style.unitySliceLeft = l;
            style.unitySliceRight = r;
        }
        /// <summary>
        /// 设置应用锚点
        /// </summary>
        /// <param name="style"></param>
        /// <param name="valueX"></param>
        /// <param name="valueY"></param>
        public static void SetTransformOrigin(this IStyle style, float valueX = 50, float valueY = 50)
        {
            var t = style.transformOrigin.value;
            t.x = valueX;
            t.y = valueY;
            style.transformOrigin = t;
        }
        /// <summary>
        /// 设置应用锚点
        /// </summary>
        /// <param name="style"></param>
        /// <param name="direction"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void SetTransformOrigin(this IStyle style, Direction direction)
        {
            switch (direction)
            {
                case Direction.UpperLeft:
                    SetTransformOrigin(style, 0, 0);
                    break;
                case Direction.Upper:
                    SetTransformOrigin(style, 50, 0);
                    break;
                case Direction.UpperRight:
                    SetTransformOrigin(style, 100, 0);
                    break;
                case Direction.Left:
                    SetTransformOrigin(style, 0, 50);
                    break;
                case Direction.Middle:
                    SetTransformOrigin(style, 50, 50);
                    break;
                case Direction.Right:
                    SetTransformOrigin(style, 100, 50);
                    break;
                case Direction.LowerLeft:
                    SetTransformOrigin(style, 0, 100);
                    break;
                case Direction.Lower:
                    SetTransformOrigin(style, 50, 100);
                    break;
                case Direction.LowerRight:
                    SetTransformOrigin(style, 100, 100);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}