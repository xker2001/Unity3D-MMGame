using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MMGame.UI.Example
{
    public class ExampleUIPoolableImage : AUIPoolableItem
    {
        public class ExampleUIPoolableData
        {
            public Sprite Image;
            public int Index;

            public ExampleUIPoolableData(Sprite image, int index)
            {
                Image = image;
                Index = index;
            }
        }

        public override void SetData(UIPoolableItemData itemData)
        {
            var data = itemData.Data as ExampleUIPoolableData;
            Assert.IsNotNull(data);
            GetComponent<Image>().sprite = data.Image;
            GetComponentInChildren<Text>().text = data.Index.ToString();
        }

        public override void ResetForSpawn() {}

        public override void ReleaseForDespawn() {}
    }
}