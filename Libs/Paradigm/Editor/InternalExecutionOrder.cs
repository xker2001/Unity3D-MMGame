using MMGame.Editor;
using UnityEditor;

namespace MMGame.Paradigm
{
    [InitializeOnLoad]
    public class InternalExecutionOrder : SetExecutionOrder<Internal>
    {
        static InternalExecutionOrder()
        {
            SetOrder(-999);
        }
    }
}