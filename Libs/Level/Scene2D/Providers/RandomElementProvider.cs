using UnityEngine;

namespace MMGame.Scene2D
{
    public class RandomElementProvider<TFactory> : AElementProvider
        where TFactory : AElementFactory
    {
        public override ASceneElement GetNext()
        {
            throw new System.NotImplementedException();
        }
    }
}