using System;
using UnityEngine;

namespace HaiPackage.Net
{
    public class ServerMessageBase : MessageBase
    {
        public void DebugMessage(DataBase dataBase)
        {
            Debug.Log(dataBase.DataJson);
        }

        public Action<ClientSocket, DataBase> DisconnectRequest;

        public override void Init()
        {
            
        }

        public override void DispatchTask(ClientSocket clientSocket, DataBase dataBase)
        {
            try
            {
                switch (dataBase.Codes.list[2])
                {
                    case 0:
                        DebugMessage(dataBase);
                        break;
                    case 1:
                        DisconnectRequest?.Invoke(clientSocket, dataBase);
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