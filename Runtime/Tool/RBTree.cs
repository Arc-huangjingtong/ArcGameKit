using System;
using System.Collections.Generic;

namespace HaiPackage.Tool
{
    /// <summary>
    /// 红黑树
    /// 它内部数值是不能相同的，如果使用相同的数值进行填充，可能会导致树出问题
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RBTree<T> where T : IComparable
    {
        private const bool Red = false;
        private const bool Black = true;
        private TreeNode<T> _root;
        public int Size { get; private set; }
        public int ModCount { get; private set; }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Put(T value)
        {
            if (value == null) return;
            var t = _root;
            if (t == null)
            {
                _root = new TreeNode<T>(value);
                Size = 1;
                ModCount++;
                return;
            }

            int cmp;
            TreeNode<T> parent;
            do
            {
                parent = t;
                cmp = value.CompareTo(t.Value);
                switch (cmp)
                {
                    case < 0:
                        t = t.Left;
                        break;
                    case > 0:
                        t = t.Right;
                        break;
                    default:
                        t.Value = value;
                        return;
                }
            } while (t != null);

            var e = new TreeNode<T>(value)
            {
                Parent = parent
            };
            if (cmp < 0)
                parent.Left = e;
            else
                parent.Right = e;
            FixAfterInsertion(e);
            Size++;
            ModCount++;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="list"></param>
        public void PutAll(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                Put(item);
            }
        }

        /// <summary>
        /// 清空所有元素
        /// </summary>
        public void Clear()
        {
            ModCount++;
            Size = 0;
            _root = null;
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="key"></param>
        public void Remove(T key)
        {
            var p = Get(key);
            if (p == null)
                return;
            DeleteEntry(p);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TreeNode<T> Get(T key)
        {
            var p = _root;
            while (p != null)
            {
                var cmp = key.CompareTo(p.Value);
                switch (cmp)
                {
                    case < 0:
                        p = p.Left;
                        break;
                    case > 0:
                        p = p.Right;
                        break;
                    default:
                        return p;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取索引树节点
        /// 索引大于树大小、可能为空
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TreeNode<T> GetIndexNode(uint index)
        {
            if (index > Size)
            {
                return null;
            }

            List<TreeNode<T>> list = new();
            MiddleOrderTraversal(_root, list, index);
            return list[^1];
        }

        /// <summary>
        /// 获取树中最小项
        /// </summary>
        /// <returns></returns>
        public TreeNode<T> GetFirstEntry()
        {
            var p = _root;
            if (p == null) return null;
            while (p.Left != null)
                p = p.Left;
            return p;
        }

        /// <summary>
        /// 获取树中最大项
        /// </summary>
        /// <returns></returns>
        public TreeNode<T> GetLastEntry()
        {
            var p = _root;
            if (p == null) return null;
            while (p.Right != null)
                p = p.Right;
            return p;
        }

        /// <summary>
        /// 修改元素
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool Replace(T oldValue, T newValue)
        {
            if (oldValue == null || newValue == null || oldValue.Equals(newValue)) return false;
            Remove(oldValue);
            Remove(newValue);
            Put(newValue);
            return true;
        }

        public List<T> ToList(TraversalMode mode = TraversalMode.MiddleOrderTraversal)
        {
            var t = new List<T>();
            switch (mode)
            {
                case TraversalMode.PreorderTraversal:
                    PreorderTraversal(_root, t);
                    break;
                case TraversalMode.MiddleOrderTraversal:
                    MiddleOrderTraversal(_root, t);
                    break;
                case TraversalMode.PostorderTraversal:
                    PostorderTraversal(_root, t);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            return t;
        }

        /// <summary>
        /// 查找是否存在
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ContainsKey(T item)
        {
            return Get(item) != null;
        }

        public enum TraversalMode
        {
            /// <summary>
            /// 前序遍历
            /// </summary>
            PreorderTraversal,
            /// <summary>
            /// 中序遍历
            /// </summary>
            MiddleOrderTraversal,
            /// <summary>
            /// 后序遍历
            /// </summary>
            PostorderTraversal,
        }

        /// <summary>
        /// 前序遍历
        /// </summary>
        /// <param name="node"></param>
        /// <param name="list"></param>
        private static void PreorderTraversal(TreeNode<T> node, ICollection<T> list)
        {
            if (node == null)
            {
                return;
            }

            list.Add(node.Value);
            PreorderTraversal(node.Left, list);
            PreorderTraversal(node.Right, list);
        }

        /// <summary>
        /// 中序遍历
        /// </summary>
        /// <param name="node"></param>
        /// <param name="list"></param>
        private static void MiddleOrderTraversal(TreeNode<T> node, ICollection<T> list)
        {
            if (node == null)
            {
                return;
            }

            MiddleOrderTraversal(node.Left, list);
            list.Add(node.Value);
            MiddleOrderTraversal(node.Right, list);
        }

        /// <summary>
        /// 中序遍历
        /// </summary>
        /// <param name="node"></param>
        /// <param name="list"></param>
        /// <param name="getIndex"></param>
        private static void MiddleOrderTraversal(TreeNode<T> node, ICollection<TreeNode<T>> list, uint getIndex)
        {
            if (node == null)
            {
                return;
            }

            MiddleOrderTraversal(node.Left, list, getIndex);
            if (list.Count - 1 >= getIndex)
            {
                return;
            }

            list.Add(node);
            MiddleOrderTraversal(node.Right, list, getIndex);
        }

        /// <summary>
        /// 后序遍历
        /// </summary>
        /// <param name="node"></param>
        /// <param name="list"></param>
        private static void PostorderTraversal(TreeNode<T> node, ICollection<T> list)
        {
            if (node == null)
            {
                return;
            }

            PostorderTraversal(node.Left, list);
            PostorderTraversal(node.Right, list);
            list.Add(node.Value);
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="p"></param>
        private void DeleteEntry(TreeNode<T> p)
        {
            ModCount++;
            Size--;

            // If strictly internal, copy successor's element to p and then make p
            // point to successor.
            if (p.Left != null && p.Right != null)
            {
                var s = Successor(p);
                p.Value = s.Value;
                p = s;
            } // p has 2 children

            // Start fixup at replacement node, if it exists.
            var replacement = p.Left ?? p.Right;
            if (replacement != null)
            {
                // Link replacement to parent
                replacement.Parent = p.Parent;
                if (p.Parent == null)
                    _root = replacement;
                else if (p == p.Parent.Left)
                    p.Parent.Left = replacement;
                else
                    p.Parent.Right = replacement;

                // Null out links so they are OK to use by fixAfterDeletion.
                p.Left = p.Right = p.Parent = null;

                // Fix replacement
                if (p.Color == Black)
                    FixAfterDeletion(replacement);
            }
            else if (p.Parent == null)
            {
                // return if we are the only node.
                _root = null;
            }
            else
            {
                //  No children. Use self as phantom replacement and unlink.
                if (p.Color == Black)
                    FixAfterDeletion(p);
                if (p.Parent != null)
                {
                    if (p == p.Parent.Left)
                        p.Parent.Left = null;
                    else if (p == p.Parent.Right)
                        p.Parent.Right = null;
                    p.Parent = null;
                }
            }
        }

        /// <summary>
        /// 删除后修复
        /// </summary>
        /// <param name="x"></param>
        private void FixAfterDeletion(TreeNode<T> x)
        {
            while (x != _root && ColorOf(x) == Black)
            {
                if (x == LeftOf(ParentOf(x)))
                {
                    var sib = RightOf(ParentOf(x));
                    if (ColorOf(sib) == Red)
                    {
                        SetColor(sib, Black);
                        SetColor(ParentOf(x), Red);
                        RotateLeft(ParentOf(x));
                        sib = RightOf(ParentOf(x));
                    }

                    if (ColorOf(LeftOf(sib)) == Black &&
                        ColorOf(RightOf(sib)) == Black)
                    {
                        SetColor(sib, Red);
                        x = ParentOf(x);
                    }
                    else
                    {
                        if (ColorOf(RightOf(sib)) == Black)
                        {
                            SetColor(LeftOf(sib), Black);
                            SetColor(sib, Red);
                            RotateRight(sib);
                            sib = RightOf(ParentOf(x));
                        }

                        SetColor(sib, ColorOf(ParentOf(x)));
                        SetColor(ParentOf(x), Black);
                        SetColor(RightOf(sib), Black);
                        RotateLeft(ParentOf(x));
                        x = _root;
                    }
                }
                else
                {
                    // symmetric
                    var sib = LeftOf(ParentOf(x));
                    if (ColorOf(sib) == Red)
                    {
                        SetColor(sib, Black);
                        SetColor(ParentOf(x), Red);
                        RotateRight(ParentOf(x));
                        sib = LeftOf(ParentOf(x));
                    }

                    if (ColorOf(RightOf(sib)) == Black &&
                        ColorOf(LeftOf(sib)) == Black)
                    {
                        SetColor(sib, Red);
                        x = ParentOf(x);
                    }
                    else
                    {
                        if (ColorOf(LeftOf(sib)) == Black)
                        {
                            SetColor(RightOf(sib), Black);
                            SetColor(sib, Red);
                            RotateLeft(sib);
                            sib = LeftOf(ParentOf(x));
                        }

                        SetColor(sib, ColorOf(ParentOf(x)));
                        SetColor(ParentOf(x), Black);
                        SetColor(LeftOf(sib), Black);
                        RotateRight(ParentOf(x));
                        x = _root;
                    }
                }
            }

            SetColor(x, Black);
        }

        /// <summary>
        /// 继承
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static TreeNode<T> Successor(TreeNode<T> t)
        {
            if (t == null)
                return null;
            else if (t.Right != null)
            {
                var p = t.Right;
                while (p.Left != null)
                    p = p.Left;
                return p;
            }
            else
            {
                var p = t.Parent;
                var ch = t;
                while (p != null && ch == p.Right)
                {
                    ch = p;
                    p = p.Parent;
                }

                return p;
            }
        }

        /// <summary>
        /// 插入后修改
        /// </summary>
        /// <param name="x"></param>
        private void FixAfterInsertion(TreeNode<T> x)
        {
            x.Color = Red;
            while (x != null && x != _root && x.Parent.Color == Red)
            {
                if (ParentOf(x) == LeftOf(ParentOf(ParentOf(x))))
                {
                    var y = RightOf(ParentOf(ParentOf(x)));
                    if (ColorOf(y) == Red)
                    {
                        SetColor(ParentOf(x), Black);
                        SetColor(y, Black);
                        SetColor(ParentOf(ParentOf(x)), Red);
                        x = ParentOf(ParentOf(x));
                    }
                    else
                    {
                        if (x == RightOf(ParentOf(x)))
                        {
                            x = ParentOf(x);
                            RotateLeft(x);
                        }

                        SetColor(ParentOf(x), Black);
                        SetColor(ParentOf(ParentOf(x)), Red);
                        RotateRight(ParentOf(ParentOf(x)));
                    }
                }
                else
                {
                    var y = LeftOf(ParentOf(ParentOf(x)));
                    if (ColorOf(y) == Red)
                    {
                        SetColor(ParentOf(x), Black);
                        SetColor(y, Black);
                        SetColor(ParentOf(ParentOf(x)), Red);
                        x = ParentOf(ParentOf(x));
                    }
                    else
                    {
                        if (x == LeftOf(ParentOf(x)))
                        {
                            x = ParentOf(x);
                            RotateRight(x);
                        }

                        SetColor(ParentOf(x), Black);
                        SetColor(ParentOf(ParentOf(x)), Red);
                        RotateLeft(ParentOf(ParentOf(x)));
                    }
                }
            }

            _root.Color = Black;
        }

        /// <summary>
        /// 左旋
        /// </summary>
        /// <param name="p"></param>
        private void RotateLeft(TreeNode<T> p)
        {
            if (p == null) return;
            var r = p.Right;
            p.Right = r.Left;
            if (r.Left != null)
                r.Left.Parent = p;
            r.Parent = p.Parent;
            if (p.Parent == null)
                _root = r;
            else if (p.Parent.Left == p)
                p.Parent.Left = r;
            else
                p.Parent.Right = r;
            r.Left = p;
            p.Parent = r;
        }

        /// <summary>
        /// 右旋
        /// </summary>
        /// <param name="p"></param>
        private void RotateRight(TreeNode<T> p)
        {
            if (p == null) return;
            var l = p.Left;
            p.Left = l.Right;
            if (l.Right != null) l.Right.Parent = p;
            l.Parent = p.Parent;
            if (p.Parent == null)
                _root = l;
            else if (p.Parent.Right == p)
                p.Parent.Right = l;
            else p.Parent.Left = l;
            l.Right = p;
            p.Parent = l;
        }

        /// <summary>
        /// 找到父亲
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static TreeNode<T> ParentOf(TreeNode<T> p)
        {
            return p?.Parent;
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        private static void SetColor(TreeNode<T> p, bool c)
        {
            if (p != null)
                p.Color = c;
        }

        /// <summary>
        /// 左孩子
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static TreeNode<T> LeftOf(TreeNode<T> p)
        {
            return p?.Left;
        }

        /// <summary>
        /// 右孩子
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static TreeNode<T> RightOf(TreeNode<T> p)
        {
            return p?.Right;
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool ColorOf(TreeNode<T> p)
        {
            return p?.Color ?? Black;
        }
    }
    public class TreeNode<T> where T : IComparable
    {
        public T Value;
        public bool Color;
        public TreeNode<T> Parent;
        public TreeNode<T> Left;
        public TreeNode<T> Right;

        public TreeNode(T value)
        {
            Value = value;
        }
    }
    #region Demo
    public class RbTreeTest
    {
        private RBTree<RbTreeNodeTest> rb;

        public void Maim()
        {
            rb = new();
            rb.Put(new RbTreeNodeTest(1));
            var obj = rb.Get(new RbTreeNodeTest(1));
            rb.Remove(obj.Value);
        }
    }
    public class RbTreeNodeTest : IComparable
    {
        private int index;

        public int CompareTo(object obj)
        {
            if (index < (int)obj)
            {
                return -1;
            }
            if (index > (int)obj)
            {
                return -1;
            }
            return 0;
        }

        public RbTreeNodeTest(int i)
        {
            index = i;
        }
    }
    #endregion
}