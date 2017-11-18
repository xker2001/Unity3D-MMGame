using System.Collections.Generic;
using UnityEngine;
using MMGame.Event;

namespace MMGame.AI.FiniteStateMachine.UnitTest
{
    public class TestEventData : EventData {}

    public class UnitTestFsm : MMGame.UnitTest
    {
        [SerializeField]
        private TestFsmSystem fsm;

        private void GetStateGraph(TestState state, out ICondition[] cons, out EventListener[] evts,
                                   out int[] exits, out IList<ServiceProvider> sps, out IList<EventListener> els)
        {
            cons = fsm.GetConditions(state);
            evts = fsm.GetEventListeners(state);
            exits = fsm.GetExitCodes(state);
            sps = fsm.GetBookedServiceProviders(state);
            els = fsm.GetBookedEventListeners(state);
        }

        //--------------------------------------------------
        // 测试状态机初始状态
        //--------------------------------------------------

        [TestMethod(0)]
        public void CheckBeforeInitializing()
        {
            IsFalse(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
        }

        //--------------------------------------------------
        // 测试状态机初始化及 OnEnter()
        //--------------------------------------------------

        [TestMethod(1)]
        public void TestInitAndGraph()
        {
            fsm.Run();

            AreSame(fsm.CurrentState, fsm.State1000);

            IsTrue(fsm.State1000.OnEnterCalled);
            AreEqual(fsm.State1000.TimesOfEnter, 1);

            IsTrue(fsm.IsRunning);
            IsFalse(fsm.IsPaused);

            //--------------------------------------------------
            // 检查 Book 指令专用 State 的服务
            //--------------------------------------------------

            IsFalse(fsm.ServiceProvider1.IsEnabled);
            IsFalse(fsm.ServiceProvider2.IsEnabled);
            IsFalse(fsm.ServiceProvider3.IsEnabled);
            IsFalse(fsm.ServiceProvider4.IsEnabled);
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsFalse(fsm.Evt1.IsEnabled);
            IsTrue(fsm.Evt2.IsEnabled);
            IsFalse(fsm.Evt3.IsEnabled);
            IsTrue(fsm.Evt4.IsEnabled);
            IsFalse(fsm.Evt5.IsEnabled);
            IsFalse(fsm.EventListener1.IsEnabled);
            IsFalse(fsm.EventListener2.IsEnabled);
            IsFalse(fsm.EventListener3.IsEnabled);
            IsFalse(fsm.EventListener4.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);

            //--------------------------------------------------
            // 检查其他所有 State 的的连接和服务
            //--------------------------------------------------

            ICondition[] cons;
            EventListener[] evts;
            int[] exits;
            IList<ServiceProvider> sps;
            IList<EventListener> els;

            FsmState to;
            FsmState[] path;

            //--------------------------------------------
            // AnyState1                          : con2
            // AnyStateNoSelf1                    : con4
            //
            // WhiteList11(State50, 51, 52)       : con3
            // WhiteList12(State50, 51, 52)       : con6
            // BlackList11(State50, 51, 52)       : con5
            // BlackList12(State11, 12, 50)       : con6
            //
            // AnyState2                          : evt2
            // AnyStateNoSelf2                    : evt4
            // AnyState3                          : EventListener5
            //
            // WhiteList21(State60, 61, 62)       : evt3
            // WhiteList22(State60, 61, 62)       : evt6
            // BlackList11(State60, 61, 62)       : evt5
            // BlackList22(State21, 22, 60)       : evt6
            //
            // AnyState3                          : ServiceProvider5
            //
            // WhiteListService(StateS1, StateS2) : ServiceProvider3, EventListener3
            // BlackListService(StateS1, StateS2) : ServiceProvider4, EventListener4

            // state11
            GetStateGraph(fsm.State11, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 4); // con1, con2, con4, con6
            AreEqual(evts.Length, 2); // evt2, evt4
            AreEqual(exits.Length, 2); // exit0, exit1
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state21
            GetStateGraph(fsm.State21, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 4); // evt1, evt2, evt4, evt6
            AreEqual(exits.Length, 1); // exit0
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 5); // evt1, evt2, evt4, evt6, eventListener5

            // state103
            GetStateGraph(fsm.State103, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state106
            GetStateGraph(fsm.State106, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state107
            GetStateGraph(fsm.State107, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state100
            GetStateGraph(fsm.State100, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state108
            GetStateGraph(fsm.State108, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state12
            GetStateGraph(fsm.State12, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 5); // con1, con3, con2, con4, con6
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state22
            GetStateGraph(fsm.State22, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 5); // evt1, evt3, evt2, evt4, evt6
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 6); // evt1, evt3, evt2, evt4, evt6, eventListener5

            // state104
            GetStateGraph(fsm.State104, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 1); // con2
            AreEqual(evts.Length, 1); // evt2
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 2); // evt2, eventListener5

            // state102
            GetStateGraph(fsm.State102, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state50
            GetStateGraph(fsm.State50, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 5); // con2, con4, con3, con5, con6
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state51
            GetStateGraph(fsm.State51, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 5); // con2, con4, con3, con5, con6
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state52
            GetStateGraph(fsm.State52, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 5); // con2, con4, con3, con5, con6
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state60
            GetStateGraph(fsm.State60, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 5); // evt2, evt4, evt3, evt5, evt6
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 6); // evt2, evt4, evt3, evt5, evt6, eventListener5

            // state61
            GetStateGraph(fsm.State61, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 5); // evt2, evt4, evt3, evt5, evt6
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 6); // evt2, evt4, evt3, evt5, evt6, eventListener5

            // state62
            GetStateGraph(fsm.State62, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 5); // evt2, evt4, evt3, evt5, evt6
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 6); // evt2, evt4, evt3, evt5, evt6, eventListener5

            // stateS1
            GetStateGraph(fsm.StateS1, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 5); // serviceProvider1, 2, 3, 4, 5
            AreEqual(els.Count, 7); // evt2, evt4, eventListener1, 2, 3, 4, 5

            // stateS2
            GetStateGraph(fsm.StateS2, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 3); // serviceProvider3, 4, 5
            AreEqual(els.Count, 5); // evt2, evt4, eventListener3, 4, 5

            // stateS3
            GetStateGraph(fsm.StateS3, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 3); // serviceProvider2, 3, 5
            AreEqual(els.Count, 5); // evt2, evt4, eventListener2, 3, 5

            // state1000
            GetStateGraph(fsm.State1000, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 4); // con1001, con1002, con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state1001
            GetStateGraph(fsm.State1001, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            AreEqual(exits.Length, 2); // exit0, exit1
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state1002
            GetStateGraph(fsm.State1002, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            AreEqual(exits.Length, 1); // exit0
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state1003
            GetStateGraph(fsm.State1003, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 4); // evt2, evt4, evt1001, evt1002
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 5); // evt2, evt4, eventListener5, evt1001, evt1002

            // state1004
            GetStateGraph(fsm.State1004, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            AreEqual(exits.Length, 1); // exit0
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state1005
            GetStateGraph(fsm.State1005, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 3); // con2, con4, con1006_1
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            IsTrue(fsm.GetConditionTo(fsm.State1005, fsm.Con1006_1, out to, out path)); // 检查路径
            AreSame(to, fsm.State1008);
            AreSame(path[0], fsm.State1006);
            AreSame(path[1], fsm.State1007);

            // state1006
            GetStateGraph(fsm.State1006, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            AreEqual(exits.Length, 1); // exit0
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state1007
            GetStateGraph(fsm.State1007, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state1008
            GetStateGraph(fsm.State1008, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 3); // con2, con4, con1009
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state1009
            GetStateGraph(fsm.State1009, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 3); // con2, con4, con1006_2
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state1010
            GetStateGraph(fsm.State1010, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 3); // evt2, evt4, evt1001
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 4); // evt2, evt4, eventListener5, evt1001

            // state1011
            GetStateGraph(fsm.State1011, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 3); // con2, con4, con1001
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // state1012
            GetStateGraph(fsm.State1012, out cons, out evts, out exits, out sps, out els);
            AreEqual(cons.Length, 2); // con2, con4
            AreEqual(evts.Length, 2); // evt2, evt4
            IsNull(exits);
            AreEqual(sps.Count, 1); // serviceProvider5
            AreEqual(els.Count, 3); // evt2, evt4, eventListener5

            // State1005 的 ExitCode+Condition 连接
            IsTrue(fsm.GetExitConditionTo(fsm.State1005, 0, fsm.Con1013, out to, out path));
            AreSame(to, fsm.State1013);
            AreSame(path[0], fsm.State1017);
            AreSame(path[1], fsm.State1018);

            IsTrue(fsm.GetExitConditionTo(fsm.State1005, 0, fsm.Con1014, out to, out path));
            AreSame(to, fsm.State1014);
            AreEqual(path.Length, 0);

            IsTrue(fsm.GetExitConditionTo(fsm.State1005, 1, fsm.Con1015, out to, out path));
            AreSame(to, fsm.State1015);
            AreEqual(path.Length, 0);

            IsTrue(fsm.GetExitConditionTo(fsm.State1005, 1, fsm.Con1016, out to, out path));
            AreSame(to, fsm.State1016);
            AreEqual(path.Length, 0);
        }

        //--------------------------------------------------
        // 测试基本流程
        //--------------------------------------------------

        [TestMethod(2)]
        public void State1000To1001()
        {
            AreSame(fsm.CurrentState, fsm.State1000);
            fsm.Con1001.Set(true);
        }

        [TestMethod(5)]
        public void TestState1000To1001()
        {
            AreSame(fsm.CurrentState, fsm.State1001);
            IsFalse(fsm.Con1001.Value); // 检查条件是否被重置，仅与 TestCondition 相关，与框架无关

            fsm.State1001.SetExitCode(0);
        }

        [TestMethod(6)]
        public void TestState1001To1000()
        {
            AreSame(fsm.CurrentState, fsm.State1000);

            fsm.Con1002.Set(false); // 反转条件
        }

        [TestMethod(7)]
        public void TestState1000To1002()
        {
            AreSame(fsm.CurrentState, fsm.State1002);
            fsm.State1002.SetExitCode(0);
        }

        // 记录服务停启用次数的变量
        private int sp5EnabledTimes;
        private int sp5DisabledTimes;
        private int el5EnabledTimes;
        private int el5DisabledTimes;
        private int evt1001EnabledTimes;
        private int evt1001DisabledTimes;
        private int evt1002EnabledTimes;
        private int evt1002DisabledTimes;

        [TestMethod(8)]
        public void TestState1002To1001()
        {
            AreSame(fsm.CurrentState, fsm.State1001);

            // 记录相关服务起停次数（忽略 Evt2、Evt4）
            sp5EnabledTimes = fsm.ServiceProvider5.EnableTimes;
            sp5DisabledTimes = fsm.ServiceProvider5.DisableTimes;
            el5EnabledTimes = fsm.EventListener5.EnableTimes;
            el5DisabledTimes = fsm.EventListener5.DisableTimes;
            evt1001EnabledTimes = fsm.Evt1001.EnableTimes;
            evt1001DisabledTimes = fsm.Evt1001.DisableTimes;
            evt1002EnabledTimes = fsm.Evt1002.EnableTimes;
            evt1002DisabledTimes = fsm.Evt1002.DisableTimes;

            // 检查服务启用状况
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);
            IsFalse(fsm.Evt1001.IsEnabled);
            IsFalse(fsm.Evt1002.IsEnabled);

            fsm.State1001.SetExitCode(1);
        }

        [TestMethod(9)]
        public void TestState1001To1003()
        {
            AreSame(fsm.CurrentState, fsm.State1003);

            // 检查相关服务起停次数
            // 下个状态依旧要启用的服务在状态迁移时不经历「停 - 启」过程
            // 下个状态新启用的服务增加启用次数一次
            AreEqual(fsm.ServiceProvider5.EnableTimes, sp5EnabledTimes);
            AreEqual(fsm.ServiceProvider5.DisableTimes, sp5DisabledTimes);
            AreEqual(fsm.EventListener5.EnableTimes, el5EnabledTimes);
            AreEqual(fsm.EventListener5.DisableTimes, el5DisabledTimes);
            AreEqual(fsm.Evt1001.EnableTimes, evt1001EnabledTimes + 1);
            AreEqual(fsm.Evt1001.DisableTimes, evt1001DisabledTimes);
            AreEqual(fsm.Evt1002.EnableTimes, evt1002EnabledTimes + 1);
            AreEqual(fsm.Evt1002.DisableTimes, evt1002DisabledTimes);

            // 检查服务启用状况
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);
            IsTrue(fsm.Evt1001.IsEnabled);
            IsTrue(fsm.Evt1002.IsEnabled);
        }

        [TestRun(15)]
        public void State1003To1004()
        {
            AreSame(fsm.CurrentState, fsm.State1003);
            this.SendEvent(fsm.Evt1001.EventType, EventPool.New<TestEventData>());
        }

        [TestMethod(16)]
        public void TestState1003To1004()
        {
            AreSame(fsm.CurrentState, fsm.State1004);
            IsFalse(fsm.Evt1001.IsTriggered); // 检查事件 IsTrigger 标记是否被正确重置

            // 检查相关服务起停次数
            AreEqual(fsm.ServiceProvider5.EnableTimes, sp5EnabledTimes);
            AreEqual(fsm.ServiceProvider5.DisableTimes, sp5DisabledTimes);
            AreEqual(fsm.EventListener5.EnableTimes, el5EnabledTimes);
            AreEqual(fsm.EventListener5.DisableTimes, el5DisabledTimes);
            AreEqual(fsm.Evt1001.EnableTimes, evt1001EnabledTimes + 1);
            AreEqual(fsm.Evt1001.DisableTimes, evt1001DisabledTimes + 1);
            AreEqual(fsm.Evt1002.EnableTimes, evt1001EnabledTimes + 1);
            AreEqual(fsm.Evt1002.DisableTimes, evt1001DisabledTimes + 1);

            // 检查服务启用状况
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);
            IsFalse(fsm.Evt1001.IsEnabled);
            IsFalse(fsm.Evt1002.IsEnabled);

            fsm.State1004.SetExitCode(0);
        }

        [TestMethod(17)]
        public void TestState1004To1003()
        {
            AreSame(fsm.CurrentState, fsm.State1003);

            // 检查相关服务起停次数
            AreEqual(fsm.ServiceProvider5.EnableTimes, sp5EnabledTimes);
            AreEqual(fsm.ServiceProvider5.DisableTimes, sp5DisabledTimes);
            AreEqual(fsm.EventListener5.EnableTimes, el5EnabledTimes);
            AreEqual(fsm.EventListener5.DisableTimes, el5DisabledTimes);
            AreEqual(fsm.Evt1001.EnableTimes, evt1001EnabledTimes + 2);
            AreEqual(fsm.Evt1001.DisableTimes, evt1001DisabledTimes + 1);
            AreEqual(fsm.Evt1002.EnableTimes, evt1001EnabledTimes + 2);
            AreEqual(fsm.Evt1002.DisableTimes, evt1001DisabledTimes + 1);

            // 检查服务启用状况
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);
            IsTrue(fsm.Evt1001.IsEnabled);
            IsTrue(fsm.Evt1002.IsEnabled);

            this.SendEvent(fsm.Evt1002.EventType, EventPool.New<TestEventData>());
        }

