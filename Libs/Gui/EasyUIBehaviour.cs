using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace MMGame.UI
{
    public class EasyUIBehaviour : UIBehaviour
    {
        protected RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
            Assert.IsNotNull(rectTransform);
        }
    }
}