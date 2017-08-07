using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 场景元素属性修改器。
    /// </summary>
    abstract public class ASpawnElementModifier : MonoBehaviour
    {
        abstract public void Modify(ASceneElement element);
    }
}