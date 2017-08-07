using UnityEngine;
using MMGame;

namespace MMGame
{
    /// <summary>
    /// Unity inspector 配置专用类。
    ///  </summary>
    /// <typeparam name="T">派生配置类的类型。</typeparam>
    /// <example>
    /// 定义一个新的配置类：
    /// <code>
    /// [System.Serializable]
    /// public class ExampleSettings : Settings<ExampleSettings>
    /// {
    ///     public float Number = 10;
    /// }
    ///  </code>
    /// 从配置类中获取配置参数：
    /// <code>
    /// float number = ExampleSettings.Params.Number;
    /// </code>
    /// </example>
    abstract public class Settings<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Params;

        protected virtual void Awake()
        {
            Params = this as T;
        }
    }
}