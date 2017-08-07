using UnityEngine;

namespace MMGame.Level
{
    abstract public class ALevelMap
    {
        /// <summary>
        /// 关卡场景名称。
        /// </summary>
        abstract public string SceneName { get; protected set; }

        /// <summary>
        /// 参考关卡。
        /// 在 Single 加载模式下参考关卡为上一个关卡；
        /// 在 Additive 加载模式下参考关卡为当前激活的关卡。
        /// </summary>
        abstract public ALevelMap ReferencedLevel { get; set; }
    }
}