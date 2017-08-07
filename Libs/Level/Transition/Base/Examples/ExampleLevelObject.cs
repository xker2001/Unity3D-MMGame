using UnityEngine;

namespace MMGame.Level.Example
{
    /// <summary>
    /// 输出关卡加载成功后 Awake() 等的调用顺序信息。
    /// </summary>
    public class ExampleLevelObject : MonoBehaviour
    {
        void Awake()
        {
            Debug.LogFormat("<b>Awake()</b>");
        }

        void OnEnable()
        {
            Debug.LogFormat("<b>OnEnable()</b>");
        }

        void Start()
        {
            Debug.LogFormat("<b>Start()</b>");
        }
    }
}