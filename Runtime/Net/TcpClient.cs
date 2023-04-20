using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace HaiPackage.Net
{
    public class TcpClient
    {
        public Socket server { get; private set; }
        private Parser _buff = new(2048);
        private bool _connecting;
        private bool _closing;
        public static Dictionary<int, MessageBase> MessageBases = new();

        /// <summary>
        /// 启动客户端并连接服务器
        /// </summary>
        /// <param name="ip">需要连接的服务器Ip地址</param>
        /// <param name="port">需要连接的服务器端口号</param>
        public virtual void StartSocket(string ip, int port)
        {
            _clientMessageBase.Init();
            _clientMessageBase.DisconnectRequest += DisconnectRequest;
            if (server is { Connected: true })
            {
                Debug.LogWarning("已经连接该服务器，无需再次连接");
                return;
            }

            if (_connecting)
            {
                Debug.Log("正在连接中，请勿重复连接");
                return;
            }

            Init();
            try
            {
                //异步连接
                server.BeginConnect(ip, port, RequestCallback, server);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        protected virtual void Init()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _buff = new Parser(2048);
            _connecting = false;
            _closing = false;
            AddMessageBase();
        }

        /// <summary>
        /// 连接服务器回调
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void RequestCallback(IAsyncResult ar)
        {
            try
            {
                var socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                _connecting = false;
                Debug.Log("已经连接到服务器");
                Debug.Log("开始监听数据");
                server.BeginReceive(_buff.Bytes, _buff.WriteIdx, _buff.Remain, 0, Callback, server);
            }
            catch (SocketException e)
            {
                Debug.LogError(e);
                _connecting = false;
            }
        }

        /// <summary>
        /// 接收数据回调
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void Callback(IAsyncResult ar)
        {
            try
            {
                var socket = (Socket)ar.AsyncState;
                var count = socket.EndReceive(ar);
                if (count <= 0)
                {
                    Close();
                    return;
                }
                _buff.WriteIdx += count;
                ReceiveData();
                if (_buff.Remain < 8)
                {
                    var expandSize = _buff.Length < _buff.DefaultSize ? _buff.DefaultSize : _buff.Length;
                    _buff.ReSize(expandSize);
                }
                server.BeginReceive(_buff.Bytes, _buff.WriteIdx, _buff.Remain, 0, Callback, server);
            }
            catch (Exception e)
            {
                Close();
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        protected virtual void ReceiveData()
        {
            var dataCount = BitConverter.ToInt32(_buff.Bytes, _buff.ReadIdx) + 4;
            if (_buff.Length < dataCount)
            {
                return;
            }
            var count = BitConverter.ToInt32(_buff.Bytes, _buff.ReadIdx);
            if (DataBase.DecryptCodes(_buff, NetTool.GetPrivateKey(), out var data))
            {
                _pendingDataBases.Enqueue(data);
            }
            else
            {
                Debug.Log("数据没有解析成功");
            }

            _buff.ReadIdx = _buff.WriteIdx;
        }

        public Action CloseServer;

        /// <summary>
        /// 关闭对服务器的连接
        /// </summary>
        /// <param name="normal"></param>
        public void Close(bool normal = true)
        {
            if (server == null || _connecting)
            {
                Debug.LogWarning("没有正常关闭与客户端的连接");
                CloseServer?.Invoke();
                return;
            }

            try
            {
                //尝试与对方沟通，告知对方自己与对方断开连接
                Send(new DataBase().SetCodes(NetCode.DisconnectRequest), NetTool.GetPrivateKey());
            }
            catch
            {
            }

            try
            {
                server.Close();
                server = null;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            try
            {
                CloseServer?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            Debug.Log("断开了对服务器的连接");
        }

        public void Send(DataBase dataBase, string key)
        {
            if (server is not { Connected: true })
            {
                return;
            }

            try
            {
                var sendData = dataBase.Pack(key);
                server.BeginSend(sendData, 0, sendData.Length, 0, NetEvent.CllServerSendOver, server);
            }
            catch (SocketException e)
            {
                Debug.LogWarning("消息发送失败" + e);
                return;
            }
        }

        public void Send(string key, string message)
        {
            if (server is not { Connected: true })
            {
                return;
            }

            try
            {
                var dataBase = new NetMessage();
                dataBase.SetCodes(NetCode.SendMessage);
                dataBase.message = message;
                Send(dataBase, key);
                Debug.Log("客户端发送数据");
            }
            catch (SocketException e)
            {
                Debug.LogWarning("消息发送失败" + e);
                return;
            }
        }

        #region 数据处理
        private Task _task;
        /// <summary>
        /// 默认事件处理器
        /// </summary>
        private readonly ClientMessageBase _clientMessageBase = new();
        /// <summary>
        /// 等待处理事件
        /// </summary>
        private readonly ConcurrentQueue<DataBase> _pendingDataBases = new();
        /// <summary>
        /// 待处理事件数量
        /// </summary>
        public int PendingDataBasesCount => _pendingDataBases.Count;

        /// <summary>
        /// 获取待处理的事件
        /// </summary>
        /// <returns></returns>
        public DataBase GetDataBase()
        {
            _pendingDataBases.TryDequeue(out var data);
            return data;
        }

        /// <summary>
        /// 数据分发
        /// </summary>
        /// <param name="dataBase"></param>
        public virtual void DistributeEvents(DataBase dataBase)
        {
            if (MessageBases.TryGetValue(dataBase.Codes.list[0], out var messageBase))
            {
                try
                {
                    messageBase.DispatchTask(server, dataBase);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        void AddMessageBase()
        {
            MessageBases.Clear();
            AddMessageBase(0, _clientMessageBase);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="index">需要处理事件的索引，请注意！默认占用0号位置</param>
        /// <param name="messageBase"></param>
        public virtual void AddMessageBase(int index, MessageBase messageBase)
        {
            MessageBases.Add(index, messageBase);
        }

        /// <summary>
        /// 服务器与自身断开连接
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="dataBase"></param>
        private void DisconnectRequest(Socket socket, DataBase dataBase)
        {
            Close();
        }
        #endregion
    }
}