using UnityEngine;
using DG.Tweening;

namespace MMGame.EffectFactory.FlashLight
{
    public class FlashLightParamObject : PlayOneShotParamObject
    {
        // public
        public float Alpha;

        // private
        private FlashLightParamFactory factory;
        private Transform groundLightXform;
        private Renderer groundLightRenderer;
        private Sequence seqLight;
        private Sequence seqGroundLight;
        private bool isPlaying;
        private Light lightSource;
        // tween params
        private float fadeInTime;
        private float fadeOutTime;
        private float livingTime;
        private float lightIntensity;
        private float groundFadeInTime;
        private float groundFadeOutTime;
        private float groundLivingTime;
        private float groundLightAlpha;

        public void SetParameters(FlashLightParamFactory factory)
        {
            this.factory = factory;
        }

        // ------------------------------------------------------

        void Awake()
        {
            lightSource = GetComponent<Light>();
        }


        void OnEnable()
        {
            lightSource.intensity = 0;
        }

        // ------------------------------------------------------

        public override void PlayAndDestroy()
        {
            if (factory.IsNull())
            {
                isPlaying = false;
                PoolManager.Despawn(transform);
                return;
            }

            if (isPlaying)
            {
                return;
            }

            PlayDynamicLight();

            if (factory.EnableGroundLight)
            {
                PlayFakeGroundLighting();
            }

            isPlaying = true;
        }

        public override void Destroy()
        {
            ReleaseResources();
            isPlaying = false;
            PoolManager.Despawn(transform);
        }

        // ------------------------------------------------------

        private void PlayDynamicLight()
        {
            InitDynamicLight();

            if (seqLight != null && !DynamicTweenParamsIsChanged())
            {
                seqLight.Restart();
            }
            else
            {
                SaveDynamicLightTweenParams();

                if (seqLight != null)
                {
                    seqLight.Kill();
                }

                seqLight = DOTween.Sequence()
                    .SetAutoKill(false)
                    .OnComplete(Destroy);

                seqLight.Append(lightSource.DOIntensity(factory.LightIntensity, factory.FadeInTime));

                if (factory.LivingTime > 0)
                {
                    seqLight.Append(lightSource.DOIntensity(factory.LightIntensity + 0.01f, factory.LivingTime));
                }

                seqLight.Append(lightSource.DOIntensity(0, factory.FadeOutTime));
                seqLight.Play();
            }
        }

        private void PlayFakeGroundLighting()
        {
            Vector3 groundPoint = FlashLightSettings.Params.Agent.GetGroundLightPosition(transform.position);

            if (!groundLightXform)
            {
                groundLightXform = PoolManager.Spawn(factory.LightPrefab.name,
                    factory.LightPrefab,
                    groundPoint,
                    Quaternion.LookRotation(Vector3.up));
                groundLightRenderer = groundLightXform.GetComponent<Renderer>();
            }
            else
            {
                groundLightXform.gameObject.SetActive(true);
                groundLightXform.position = groundPoint;
                groundLightXform.forward = Vector3.up;
            }

            float scale = factory.LightRange * factory.GroundLightScale;
            groundLightXform.localScale = new Vector3(scale, scale, scale);

            Color color = factory.LightColor;
            color.a = 0;
            groundLightRenderer.material.SetColor("_TintColor", color);

            // tween
            if (seqGroundLight != null && !GroundLightTweenParamsIsChanged())
            {
                seqGroundLight.Restart();
            }
            else
            {
                SaveGroundLightTweenParams();

                if (seqGroundLight != null)
                {
                    seqGroundLight.Kill();
                }

                seqGroundLight = DOTween.Sequence()
                    .SetAutoKill(false)
                    .OnUpdate(UpdateGroundLight);

                seqGroundLight.Append(DOTween.To(() => Alpha, x => Alpha = x,
                    factory.GroundLightAlpha, factory.FadeInTime));

                if (factory.LivingTime > 0)
                {
                    seqGroundLight.Append(DOTween.To(() => Alpha, x => Alpha = x,
                        factory.GroundLightAlpha + 0.01f, factory.LivingTime));
                }

                seqGroundLight.Append(DOTween.To(() => Alpha, x => Alpha = x,
                    0, factory.FadeOutTime));
                seqGroundLight.Play();
            }
        }

        private void InitDynamicLight()
        {
            lightSource.color = factory.LightColor;
            lightSource.range = factory.LightRange;
            lightSource.intensity = 0;
        }

        private void SaveDynamicLightTweenParams()
        {
            fadeInTime = factory.FadeInTime;
            livingTime = factory.LivingTime;
            fadeOutTime = factory.FadeOutTime;
            lightIntensity = factory.LightIntensity;
        }

        private bool DynamicTweenParamsIsChanged()
        {
            return !Mathf.Approximately(fadeInTime, factory.FadeInTime) ||
                   !Mathf.Approximately(livingTime, factory.LivingTime) ||
                   !Mathf.Approximately(fadeOutTime, factory.FadeOutTime) ||
                   !Mathf.Approximately(lightIntensity, factory.LightIntensity);
        }

        private void SaveGroundLightTweenParams()
        {
            groundFadeInTime = factory.FadeInTime;
            groundLivingTime = factory.LivingTime;
            groundFadeOutTime = factory.FadeOutTime;
            groundLightAlpha = factory.GroundLightAlpha;
        }

        private bool GroundLightTweenParamsIsChanged()
        {
            return !Mathf.Approximately(groundFadeInTime, factory.FadeInTime) ||
                   !Mathf.Approximately(groundLivingTime, factory.LivingTime) ||
                   !Mathf.Approximately(groundFadeOutTime, factory.FadeOutTime) ||
                   !Mathf.Approximately(groundLightAlpha, factory.GroundLightAlpha);
        }


        private void UpdateGroundLight()
        {
            Color color = factory.LightColor;
            color.a = Alpha;
            groundLightRenderer.material.color = color;
        }

        private void ReleaseResources()
        {
            if (seqLight != null)
            {
                seqLight.Kill();
                seqLight = null;
            }

            if (seqGroundLight != null)
            {
                seqGroundLight.Kill();
                seqGroundLight = null;
            }

            if (groundLightXform)
            {
                PoolManager.Despawn(groundLightXform);
                groundLightXform = null;
            }
        }
    }
}