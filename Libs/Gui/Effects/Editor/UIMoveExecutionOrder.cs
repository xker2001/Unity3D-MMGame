using System;
using UnityEditor;

namespace MMGame.UI
{
    [InitializeOnLoad]
    public class UIMoveExecutionOrder : Editor
    {
        private const int order = 1;

        static UIMoveExecutionOrder()
        {
            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                Type classType = monoScript.GetClass();

                if (classType != null && monoScript.GetClass().IsSubclassOf(typeof(AUIMove)))
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