using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace HaiPackage.Tool
{
    public static class GTool
    {
        /// <summary>
        /// Vector2 转 Vector2Int 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="k">转换方式</param>
        /// <returns></returns>
        public static Vector2Int ToVector2Int(this Vector2 value, GetValue k = GetValue.Round)
        {
            var v2Int = Vector2Int.zero;
            switch (k)
            {
                case GetValue.Round:
                    v2Int = new Vector2Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));
                    break;
                case GetValue.Ceil:
                    v2Int = new Vector2Int(Mathf.CeilToInt(value.x), Mathf.CeilToInt(value.y));
                    break;
                case GetValue.Floor:
                    v2Int = new Vector2Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
                    break;
            }

            return v2Int;
        }

        /// <summary>
        /// Vector3 转 Vector3Int 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="k">转换方式</param>
        /// <returns></returns>
        public static Vector3Int ToVector3Int(this Vector3 value, GetValue k = GetValue.Round)
        {
            var v3Int = Vector3Int.zero;
            switch (k)
            {
                case GetValue.Round:
                    v3Int = new Vector3Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y),
                        Mathf.RoundToInt(value.z));
                    break;
                case GetValue.Ceil:
                    v3Int = new Vector3Int(Mathf.CeilToInt(value.x), Mathf.CeilToInt(value.y),
                        Mathf.CeilToInt(value.z));
                    break;
                case GetValue.Floor:
                    v3Int = new Vector3Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y),
                        Mathf.FloorToInt(value.z));
                    break;
            }

            return v3Int;
        }

        /// <summary>
        /// 获得两条线的交点
        /// </summary>
        /// <param name="lineAStart"></param>
        /// <param name="lineAEnd"></param>
        /// <param name="lineBStart"></param>
        /// <param name="lineBEnd"></param>
        /// <returns></returns>
        public static Vector2 GetIntersection(Vector2 lineAStart, Vector2 lineAEnd, Vector2 lineBStart, Vector2 lineBEnd)
        {
            float x1 = lineAStart.x, y1 = lineAStart.y;
            float x2 = lineAEnd.x, y2 = lineAEnd.y;
            float x3 = lineBStart.x, y3 = lineBStart.y;
            float x4 = lineBEnd.x, y4 = lineBEnd.y;

            //两向量相互垂直，返回0
            if (x1 == x2 && x3 == x4 && x1 == x3)
            {
                return Vector2.zero;
            }

            //两向量相互平行。返回0
            if (y1 == y2 && y3 == y4 && y1 == y3)
            {
                return Vector2.zero;
            }

            //两向量相互垂直，返回0
            if (x1 == x2 && x3 == x4)
            {
                return Vector2.zero;
            }

            //两向量相互平行。返回0
            if (y1 == y2 && y3 == y4)
            {
                return Vector2.zero;
            }

            float x, y;
            if (x1 == x2)
            {
                var m2 = (y4 - y3) / (x4 - x3);
                var c2 = -m2 * x3 + y3;
                x = x1;
                y = c2 + m2 * x1;
            }
            else if (x3 == x4)
            {
                var m1 = (y2 - y1) / (x2 - x1);
                var c1 = -m1 * x1 + y1;
                x = x3;
                y = c1 + m1 * x3;
            }
            else
            {
                var m1 = (y2 - y1) / (x2 - x1);
                var c1 = -m1 * x1 + y1;
                var m2 = (y4 - y3) / (x4 - x3);
                var c2 = -m2 * x3 + y3;
                x = (c1 - c2) / (m2 - m1);
                y = c2 + m2 * x;
            }

            if (IsInsideLine(lineAStart, lineAEnd, x, y) && IsInsideLine(lineBStart, lineBEnd, x, y))
            {
                return new Vector2(x, y);
            }

            return Vector2.zero;
        }

        /// <summary>
        /// 交点是否在线以内
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool IsInsideLine(Vector3 start, Vector3 end, float x, float y)
        {
            return (x >= start.x && x <= end.x || x >= end.x && x <= start.x) && (y >= start.y && y <= end.y || y >= end.y && y <= start.y);
        }

        /// <summary>
        /// 计算AB与CD两条线段的交点.
        /// </summary>
        /// <param name="a">A点</param>
        /// <param name="b">B点</param>
        /// <param name="c">C点</param>
        /// <param name="d">D点</param>
        /// <param name="intersectPos">AB与CD的交点</param>
        /// <returns>是否相交 true:相交 false:未相交</returns>
        public static bool TryGetIntersectPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 intersectPos)
        {
            intersectPos = Vector3.zero;
            var ab = b - a;
            var ca = a - c;
            var cd = d - c;
            var v1 = Vector3.Cross(ca, cd);
            if (Mathf.Abs(Vector3.Dot(v1, ab)) > 1e-6)
            {
                // 不共面
                return false;
            }

            if (Vector3.Cross(ab, cd).sqrMagnitude <= 1e-6)
            {
                // 平行
                return false;
            }

            var ad = d - a;
            var cb = b - c;
            // 快速排斥
            if (Mathf.Min(a.x, b.x) > Mathf.Max(c.x, d.x) || Mathf.Max(a.x, b.x) < Mathf.Min(c.x, d.x) || Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y) || Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y) || Mathf.Min(a.z, b.z) > Mathf.Max(c.z, d.z) || Mathf.Max(a.z, b.z) < Mathf.Min(c.z, d.z)
               )
                return false;

            // 跨立试验
            if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0 && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
            {
                var v2 = Vector3.Cross(cd, ab);
                var ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;
                intersectPos = a + ab * ratio;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 把 byte数组转成字符串输出
        /// </summary>
        /// <param name="bytes"></param>
        public static string DebugByte(this IEnumerable<byte> bytes)
        {
            return bytes.Aggregate(string.Empty, (current, b) => current + (b + " "));
        }

        /// <summary>
        /// 获取一个UID
        /// </summary>
        /// <returns></returns>
        public static long GetUid()
        {
            var buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// 移除列表中所有空元素
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveNull<T>(List<T> list)
        {
            var count = list.Count;
            for (var i = 0; i < count; i++)
                if (list[i] == null)
                {
                    var newCount = i++;
                    for (; i < count; i++)
                        if (list[i] != null)
                            list[newCount++] = list[i];
                    list.RemoveRange(newCount, count - newCount);
                    break;
                }
        }

        private static Random rng;

        /// <summary>
        /// 为提供随机洗牌的泛型列表定义扩展方法的实用程序类
        /// 基于Fisher-Yates洗牌算法 (https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle).
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                var k = (rng.Next(0, n) % n);
                n--;
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        /// <summary>
        /// 移除重复
        /// </summary>
        public static IEnumerable<T> RemoveDuplicates<T>(this IEnumerable<T> list)
        {
            return list.Distinct().ToList();
        }
    }
}