using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MMGame.DataType
{
    [Serializable]
    public class RandomFloat
    {
        [SerializeField]
        private bool random;

        [ShowIf("random")]
        [SerializeField]
        private Vector2 valueRange = new Vector2(0, 1);

        [HideIf("random")]
        [SerializeField]
        private float value = 1;

        public float Value
        {
            get { return random ? Random.Range(valueRange.x, valueRange.y) : value; }
        }
    }
}