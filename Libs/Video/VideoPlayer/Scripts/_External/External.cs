using Sirenix.OdinInspector;
using UnityEngine;

namespace MMGame.VideoPlayer
{
    public class External : Paradigm.External
    {
        [SerializeField, Required]
        [ValidateInput("IsIExternalAction", "MMGame.VideoPlayer.IExternalAction is required.")]
        private MonoBehaviour action;

        private bool IsIExternalAction(MonoBehaviour field)
        {
            return field is IExternalAction;
        }

        public static IExternalAction Action { get; private set; }

        private void Awake()
        {
            Action = action as IExternalAction;
        }
    }
}