        [TestMethod(18)]
        public void TestState1003To1005()
        {
            AreSame(fsm.CurrentState, fsm.State1005);
            fsm.Con1006_1.Set(true);
        }

        //--------------------------------------------------
        // 测试路径
        //--------------------------------------------------

        // 测试基本路径

        [TestMethod(19)]
        public void TestState1005To1006()
        {
            AreSame(fsm.CurrentState, fsm.State1006);
            fsm.State1006.SetExitCode(0);
        }

        [TestMethod(20)]
        public void TestState1006To1007()
        {
            AreSame(fsm.CurrentState, fsm.State1007);
            fsm.State1007.SetExitCode(0);
        }

        [TestMethod(21)]
        public void TestState1007To1008()
        {
            AreSame(fsm.CurrentState, fsm.State1008);
            fsm.Con1009.Set(true);
        }

        // 测试路径中断

        [TestMethod(22)]
        public void TestState1008To1009()
        {
            AreSame(fsm.CurrentState, fsm.State1009);
            fsm.Con1006_2.Set(true);
        }

        [TestMethod(23)]
        public void TestState1009To1006()
        {
            AreSame(fsm.CurrentState, fsm.State1006);
            fsm.State1006.SetExitCode(0);
        }

        // 没有走默认出口，而是沿着路径到达 State1011
        [TestMethod(24)]
        public void TestState1006To1011()
        {
            AreSame(fsm.CurrentState, fsm.State1011);
            fsm.Con1001.Set(true); // 测试 Con1001 在第二个连线上重复使用
        }

