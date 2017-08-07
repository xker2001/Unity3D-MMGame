using UnityEngine;
using System.Collections.Generic;
using System;

namespace MMGame
{
    public static class Prefab
    {
        private static Dictionary<string, Transform> prefabDictionary = new Dictionary<string, Transform>();

        /// <summary>
        /// 根据 prefab 名称加载 prefab 资源
        /// </summary>
        /// <param name="name">Prefab 名称</param>
        /// <returns>Prefab 资源</returns>
        /// <exception cref="ApplicationException">Prefab 资源没有找到</exception>
        public static Transform Load(string name)
        {
            Transform prefab;

            if (!prefabDictionary.TryGetValue(name, out prefab))
            {
                GameObject go = Resources.Load(name) as GameObject;

                if (go == null)
                {
                    throw new ApplicationException(string.Format("Prefab resource {0} not found", name));
                }

                prefab = go.transform;
                prefabDictionary.Add(name, prefab);
            }

            return prefab;
        }
    }

    // TODO 释放资源、释放所有资源

}