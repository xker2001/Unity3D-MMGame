using UnityEngine;
using System.Collections.Generic;

namespace MMGame
{
    public static class ExtendTransform
    {
        /// <summary>
        /// 从所有子树中查找第一个指定名称的节点。
        /// </summary>
        /// <param name="name">节点名称。</param>
        /// <returns>节点 Transform。</returns>
        public static Transform FindInSubtree(this Transform self, string name)
        {
            foreach (Transform xform in self)
            {
                if (xform.name == name)
                {
                    return xform;
                }

                Transform result = xform.FindInSubtree(name);

                if (result)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// 从所有子树中查找所有指定名称的节点。
        /// </summary>
        /// <param name="name">节点名称。</param>
        /// <param name="result">节点 Transform 列表。</param>
        public static void FindAllInSubtree(this Transform self, string name, ref List<Transform> result)
        {
            foreach (Transform xform in self)
            {
                if (xform.name == name)
                {
                    result.Add(xform);
                }

                xform.FindAllInSubtree(name, ref result);
            }
        }
    }
}