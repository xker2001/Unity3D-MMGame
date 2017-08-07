using System;
using UnityEngine;

namespace MMGame
{
    /// <summary>
    /// 配合「编写高内聚低耦合的模块」范式的依赖注入工具，对注入的对象进行类型判断。
    /// 该范式主要通过在 Inspector 面板中拖入继承/实现了指定类型的组件或 prefab 来完成注入。
    /// </summary>
    public static class ComponentDIExtension
    {
        //--------------------------------------------------
        // 检测必须的注入
        //--------------------------------------------------

        /// <summary>
        /// 检查在 Inspector 中必须注入的组件。
        /// </summary>
        /// <param name="self">被注入组件所在的 Component。</param>
        /// <param name="component">注入的组件。</param>
        /// <param name="type">约束类型，通常是接口类型。</param>
        /// <returns>如果组件正确注入返回 true，反之返回 false。</returns>
        public static bool CheckMustHaveInjection(this Component self, Component component, Type type)
        {
            if (component == null)
            {
                Debug.LogError(string.Format("{0} 组件未注入，类型：{1}!", self.name, type.FullName));
                return false;
            }
            else if (!type.IsInstanceOfType(component))
            {
                Debug.LogError(string.Format("{0} 注入错误组件，期待: {1}!", self.name, type.FullName));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查在 Inspector 中必须注入的 GameObject。
        /// </summary>
        /// <param name="self">被注入组件所在的 Component。</param>
        /// <param name="prefab">注入的 prefab。</param>
        /// <param name="type">约束类型。</param>
        /// <returns>如果组件正确注入返回 true，反之返回 false。</returns>
        public static bool CheckMustHaveInjection(this Component self, GameObject prefab, Type type)
        {
            if (prefab == null)
            {
                Debug.LogError(string.Format("{0} 组件未注入，类型：{1}!", self.name, type));
                return false;
            }
            else if (prefab.GetComponent(type) == null)
            {
                Debug.LogError(string.Format("{0} 注入错误组件，期待: {1}!", self.name, type));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查在 Inspector 中必须注入的 Transform。
        /// </summary>
        /// <param name="self">被注入组件所在的 Component。</param>
        /// <param name="prefab">注入的 prefab。</param>
        /// <param name="type">约束类型。</param>
        /// <returns>如果没有注入，或正确注入，返回 true，反之返回 false。</returns>
        public static bool CheckMustHaveInjection(this Component self, Transform prefab, Type type)
        {
            if (prefab == null)
            {
                Debug.LogError(string.Format("{0} 组件未注入，类型：{1}!", self.name, type));
                return false;
            }
            else if (prefab.GetComponent(type) == null)
            {
                Debug.LogError(string.Format("{0} 注入错误组件，期待: {1}!", self.name, type));
                return false;
            }

            return true;
        }

        //--------------------------------------------------
        // 检测可选的注入
        //--------------------------------------------------

        /// <summary>
        /// 检查在 Inspector 中可选注入的组件。
        /// </summary>
        /// <param name="self">被注入组件所在的 Component。</param>
        /// <param name="component">注入的组件。</param>
        /// <param name="type">约束类型，通常是接口类型。</param>
        /// <returns>如果没有注入，或正确注入，返回 true，反之返回 false。</returns>
        public static bool CheckOptionalInjection(this Component self, Component component, Type type)
        {
            bool result = component == null || type.IsInstanceOfType(component);

            if (!result)
            {
                Debug.LogError(string.Format("{0} 注入错误组件，期待: {1}!", self.name, type.FullName));
            }

            return result;
        }

        /// <summary>
        /// 检查在 Inspector 中可选注入的 GameObject。
        /// </summary>
        /// <param name="self">被注入组件所在的 Component。</param>
        /// <param name="prefab">注入的 prefab。</param>
        /// <param name="type">约束类型。</param>
        /// <returns>如果没有注入，或正确注入，返回 true，反之返回 false。</returns>
        public static bool CheckOptionalInjection(this Component self, GameObject prefab, Type type)
        {
            bool result = prefab == null || prefab.GetComponent(type) != null;

            if (!result)
            {
                Debug.LogError(string.Format("{0} 注入错误组件，期待: {1}!", self.name, type));
            }

            return result;
        }

        /// <summary>
        /// 检查在 Inspector 中可选注入的 Transform。
        /// </summary>
        /// <param name="self">被注入组件所在的 Component。</param>
        /// <param name="prefab">注入的 prefab。</param>
        /// <param name="type">约束类型。</param>
        /// <returns>如果没有注入，或正确注入，返回 true，反之返回 false。</returns>
        public static bool CheckOptionalInjection(this Component self, Transform prefab, Type type)
        {
            bool result = prefab == null || prefab.GetComponent(type) != null;

            if (!result)
            {
                Debug.LogError(string.Format("{0} 注入错误组件，期待: {1}!", self.name, type));
            }

            return result;
        }
    }
}