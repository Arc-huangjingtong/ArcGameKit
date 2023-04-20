using System.Net;
using System.Net.Sockets;

namespace HaiPackage.Net
{
    public abstract class MessageBase
    {
        public abstract void Init();
        
        
        /// <summary>
        /// TCP服务器
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="dataBase"></param>
        public virtual void DispatchTask(ClientSocket clientSocket, DataBase dataBase)
        {
        }

        /// <summary>
        /// TCP客户端
        /// </summary>
        /// <param name="dataBase"></param>
        public virtual void DispatchTask(Socket socket, DataBase dataBase)
        {
        }

        /// <summary>
        /// UDP接收数据
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="dataBase"></param>
        public virtual void DispatchTask(EndPoint endPoint, DataBase dataBase)
        {
        }
    }
}