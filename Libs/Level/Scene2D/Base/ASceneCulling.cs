using System.Collections.Generic;
using UnityEngine;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 场景元素可见性管理器，根据指定规则对场景元素进行可见性管理，
    /// 包括显示/隐藏/销毁场景元素。
    ///
    /// 注意，场景元素重置、重生等逻辑在场景元素的回调方法中实现。
    /// </summary>
    abstract public class ASceneCulling : MonoBehaviour
    {
        [SerializeField]
        protected ALayerCamera layerCamera;

        virtual protected void OnEnable()
        {
            layerCamera.CameraLateUpdated += UpdateCulling;
        }

        virtual protected void OnDisable()
        {
            layerCamera.CameraLateUpdated -= UpdateCulling;
        }

        /// <summary>
        /// 添加一个场景元素。
        /// </summary>
        /// <param name="element">场景元素。</param>
        abstract public void AddElement(ASceneElement element);

        /// <summary>
        /// 添加一批场景元素。
        /// </summary>
        /// <param name="elements">场景元素列表。</param>
        abstract public void AddElements(IList<ASceneElement> elements);

        /// <summary>
        /// 清除所有场景元素。
        /// </summary>
        abstract public void Clear();

        /// <summary>
        /// 更新场景元素的可见性。
        /// </summary>
        /// <param name="layerCamera"></param>
        abstract protected void UpdateCulling(ALayerCamera layerCamera);
    }
}