using System;
using System.Collections.Generic;
using EasyEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame
{
    /// <summary>
    /// 组件初始化器，可以对实现了 IInitializable 接口的组件按顺序进行初始化。
    /// 通常用于初始化 Manager 级别的组件，注意可以嵌套使用。
    /// </summary>
    public class ComponentInitializer : MonoBehaviour, IInitializable
    {
        public enum ExecuteInit
        {
            Manually,
            OnAwake,
            OnStart
        }

        [SerializeField]
        private ExecuteInit executeInit;

        [Message(text = "有对象没有实现 IInitializable 接口！", messageType = MessageType.Error, method = "IsNotValid")]
        [SerializeField]
        private List<MonoBehaviour> components;

        private bool IsNotValid()
        {
            bool result = false;
            Type type = typeof(IInitializable);

            foreach (MonoBehaviour component in components)
            {
                bool isValid = component == null || type.IsInstanceOfType(component);

                if (!isValid)
                {
                    Debug.LogError(string.Format("组件 {0} 未实现 Initializable!", component.name));
                    result = true;
                }
            }

            return result;
        }

        private void Awake()
        {
            if (executeInit == ExecuteInit.OnAwake)
            {
                Init();
            }
        }

        private void Start()
        {
            if (executeInit == ExecuteInit.OnStart)
            {
                Init();
            }
        }

        public void Init()
        {
            for (int i = 0; i < components.Count; i++)
            {
                MonoBehaviour component = components[i];

                if (component == null)
                {
                    continue;
                }

                var initializable = component as IInitializable;
                Assert.IsNotNull(initializable, string.Format("组件 {0} 未实现 Initializable!", component.name));
                initializable.Init();
            }
        }
    }
}