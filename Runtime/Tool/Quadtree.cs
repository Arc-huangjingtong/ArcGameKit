using System;
using System.Collections.Generic;
using UnityEngine;

namespace HaiPackage.Tool
{
    /// <summary>
    /// 四叉树，针对A*
    /// </summary>
    public class Quadtree
    {
        public QuadtreeNode root;

        /// <summary>
        /// 获取地图最佳边界
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float GetMapSize(Vector2 v2)
        {
            float size = 1;
            if (v2 == Vector2.zero)
            {
                return size;
            }

            var dataSize = v2.x >= v2.y ? v2.x : v2.y;
            do
            {
                if (size >= dataSize)
                {
                    return size;
                }

                size *= 2;
            } while (size < dataSize);

            return size;
        }

        /// <summary>
        /// 添加障碍
        /// </summary>
        /// <param name="node"></param>
        /// <param name="rect"></param>
        public void AddBarriers(QuadtreeNode node, Rect rect)
        {
            if (!node.Rect.Overlaps(rect)) return;

            //判断是否还能细分
            if (Contrast(node, rect))
            {
                if (node.Ld == null) node.Subdivision();
                AddBarriers(node.Ld, rect);
                AddBarriers(node.Lu, rect);
                AddBarriers(node.Rd, rect);
                AddBarriers(node.Ru, rect);
                if (node.Ld.Obstacle && node.Lu.Obstacle && node.Rd.Obstacle && node.Ru.Obstacle)
                {
                    node.Ld = node.Lu = node.Rd = node.Ru = null;
                    node.Obstacle = true;
                    return;
                }
            }
            else
            {
                node.Obstacle = true;
            }
        }

        /// <summary>
        /// 对比
        /// </summary>
        /// <param name="node"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Contrast(QuadtreeNode node, Rect rect)
        {
            return node.Rect.width > rect.width && node.Rect.width > rect.width;
        }

        /// <summary>
        /// 查找周围格子
        /// </summary>
        /// <param name="findNode"></param>
        /// <param name="around">周围 或 上下左右</param>
        /// <returns></returns>
        public List<QuadtreeNode> FindTheSurroundingGrid(Rect findNode, bool around = true)
        {
            List<QuadtreeNode> list = new();
            if (root == null) return list;
            if (around)
            {
                //TODO 这里应该是加上最小层级的差值，但理论上每个格子的最小大小不会小于1 
                var rect = new Rect(findNode.position - new Vector2(.5f, .5f), findNode.size + Vector2.one);
                FindMinOverlap(root, rect, list);
            }
            else
            {
                var rect = new Rect(findNode.position - new Vector2(.5f, 0), findNode.size + new Vector2(1, 0));
                FindMinOverlap(root, rect, list);
                rect = new Rect(findNode.position - new Vector2(0, .5f), findNode.size + new Vector2(0, 1));
                FindMinOverlap(root, rect, list);
            }

            return list;
        }

        private void FindMinOverlap(QuadtreeNode rootNode, Rect findNode, ICollection<QuadtreeNode> list)
        {
            //没有重叠
            if (!rootNode.Rect.Overlaps(findNode)) return;
            //没有子项了
            if (rootNode.Ld == null)
            {
                list.Add(rootNode);
                return;
            }

            //递归查找
            FindMinOverlap(rootNode.Ld, findNode, list);
            FindMinOverlap(rootNode.Lu, findNode, list);
            FindMinOverlap(rootNode.Rd, findNode, list);
            FindMinOverlap(rootNode.Ru, findNode, list);
        }

        /// <summary>
        /// 查找点所在的格子
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="findNode"></param>
        /// <returns></returns>
        public QuadtreeNode FindGrid(QuadtreeNode rootNode, Vector2 findNode)
        {
            //没有重叠
            if (!rootNode.Rect.Contains(findNode)) return null;
            //没有子项了
            if (rootNode.Ld == null)
            {
                return rootNode;
            }

            var node = FindGrid(rootNode.Ld, findNode);
            if (node != null) return node;
            node = FindGrid(rootNode.Lu, findNode);
            if (node != null) return node;
            node = FindGrid(rootNode.Rd, findNode);
            if (node != null) return node;
            node = FindGrid(rootNode.Ru, findNode);
            return node;
        }

        #region Debug
        /// <summary>
        /// 画图
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void GizmosDrawCube(Vector3 pos, Vector3 size, Color color)
        {
            color.a = .5f;
            Gizmos.color = color;
            Gizmos.DrawCube(pos, size);
        }

        /// <summary>
        /// 查找画图
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static bool GizmosDrawCubeNode(QuadtreeNode node, List<QuadtreeNode> nodes)
        {
            if (node.Ld == null)
            {
                nodes.Add(node);
                return true;
            }

            GizmosDrawCubeNode(node.Lu, nodes);
            GizmosDrawCubeNode(node.Ld, nodes);
            GizmosDrawCubeNode(node.Ru, nodes);
            GizmosDrawCubeNode(node.Rd, nodes);
            return false;
        }
        #endregion
    }
    public class QuadtreeNode : IComparable<QuadtreeNode>
    {
        public QuadtreeNode Lu, Ru, Ld, Rd, Parent;
        public Rect Rect;
        public bool Obstacle;
        public int Leve;

        /// <summary>
        /// 距离Start格子的距离
        /// </summary>
        public int gCost = 0;
        /// <summary>
        /// 距离Target格子的距离
        /// </summary>
        public int hCost = 0;
        /// <summary>
        /// 当前格子的权重
        /// </summary>
        public int FCost => gCost + hCost;
        public QuadtreeNode FNode;

        public QuadtreeNode(Rect rect, QuadtreeNode parent = null, int leve = 0)
        {
            Rect = rect;
            Parent = parent;
            Leve = leve;
        }

        /// <summary>
        /// 细分
        /// </summary>
        public void Subdivision()
        {
            var size = Rect.size / 2;
            Ld = new QuadtreeNode(new Rect(new Vector2(Rect.x, Rect.y), size), this, Leve + 1);
            Rd = new QuadtreeNode(new Rect(new Vector2(Rect.x + size.x, Rect.y), size), this, Leve + 1);
            Lu = new QuadtreeNode(new Rect(new Vector2(Rect.x, Rect.y + size.y), size), this, Leve + 1);
            Ru = new QuadtreeNode(new Rect(new Vector2(Rect.x + size.x, Rect.y + size.y), size), this, Leve + 1);
        }

        public int CompareTo(QuadtreeNode other)
        {
            //比较选出最低的F值，返回-1，0，1
            int result = FCost.CompareTo(other.FCost);
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }

            return result;
        }
    }
}