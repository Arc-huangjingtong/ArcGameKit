using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HaiPackage
{
    /// <summary>
    /// 池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> where T : Object
    {
        private readonly Queue<T> _pool = new();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T Get(T obj)
        {
            return _pool.Count == 0 ? Object.Instantiate(obj) : _pool.Dequeue();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public T Get(T obj, Transform rect)
        {
            return _pool.Count == 0 ? Object.Instantiate(obj, rect) : _pool.Dequeue();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rect"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public T Get(T obj, Vector3 rect, Quaternion quaternion)
        {
            return _pool.Count == 0 ? Object.Instantiate(obj, rect, quaternion) : _pool.Dequeue();
        }

        /// <summary>
        /// 存入
        /// </summary>
        /// <param name="obj"></param>
        public void Set(T obj)
        {
            _pool.Enqueue(obj);
        }

        /// <summary>
        /// 全部销毁
        /// </summary>
        public void AllDestroy()
        {
            foreach (var o in _pool.Where(o => o != null))
            {
                Object.Destroy(o);
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        /// <returns></returns>
        public T[] Clear()
        {
            var list = _pool.ToArray();
            _pool.Clear();
            return list;
        }
    }
}