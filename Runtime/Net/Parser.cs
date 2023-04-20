using System;

namespace HaiPackage.Net
{
    /// <summary>
    /// 解析器
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// 默认大小
        /// </summary>
        public int DefaultSize { get; private set; }
        /// <summary>
        /// 初始大小
        /// </summary>
        private int _initSize;
        /// <summary>
        /// 缓冲区
        /// </summary>
        public byte[] Bytes;
        //读写位置, ReadIdx = 开始读的索引，WriteIdx = 已经写入的索引
        /// <summary>
        /// 开始读的索引
        /// </summary>
        public int ReadIdx;
        /// <summary>
        /// 已经写入的索引
        /// </summary>
        public int WriteIdx;
        /// <summary>
        /// 容量
        /// </summary>
        private int _capacity;
        /// <summary>
        /// 剩余空间
        /// </summary>
        public int Remain => _capacity - WriteIdx;
        /// <summary>
        /// 数据长度
        /// </summary>
        public int Length => WriteIdx - ReadIdx;

        public Parser(int defaultSize)
        {
            Init(defaultSize);
        }
        
        public void Init(int defaultSize)
        {
            DefaultSize = defaultSize;
            Bytes = new byte[DefaultSize];
            _capacity = DefaultSize;
            _initSize = DefaultSize;
            ReadIdx = 0;
            WriteIdx = 0;
        }

        /// <summary>
        /// 检测并移动数据
        /// </summary>
        public void CheckAndMoveBytes()
        {
            if (Length < 8)
            {
                MoveBytes();
            }
        }

        /// <summary>
        /// 移动数据
        /// </summary>
        public void MoveBytes()
        {
            if (ReadIdx < 0)
                return;
            Array.Copy(Bytes, ReadIdx, Bytes, 0, Length);
            ReadIdx = 0;
            WriteIdx = Length;
        }

        /// <summary>
        /// 重设尺寸
        /// </summary>
        /// <param name="size"></param>
        public void ReSize(int size)
        {
            if (ReadIdx < 0) return;
            if (size < Length) return;
            if (size < _initSize) return;
            short n = 1024;
            while (n < size) n *= 2;
            _capacity = n;
            byte[] newBytes = new byte[_capacity];
            Array.Copy(Bytes, ReadIdx, newBytes, 0, Length);
            Bytes = newBytes;
            WriteIdx = Length;
            ReadIdx = 0;
        }
    }
}