using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace HaiPackage.Net
{
    public class UdpSocket
    {
        public UdpClient UDPSocket { get; private set; }
        public Action<IAsyncResult> ReceiveBack;
        public Action<IAsyncResult> sendBack;
        public readonly Dictionary<int, MessageBase> MessageBases = new();
        /// <summary>
        /// 接收的数据
        /// </summary>
        public static int BuffCount;
        private readonly Queue<Parsers> _buffs = new();
        private class Parsers
        {
            public byte[] buff;
            public EndPoint EndPoint;
        }

        private readonly object _buffKey = new();

        public virtual void Start(IPEndPoint ip)
        {
            Init();
            UDPSocket = new UdpClient(ip);
            ReceiveStart();
            Debug.Log("启动Udp");
        }

        public virtual void Close()
        {
            _threadReceiveKey = false;
            _threadParseDataKey = false;
            UDPSocket.Close();
            Debug.Log("关闭Udp");
        }

        protected virtual void Init()
        {
            AddMessageBase();
        }

        protected virtual void AddMessageBase()
        {
            MessageBases.Clear();
            MessageBases.Add(0, new UdpMessageBase());
        }

        public virtual void AddMessageBase(int index, MessageBase messageBase)
        {
            MessageBases.Add(index, messageBase);
        }

        #region Receive
        /// <summary>
        /// 接收数据
        /// </summary>
        private Thread _threadReceive;
        private bool _threadReceiveKey; //启动线程

        /// <summary>
        /// 解析数据
        /// </summary>
        private Thread _threadParseData;
        private bool _threadParseDataKey; //启动线程

        /// <summary>
        /// 接收
        /// </summary>
        protected virtual void ReceiveStart(int bufCont = 2048)
        {
            BuffCount = bufCont;
            _threadReceiveKey = true;
            _threadParseDataKey = true;
            _threadReceive = new Thread(Receive)
            {
                IsBackground = true
            };
            _threadReceive.Start();
            _threadParseData = new Thread(ParseData)
            {
                IsBackground = true
            };
            _threadParseData.Start();
        }

        protected virtual void Receive()
        {
            while (_threadReceiveKey)
            {
                try
                {
                    var clintPoint = new IPEndPoint(IPAddress.Any, 0);
                    var buff = UDPSocket.Receive(ref clintPoint);
                  
                    lock (_buffKey)
                    {
                      
                        _buffs.Enqueue(new Parsers { buff = buff, EndPoint = clintPoint });
                    }
                }
                catch (SocketException e)
                {
                    Debug.LogWarning(e);
                    if (e.ErrorCode == (int)SocketError.MessageSize)
                    {
                        BuffCount *= 2;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
        }

        protected virtual void ParseData()
        {
            while (_threadParseDataKey)
            {
                lock (_buffKey)
                {
                    if (_buffs.Count == 0) continue;
                    var buf = _buffs.Dequeue();
                    ReceiveData(buf);
                }
            }
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        private void ReceiveData(Parsers buf)
        {
            var dataCount = BitConverter.ToInt32(buf.buff) + 4;
            if (buf.buff.Length < dataCount)
            {
                Debug.Log("这个数据不完整 ， 抛弃这个数据");
                return;
            }

            try
            {
                //解包分析
                var parser = new Parser(1) { Bytes = buf.buff, WriteIdx = buf.buff.Length };
                if (DataBase.DecryptCodes(parser, NetTool.GetPrivateKey(), out var data))
                {
                    DataDistribution(data, buf.EndPoint); //分发数据
                }
                else
                {
                    Debug.Log("数据没有解析成功");
                }
            }
            catch (Exception e)
            {
                Debug.Log("数据解析失败" + e);
            }
        }

        /// <summary>
        /// 数据分发
        /// </summary>
        /// <param name="dataBase"></param>
        /// <param name="endPoint"></param>
        protected virtual void DataDistribution(DataBase dataBase, EndPoint endPoint)
        {
            if (MessageBases.TryGetValue(dataBase.Codes.list[0], out var messageBase))
            {
                try
                {
                    messageBase.DispatchTask(endPoint, dataBase);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        #endregion
        #region Send
        public virtual void Send(string message, EndPoint endPoint, string key = null)
        {
            var dataBase = new NetMessage();
            dataBase.SetCodes(NetCode.SendMessage);
            dataBase.message = message;
            Send(dataBase, endPoint, key);
        }

        public virtual void Send(DataBase data, EndPoint endPoint, string key = null)
        {
            Send(data.Pack(key ?? NetTool.GetPrivateKey()), (IPEndPoint)endPoint);
        }

        public virtual void Send(byte[] data, IPEndPoint endPoint)
        {
            try
            {
                UDPSocket.BeginSend(data, data.Length, endPoint, SendBack, UDPSocket);
            }
            catch (Exception e)
            {
                Debug.Log("发送失败");
            }
        }

        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void SendBack(IAsyncResult ar)
        {
            if (sendBack != null)
            {
                sendBack.Invoke(ar);
            }
            else
            {
                try
                {
                    //  var socket = (UdpClient)ar.AsyncState;
                }
                catch (Exception e)
                {
                    Debug.LogError("Udp发送出现错误" + e);
                }
            }
        }
        #endregion
    }
}