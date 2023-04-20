using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HaiPackage.MeshEffect.TMPEffect
{
    /// <summary>
    /// 逐字显示
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPShowGraduallyEffect : BaseMeshEffect
    {
        private int Index => Mathf.FloorToInt(progress / (1f / (configure.AfterCount + textMeshPro.textInfo.characterInfo.Length - 1)));
        public Configure configure; //= new() { AfterCount = 5 };
        public float progress;

        private List<VertexInfo> _vertexInfos = new();
        private List<LineVertexInfo> _strikethroughLine = new();
        private List<LineVertexInfo> _underlineLine = new();

        public override void ModifyMesh(VertexHelper vh)
        {
            try
            {
                #region 重新计算网格
                var characterInfo = textMeshPro.textInfo.characterInfo;
                UIVertex v = default;
                _vertexInfos = new List<VertexInfo>();
                _strikethroughLine = new List<LineVertexInfo>();
                _underlineLine = new List<LineVertexInfo>();
                var startPos = Index - configure.AfterCount;
                var notSee = 0; //不可视字符数量
                for (var i = 0; i < textMeshPro.textInfo.characterCount; ++i)
                {
                    var alpha = 0f;
                    var info = characterInfo[i];
                    if (!info.isVisible)
                    {
                        notSee++;
                        continue;
                    }

                    if (i - notSee < startPos + configure.AfterCount)
                    {
                        alpha = 1 - Mathf.Clamp01(1f / configure.AfterCount * (i - notSee - startPos));
                    }
                    else if (i - notSee > startPos + configure.AfterCount)
                    {
                        alpha = 0f;
                    }

                    if (_vertexInfos.Count != 0) _vertexInfos[^1].endAlpha = alpha;
                    _vertexInfos.Add(new VertexInfo { infoVertexIndex = info.vertexIndex, startAlpha = alpha, endAlpha = alpha });
                    CreateLine(ref _strikethroughLine, info, info.strikethroughVertexIndex, alpha);
                    CreateLine(ref _underlineLine, info, info.underlineVertexIndex, alpha);
                }
                #endregion
                #region 重新绘画网格
                //重新绘画文字
                foreach (var item in _vertexInfos)
                {
                    var start = item.infoVertexIndex;
                    var end = start + 4;
                    for (; start < end; start++)
                    {
                        vh.PopulateUIVertex(ref v, start);
                        if (start < item.infoVertexIndex + 2)
                        {
                            v.color.a = (byte)(v.color.a * item.startAlpha);
                        }
                        else
                        {
                            v.color.a = (byte)(v.color.a * item.endAlpha);
                        }

                        vh.SetUIVertex(v, start);
                    }
                }

                foreach (var item in _strikethroughLine)
                {
                    for (var i = 0; i < 12; i++)
                    {
                        vh.PopulateUIVertex(ref v, item.infoVertexIndex + i);
                        if (i is 0 or 1)
                        {
                            UIVertex vv = default;
                            vh.PopulateUIVertex(ref vv, item.startInfo.vertexIndex);
                            v.color = vv.color;
                        }

                        if (i is 2 or 3 or 4 or 5)
                        {
                            UIVertex vv = default;
                            vh.PopulateUIVertex(ref vv, item.startInfo.vertexIndex + 2);
                            v.color = vv.color;
                        }

                        if (i is 6 or 7 or 8 or 9)
                        {
                            UIVertex vv = default;
                            //开始渐变索引 
                            int index;
                            if (item.GradientStartInfoIndex > 0)
                            {
                                index = item.GradientStartInfoIndex;
                            }
                            else
                            {
                                index = item.endInfo.vertexIndex + 2;
                            }

                            vh.PopulateUIVertex(ref vv, index);
                            var vector3 = v.position;
                            vector3.x = vv.position.x;
                            v.position = vector3;
                            v.color = vv.color;
                        }

                        if (i is 10 or 11)
                        {
                            UIVertex vv = default;
                            vh.PopulateUIVertex(ref vv, item.endInfo.vertexIndex + 2);
                            var vector3 = v.position;
                            vector3.x = vv.position.x;
                            v.position = vector3;
                            v.color = vv.color;
                        }

                        vh.SetUIVertex(v, item.infoVertexIndex + i);
                    }
                }

                foreach (var item in _underlineLine)
                {
                    for (var i = 0; i < 12; i++)
                    {
                        vh.PopulateUIVertex(ref v, item.infoVertexIndex + i);
                        if (i is 0 or 1)
                        {
                            UIVertex vv = default;
                            vh.PopulateUIVertex(ref vv, item.startInfo.vertexIndex);
                            v.color = vv.color;
                        }

                        if (i is 2 or 3 or 4 or 5)
                        {
                            UIVertex vv = default;
                            vh.PopulateUIVertex(ref vv, item.startInfo.vertexIndex + 2);
                            v.color = vv.color;
                        }

                        if (i is 6 or 7 or 8 or 9)
                        {
                            UIVertex vv = default;
                            //开始渐变索引 
                            int index;
                            if (item.GradientStartInfoIndex > 0)
                            {
                                index = item.GradientStartInfoIndex;
                            }
                            else
                            {
                                index = item.endInfo.vertexIndex + 2;
                            }

                            vh.PopulateUIVertex(ref vv, index);
                            var vector3 = v.position;
                            vector3.x = vv.position.x;
                            v.position = vector3;
                            v.color = vv.color;
                        }

                        if (i is 10 or 11)
                        {
                            UIVertex vv = default;
                            vh.PopulateUIVertex(ref vv, item.endInfo.vertexIndex + 2);
                            var vector3 = v.position;
                            vector3.x = vv.position.x;
                            v.position = vector3;
                            v.color = vv.color;
                        }

                        vh.SetUIVertex(v, item.infoVertexIndex + i);
                    }
                }
            }
            catch
            {
            }
            #endregion
        }

        /// <summary>
        /// 创建线条
        /// </summary>
        /// <param name="list"></param>
        /// <param name="characterInfo"></param>
        /// <param name="index"></param>
        /// <param name="a"></param>
        private void CreateLine(ref List<LineVertexInfo> list, TMP_CharacterInfo characterInfo, int index, float a)
        {
            if (index != 0)
            {
                var line = list.Find(x => x.infoVertexIndex == index);
                if (line == null)
                {
                    //开始渐变索引
                    var infoIndex = -1;
                    if (a is < 1 and > 0)
                    {
                        infoIndex = characterInfo.vertexIndex;
                    }

                    list.Add(new LineVertexInfo { infoVertexIndex = index, startInfo = characterInfo, endInfo = characterInfo, GradientStartInfoIndex = infoIndex });
                }
                else
                {
                    if (a != 0)
                    {
                        line.endInfo = characterInfo;
                    }

                    //开始渐变索引
                    if (a is < 1 and > 0 && line.GradientStartInfoIndex < 0)
                    {
                        line.GradientStartInfoIndex = characterInfo.vertexIndex;
                    }
                }
            }
        }

        public struct Configure
        {
            /// <summary>
            /// 后面的字符数量
            /// </summary>
            public uint AfterCount;

            public Configure(uint afterCount)
            {
                AfterCount = afterCount;
            }
        }
        class VertexInfo
        {
            //过度透明度
            public float startAlpha;
            public float endAlpha;
            public int infoVertexIndex;
        }
        class LineVertexInfo
        {
            public int infoVertexIndex;
            public int GradientStartInfoIndex = -1;
            public TMP_CharacterInfo startInfo;
            public TMP_CharacterInfo endInfo;
        }
    }
}