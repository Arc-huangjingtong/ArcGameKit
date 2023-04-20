using System.Collections.Generic;

namespace HaiPackage.Net
{
    public static  class NetCode
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        public static readonly List<int> SendMessage = new() { 0, 0, 0 };
        /// <summary>
        /// 断开连接
        /// </summary>
        public static readonly List<int> DisconnectRequest = new() { 0, 0, 1 };
    }
}