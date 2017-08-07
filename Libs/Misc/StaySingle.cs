using System.Collections.Generic;
using UnityEngine;

namespace MMGame
{
    public class StaySingle : MonoBehaviour
    {
        private static HashSet<string> instances = new HashSet<string>();

        void Awake()
        {
            if (instances.Contains(name))
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                instances.Add(name);
            }
        }
    }
}