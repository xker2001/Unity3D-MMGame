using UnityEngine;
using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace MMGame
{
    //--------------------------------------------------
    // 修饰属性
    //--------------------------------------------------

    /// <summary>
    /// 按帧调用的基本 Attribute。
    /// </summary>
    abstract public class TestBaseAttribute : Attribute
    {
        public int Frame { get; set; }
    }

    /// <summary>
    /// 按时间（秒）调用的基本 Attribute。
    /// </summary>
    abstract public class TestBaseTimeAttribute : Attribute
    {
        public float Time { get; set; }
    }

    /// <summary>
    /// 按帧调用的测试方法，如果抛出异常则测试失败。
    /// </summary>
    public class TestMethod : TestBaseAttribute
    {
        public TestMethod(int frame = 0)
        {
            Frame = frame;
        }
    }

    /// <summary>
    /// 按时间调用的测试方法，如果抛出异常则测试失败。
    /// </summary>
    public class TimeTestMethod : TestBaseTimeAttribute
    {
        public TimeTestMethod(float time = 0)
        {
            Time = time;
        }
    }

    /// <summary>
    /// 按帧调用的测试方法，抛出异常则测试成功。
    /// 注意 TestError 装饰的方法必须返回 int 类型，且返回继承的属性 LineNumber。
    /// 注意 TestError 拦截不了 Unity.Assertions 的输出信息（用的是 Log 输出）。
    /// </summary>
    public class TestError : TestBaseAttribute
    {
        public TestError(int frame = 0)
        {
            Frame = frame;
        }
    }

    /// <summary>
    /// 按时间调用的测试方法，抛出异常则测试成功。
    /// </summary>
    public class TimeTestError : TestBaseTimeAttribute
    {
        public TimeTestError(float time = 0)
        {
            Time = time;
        }
    }

    /// <summary>
    /// 按帧调用的测试方法，不捕捉异常。
    /// 在 TestMethod 属性下只有由 UnitTest 执行的 Assert 会被正确输出，要调试其他错误信息，请使用 TestDebug 属性修饰。
    /// 通常的情境是将 TestMethod 或 TestError 临时修改成 TestDebug 进行调试。
    /// </summary>
    public class TestDebug : TestBaseAttribute
    {
        public TestDebug(int frame = 0)
        {
            Frame = frame;
        }
    }

    /// <summary>
    /// 按时间调用的测试方法，不捕捉异常。
    /// </summary>
    public class TimeTestDebug : TestBaseTimeAttribute
    {
        public TimeTestDebug(float time = 0)
        {
            Time = time;
        }
    }

    /// <summary>
    /// 按帧调用的方法，用来在测试前做一些准备工作。
    /// </summary>
    public class TestRun : TestBaseAttribute
    {
        public TestRun(int frame = 0)
        {
            Frame = frame;
        }
    }

    /// <summary>
    /// 按时间调用的方法，用来在测试前做一些准备工作。
    /// </summary>
    public class TimeTestRun : TestBaseTimeAttribute
    {
        public TimeTestRun(float time = 0)
        {
            Time = time;
        }
    }


    /// <summary>
    /// 所有单元测试的基类。提供断言方法，并负责分批次调用测试方法。
    /// </summary>
    abstract public class UnitTest : MonoBehaviour
    {
        /// 当前执行的批次
        private int frame;
        private float time;

        /// 所有需要执行的帧批次列表
        private List<int> frameIndexes = new List<int>();

        /// 所有的测试方法
        private MethodInfo[] methods;
        private List<MethodInfo> frameMethods = new List<MethodInfo>();
        private List<MethodInfo> timeMethods = new List<MethodInfo>();

        public int LineNumber
        {
            get { return (new StackFrame(1, true)).GetFileLineNumber(); }
        }

        void Start()
        {
            FilterTestMethods(); // 获取所有方法并分类到 frame 和 time 两种列表中
            CreateBatchIndexes(); // 获取所有有效帧批次列表
        }

        void Update()
        {
            InvokeFrameMethods();
            InvokeTimeMethods();
        }

        private void InvokeTimeMethods()
        {
            string testFileName = this.GetType().Name;

            float newTime = time + Time.deltaTime;

            for (int i = 0; i < timeMethods.Count; i++)
            {
                MethodInfo method = timeMethods[i];

                TimeTestDebug timeDebugAttr =
                    Attribute.GetCustomAttribute(method, typeof(TimeTestDebug)) as TimeTestDebug;

                TimeTestMethod timeMethodAttr =
                    Attribute.GetCustomAttribute(method, typeof(TimeTestMethod)) as TimeTestMethod;

                TimeTestError timeErrorAttr =
                    Attribute.GetCustomAttribute(method, typeof(TimeTestError)) as TimeTestError;

                TimeTestRun timeRunAttr =
                    Attribute.GetCustomAttribute(method, typeof(TimeTestRun)) as TimeTestRun;

                if (timeDebugAttr != null && timeDebugAttr.Time >= time && timeDebugAttr.Time <= newTime)
                {
                    // ReSharper disable once HeapView.BoxingAllocation
                    UnityEngine.Debug.LogWarning(
                                                 string.Format("--------------- {0} Time: {1} / {2} ----------------",
                                                               testFileName, timeDebugAttr.Time, newTime));
                    method.Invoke(this, null);
                }

                if (timeRunAttr != null && timeRunAttr.Time >= time && timeRunAttr.Time <= newTime)
                {
                    UnityEngine.Debug.LogWarning(
                                                 string.Format("--------------- {0} Time: {1} / {2} ----------------",
                                                               testFileName, timeRunAttr.Time, newTime));
                    method.Invoke(this, null);
                }

                if (timeMethodAttr != null && timeMethodAttr.Time >= time && timeMethodAttr.Time <= newTime)
                {
                    UnityEngine.Debug.LogWarning(
                                                 string.Format("--------------- {0} Time: {1} / {2} ----------------",
                                                               testFileName, timeMethodAttr.Time, newTime));
                    InvokeTestMethod(method, testFileName);
                }

                if (timeErrorAttr != null && timeErrorAttr.Time >= time && timeErrorAttr.Time <= newTime)
                {
                    UnityEngine.Debug.LogWarning(
                                                 string.Format("--------------- {0} Time: {1} / {2} ----------------",
                                                               testFileName, timeErrorAttr.Time, newTime));
                    InvokeTestError(method);
                }
            }

            time += Time.deltaTime;
        }

        private void InvokeFrameMethods()
        {
            if (frameIndexes.Count > 0 && frame <= frameIndexes[frameIndexes.Count - 1] && frameIndexes.Contains(frame))
            {
                string testFileName = this.GetType().Name;

                // ReSharper disable once HeapView.BoxingAllocation
                UnityEngine.Debug.LogWarning(
                                             string.Format("--------------- {0} Frame: {1}----------------",
                                                           testFileName, frame));

                for (int i = 0; i < frameMethods.Count; i++)
                {
                    MethodInfo method = frameMethods[i];

                    TestDebug debugAttr =
                        Attribute.GetCustomAttribute(method, typeof(TestDebug)) as TestDebug;

                    TestMethod methodAtt =
                        Attribute.GetCustomAttribute(method, typeof(TestMethod)) as TestMethod;

                    TestError errorAttr =
                        Attribute.GetCustomAttribute(method, typeof(TestError)) as TestError;

                    TestRun runAttr =
                        Attribute.GetCustomAttribute(method, typeof(TestRun)) as TestRun;

                    // 直接调用当前批次的 TestDebug 和 TestRun，不捕捉异常
                    if ((debugAttr != null && debugAttr.Frame == frame) ||
                        (runAttr != null && runAttr.Frame == frame))
                    {
                        method.Invoke(this, null);
                    }

                    // 执行当前批次的 TestMethod，捕捉异常、定位异常在文件中的行号
                    if (methodAtt != null && methodAtt.Frame == frame)
                    {
                        InvokeTestMethod(method, testFileName);
                    }

                    // 执行当前批次的 TestError，如果捕捉到异常则通过测试，反之测试失败
                    if (errorAttr != null && errorAttr.Frame == frame)
                    {
                        InvokeTestError(method);
                    }
                }
            }

            frame += 1;
        }

        private void FilterTestMethods()
        {
            methods = this.GetType().GetMethods();

            foreach (MethodInfo minfo in methods)
            {
                TestBaseAttribute attr =
                    Attribute.GetCustomAttribute(minfo, typeof(TestBaseAttribute), true) as TestBaseAttribute;

                if (attr != null)
                {
                    frameMethods.Add(minfo);
                }

                TestBaseTimeAttribute timeAttr =
                    Attribute.GetCustomAttribute(minfo, typeof(TestBaseTimeAttribute), true) as TestBaseTimeAttribute;

                if (timeAttr != null)
                {
                    timeMethods.Add(minfo);
                }
            }
        }

        private void CreateBatchIndexes()
        {
            foreach (MethodInfo minfo in frameMethods)
            {
                TestBaseAttribute attr =
                    Attribute.GetCustomAttribute(minfo, typeof(TestBaseAttribute), true) as TestBaseAttribute;

                if (attr != null && !frameIndexes.Contains(attr.Frame))
                {
                    frameIndexes.Add(attr.Frame);
                }
            }

            frameIndexes.Sort();
        }

        private void InvokeTestError(MethodInfo method)
        {
            string methodName = method.Name;
            string fileName = method.DeclaringType.Name;

            try
            {
                // TestError 的方法必须返回 LineNumber（在其方法中调用 LineNumber）才能获得正确的错误行号
                int lineNumber = (int) method.Invoke(this, null);
                LogFailed(fileName, methodName, lineNumber);
            }
#pragma warning disable 0168
            catch (Exception e)
#pragma warning restore 0168
            {
                LogPassed(fileName, methodName);
            }
        }

        private void InvokeTestMethod(MethodInfo method, string testFileName)
        {
            string methodName = method.Name;
            string fileName = method.DeclaringType.Name;

            try
            {
                method.Invoke(this, null);
                LogPassed(fileName, methodName);
            }
            catch (Exception e)
            {
                Exception ie = e.InnerException;
                StreamingContext ctx = new StreamingContext(StreamingContextStates.CrossAppDomain);
                ObjectManager mgr = new ObjectManager(null, ctx);
                SerializationInfo si = new SerializationInfo(ie.GetType(), new FormatterConverter());

                ie.GetObjectData(si, ctx);
                mgr.RegisterObject(ie, 1, si);
                mgr.DoFixups();

                StackTrace trace = new StackTrace(ie, true);
                StackFrame frame = null;

                for (int j = 0; j < trace.FrameCount; j++)
                {
                    frame = trace.GetFrame(j);
                    string frameFileName =
                        System.IO.Path.GetFileNameWithoutExtension(frame.GetFileName());

                    if (frameFileName == testFileName)
                    {
                        break;
                    }
                }

                int lineNumber = frame.GetFileLineNumber();
                LogFailed(fileName, methodName, lineNumber);
            }
        }

        private void LogPassed(string fileName, string methodName)
        {
            UnityEngine.Debug.LogWarningFormat("<b><color=green>Passed: {0}.{1}()</color></b>", fileName, methodName);
        }

        private void LogFailed(string fileName, string methodName, int lineNumber)
        {
            UnityEngine.Debug.LogWarningFormat(
                                               string.Format(
                                                             "<b><color=red>Failed: {0}.{1}(): {2}</color></b>",
                                                             fileName, methodName, lineNumber));
        }

        //--------------------------------------------------
        // 断言方法
        //--------------------------------------------------

        // int
        public void AreEqual(int v1, int v2)
        {
            if (v1 != v2)
            {
                Fail();
            }
        }

        public void AreNotEqual(int v1, int v2)
        {
            if (v1 == v2)
            {
                Fail();
            }
        }

        // float
        public void AreEqual(float v1, float v2)
        {
            if (!Mathf.Approximately(v1, v2))
            {
                Fail();
            }
        }

        public void AreNotEqual(float v1, float v2)
        {
            if (Mathf.Approximately(v1, v2))
            {
                Fail();
            }
        }

        // string
        public void AreEqual(string v1, string v2)
        {
            if (v1 != v2)
            {
                Fail();
            }
        }

        public void AreNotEqual(string v1, string v2)
        {
            if (v1 == v2)
            {
                Fail();
            }
        }

        // Vector3
        public void AreEqual(Vector3 v1, Vector3 v2)
        {
            if (v1 != v2)
            {
                Fail();
            }
        }

        public void AreNotEqual(Vector3 v1, Vector3 v2)
        {
            if (v1 == v2)
            {
                Fail();
            }
        }

        // Vector2
        public void AreEqual(Vector2 v1, Vector2 v2)
        {
            if (v1 != v2)
            {
                Fail();
            }
        }

        public void AreNotEqual(Vector2 v1, Vector2 v2)
        {
            if (v1 == v2)
            {
                Fail();
            }
        }

        // Quaternion
        public void AreEqual(Quaternion v1, Quaternion v2)
        {
            if (v1 != v2)
            {
                Fail();
            }
        }

        public void AreNotEqual(Quaternion v1, Quaternion v2)
        {
            if (v1 == v2)
            {
                Fail();
            }
        }

        // object
        public void AreSame(object v1, object v2)
        {
            if (!System.Object.ReferenceEquals(v1, v2))
            {
                Fail();
            }
        }

        public void AreNotSame(object v1, object v2)
        {
            if (System.Object.ReferenceEquals(v1, v2))
            {
                Fail();
            }
        }

        public void IsNull(object v)
        {
            if (v != null)
            {
                Fail();
            }
        }

        public void IsNotNull(object v)
        {
            if (v == null)
            {
                Fail();
            }
        }

        // bool
        public void IsTrue(bool v)
        {
            if (!v)
            {
                Fail();
            }
        }

        public void IsFalse(bool v)
        {
            if (v)
            {
                Fail();
            }
        }

        public void Fail(string message = "")
        {
            throw new Exception(message);
        }
    }
}