        // 测试条件复用
        [TestMethod(25)]
        public void TestState1011To1010()
        {
            AreSame(fsm.CurrentState, fsm.State1010);
            this.SendEvent(fsm.Evt1001.EventType, EventPool.New<TestEventData>()); // 测试 Evt1001 在第二个连线上重复使用
        }

        // 测试事件复用
        [TestMethod(26)]
        public void TestState1010To1005()
        {
            AreSame(fsm.CurrentState, fsm.State1005);
        }

        //--------------------------------------------------
        // 测试 Exit + Condition
        //--------------------------------------------------

        // 1005 - 1013
        [TestRun(30)]
        public void State1005ToState1013()
        {
            AreSame(fsm.CurrentState, fsm.State1005);

            fsm.Con1013.Set(true);
            fsm.State1005.SetExitCode(0);
        }

        [TestMethod(31)]
        public void TestState1005To1017()
        {
            AreSame(fsm.CurrentState, fsm.State1017);
            fsm.State1017.SetExitCode(0);
        }

        [TestMethod(32)]
        public void TestState1017To1018()
        {
            AreSame(fsm.CurrentState, fsm.State1018);
            fsm.State1018.SetExitCode(0);
        }

        [TestMethod(33)]
        public void TestState1018To1013()
        {
            AreSame(fsm.CurrentState, fsm.State1013);
            fsm.State1013.SetExitCode(0);
        }

