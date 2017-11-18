using MMGame.Editor;
using UnityEditor;

namespace MMGame.Paradigm
{
    [InitializeOnLoad]
    public class AliasExecutionOrder : SetExecutionOrder<Alias>
    {
        static AliasExecutionOrder()
        {
            SetOrder(-999);
        }
    }
}