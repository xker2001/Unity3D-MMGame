using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame
{
    /// <summary>
    /// 服务定位器。
    /// MMGame 工具库服务定位器模式的支持类。理想情况下，整个项目所有单例都应该通过 Service 访问。
    /// 由于 Service Locator 的反模式问题，请最小化使用 Service，并善用 prefab 来管理服务类。
    ///
    /// 使用方法：
    /// 将 Service 挂接到场景中的物体，Service 会跨场景存在。
    ///
    /// 严重注意！
    /// 因为 Service 以 DontDestroyOnLoad 的形式存在，服务类在 Disable 时必须记得将自己
    /// 从 Service 注销。
    /// </summary>
    public class ServiceLocator : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnDisable()
        {
            Clear();
        }

        // ------------------------------------------------------

        private static Dictionary<Type, object> dic = new Dictionary<Type, object>();

        /// <summary>
        /// 将服务实例注册到指定服务类型。
        /// </summary>
        /// <param name="instance">服务实例。</param>
        /// <typeparam name="T">服务类型。</typeparam>
        public static void Register<T>(object instance) where T : class
        {
            Register(typeof(T), instance);
        }

        /// <summary>
        /// 将服务实例注册到指定服务类型。
        /// </summary>
        /// <param name="type">服务类型。</param>
        /// <param name="instance">服务实例。</param>
        /// <exception cref="InvalidCastException">服务实例未继承或未实现服务类型的异常。</exception>
        public static void Register(Type type, object instance)
        {
            Assert.IsTrue(type != null);
            Assert.IsTrue(instance != null);

            if (!(type.IsInstanceOfType(instance)))
            {
                throw new InvalidCastException(
                    string.Format("Service instance does not implement interface: {0}", type.FullName));
            }

            if (dic.ContainsKey(type))
            {
                Debug.LogFormat("<color=maroon><b>Service is already registered : {0}</b></color>", type.FullName);
            }

            dic[type] = instance;
        }

        /// <summary>
        /// 注销指定的服务类型。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        public static void Unregister<T>() where T : class
        {
            Unregister(typeof(T));
        }

        /// <summary>
        /// 注销指定的服务类型。
        /// </summary>
        /// <param name="type">服务类型。</param>
        public static void Unregister(Type type)
        {
            if (dic.ContainsKey(type))
            {
                dic.Remove(type);
            }
        }

        /// <summary>
        /// 获取指定服务类型的服务实例。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>服务实例。</returns>
        /// <exception cref="ArgumentException">获取服务实例失败的异常。</exception>
        public static T Get<T>() where T : class
        {
            object instance;

            if (!dic.TryGetValue(typeof(T), out instance))
            {
                throw new ArgumentException(
                    string.Format("The type to be gotten is not registered : {0}.", typeof(T).FullName));
            }

            return instance as T;
        }

        /// <summary>
        /// 指定的服务类型是否已经注册。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>如果已经注册，返回 true，反之返回 false。</returns>
        public static bool Has<T>() where T : class
        {
            return Has(typeof(T));
        }

        /// <summary>
        /// 指定的服务类型是否已经注册。
        /// </summary>
        /// <param name="type">服务类型。</param>
        /// <returns>如果已经注册，返回 true，反之返回 false。</returns>
        public static bool Has(Type type)
        {
            return dic.ContainsKey(type);
        }

        /// <summary>
        /// 注销所有服务类型。
        /// </summary>
        public static void Clear()
        {
            dic.Clear();
        }
    }
}