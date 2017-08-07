using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 摄像机控制组件和摄像机目标之间的桥梁，通常挂接在 Player 角色上。
    ///
    /// 桥接方式有：
    /// 1. 获取摄像机控制组件并将 target 加入进去。
    /// 2. 被注入到摄像机组件并提供 target。
    ///
    /// 该接口设计尚未定型，需要更多实践经验。
    /// </summary>
    abstract public class ACameraTarget : MonoBehaviour
    {
        abstract public Transform Target { get; }
    }
}