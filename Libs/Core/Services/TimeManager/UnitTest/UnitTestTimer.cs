using UnityEngine;

namespace MMGame
{
    public class UnitTestTimer : UnitTest
    {
        private Timer ta, tb;
        private bool a, b, c, d;

        //--------------------------------------------------
        // 创建、销毁，测试结果
        //--------------------------------------------------
        [TimeTestMethod(0)]
        public void TestCreateTimers()
        {
            // 创建并检查 Timer A 参数
            ta = Timer.Create(0.5f, CallbackA);
            IsTrue(ta.GoalTime > 0.4f);
            IsNotNull(ta.Callback);
            IsFalse(a);
            // 创建 Timer B
            tb = Timer.Create(0.5f, CallbackB);
            // 创建 Timer C
            Timer.Create(0.55f, CallbackC);
        }

        // 销毁 B
        [TimeTestMethod(0.2f)]
        public void TestDestroyTimerB()
        {
            tb.Destroy();
            IsTrue(tb.GoalTime == 0);
            IsNull(tb.Callback);
            IsFalse(b);
        }

        [TimeTestMethod(0.52f)]
        public void CheckTimersAB()
        {
            IsTrue(a);
            IsFalse(b);
        }

        [TimeTestMethod(0.6f)]
        public void CheckTimersC()
        {
            IsTrue(c);
        }

        //--------------------------------------------------
        // 测试 TimerPool
        //--------------------------------------------------

        private int poolCount;

        [TimeTestRun(1)]
        public void FillTimerPool()
        {
            Timer.Create(0.1f, null);
            Timer.Create(0.1f, null);
            Timer.Create(0.1f, null);
            Timer.Create(0.1f, null);
        }

        [TimeTestRun(1.2f)]
        public void GetFullTimerPool()
        {
            poolCount = TimerManager.PoolCount;
        }

        [TimeTestMethod(1.5f)]
        public void CheckTimerPool()
        {
            Timer.Create(0.1f, null);
            Timer.Create(0.1f, null);
            Timer.Create(0.1f, null);
            AreEqual(poolCount - 3, TimerManager.PoolCount);
        }

        //--------------------------------------------------
        // 测试 TimerManager.Clear()
        //--------------------------------------------------

        [TimeTestRun(2)]
        public void CreateTimersToBeCleared()
        {
            a = false;
            b = false;
            c = false;
            d = false;

            Timer.Create(0.5f, CallbackA);
            Timer.Create(0.5f, CallbackB);
            Timer.Create(0.5f, CallbackC);
            Timer.Create(0.5f, CallbackD);
        }

        [TimeTestRun(2.1f)]
        public void ClearTimers()
        {
            TimerManager.Clear();
        }

        [TimeTestMethod(2.6f)]
        public void CheckClearedTimers()
        {
            IsFalse(a);
            IsFalse(b);
            IsFalse(c);
            IsFalse(d);
        }

        //--------------------------------------------------
        // 测试 TimeManager.Pause()
        //--------------------------------------------------

        [TimeTestRun(3)]
        public void CreateTimers()
        {
            a = false;
            b = false;
            c = false;
            d = false;

            Timer.Create(0.5f, CallbackA);
            Timer.Create(0.5f, CallbackB);
            Timer.Create(0.5f, CallbackC);
            Timer.Create(0.5f, CallbackD);
        }

        [TimeTestRun(3.1f)]
        public void PauseTimers()
        {
            TimerManager.Pause();
        }

        [TimeTestMethod(3.6f)]
        public void CheckPausedTimers()
        {
            IsFalse(a);
            IsFalse(b);
            IsFalse(c);
            IsFalse(d);
        }

        [TimeTestRun(3.7f)]
        public void ResumeTimers()
        {
            // 还剩 0.4 s
            TimerManager.Resume();
        }

        [TimeTestMethod(4.0f)]
        public void CheckTimersBeforeInvoke()
        {
            IsFalse(a);
            IsFalse(b);
            IsFalse(c);
            IsFalse(d);
        }

        [TimeTestMethod(4.2f)]
        public void CheckTimersArrived()
        {
            IsTrue(a);
            IsTrue(b);
            IsTrue(c);
            IsTrue(d);
        }

        //--------------------------------------------------
        // 在回调中创建和销毁 Timer
        //--------------------------------------------------

        private Timer timerToBeDestroyed;

        [TimeTestRun(5f)]
        public void TestInitCreateAndDestroyInCallback()
        {
            a = false;
            b = false;
            c = false;
            d = false;

            // 创建修改 a、b、c 的计时器序列
            Timer.Create(0.5f, CallbackA);
            timerToBeDestroyed = Timer.Create(0.7f, CallbackB);
            Timer.Create(0.9f, CallbackC);

            // 创建一个计时器提前删除 b
            Timer.Create(0.2f, CallbackDestroy);

            // 创建一个计时器在序列末尾创建修改 d 的计时器
            Timer.Create(0.6f, CallbackCreate);

            // 结果应该是 a, c, d 按顺序被修改（0.5s, 0.9s, 1s）
        }

        [TimeTestMethod(5.45f)]
        public void TestABCDIsFalse()
        {
            IsFalse(a);
            IsFalse(b);
            IsFalse(c);
            IsFalse(d);
        }

        [TimeTestMethod(5.55f)]
        public void TestAIsTrue()
        {
            IsTrue(a);
            IsFalse(b);
            IsFalse(c);
            IsFalse(d);
        }

        [TimeTestMethod(5.85f)]
        public void TestBCDIsFalse()
        {
            IsTrue(a);
            IsFalse(b);
            IsFalse(c);
            IsFalse(d);
        }

        [TimeTestMethod(5.95f)]
        public void TestACIsTrue()
        {
            IsTrue(a);
            IsFalse(b);
            IsTrue(c);
            IsFalse(d);
        }

        [TimeTestMethod(6.5f)]
        public void TestACDIsTrue()
        {
            IsTrue(a);
            IsFalse(b);
            IsTrue(c);
            IsTrue(d);
        }

        // ------------------------------------------------------
        // 测试用接口和类
        // ------------------------------------------------------

        private void CallbackA()
        {
            a = true;
        }

        private void CallbackB()
        {
            b = true;
        }

        private void CallbackC()
        {
            c = true;
        }

        private void CallbackD()
        {
            d = true;
        }

        private void CallbackCreate()
        {
            Timer.Create(0.4f, CallbackD);
        }

        private void CallbackDestroy()
        {
            timerToBeDestroyed.Destroy();
        }
    }
}