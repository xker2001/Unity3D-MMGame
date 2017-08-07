using UnityEngine;

namespace MMGame.EffectFactory.Explosion
{
    /// <summary>
    /// 这是一个实现的范例。
    /// </summary>
    public class CustomExplosionParamAgent : MonoBehaviour, IExplosionParamAgent
    {
        public Vector3 GetGroundEffectPosition(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public LayerMask GetHurtableLayers(ExplosionParamObject bomb)
        {
            throw new System.NotImplementedException();
        }
    }
}