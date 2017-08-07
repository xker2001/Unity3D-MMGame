using System.Collections.Generic;
using System;
using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 场景元素对象池，参见 EventPool。
    /// </summary>
    public static class SceneElementPool
    {
        private static Dictionary<Type, Stack<ASceneElement>> freeDic = new Dictionary<Type, Stack<ASceneElement>>();
        private static int totalCreated = 0;

        /// <summary>
        /// 从对象池获取一个指定类型场景元素的实例。
        /// </summary>
        /// <typeparam name="T">场景元素的类型。</typeparam>
        /// <returns>场景元素的实例。</returns>
        public static T New<T>() where T : ASceneElement
        {
            Stack<ASceneElement> freeElements;
            ASceneElement element;

            Type type = typeof(T);

            if (!freeDic.TryGetValue(type, out freeElements))
            {
                freeElements = new Stack<ASceneElement>();
                freeDic.Add(type, freeElements);
            }

            if (freeElements.Count > 0)
            {
                element = freeElements.Pop();
            }
            else
            {
                element = (ASceneElement) Activator.CreateInstance(type);
                totalCreated += 1;
//                Debug.LogFormat("Total created SceneElements: {0}", totalCreated);
            }

            return (T) element;
        }

        /// <summary>
        /// 回收一个场景元素实例到对象池。
        /// </summary>
        /// <param name="element">场景元素实例。</param>
        public static void Delete(ASceneElement element)
        {
            Type type = element.GetType();
            freeDic[type].Push(element);
            element.Reset();
        }
    }
}