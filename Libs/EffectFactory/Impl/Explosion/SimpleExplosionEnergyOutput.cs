using System;
using UnityEngine;

namespace MMGame.EffectFactory.Explosion
{
    public class SimpleExplosionEnergyOutput : PoolBehaviour, IExplosionEnergyOutput
    {
        [SerializeField]
        private float delayOfDamage = 0.1f;
        [SerializeField]
        private float damageRange = 10;
        [SerializeField]
        private int maxVictimNumber = 20;

        /// <summary>
        /// 伤害计算组件。
        /// </summary>
        public IDamage Damage { get; private set; }

        /// <summary>
        /// 摄像机震动组件。
        /// </summary>
        public ICameraShake CameraShake { get; private set; }

        private GameObject sender;
        private Action onComplete;
        private ExplosionParamObject bomb;
        private Collider[] colliders;
        private Transform xform;

        void Awake()
        {
            xform = transform;
            Damage = GetComponent<IDamage>();
            CameraShake = GetComponent<ICameraShake>();
            colliders = new Collider[maxVictimNumber];
        }

        void OnDisable()
        {
            Damage.OnReset();
        }

        public void Send(GameObject sender, ExplosionParamObject bomb, Vector3 position, Action onComplete)
        {
            this.sender = sender;
            this.bomb = bomb;
            this.onComplete = onComplete;

            if (CameraShake != null)
            {
                CameraShake.Shake();
            }

            Invoke("SendDamage", delayOfDamage);
        }

        private void SendDamage()
        {
            if (damageRange > 0 && Damage != null)
            {
                Damage.Sender = sender; // 如果伤害有阵营等附加信息，在 Sender 中携带。
                int victimNum = GetVictimsByLayer(colliders);

                for (int i = 0; i < victimNum; i++)
                {
                    Damage.Apply(colliders[i].gameObject, colliders[i].transform.position - xform.position);
                }
            }

            if (onComplete != null)
            {
                onComplete();
            }
        }

        private int GetVictimsByLayer(Collider[] results)
        {
            return Physics.OverlapSphereNonAlloc(xform.position, damageRange, results,
                                                 ExplosionParamSettings.Params.Agent.GetHurtableLayers(bomb));
        }

        public override void ResetForSpawn()
        {
        }

        public override void ReleaseForDespawn()
        {
        }
    }
}