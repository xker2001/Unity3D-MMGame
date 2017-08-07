using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.Scene2D
{
    [Serializable]
    public class SimpleElementFactory : AElementFactory
    {
        [SerializeField]
        private Transform prefab;

        [SerializeField]
        private string poolName;

        public override bool IsNull()
        {
            return prefab == null;
        }

        public override ASceneElement Create()
        {
            Assert.IsFalse(IsNull());
            SimpleElement element = SceneElementPool.New<SimpleElement>();
            element.SetPrefab(prefab, poolName);
            return element;
        }
    }
}