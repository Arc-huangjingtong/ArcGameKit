using UnityEngine;

namespace HaiPackage.Net
{
    public class NetClass
    {
    }
    [System.Serializable]
    public class NetMessage : DataBase
    {
        public string message;
        public override void ToJson()
        {
            DataJson = JsonUtility.ToJson(this);
        }

        public override void ToObject()
        {
            var data = JsonUtility.FromJson<NetMessage>(DataJson);
            message = data.message;
        }
    }
}