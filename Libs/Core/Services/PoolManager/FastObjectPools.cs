using UnityEngine;

namespace MMGame
{
    [DisallowMultipleComponent]
    public class FastObjectPools : MonoBehaviour
    {
        private void OnDisable()
        {
            FastPoolManager.DestroyAll();
        }
    }
}