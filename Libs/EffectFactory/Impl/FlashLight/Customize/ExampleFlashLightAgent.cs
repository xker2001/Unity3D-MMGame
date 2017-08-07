using MMGame;
using UnityEngine;

namespace MMGame.EffectFactory.FlashLight
{
    public class ExampleFlashLightAgent : MonoBehaviour, IFlashLightAgent
    {
        public Vector3 GetGroundLightPosition(Vector3 position)
        {
            position.y = 0 + FlashLightSettings.Params.GroundOffset;
            return position;
        }
    }
}