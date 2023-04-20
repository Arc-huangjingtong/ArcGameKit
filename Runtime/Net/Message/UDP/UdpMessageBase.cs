using System;
using System.Net;
using UnityEngine;

namespace HaiPackage.Net
{
    public class UdpMessageBase :MessageBase
    {
        public void DebugMessage(DataBase dataBase)
        {
            Debug.Log(dataBase.DataJson);
        }

        public override void Init()
        {
            
        }

        public override void DispatchTask(EndPoint endPoint, DataBase dataBase)
        {
            try
            {
                switch (dataBase.Codes.list[2])
                {
                    case 0:
                        DebugMessage(dataBase);
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