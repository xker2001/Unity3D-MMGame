using UnityEngine;

namespace MMGame.UI
{
    /// <summary>
    /// UI 对象池控件数据类，用于支持通过数据类创建实际控件。
    /// </summary>
    public class UIPoolableItemData
    {
        /// <summary>
        /// 实际控件的 prefab。
        /// </summary>
        public Transform Prefab { get; set; }

        /// <summary>
        /// 实际控件的附加数据。
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 构造函数。
        /// </summary>
        public UIPoolableItemData(Transform prefab, object data)
        {
            Prefab = prefab;
            Data = data;
        }
    }
}