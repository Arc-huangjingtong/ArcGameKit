using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Random = System.Random;

namespace HaiPackage.Net
{
    public static class NetTool
    {
        #region 常量
        private static string _publicKey = "hai15115";
        private static string _privateKey = "hai15115";
        /// <summary>
        /// 获取公开密钥
        /// </summary>
        /// <returns></returns>
        public static string GetPublicKey()
        {
            return _publicKey;
        }
        /// <summary>
        /// 设置公开密钥
        /// </summary>
        /// <param name="key"></param>
        public static void SetPublicKey(string key)
        {
            _publicKey = key;
        }
        /// <summary>
        /// 获取私有密钥
        /// </summary>
        /// <returns></returns>
        public static string GetPrivateKey()
        {
            return _privateKey;
        }
        /// <summary>
        /// 设置私有密钥
        /// </summary>
        /// <param name="key"></param>
        public static void SetPrivateKey(string key)
        {
            _privateKey = key;
        }
        #endregion
        #region Tool
        public static string GetThisIPv4
        {
            get
            {
                var add = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                return add.Length < 1 ? "" : add[1].ToString();
            }
        }
        public static string GetThisIPv6
        {
            get
            {
                var add = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                return add.Length < 1 ? "" : add[0].ToString();
            }
        }
        /// <summary>
        /// 获取当前心跳
        /// 从 1970-1-1-0-0-0-0 开始
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        /// <summary>
        /// 将字符串形式的IP地址转换成IPAddress对象
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns></returns>
        public static IPAddress StringToIPAddress(string ip)
        {
            return IPAddress.Parse(ip);
        }
        /// <summary>
        /// 创建一个IPEndPoint对象
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public static IPEndPoint CreateIPEndPoint(string ip, int port)
        {
            return new IPEndPoint(StringToIPAddress(ip), port);
        }
        /// <summary>
        /// 绑定终结点
        /// </summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="endPoint">要绑定的终结点</param>
        public static Socket BindEndPoint(this Socket socket, IPEndPoint endPoint)
        {
            if (!socket.IsBound)
            {
                socket.Bind(endPoint);
            }
            else
            {
                Debug.LogWarning("Socket 是空的无法绑定，请检查");
            }

            return socket;
        }
        /// <summary>
        /// 绑定终结点
        /// </summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        public static void BindEndPoint(this Socket socket, string ip, int port)
        {
            BindEndPoint(socket, CreateIPEndPoint(ip, port));
        }
        /// <summary>
        /// 指定Socket对象执行监听
        /// </summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="maxConnection">允许最大的挂起数 </param>
        public static void StartListen(this Socket socket, string ip, int port, int maxConnection)
        {
            //绑定到本地终结点
            BindEndPoint(socket, ip, port);
            //开始监听
            socket.Listen(maxConnection);
        }
        /// <summary>
        /// 指定Socket对象执行监听
        /// </summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="endPoint"></param>
        /// <param name="maxConnection">允许最大的挂起数 </param>
        public static void StartListen(this Socket socket,IPEndPoint endPoint  , int maxConnection)
        {
            //绑定到本地终结点
            BindEndPoint(socket, endPoint);
            //开始监听
            socket.Listen(maxConnection);
        }
        /// <summary>
        /// 获取远程客户端的IP地址
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static string GetClientIp(this Socket socket)
        {
            return (socket.RemoteEndPoint as IPEndPoint)?.Address.ToString();
        }
        /// <summary>
        /// 获取远程客户端的IP地址
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static string GetClientIp(this EndPoint endPoint)
        {
            return (endPoint as IPEndPoint)?.Address.ToString();
        }
        /// <summary>
        /// 获取远程客户端的端口号
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static string GetClientPort(this Socket socket)
        {
            return (socket.RemoteEndPoint as IPEndPoint)?.Port.ToString();
        }
        /// <summary>
        /// 获取远程客户端的端口号
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static string GetClientPort(this EndPoint endPoint)
        {
            return (endPoint as IPEndPoint)?.Port.ToString();
        }
        /// <summary>
        /// 获取远程用户的IP地址和端口号
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static string GetClientIpAndPort(this EndPoint endPoint)
        {
            return (endPoint as IPEndPoint)?.ToString();
        }
        /// <summary>
        /// 获取远程用户的IP地址和端口号
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static string GetClientIpAndPort(this Socket socket)
        {
            return (socket.RemoteEndPoint as IPEndPoint)?.ToString();
        }
        /// <summary>
        /// 连接到基于TCP协议的服务器,连接成功返回true，否则返回false
        /// </summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="port">服务器端口号</param>     
        public static bool ConnectServer(this Socket socket, string ip, int port)
        {
            try
            {
                //连接服务器
                socket.Connect(ip, port);
                //检测连接状态
                return socket.Poll(-1, SelectMode.SelectWrite);
            }
            catch (SocketException ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }
        /// <summary>
        /// 随机获取一个组播地址
        /// </summary>
        /// <returns></returns>
        public static string ObtainMulticastAddressRandomly()
        {
            var random = new Random((int)DateTime.UtcNow.Ticks);
            var ip = string.Empty;
            ip += random.Next(225, 239) + ".";
            ip += random.Next(0, 255) + ".";
            ip += random.Next(0, 255) + ".";
            ip += random.Next(0, 255);
            return ip;
        }
        
        #endregion
    }
}