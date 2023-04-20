using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HaiPackage.Data;
using UnityEngine;

namespace HaiPackage.To
{
    public class ToControlManager : SingletonMonoBehaviour<ToControlManager>
    {
        static Dictionary<long, To> _toUpdate = new();
        static Dictionary<long, To> _toLateUpdate = new();
        static Dictionary<long, To> _toFixedUpdate = new();

        /// <summary>
        /// 当前播放中的动画数量
        /// </summary>
        public static int AllToAnimCount => UpdateToAnimCount + LateUpdateToAnimCount + FixedUpdateToAnimCount;
        /// <summary>
        /// 在Update中播放的动画数量
        /// </summary>
        public static int UpdateToAnimCount => _toUpdate.Count;
        /// <summary>
        /// 在LateUpdate中播放的动画数量
        /// </summary>
        public static int LateUpdateToAnimCount => _toLateUpdate.Count;
        /// <summary>
        /// 在FixedUpdate中播放的动画数量
        /// </summary>
        public static int FixedUpdateToAnimCount => _toFixedUpdate.Count;

        private void Update()
        {
            ToAnim(ref _toUpdate);
        }

        private void LateUpdate()
        {
            ToAnim(ref _toLateUpdate);
        }

        private void FixedUpdate()
        {
            ToAnim(ref _toFixedUpdate);
        }

        private void ToAnim(ref Dictionary<long, To> tos)
        {
            if (tos.Count == 0)
            {
                return;
            }

            try
            {
                List<To> removeList = new();
                foreach (var (_, v) in tos)
                {
                    if (v.currentTime == 0)
                    {
                        v.Start?.Invoke(v);
                    }

                    //它可能在动画的过程中被销毁
                    try
                    {
                        if (!v.Stop)
                        {
                            v.currentTime += Time.deltaTime * Constant.TimeSpeed;
                            v.UpDate?.Invoke(v);
                        }
                    }
                    catch (Exception e)
                    {
                        removeList.Add(v);
                        Debug.LogWarning("动画在播放过程中可能被销毁" + e);
                    }

                    if (v.currentTime >= v.overTime)
                    {
                        v.Over?.Invoke(v);
                        if (v.currentTime >= v.overTime) tos.Remove(v.ID);
                    }
                }

                if (removeList.Count != 0)
                {
                    foreach (var to in removeList)
                    {
                        RemoveAll(to.ID);
                    }
                }
            }
            catch
            {
            }
        }

        public static To Get(long id, UpdateType updateType = UpdateType.Update)
        {
            switch (updateType)
            {
                case UpdateType.Update:
                    return Get(ref _toUpdate, id);
                case UpdateType.LateUpdate:
                    return Get(ref _toLateUpdate, id);
                case UpdateType.FixedUpdate:
                    return Get(ref _toFixedUpdate, id);
            }

            return null;
        }

        public static List<To> GetAll(long id)
        {
            List<To> toList = new()
            {
                Get(ref _toUpdate, id),
                Get(ref _toLateUpdate, id),
                Get(ref _toFixedUpdate, id)
            };
            for (var i = 0; i < toList.Count; i++)
            {
                if (toList[i] != null) continue;
                toList.RemoveAt(i);
                i--;
            }

            return toList;
        }

        private static To Get(ref Dictionary<long, To> dictionary, long id)
        {
            if (!dictionary.ContainsKey(id)) return null;
            return dictionary[id];
        }

        public static void Add(long id, To to, UpdateType updateType = UpdateType.Update)
        {
            switch (updateType)
            {
                case UpdateType.Update:
                    Add(ref _toUpdate, id, to);
                    break;
                case UpdateType.LateUpdate:
                    Add(ref _toLateUpdate, id, to);
                    break;
                case UpdateType.FixedUpdate:
                    Add(ref _toFixedUpdate, id, to);
                    break;
            }
        }

        private static void Add(ref Dictionary<long, To> dictionary, long id, To to)
        {
            if (dictionary.ContainsKey(id))
            {
                try
                {
                    dictionary[id].Over?.Invoke(dictionary[id]);
                }
                catch
                {
                }

                dictionary.Remove(id);
            }

            dictionary.Add(id, to);
        }

        public static void RemoveAll(long id)
        {
            var to = GetAll(id);
            if (to.Count != 0)
            {
                foreach (var t in to)
                {
                    switch (t.UpdateType)
                    {
                        case UpdateType.Update:
                            Remove(ref _toUpdate, id);
                            break;
                        case UpdateType.LateUpdate:
                            Remove(ref _toLateUpdate, id);
                            break;
                        case UpdateType.FixedUpdate:
                            Remove(ref _toFixedUpdate, id);
                            break;
                    }
                }
            }
        }

        public static void Remove(long id, UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.Update:
                    Remove(ref _toUpdate, id);
                    break;
                case UpdateType.LateUpdate:
                    Remove(ref _toLateUpdate, id);
                    break;
                case UpdateType.FixedUpdate:
                    Remove(ref _toFixedUpdate, id);
                    break;
            }
        }

        private static void Remove(ref Dictionary<long, To> dictionary, long id)
        {
            if (!dictionary.ContainsKey(id)) return;
            dictionary.Remove(id);
        }

        public enum UpdateType
        {
            Update,
            LateUpdate,
            FixedUpdate
        }
    }
}