        [TestMethod(34)]
        public void TestState1013To1005()
        {
            AreSame(fsm.CurrentState, fsm.State1005);

            fsm.Con1014.Set(true);
            fsm.State1005.SetExitCode(0);
        }

        // 1005 - 1014
        [TestMethod(35)]
        public void TestState1005To1014()
        {
            AreSame(fsm.CurrentState, fsm.State1014);
            fsm.State1014.SetExitCode(0);
        }

        [TestMethod(36)]
        public void TestState1014To1005()
        {
            AreSame(fsm.CurrentState, fsm.State1005);

            fsm.Con1015.Set(true);
            fsm.State1005.SetExitCode(1);
        }

        // 1005 - 1015
        [TestMethod(37)]
        public void TestState1005To1015()
        {
            AreSame(fsm.CurrentState, fsm.State1015);
            fsm.State1015.SetExitCode(0);
        }

        [TestMethod(38)]
        public void TestState1015To1005()
        {
            AreSame(fsm.CurrentState, fsm.State1005);

            fsm.Con1016.Set(true);
            fsm.State1005.SetExitCode(1);
        }

        // 1005 - 1016
        [TestMethod(39)]
        public void TestState1005To1016()
        {
            AreSame(fsm.CurrentState, fsm.State1016);
            fsm.State1016.SetExitCode(0);
        }

