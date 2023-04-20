using UnityEngine;
using UnityEngine.UI;

namespace HaiPackage.MeshEffect
{
    /// <summary>
    /// 翻转
    /// </summary>
    public class UIFlipEffect : BaseMeshEffect
    {
        [Tooltip("水平")][SerializeField] private bool m_Horizontal = false;

        [Tooltip("垂直")][ SerializeField] private bool m_Veritical = false;
        public bool Vertical
        {
            get => m_Veritical;
            set
            {
                m_Veritical = value;
                SetVerticesDirty();
            }
        }
        public bool Horizontal
        {
            get => m_Horizontal;
            set
            {
                m_Horizontal = value;
                SetVerticesDirty();
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            var rt = graphic.rectTransform;
            var vt = default(UIVertex);
            for (var i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vt, i);
                var pos = vt.position;
                vt.position = new Vector3(m_Horizontal ? -pos.x : pos.x, m_Veritical ? -pos.y : pos.y);
                vh.SetUIVertex(vt, i);
            }
        }
    }
}