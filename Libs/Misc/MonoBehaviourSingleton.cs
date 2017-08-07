using UnityEngine;
using System.Collections;

/// <summary>
/// 继承自 MonoBehaviour 的单例基类。
/// 这里的 Singleton 在获取失败的时候不会自动创建，必须事先放置到场景中。这样做的考量在于，
/// 大部分基于 MonoBehaviour 的单例类需要事先在 Inspector 中进行一些配置，因此强迫使用
/// 者手动将其添加到场景中，以防止可能的疏漏。
///
/// 2016.12.13 注：从现在起所有的 Singleton 都应该改为注册到 Service，因此不应当再有类
/// 继承 MonoBehaviourSingleton。
/// </summary>
abstract public class MonoBehaviourSingleton<T> : MonoBehaviour
    where T : MonoBehaviourSingleton<T>
{
    protected static T singleton = null;

    public static T Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = (T) FindObjectOfType (typeof (T));
            }

            return singleton;
        }
    }
}
