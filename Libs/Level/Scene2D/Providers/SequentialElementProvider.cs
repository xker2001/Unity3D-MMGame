using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.Scene2D
{
    public class SequentialElementProvider<TFactory> : AElementProvider
        where TFactory : AElementFactory
    {
        [SerializeField]
        private TFactory[] elementFactories;

        [SerializeField]
        private bool loop;

        private int index = 0;

        public override ASceneElement GetNext()
        {
            if (elementFactories.Length == 0)
            {
                return null;
            }

            if (!loop && index >= elementFactories.Length)
            {
                return null;
            }

            if (loop && index == elementFactories.Length - 1)
            {
                index = 0;
            }

            Assert.IsFalse(elementFactories[index].IsNull(), "elementFactories[index].IsNull()");
            return elementFactories[index].Create();
        }
    }
}