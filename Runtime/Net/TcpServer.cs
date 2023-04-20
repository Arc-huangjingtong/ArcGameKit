using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HaiPackage.Net
{
    public class TcpServer
    {
        public Socket ServerSocket { get; private set; }
        public List<Socket> Temporary { get; } = new();
        public Dictionary<Socket, ClientSocket> ClientSocketDic { get; } = new();
        public Dictionary<int, MessageBase> MessageBases = new();
        private Thread serverThread;
        public  Action<ClientSocket> CloseClientAction;

        /// <summary>
        /// 启动服务器
        /// </summary>
        public virtual TcpServer StartServer(IPEndPoint endPoint, int maxCont)
        {
            _serverMessageBase.DisconnectRequest += DisconnectRequest;
            Recovery();
            AddMessageBase();
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.StartListen(endPoint, maxCont);
            Debug.Log("服务器启动，开始挂起监听");
            serverThread = new Thread(Start)
            {
                IsBackground = true,
            };
            serverThread.Start();
            return this;
        }
        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Close()
        {
            _serverMessageBase.DisconnectRequest -= DisconnectRequest;
            try
            {
                foreach (var client in ClientSocketDic)
                {
                    CloseClient(client.Value);
                }

                ClientSocketDic.Clear();
                ServerSocket.Close();
                serverThread.Abort();
                Recovery();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        /// <summary>
        /// 启动服务器
        /// </summary>
        protected virtual void Start()
        {
            while (true)
            {
                try
                {
                    ResetCheckRead();
                    try
                    {
                        Socket.Select(Temporary, null, null, 1000);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }

                    for (var i = Temporary.Count - 1; i >= 0; i--)
                    {
                        var s = Temporary[i];
                        if (s == ServerSocket)
                        {
                            ReadListen(s); //连接客户端
                        }
                        else
                        {
                            ReadClient(s); //处理客户端消息
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
        /// <summary>
        /// 检查所有Socket
        /// </summary>
        protected virtual void ResetCheckRead()
        {
            Temporary.Clear();
            Temporary.Add(ServerSocket);
            foreach (var s in ClientSocketDic.Keys)
            {
                Temporary.Add(s);
            }
        }
        /// <summary>
        /// 连接客户端
        /// </summary>
        protected virtual void ReadListen(Socket client)
        {
            try
            {
                var aClient = client.Accept();
                var clientSocket = new ClientSocket
                {
                    Socket_Tcp = aClient
                };
                ClientSocketDic.Add(aClient, clientSocket);
                Debug.Log(clientSocket.Socket_Tcp.GetClientIp() + "已连接");
                Debug.Log("现在服务器中已经连接 " + ClientSocketDic.Count + " 个客户端");
                NetEvent.CllConnectClient(clientSocket);
            }
            catch (SocketException e)
            {
                Debug.Log(e);
            }
        }
        /// <summary>
        /// 接收客户端消息
        /// </summary>
        /// <param name="client"></param>
        protected virtual void ReadClient(Socket client)
        {
            try
            {
                var clientSocket = ClientSocketDic[client];
                var buff = clientSocket.ReadBuff;
                int count = 0;
                if (buff.Remain <= 0)
                {
                    ReceiveData(clientSocket, NetTool.GetPrivateKey());
                    buff.CheckAndMoveBytes();
                    //若存储空间还是不足，放大存储空间，直到能装下数据为止
                    while (buff.Remain <= 0)
                    {
                        var expandSize = buff.Length < buff.DefaultSize ? buff.DefaultSize : buff.Length;
                        buff.ReSize(expandSize * 2);
                    }
                }

                try
                {
                    count = client.Receive(buff.Bytes, buff.WriteIdx, buff.Remain, 0);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    CloseClient(clientSocket);
                    return;
                }

                //客户端可能断开连接
                if (count <= 0)
                {
                    CloseClient(clientSocket);
                    return;
                }

                buff.WriteIdx += count;
                ReceiveData(clientSocket, NetTool.GetPrivateKey()); //解析数据
            }
            catch (Exception e)
            {
                CloseClient(client);
                return;
            }
        }
        /// <summary>
        /// 关闭客户端
        /// </summary>
        /// <param name="client"></param>
        public virtual void CloseClient(Socket client)
        {
            try
            {
                //尝试与对方沟通，告知对方自己与对方断开连接
                Send(client, new DataBase().SetCodes(NetCode.DisconnectRequest), NetTool.GetPrivateKey());
            }
            catch
            {
            }

            try
            {
                Debug.Log("与" + client.GetClientIp() + "断开连接");
                client.Close();
                try
                {
                    ClientSocketDic.TryGetValue(client, out var clientSocket);

                        CloseClientAction?.Invoke(clientSocket);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                ClientSocketDic.Remove(client);
                Debug.Log("有一个客户端已经断开了连接现在还有 " + ClientSocketDic.Count + " 个客户端在线");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        /// <summary>
        /// 关闭客户端
        /// </summary>
        /// <param name="client"></param>
        public virtual void CloseClient(ClientSocket client)
        {
            client.Close();
            NetEvent.CllClientDisconnected(client);
            CloseClient(client.Socket_Tcp);
        }
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="clientSocket"></param>
        protected virtual void ReceiveData(ClientSocket clientSocket, string key)
        {
            var buff = clientSocket.ReadBuff;
            var dataCount = BitConverter.ToInt32(buff.Bytes, buff.ReadIdx) + 4;
            if (buff.Length < dataCount)
            {
                return;
            }

            DataBase.DecryptCodes(buff, key, out var data);
            _pendingDataBases.Enqueue(Get(clientSocket, data));
            buff.ReadIdx = buff.WriteIdx;
        }
        public virtual void Send(ClientSocket client, string key, string message, AsyncCallback back = null)
        {
            if (client == null || !client.Socket_Tcp.Connected)
            {
                return;
            }

            try
            {
                var dataBase = new NetMessage();
                dataBase.SetCodes(NetCode.SendMessage);
                dataBase.message = message;
                Send(client, dataBase, key, back);
            }
            catch (SocketException e)
            {
                Debug.LogWarning("消息发送失败" + e);
                return;
            }
        }
        public virtual void Send(ClientSocket client, DataBase dataBase, string key, AsyncCallback back = null)
        {
            if (client == null)
            {
                return;
            }

            try
            {
                Send(client.Socket_Tcp, dataBase, key, back);
            }
            catch (SocketException e)
            {
                Debug.LogWarning("消息发送失败" + e);
                return;
            }
        }
        public virtual void Send(Socket socket, DataBase dataBase, string key, AsyncCallback back = null)
        {
            try
            {
                if (socket is not { Connected: true }) return;
                var sendData = dataBase.Pack(key);
                socket.BeginSend(sendData, 0, sendData.Length, 0, back ?? NetEvent.CllServerSendOver, socket);
                Debug.Log(dataBase.DataJson);
            }
            catch (SocketException e)
            {
                Debug.LogWarning("消息发送失败" + e);
                return;
            }
        }
        public ClientSocket GetClientSocket(Socket socket)
        {
            ClientSocketDic.TryGetValue(socket, out var clientSocket);
            return clientSocket;
        }

        #region 数据处理
        private Task _task;
        /// <summary>
        /// 等待处理事件
        /// </summary>
        private readonly ConcurrentQueue<ClientDataBase> _pendingDataBases = new();
        /// <summary>
        /// 待处理事件数量
        /// </summary>
        public int PendingDataBasesCount => _pendingDataBases.Count;
        /// <summary>
        /// 回收已经处理的事件
        /// </summary>
        private readonly ConcurrentQueue<ClientDataBase> _recoveryDataBasesPool = new();
        /// <summary>
        /// 默认事件处理器
        /// </summary>
        private readonly ServerMessageBase _serverMessageBase = new();
        private ClientDataBase Get(ClientSocket clientSocket, DataBase dataBase)
        {
            _recoveryDataBasesPool.TryDequeue(out var data);
            if (data == null)
            {
                data = new ClientDataBase(clientSocket, dataBase);
            }
            else
            {
                data.Init(clientSocket, dataBase);
            }

            return data;
        }
        private void Set(ClientDataBase data)
        {
            _recoveryDataBasesPool.Enqueue(data);
        }
        private void Recovery()
        {
            while (_pendingDataBases.Count != 0)
            {
                _pendingDataBases.TryDequeue(out var data);
                Set(data);
            }
        }

        /// <summary>
        /// 获取待处理的事件
        /// </summary>
        /// <returns></returns>
        public ClientDataBase GetDataBase()
        {
            _pendingDataBases.TryDequeue(out var data);
            _task = Task.Run(() =>
            {
                Thread.Sleep(100);
                Set(data);
            });
            return data;
        }
        /// <summary>
        /// 分发事件
        /// </summary>
        /// <param name="data"></param>
        public void DistributeEvents(ClientDataBase data)
        {
            try
            {
                MessageBases.TryGetValue(data.DataBase.Codes.list[0], out var database);
                database?.DispatchTask(data.ClientSocket, data.DataBase);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        protected virtual void AddMessageBase()
        {
            MessageBases.Clear();
            AddMessageBase(0, _serverMessageBase);
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
        /// 有客户端切断连接
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="dataBase"></param>
        private void DisconnectRequest(ClientSocket clientSocket, DataBase dataBase)
        {
            CloseClient(clientSocket);
        }
        #endregion
        public class ClientDataBase
        {
            public ClientSocket ClientSocket;
            public DataBase DataBase;

            public ClientDataBase(ClientSocket clientSocket, DataBase dataBase)
            {
                Init(clientSocket, dataBase);
            }

            public void Init(ClientSocket clientSocket, DataBase dataBase)
            {
                ClientSocket = clientSocket;
                DataBase = dataBase;
            }
        }
    }
}