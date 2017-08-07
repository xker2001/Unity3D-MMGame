using System.Collections.Generic;
using UnityEngine;

namespace MMGame
{
    public static class LayerMaskExtension
    {
        /// <summary>
        /// LayerMask 是否包含 Layer。
        /// </summary>
        /// <param name="layer">Layer。</param>
        /// <returns>如果包含返回 true，反之返回 false。</returns>
        public static bool HasLayer(this LayerMask layerMask, int layer)
        {
            return ((1 << layer) & layerMask.value) != 0;
        }

        /// <summary>
        /// 获取 LayerMask 所包含的 Layer 列表。
        /// </summary>
        /// <returns>Layer 列表。</returns>
        public static IList<int> ToLayers(this LayerMask layerMask)
        {
            List<int> layers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                if (layerMask.HasLayer(i))
                {
                    layers.Add(i);
                }
            }

            return layers;
        }

        /// <summary>
        /// 获取 LayerMask 包含的第一个 Layer。
        /// </summary>
        /// <returns>Layer 值。如果不包含任何 Layer 则返回 -1。</returns>
        public static int GetFirstLayer(this LayerMask layerMask)
        {
            int layer = -1;

            for (int i = 0; i < 32; i++)
            {
                if (layerMask.HasLayer(i))
                {
                    layer = i;
                    break;
                }
            }

            return layer;
        }
    }
}