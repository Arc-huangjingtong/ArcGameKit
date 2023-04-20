using System;
using HaiPackage.Tool;
using UnityEngine;

namespace HaiPackage.To
{
    public class To
    {
        public object StartValue;
        public object EndValue;
        /// <summary>
        /// 当前时间
        /// </summary>
        public float currentTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public float overTime;
        /// <summary>
        /// 执行百分比
        /// </summary>
        public float Progress => currentTime / overTime;
        private long _id;
        /// <summary>
        /// 唯一ID
        /// 虽然我让你设置这个id，但请尽可能不要去设置它,它会在初始化的时候被设置
        /// </summary>
        public long ID
        {
            get => _id;
            set => _id = value;
        }
        public bool Stop; 
        public Action<To> Start;
        public Action<To> UpDate;
        public Action<To> Over;
        private ToControlManager.UpdateType _updateType;
        public ToControlManager.UpdateType UpdateType
        {
            get => _updateType;
            private set => _updateType = value;
        }

        /// <summary>
        /// 添加动画，如果相同的动画Id在的话，会尝试的去销毁它，并执行销毁动画的Over事件
        /// </summary>
        /// <param name="updateType"></param>
        public void Add(ToControlManager.UpdateType updateType = ToControlManager.UpdateType.Update)
        {
            _updateType = updateType;
            if (ToControlManager.Instance == null)
            {
                var obj = new GameObject();
                obj.AddComponent<ToControlManager>();
            }

            ToControlManager.Add(ID, this, updateType);
        }

        public To()
        {
            _id = GTool.GetUid();
            Stop = false;
        }
    }
}