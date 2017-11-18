using System;
using UnityEditor;

namespace MMGame.Editor
{
    [InitializeOnLoad]
    public class SetExecutionOrder<T> : UnityEditor.Editor
    {
        protected static void SetOrder(int order)
        {
            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                Type classType = monoScript.GetClass();

                if (classType != null && monoScript.GetClass().IsSubclassOf(typeof(T)))
                {
                    if (MonoImporter.GetExecutionOrder(monoScript) != order)
                    {
                        MonoImporter.SetExecutionOrder(monoScript, order);
                    }
                }
            }
        }
    }
}