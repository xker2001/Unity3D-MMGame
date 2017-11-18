using System;
using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// 支持 Layout 适配其内容大小的接口。
    /// 第三方组件可以根据该接口的信息将 Layout 修改为合适的大小。
    /// </summary>
    public interface IUISizeFitableLayout
    {
        /// <summary>
        /// 内容大小改变事件。
        /// </summary>
        event Action<float, float> SizeChanged;

        /// <summary>
        /// 内容的大小。
        /// </summary>
        Vector2 Size { get; }

        /*
        /// <summary>
        /// 计算内容的大小。
        /// </summary>
        private void CalculateFitableSize()
        {
            ...
            ...
            SizeDelta = ...;

            if (SizeChanged != null)
            {
                SizeChanged(SizeDelta);
            }
        }
        */
    }
}