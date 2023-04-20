using System;
using System.Net.Sockets;

namespace HaiPackage.Net
{
    public static class NetEvent
    {
        #region Server
        /// <summary>
        /// 连接上一个客户端
        /// </summary>
        public static event Action<ClientSocket> ConnectClient;

        /// <summary>
        /// 连接上一个客户端
        /// </summary>
        public static void CllConnectClient(ClientSocket client)
        {
            ConnectClient?.Invoke(client);
        }

        /// <summary>
        /// 一个客户端离线
        /// 不需要在这里处理客户端关闭事件，
        /// </summary>
        public static event Action<ClientSocket> ClientDisconnected;

        /// <summary>
        /// 一个客户端离线
        /// 不需要在这里处理客户端关闭事件，
        /// </summary>
        public static void CllClientDisconnected(ClientSocket client)
        {
            ClientDisconnected?.Invoke(client);
        }

        /// <summary>
        /// 服务器发送结束
        /// </summary>
        public static event Action<IAsyncResult> ServerSendOver;

        public static void CllServerSendOver(IAsyncResult ar)
        {
            ServerSendOver?.Invoke(ar);
        }
        #endregion
    }
}