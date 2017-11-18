using MMGame.Editor;
using UnityEditor;

namespace MMGame.Paradigm
{
    [InitializeOnLoad]
    public class ExternalExecutionOrder : SetExecutionOrder<External>
    {
        static ExternalExecutionOrder()
        {
            SetOrder(-999);
        }
    }
}