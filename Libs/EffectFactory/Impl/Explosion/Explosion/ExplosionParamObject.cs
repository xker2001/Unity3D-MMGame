using UnityEngine;
using System.Collections;

namespace MMGame.EffectFactory.Explosion
{
    /// <summary>
    /// 爆炸专用 ParamObject，向指定的 layers 输出伤害。
    /// 爆炸的输出由挂接的 IExplosionEnergyOutput 组件完成。
    /// </summary>
    public class ExplosionParamObject : PlayOneShotParamObject
    {
        private ExplosionParamFactory factory;
        private PlayOneShotParamObject explodeEffObj;
        private PlayOneShotParamObject groundEffObj;
        private bool isPlaying;
        private Transform xform;

        /// <summary>
        /// 爆炸的施放者。
        /// </summary>
        public GameObject Sender { get; set; }

        public void SetParameters(ExplosionParamFactory factory)
        {
            this.factory = factory;
        }

        // ------------------------------------------------------

        void Awake()
        {
            xform = transform;
        }

        public override void PlayAndDestroy()
        {
            if (factory.IsNull())
            {
                PoolManager.Despawn(xform);
                return;
            }

            if (isPlaying)
            {
                return;
            }

            Explode();
        }

        public override void Destroy()
        {
            CancelInvoke();

            if (explodeEffObj)
            {
                explodeEffObj.Destroy();
                explodeEffObj = null;
            }

            if (groundEffObj)
            {
                groundEffObj.Destroy();
                groundEffObj = null;
            }

            isPlaying = false;
            PoolManager.Despawn(xform);
        }

        private void Explode()
        {
            if (!factory.ExplodeEffect.IsNull())
            {
                explodeEffObj = factory.ExplodeEffect.Create(xform.position);
                explodeEffObj.PlayAndDestroy();
            }

            if (!factory.GroundEffect.IsNull())
            {
                Vector3 groundPoint = ExplosionParamSettings.Params.Agent.GetGroundEffectPosition(xform.position);

                if (xform.position.y - groundPoint.y <= factory.MaxHeightToShowGroundEffect)
                {
                    groundEffObj = factory.GroundEffect.Create(groundPoint);
                    groundEffObj.PlayAndDestroy();
                }
            }

            if (factory.EnergyOutput != null)
            {
                factory.EnergyOutput.Send(Sender, this, xform.position, OnOutputComplete);
            }
        }

        private void OnOutputComplete()
        {
            explodeEffObj = null; // 放它自生自灭
            groundEffObj = null;
            isPlaying = false;
            PoolManager.Despawn(xform);
        }
    }
}