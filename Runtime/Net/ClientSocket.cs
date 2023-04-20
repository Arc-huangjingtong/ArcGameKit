using System.Net.Sockets;

namespace HaiPackage.Net
{
    public class ClientSocket
    {
        /// <summary>
        /// 客户端
        /// </summary>
        public Socket Socket_Tcp { get; set; }
        /// <summary>
        /// 心跳
        /// </summary>
        public long LastPingTime { get; set; } = 0;
        /// <summary>
        /// 解析器
        /// </summary>
        public Parser ReadBuff = new Parser(2048);
        public virtual void Close()
        {
            
        }
    }
}