using UnityEngine;

namespace HaiPackage
{
    public static class TransformTool
    {
        /// <summary>
        /// 从数据里面赋值Transform
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="transform">对象</param>
        public static void ToLocalTransform(TransformData data, Transform transform)
        {
            transform.localPosition = data.Position;
            transform.localRotation = data.Rotation;
            transform.localScale = data.Scale;
        }

        /// <summary>
        /// 复制一个 Transform 数值到另外一个 Transform
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="transform">对象</param>
        public static void ToLocalTransform(Transform data, Transform transform)
        {
            transform.localPosition = data.localPosition;
            transform.localRotation = data.localRotation;
            transform.localScale = data.localScale;
        }
    }

    public class TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public TransformData(Transform transform)
        {
            Position = transform.localPosition;
            Rotation = transform.localRotation;
            Scale = transform.localScale;
        }

        /// <summary>
        /// 获取 Local Transform Data
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public TransformData GetLocalTransformData(Transform transform)
        {
            Position = transform.localPosition;
            Rotation = transform.localRotation;
            Scale = transform.localScale;
            return this;
        }

        /// <summary>
        /// 获取  Transform Data
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public TransformData GetTransformData(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
            Scale = transform.localScale;
            return this;
        }
    }
}