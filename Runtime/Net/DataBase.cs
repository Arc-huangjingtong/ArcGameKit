using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HaiPackage.Tool;
using UnityEngine;

namespace HaiPackage.Net
{
    [Serializable]
    public class JList<T>
    {
        public List<T> list = new();

        public JList(List<T> target)
        {
            this.list = target;
        }

        public JList()
        {
        }
    }

    [Serializable]
    public class DataBase
    {
        public JList<int> Codes = new(new List<int>(2));
        public string DataJson;
        private static bool keys ;//= true;

        public byte[] Pack(string key, bool debug = true)
        {
            DataJson = " ";
            ToJson();
            var dataByte = EncryptionData(this, key, out var i);
            var codeCount = BitConverter.GetBytes(i); //codes长度
            var l = codeCount.Length + dataByte.Length;
            var byteCount = BitConverter.GetBytes(l); //总长度
            var sendData = new byte[l + byteCount.Length]; //发送的数据
            Array.Copy(byteCount, 0, sendData, 0, byteCount.Length);
            Array.Copy(codeCount, 0, sendData, byteCount.Length, codeCount.Length);
            Array.Copy(dataByte, 0, sendData, byteCount.Length + codeCount.Length, dataByte.Length);
            if (debug && keys)
            {
                Debug.Log("正在打包一个数据包，它的长度是" + sendData.Length + "\n" + sendData.DebugByte());
                Debug.Log(DataJson);
            }

            return sendData;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="dataBase">数据</param>
        /// <param name="key">加密钥匙</param>
        /// <param name="count">执行码长度</param>
        /// <returns></returns>
        public static byte[] EncryptionData(DataBase dataBase, string key, out int count)
        {
            count = 0;
            var code = JsonUtility.ToJson(dataBase.Codes);
            var codeByte = Aes.AesEncrypt(Encoding.UTF8.GetBytes(code), key);
            count = codeByte.Length;
            var jsonByte = Aes.AesEncrypt(Encoding.UTF8.GetBytes(dataBase.DataJson), key + codeByte.Length);
            var data = new byte[codeByte.Length + jsonByte.Length];
            Array.Copy(codeByte, 0, data, 0, codeByte.Length);
            Array.Copy(jsonByte, 0, data, codeByte.Length, jsonByte.Length);
            return data;
        }
        
        
        
        

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">加密钥匙</param>
        /// <param name="dataBase">数据</param>
        /// <param name="debug"></param>
        /// <returns>是否解包成功</returns>
        public static bool DecryptCodes(Parser data, string key, out DataBase dataBase, bool debug = true)
        {
            var offset = data.ReadIdx;
            var buff = data.Bytes;
            if (debug && keys)
            {
                Debug.Log("位移" + data.ReadIdx);
                Debug.Log("写入" + data.WriteIdx);
                Debug.Log("正在解析一个长度是" + data.Length + "的数据包\n" + data.Bytes.DebugByte());
            }

            dataBase = new DataBase();
            if (data.Length <= 0)
            {
                return false;
            }

            try
            {
                var count = BitConverter.ToInt32(buff, offset);
                var codeCount = BitConverter.ToInt32(buff, offset + 4);
                var codeByte = new byte[codeCount];
                var dataCount = count - 4 - codeCount;
                var dataByte = new byte[dataCount];
                Array.Copy(buff, offset + 8, codeByte, 0, codeCount);
                Array.Copy(buff, offset + codeCount + 8, dataByte, 0, dataCount);
                dataBase.DataJson = Encoding.UTF8.GetString(Aes.AesDecrypt(dataByte, key + codeByte.Length));
                dataBase.Codes = JsonUtility.FromJson<JList<int>>(Encoding.UTF8.GetString(Aes.AesDecrypt(codeByte, key)));
                if (debug && keys)
                {
                    Debug.Log(dataBase.DataJson);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("解包失败" + e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 设置执行码
        /// </summary>
        /// <param name="idea"></param>
        /// <param name="error"></param>
        public DataBase SetCodes(int idea = 0, int error = 0, int code = 0)
        {
            if (Codes == null)
            {
                Codes = new JList<int>();
            }

            if (Codes.list.Count == 0)
            {
                Codes.list.Add(idea);
                Codes.list.Add(error);
            }
            else if (Codes.list.Count == 1)
            {
                Codes.list[0] = idea;
                Codes.list.Add(error);
            }
            else
            {
                Codes.list[0] = idea;
                Codes.list[1] = error;
            }

            if (Codes.list.Count >= 3)
            {
                Codes.list[2] = code;
            }
            else
            {
                Codes.list.Add(code);
            }

            return this;
        }

        public DataBase SetCodes(List<int> codes)
        {
            Codes.list = codes;
            return this;
        }

        public virtual void ToJson()
        {
            DataJson = JsonUtility.ToJson(this);
        }

        public virtual void ToObject()
        {
            
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="dataBase"></param>
        public DataBase Copy(DataBase dataBase)
        {
            DataJson = dataBase.DataJson;
            Codes = dataBase.Codes;
            return this;
        }
    }
}