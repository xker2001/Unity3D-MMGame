using UnityEngine;
using System.Collections.Generic;

namespace MMGame
{
    public class ShowFps : MonoBehaviour
    {
        [SerializeField]
        private int fontSize = 40;

        [SerializeField]
        private int paddingH = 20;

        [SerializeField]
        private int paddingV = 20;

        [SerializeField]
        private int smoothTimes = 5;

        [SerializeField]
        private float interval = 0.5f;

        [SerializeField]
        private TextAnchor textAlign;

        [SerializeField]
        private bool pauseOnLowFps;

        [SerializeField]
        private int pauseWhenFpsLowerThan = 40;

        private Queue<float> fpsList = new Queue<float>();
        private float fps;
        private float showFps;
        private GUIStyle style;
        private Rect rect;

        private void Start()
        {
            InvokeRepeating("DoShowFps", 0.1f, interval);
            rect = new Rect(paddingH, paddingV, Screen.width - 2 * paddingH, Screen.height - 2 * paddingV);
            style = new GUIStyle();
            style.fontSize = fontSize;
            style.alignment = textAlign;
        }

        private void UpdateFps()
        {
            fps = 1 / Time.deltaTime;
            fpsList.Enqueue(fps);

            if (fpsList.Count > smoothTimes)
            {
                fpsList.Dequeue();
            }

            fps = 0;

            foreach (float f in fpsList)
            {
                fps += f;
            }

            fps /= fpsList.Count;

            if (pauseOnLowFps && fps <= pauseWhenFpsLowerThan)
            {
                Debug.Break();
            }
        }

        private void DoShowFps()
        {
            showFps = fps;
        }

        private void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }

            UpdateFps();
        }

        private void OnGUI()
        {
            if (fps < 30)
            {
                style.normal.textColor = Color.yellow;
            }
            else
            {
                style.normal.textColor = Color.green;
            }

            GUI.Label(rect, string.Format("{0:F2}", showFps), style);
        }
    }
}