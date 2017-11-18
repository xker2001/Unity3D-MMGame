using MMGame.Editor;
using UnityEditor;

namespace MMGame.UI
{
    [InitializeOnLoad]
    public class UIMoveExecutionOrder : SetExecutionOrder<AUIMove>
    {
        static UIMoveExecutionOrder()
        {
            SetOrder(1);
        }
    }
}