using System.Collections.Generic;
using EasyEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.UI.Example
{
    public class ExampleGridItemDataInitializer : MonoBehaviour
    {
        [SerializeField]
        private List<Sprite> sprites;

        [SerializeField]
        private ExampleUIPoolableImage itemPrefab;

        [Message(text = "未实现 IUIPoolableItemLayout 和 AUILayout", messageType = MessageType.Error,
            method = "IsNotIUIPoolableItemLayoutAndAUIlayout")]
        [SerializeField]
        private MonoBehaviour layout;

        private bool IsNotIUIPoolableItemLayoutAndAUIlayout()
        {
            // 必须注入组件/Prefab
            return !this.CheckMustHaveInjection(layout, typeof(IUIPoolableItemLayout))
                   || !this.CheckMustHaveInjection(layout, typeof(AUILayout));
        }

        void Start()
        {
            Assert.IsNotNull(layout);
            var itemDatas = new List<UIPoolableItemData>();

            for (int i = 0; i < sprites.Count; i++)
            {
                var itemData = new UIPoolableItemData(itemPrefab.transform,
                                                      new ExampleUIPoolableImage.ExampleUIPoolableData(sprites[i], i));
                itemDatas.Add(itemData);
            }

            var iLayout = layout as IUIPoolableItemLayout;
            Assert.IsNotNull(iLayout);
            iLayout.SetItemDatas(itemDatas);

            var aLayout = layout as AUILayout;
            Assert.IsNotNull(aLayout);
            aLayout.Layout();
        }
    }
}