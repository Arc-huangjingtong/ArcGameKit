using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HaiPackage.MeshEffect.TMPEffect
{
    /// <summary>
    /// 文字海浪效果
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPWaveEffect : BaseMeshEffect
    {
        private int _index => Mathf.FloorToInt(progress / (1f / (textMeshPro.textInfo.characterInfo.Length - 1 + configure.FrontCount)));
        public float progress;

        public Configure configure = new() { FrontCount = 3, AfterCount = 3, Top = 10 };
        private List<int> _vertexInfos = new();

        public override void ModifyMesh(VertexHelper vh)
        {
            var characterInfo = textMeshPro.textInfo.characterInfo;
            _vertexInfos = new List<int>();
            UIVertex uiVertex = default;
            for (var i = 0; i < textMeshPro.textInfo.characterCount; i++)
            {
                var info = characterInfo[i];
                if (!info.isVisible)
                {
                    continue;
                }

                _vertexInfos.Add(info.vertexIndex);
            }

            for (var i = -configure.FrontCount; i <= configure.AfterCount; i++)
            {
                var index = i + _index;
                if (index >= 0 && index < _vertexInfos.Count)
                {
                    var start = _vertexInfos[(int)(_index + i)];
                    var end = start + 4;
                    for (; start < end; start++)
                    {
                        if (start == 0 && _index != 0) continue;
                        vh.PopulateUIVertex(ref uiVertex, start);
                        var countOffset = i switch
                        {
                            0 => configure.Top,
                            < 0 => configure.Top / configure.FrontCount * (configure.FrontCount - Mathf.Abs(i)),
                            _ => configure.Top / configure.AfterCount * (Mathf.Abs(configure.AfterCount - i))
                        };
                        uiVertex.position += new Vector3(0, countOffset, 0);
                        vh.SetUIVertex(uiVertex, start);
                    }
                }
            }
        }

        public struct Configure
        {
            /// <summary>
            /// 前面的字符数量
            /// </summary>
            public uint FrontCount;
            /// <summary>
            /// 后面的字符数量
            /// </summary>
            public uint AfterCount;
            /// <summary>
            /// 跳动高度
            /// </summary>
            public float Top;

            public Configure(uint frontCount, uint afterCount, float top)
            {
                FrontCount = frontCount;
                AfterCount = afterCount;
                Top = top;
            }
        }
    }
}