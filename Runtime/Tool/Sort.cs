using System;
using System.Collections.Generic;
using System.Linq;

namespace HaiPackage.Tool
{
    public class Sort<T> where T : IComparable
    {
        /// <summary>
        /// 插入排序
        /// </summary>
        public static void Insert(ref List<T> list)
        {
            //无须序列
            for (var i = 1; i < list.Count; i++)
            {
                var temp = list[i];
                int j;

                //有序序列
                for (j = i - 1; j >= 0 && temp.CompareTo(list[j]) == -1; j--)
                {
                    list[j + 1] = list[j];
                }

                list[j + 1] = temp;
            }
        }

        ///<summary>
        /// 希尔排序
        ///</summary>
        ///<param name="list"></param>
        public static void ShellSort(ref List<T> list)
        {
            //取增量
            var step = list.Count / 2;
            while (step >= 1)
            {
                //无须序列
                for (var i = step; i < list.Count; i++)
                {
                    var temp = list[i];
                    int j;

                    //有序序列
                    for (j = i - step; j >= 0 && temp.CompareTo(list[j]) == -1; j = j - step)
                    {
                        list[j + step] = list[j];
                    }

                    list[j + step] = temp;
                }

                step = step / 2;
            }
        }

        /// <summary>
        /// 选择排序
        /// </summary>
        /// <param name="list"></param>
        public static void Selection(ref List<T> list)
        {
            for (var i = 0; i < list.Count - 1; ++i)
            {
                var index = i;
                for (var j = i + 1; j < list.Count; ++j)
                {
                    if (list[j].CompareTo(list[index]) == -1)
                        index = j;
                }

                (list[index], list[i]) = (list[i], list[index]);
            }
        }

        /// <summary>
        /// 冒泡排序
        /// </summary>
        public static void BubbleSort(ref List<T> list)
        {
            for (var i = 0; i < list.Count; i++) //最多做R.Length-1趟排序
            {
                var key = false;
                for (var j = list.Count - 2; j >= i; j--)
                {
                    if (list[j + 1].CompareTo(list[j]) != -1) continue;
                    (list[j + 1], list[j]) = (list[j], list[j + 1]);
                    key = true; //发生了交换，故将交换标志置为真
                }

                if (!key) //本趟排序未发生交换，提前终止算法
                {
                    break;
                }
            }
        }

        #region 归并排序
        /// <summary>
        /// 归并排序
        /// </summary>
        /// <param name="list"></param>
        public static void Merge(ref List<T> list)
        {
            var array = list.ToArray();
            MergeSort(array, new T[array.Length], 0, array.Length - 1);
            list = array.ToList();
        }

        ///<summary>
        /// 数组的划分
        ///</summary>
        ///<param name="array">待排序数组</param>
        ///<param name="temparray">临时存放数组</param>
        ///<param name="left">序列段的开始位置，</param>
        ///<param name="right">序列段的结束位置</param>
        static void MergeSort(T[] array, T[] temparray, int left, int right)
        {
            if (left >= right) return;
            //取分割位置
            var middle = (left + right) / 2;

            //递归划分数组左序列
            MergeSort(array, temparray, left, middle);

            //递归划分数组右序列
            MergeSort(array, temparray, middle + 1, right);

            //数组合并操作
            Merge(array, temparray, left, middle + 1, right);
        }

        ///<summary>
        /// 数组的两两合并操作
        ///</summary>
        ///<param name="array">待排序数组</param>
        ///<param name="temparray">临时数组</param>
        ///<param name="left">第一个区间段开始位置</param>
        ///<param name="middle">第二个区间的开始位置</param>
        ///<param name="right">第二个区间段结束位置</param>
        static void Merge(T[] array, T[] temparray, int left, int middle, int right)
        {
            //左指针尾
            var leftEnd = middle - 1;

            //右指针头
            var rightStart = middle;

            //临时数组的下标
            var tempIndex = left;

            //数组合并后的length长度
            var tempLength = right - left + 1;

            //先循环两个区间段都没有结束的情况
            while ((left <= leftEnd) && (rightStart <= right))
            {
                //如果发现有序列大，则将此数放入临时数组
                if (array[left].CompareTo(array[rightStart]) == -1)
                    temparray[tempIndex++] = array[left++];
                else
                    temparray[tempIndex++] = array[rightStart++];
            }

            //判断左序列是否结束
            while (left <= leftEnd)
                temparray[tempIndex++] = array[left++];

            //判断右序列是否结束
            while (rightStart <= right)
                temparray[tempIndex++] = array[rightStart++];

            //交换数据
            for (var i = 0; i < tempLength; i++)
            {
                array[right] = temparray[right];
                right--;
            }
        }
        #endregion
        #region 快速排序
        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="list"></param>
        public static void Quick(ref List<T> list)
        {
            QuickSort(list, 0, list.Count - 1);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="array">需要排列的数组</param>
        /// <param name="low">低位</param>
        /// <param name="high">高位</param>
        static void QuickSort(IList<T> array, int low, int high)
        {
            if (low >= high)
            {
                return;
            }

            // 基准值 选取当前数组第一个值
            var index = array[low];
            // 低位i从左向右扫描
            var i = low;
            // 高位j从右向左扫描
            var j = high;
            while (i < j)
            {
                while (i < j && array[j].CompareTo(index) >= 0)
                {
                    j--;
                }

                if (i < j)
                {
                    array[i] = array[j];
                    i++;
                }

                while (i < j && array[i].CompareTo(index) == -1)
                {
                    i++;
                }

                if (i >= j) continue;
                array[j] = array[i];
                j--;
            }

            array[i] = index;
            // 左边的继续递归排序
            QuickSort(array, low, i - 1);
            // 右边的继续递归排序
            QuickSort(array, i + 1, high);
        }
        #endregion
    }
}