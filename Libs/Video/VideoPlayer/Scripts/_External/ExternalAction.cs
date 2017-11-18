using MMGame.Sound;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMGame.VideoPlayer
{
    public class ExternalAction : MonoBehaviour, IExternalAction
    {
        [SerializeField, Required]
        private MusicPlayer musicPlayer;

        [SerializeField]
        private float turnOffMusicDuration = 1;

        [SerializeField]
        private float turnOnMusicDuration = 1;

        public void TurnOffMusic()
        {
            musicPlayer.TurnOff(turnOffMusicDuration);
        }

        public void TurnOnMusic()
        {
            musicPlayer.TurnOn(turnOnMusicDuration);
        }
    }
}