        [TestMethod(40)]
        public void TestState1016To1005()
        {
            AreSame(fsm.CurrentState, fsm.State1005);
        }

        //--------------------------------------------------
        // 测试暂停、恢复、停止和重新运行
        //--------------------------------------------------

        private int ticks = 0; // 检测状态活跃的帧数

        [TestMethod(42)]
        public void TestPause()
        {
            AreSame(fsm.CurrentState, fsm.State1005);

            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsTrue(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);

            ticks = fsm.State1005.Ticks;
            fsm.Pause();

            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsTrue(fsm.IsPaused);
            IsTrue(fsm.State1005.IsRunning);
            IsTrue(fsm.State1005.IsPaused);
        }

        [TestMethod(45)]
        public void TestResume()
        {
            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsTrue(fsm.IsPaused);
            IsTrue(fsm.State1005.IsRunning);
            IsTrue(fsm.State1005.IsPaused);

            AreEqual(ticks, fsm.State1005.Ticks); // 不再 Update

            fsm.Resume();

            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsTrue(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);
        }

        [TestMethod(50)]
        public void TestStop()
        {
            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsTrue(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);
            AreEqual(ticks + 5, fsm.State1005.Ticks); // 继续 Update

            // 检查服务
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.Evt2.IsEnabled);
            IsTrue(fsm.Evt4.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);

