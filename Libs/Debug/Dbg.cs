using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Reflection;
using UnityEngine.Assertions;

namespace MMGame
{
    public class Dbg : MonoBehaviour
    {
        [SerializeField]
        private int maxFps = 60;
        [SerializeField]
        private bool showLog = true;
        [SerializeField]
        private GameObject logBox;
        [SerializeField]
        private GameObject buttonSwitchLog;
        [SerializeField]
        private GameObject buttonClearLog;
        [SerializeField]
        private Text logText;

        private static Text textMessage;
        private static float currentTime;

        private void Awake()
        {
            textMessage = logText;
            textMessage.text = "\n";
            Application.targetFrameRate = maxFps;

            if (showLog)
            {
                buttonSwitchLog.SetActive(true);
                buttonClearLog.SetActive(true);
                Application.logMessageReceived += HandleLog;
            }
            else
            {
                buttonSwitchLog.SetActive(false);
                buttonClearLog.SetActive(false);
            }
        }

        // ------------------------------------------------------
        // Log panel

        /// <summary>
        /// 开关消息面板的显示状态。
        /// </summary>
        public void SwitchMessageBox()
        {
            logBox.SetActive(!logBox.activeSelf);
        }

        /// <summary>
        /// 清空消息面板中的内容。
        /// </summary>
        public void ClearMessageBox()
        {
            textMessage.text = "\n";
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                Log(logString, Color.red);
                Log("<i>" + stackTrace + "</i>");
            }
            else if (type == LogType.Log)
            {
                Log(logString, Color.white);
            }
            else if (type == LogType.Assert)
            {
                Log(logString, Color.yellow);
            }
        }

        /// <summary>
        /// 将对象的信息打印到消息面板。
        /// </summary>
        /// <param name="obj">待打印其信息的对象。</param>
        public static void Log(object obj)
        {
            textMessage.text += "\n" + obj.ToString();
        }

        /// <summary>
        /// 将对象的信息用指定颜色打印到消息面板。
        /// </summary>
        /// <param name="obj">待打印其信息的对象。</param>
        /// <param name="color">信息文本的颜色。</param>
        public static void Log(object obj, Color color)
        {
            string colorName = "white";

            if (color == Color.blue)
            {
                colorName = "blue";
            }
            else if (color == Color.cyan)
            {
                colorName = "cyan";
            }
            else if (color == Color.gray)
            {
                colorName = "grey";
            }
            else if (color == Color.green)
            {
                colorName = "green";
            }
            else if (color == Color.grey)
            {
                colorName = "grey";
            }
            else if (color == Color.magenta)
            {
                colorName = "magenta";
            }
            else if (color == Color.red)
            {
                colorName = "red";
            }
            else if (color == Color.white)
            {
                colorName = "white";
            }
            else if (color == Color.yellow)
            {
                colorName = "yellow";
            }

            textMessage.text += "\n<color=" + colorName + ">" + obj.ToString() + "</color>";
        }

        // ------------------------------------------------------

        /// <summary>
        /// 创建一个指定颜色和大小的立方体。
        /// </summary>
        /// <param name="position">立方体的创建位置。</param>
        /// <param name="scale">立方体边长，单位米。</param>
        /// <param name="color">立方体颜色</param>
        public static GameObject Cube(Vector3 position, float scale, Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.transform.localScale = new Vector3(scale, scale, scale);
            cube.GetComponent<Renderer>().material.color = color;
            return cube;
        }

        /// <summary>
        /// 获取调用者的类名和方法名。
        /// </summary>
        public static string GetCallerName(int index)
        {
            var st = new StackTrace(true);
            MethodBase method = st.GetFrame(index).GetMethod();
            Assert.IsNotNull(method.ReflectedType);
            return method.ReflectedType.Name + "." + method.Name;
        }

        /// <summary>
        /// 将调用者（打印信息者）的类名和方法名添加到信息的前面。
        /// </summary>
        /// <param name="message">待处理的消息文本。</param>
        public static string AddCallerToMessage(string message)
        {
            return string.Format("[{0}]: {1}", GetCallerName(2), message);
        }

        /// <summary>
        /// 开始记录当前运行的时间（供检查时间开销）。
        /// </summary>
        public static void BeginTimeCost()
        {
            currentTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 打印时间开销信息。
        /// </summary>
        /// <param name="message">时间开销数值前的提示信息。</param>
        public static void LogTimeCost(string message = "time cost")
        {
            UnityEngine.Debug.Log(string.Format("{0}: {1}", message, Time.realtimeSinceStartup - currentTime));
            currentTime = Time.realtimeSinceStartup;
        }
    }
}