using System;
using System.Net.Sockets;
using UnityEngine;

namespace HaiPackage.Net
{
    /// <summary>
    /// 默认消息处理 占用0号位置
    /// </summary>
    public class ClientMessageBase : MessageBase
    {
        public void DebugMessage(DataBase dataBase)
        {
            Debug.Log(dataBase.DataJson);
        }

        public Action<Socket, DataBase> DisconnectRequest;

        public override void Init()
        {
            
        }

        public override void DispatchTask(Socket socket, DataBase dataBase)
        {
            try
            {
                switch (dataBase.Codes.list[2])
                {
                    case 0:
                        DebugMessage(dataBase);
                        break;
                    case 1:
                        DisconnectRequest?.Invoke(socket , dataBase);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}