            fsm.Stop();

            // 检查运行和暂停状态
            IsFalse(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsFalse(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);

            // 检查服务
            IsFalse(fsm.ServiceProvider5.IsEnabled);
            IsFalse(fsm.Evt2.IsEnabled);
            IsFalse(fsm.Evt4.IsEnabled);
            IsFalse(fsm.EventListener5.IsEnabled);
        }

        [TestMethod(55)]
        public void TestRun()
        {
            // 检查运行和暂停状态
            IsFalse(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsFalse(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);

            // 检查服务
            IsFalse(fsm.ServiceProvider5.IsEnabled);
            IsFalse(fsm.Evt2.IsEnabled);
            IsFalse(fsm.Evt4.IsEnabled);
            IsFalse(fsm.EventListener5.IsEnabled);

            fsm.Run(fsm.State1005);

            AreSame(fsm.CurrentState, fsm.State1005);

            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsTrue(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);

            // 检查服务
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.Evt2.IsEnabled);
            IsTrue(fsm.Evt4.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);
        }

        // 测试在暂停中停止
        [TestMethod(57)]
        public void TestPauseAndStop1()
        {
            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsTrue(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);

            // 检查服务
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.Evt2.IsEnabled);
            IsTrue(fsm.Evt4.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);

            fsm.Pause();

            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsTrue(fsm.IsPaused);
            IsTrue(fsm.State1005.IsRunning);
            IsTrue(fsm.State1005.IsPaused);

            // 检查服务
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.Evt2.IsEnabled);
            IsTrue(fsm.Evt4.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);
        }

        [TestMethod(58)]
        public void TestPauseAndStop2()
        {
            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsTrue(fsm.IsPaused);
            IsTrue(fsm.State1005.IsRunning);
            IsTrue(fsm.State1005.IsPaused);

            // 检查服务
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.Evt2.IsEnabled);
            IsTrue(fsm.Evt4.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);

            fsm.Stop();

            // 检查运行和暂停状态
            IsFalse(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsFalse(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);

            // 检查服务
            IsFalse(fsm.ServiceProvider5.IsEnabled);
            IsFalse(fsm.Evt2.IsEnabled);
            IsFalse(fsm.Evt4.IsEnabled);
            IsFalse(fsm.EventListener5.IsEnabled);
        }

        // 直接运行到指定状态
        [TestMethod(60)]
        public void TestRunWithState()
        {
            // 检查运行和暂停状态
            IsFalse(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsFalse(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);

            // 检查服务
            IsFalse(fsm.ServiceProvider5.IsEnabled);
            IsFalse(fsm.Evt2.IsEnabled);
            IsFalse(fsm.Evt4.IsEnabled);
            IsFalse(fsm.EventListener5.IsEnabled);

            fsm.Run(fsm.State1007);

            AreSame(fsm.CurrentState, fsm.State1007);

            // 检查运行和暂停状态
            IsTrue(fsm.IsRunning);
            IsFalse(fsm.IsPaused);
            IsFalse(fsm.State1005.IsRunning);
            IsFalse(fsm.State1005.IsPaused);
            IsTrue(fsm.State1007.IsRunning);
            IsFalse(fsm.State1007.IsPaused);

            // 检查服务
            IsTrue(fsm.ServiceProvider5.IsEnabled);
            IsTrue(fsm.Evt2.IsEnabled);
            IsTrue(fsm.Evt4.IsEnabled);
            IsTrue(fsm.EventListener5.IsEnabled);
        }
    }
}