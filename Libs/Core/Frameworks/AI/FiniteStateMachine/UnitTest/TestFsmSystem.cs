using System;

namespace MMGame.AI.FiniteStateMachine.UnitTest
{
    public class TestFsmSystem : FsmSystem
    {
        [NonSerialized]
        public TestState State11;
        [NonSerialized]
        public TestState State12;
        [NonSerialized]
        public TestState State21;
        [NonSerialized]
        public TestState State22;
        [NonSerialized]
        public TestState State50;
        [NonSerialized]
        public TestState State51;
        [NonSerialized]
        public TestState State52;
        [NonSerialized]
        public TestState State60;
        [NonSerialized]
        public TestState State61;
        [NonSerialized]
        public TestState State62;
        [NonSerialized]
        public TestState State100;
        [NonSerialized]
        public TestState State102;
        [NonSerialized]
        public TestState State103;
        [NonSerialized]
        public TestState State104;
        [NonSerialized]
        public TestState State105;
        [NonSerialized]
        public TestState State106;
        [NonSerialized]
        public TestState State107;
        [NonSerialized]
        public TestState State108;
        [NonSerialized]
        public TestState State1000;
        [NonSerialized]
        public TestState State1001;
        [NonSerialized]
        public TestState State1002;
        [NonSerialized]
        public TestState State1003;
        [NonSerialized]
        public TestState State1004;
        [NonSerialized]
        public TestState State1005;
        [NonSerialized]
        public TestState State1006;
        [NonSerialized]
        public TestState State1007;
        [NonSerialized]
        public TestState State1008;
        [NonSerialized]
        public TestState State1009;
        [NonSerialized]
        public TestState State1010;
        [NonSerialized]
        public TestState State1011;
        [NonSerialized]
        public TestState State1012;
        [NonSerialized]
        public TestState State1013;
        [NonSerialized]
        public TestState State1014;
        [NonSerialized]
        public TestState State1015;
        [NonSerialized]
        public TestState State1016;
        [NonSerialized]
        public TestState State1017;
        [NonSerialized]
        public TestState State1018;
        [NonSerialized]
        public TestState StateS1;
        [NonSerialized]
        public TestState StateS2;
        [NonSerialized]
        public TestState StateS3;
        [NonSerialized]
        public TestCondition Con1;
        [NonSerialized]
        public TestCondition Con2;
        [NonSerialized]
        public TestCondition Con3;
        [NonSerialized]
        public TestCondition Con4;
        [NonSerialized]
        public TestCondition Con5;
        [NonSerialized]
        public TestCondition Con6;
        [NonSerialized]
        public TestCondition Con1001;
        [NonSerialized]
        public TestInverseCondition Con1002;
        [NonSerialized]
        public TestCondition Con1006_1;
        [NonSerialized]
        public TestCondition Con1006_2;
        [NonSerialized]
        public TestCondition Con1009;
        [NonSerialized]
        public TestCondition Con1013;
        [NonSerialized]
        public TestCondition Con1014;
        [NonSerialized]
        public TestCondition Con1015;
        [NonSerialized]
        public TestCondition Con1016;
        [NonSerialized]
        public TestEventListener Evt1;
        [NonSerialized]
        public TestEventListener Evt2;
        [NonSerialized]
        public TestEventListener Evt3;
        [NonSerialized]
        public TestEventListener Evt4;
        [NonSerialized]
        public TestEventListener Evt5;
        [NonSerialized]
        public TestEventListener Evt6;
        [NonSerialized]
        public TestEventListener Evt1001;
        [NonSerialized]
        public TestEventListener Evt1002;
        [NonSerialized]
        public TestServiceProvider ServiceProvider1;
        [NonSerialized]
        public TestServiceProvider ServiceProvider2;
        [NonSerialized]
        public TestServiceProvider ServiceProvider3;
        [NonSerialized]
        public TestServiceProvider ServiceProvider4;
        [NonSerialized]
        public TestServiceProvider ServiceProvider5;
        [NonSerialized]
        public TestEventListener EventListener1;
        [NonSerialized]
        public TestEventListener EventListener2;
        [NonSerialized]
        public TestEventListener EventListener3;
        [NonSerialized]
        public TestEventListener EventListener4;
        [NonSerialized]
        public TestEventListener EventListener5;

        void Awake()
        {
            State11 = gameObject.AddComponent<TestState>().SetInfo("State11");
            State12 = gameObject.AddComponent<TestState>().SetInfo("State12");
            State21 = gameObject.AddComponent<TestState>().SetInfo("State21");
            State22 = gameObject.AddComponent<TestState>().SetInfo("State22");
            State50 = gameObject.AddComponent<TestState>().SetInfo("State50");
            State51 = gameObject.AddComponent<TestState>().SetInfo("State51");
            State52 = gameObject.AddComponent<TestState>().SetInfo("State52");
            State60 = gameObject.AddComponent<TestState>().SetInfo("State60");
            State61 = gameObject.AddComponent<TestState>().SetInfo("State61");
            State62 = gameObject.AddComponent<TestState>().SetInfo("State62");
            State100 = gameObject.AddComponent<TestState>().SetInfo("State100");
            State102 = gameObject.AddComponent<TestState>().SetInfo("State102");
            State103 = gameObject.AddComponent<TestState>().SetInfo("State103");
            State104 = gameObject.AddComponent<TestState>().SetInfo("State104");
            State105 = gameObject.AddComponent<TestState>().SetInfo("State105");
            State106 = gameObject.AddComponent<TestState>().SetInfo("State106");
            State107 = gameObject.AddComponent<TestState>().SetInfo("State107");
            State108 = gameObject.AddComponent<TestState>().SetInfo("State108");
            State1000 = gameObject.AddComponent<TestState>().SetInfo("State1000");
            State1001 = gameObject.AddComponent<TestState>().SetInfo("State1001");
            State1002 = gameObject.AddComponent<TestState>().SetInfo("State1002");
            State1003 = gameObject.AddComponent<TestState>().SetInfo("State1003");
            State1004 = gameObject.AddComponent<TestState>().SetInfo("State1004");
            State1005 = gameObject.AddComponent<TestState>().SetInfo("State1005");
            State1006 = gameObject.AddComponent<TestState>().SetInfo("State1006");
            State1007 = gameObject.AddComponent<TestState>().SetInfo("State1007");
            State1008 = gameObject.AddComponent<TestState>().SetInfo("State1008");
            State1009 = gameObject.AddComponent<TestState>().SetInfo("State1009");
            State1010 = gameObject.AddComponent<TestState>().SetInfo("State1010");
            State1011 = gameObject.AddComponent<TestState>().SetInfo("State1011");
            State1012 = gameObject.AddComponent<TestState>().SetInfo("State1012");
            State1013 = gameObject.AddComponent<TestState>().SetInfo("State1013");
            State1014 = gameObject.AddComponent<TestState>().SetInfo("State1014");
            State1015 = gameObject.AddComponent<TestState>().SetInfo("State1015");
            State1016 = gameObject.AddComponent<TestState>().SetInfo("State1016");
            State1017 = gameObject.AddComponent<TestState>().SetInfo("State1017");
            State1018 = gameObject.AddComponent<TestState>().SetInfo("State1018");
            StateS1 = gameObject.AddComponent<TestState>().SetInfo("StateService1");
            StateS2 = gameObject.AddComponent<TestState>().SetInfo("StateService2");
            StateS3 = gameObject.AddComponent<TestState>().SetInfo("StateService3");
            Con1 = gameObject.AddComponent<TestCondition>().SetInfo("Con1");
            Con2 = gameObject.AddComponent<TestCondition>().SetInfo("Con2");
            Con3 = gameObject.AddComponent<TestCondition>().SetInfo("Con3");
            Con4 = gameObject.AddComponent<TestCondition>().SetInfo("Con4");
            Con5 = gameObject.AddComponent<TestCondition>().SetInfo("Con5");
            Con6 = gameObject.AddComponent<TestCondition>().SetInfo("Con6");
            Con1001 = gameObject.AddComponent<TestCondition>().SetInfo("Con1001");
            Con1002 = gameObject.AddComponent<TestInverseCondition>().SetInfo("Con1002");
            Con1006_1 = gameObject.AddComponent<TestCondition>().SetInfo("Con1006_1");
            Con1006_2 = gameObject.AddComponent<TestCondition>().SetInfo("Con1006_2");
            Con1009 = gameObject.AddComponent<TestCondition>().SetInfo("Con1009");
            Con1013 = gameObject.AddComponent<TestCondition>().SetInfo("Con1013");
            Con1014 = gameObject.AddComponent<TestCondition>().SetInfo("Con1014");
            Con1015 = gameObject.AddComponent<TestCondition>().SetInfo("Con1015");
            Con1016 = gameObject.AddComponent<TestCondition>().SetInfo("Con1016");
            Evt1 = gameObject.AddComponent<TestEventListener>().SetInfo("Evt1").SetEventType(EventType.Type1);
            Evt2 = gameObject.AddComponent<TestEventListener>().SetInfo("Evt2").SetEventType(EventType.Type2);
            Evt3 = gameObject.AddComponent<TestEventListener>().SetInfo("Evt3").SetEventType(EventType.Type3);
            Evt4 = gameObject.AddComponent<TestEventListener>().SetInfo("Evt4").SetEventType(EventType.Type4);
            Evt5 = gameObject.AddComponent<TestEventListener>().SetInfo("Evt5").SetEventType(EventType.Type5);
            Evt6 = gameObject.AddComponent<TestEventListener>().SetInfo("Evt6").SetEventType(EventType.Type6);
            Evt1001 = gameObject.AddComponent<TestEventListener>().SetInfo("Evt1001").SetEventType(EventType.Type1001);
            Evt1002 = gameObject.AddComponent<TestEventListener>().SetInfo("Evt1002").SetEventType(EventType.Type1002);
            ServiceProvider1 = gameObject.AddComponent<TestServiceProvider>().SetInfo("ServiceProvider1");
            ServiceProvider2 = gameObject.AddComponent<TestServiceProvider>().SetInfo("ServiceProvider2");
            ServiceProvider3 = gameObject.AddComponent<TestServiceProvider>().SetInfo("ServiceProvider3");
            ServiceProvider4 = gameObject.AddComponent<TestServiceProvider>().SetInfo("ServiceProvider4");
            ServiceProvider5 = gameObject.AddComponent<TestServiceProvider>().SetInfo("ServiceProvider5");
            EventListener1 = gameObject.AddComponent<TestEventListener>().SetInfo("EventListener1");
            EventListener2 = gameObject.AddComponent<TestEventListener>().SetInfo("EventListener2");
            EventListener3 = gameObject.AddComponent<TestEventListener>().SetInfo("EventListener3");
            EventListener4 = gameObject.AddComponent<TestEventListener>().SetInfo("EventListener4");
            EventListener5 = gameObject.AddComponent<TestEventListener>().SetInfo("EventListener5");
        }

        protected override void Design()
        {
            SetStates(State11, State12, State21, State22,
                      State50, State51, State52, State60, State61, State62,
                      State100, State102, State103, State104, State105, State106, State107, State108,
                      State1000, State1001, State1002, State1003, State1004, State1005, State1006,
                      State1007, State1008, State1009, State1010, State1011, State1012,
                      StateS1, StateS2, StateS3);

            //--------------------------------------------------
            // 添加方法测试
            //--------------------------------------------------

            // Link Condition 白名单
            Link(State11, State100, Con1, false);
            LinkWhiteList(State100, Con3, false, State50, State51, State52);

            // Link Condition 黑名单
            Link(State12, State102, Con3, false);
            // 50, 51, 52
            LinkBlackList(State102, Con5, false,
                          State11, State12, State21, State22,
                          State60, State61, State62,
                          State100, State102, State103, State104, State105, State106, State107, State108,
                          State1000, State1001, State1002, State1003, State1004, State1005, State1006,
                          State1007, State1008, State1009, State1010, State1011, State1012,
                          StateS1, StateS2, StateS3);

            // Link Condition AnyState
            Link(State11, State103, Con2, false);
            Link(State103, Con2, false, true);

            Link(State12, State104, Con1, false);
            Link(State104, Con4, false, false);

            // Link Condition 混合冲突
            Link(State51, State105, Con6, false);
            LinkWhiteList(State105, Con6, false, State50, State51, State52);
            // 11, 12, 50
            LinkBlackList(State105, Con6, false,
                          State21, State22,
                          State51, State52, State60, State61, State62,
                          State100, State102, State103, State104, State105, State106, State107, State108,
                          State1000, State1001, State1002, State1003, State1004, State1005, State1006,
                          State1007, State1008, State1009, State1010, State1011, State1012,
                          StateS1, StateS2, StateS3);

            // Link EventListener 白名单
            Link(State21, State100, Evt1);
            LinkWhiteList(State100, Evt3, State60, State61, State62);

            // Link EventListener 黑名单
            Link(State22, State102, Evt3);
            // 60, 61, 62
            LinkBlackList(State102, Evt5,
                          State11, State12, State21, State22,
                          State50, State51, State52,
                          State100, State102, State103, State104, State105, State106, State107, State108,
                          State1000, State1001, State1002, State1003, State1004, State1005, State1006,
                          State1007, State1008, State1009, State1010, State1011, State1012,
                          StateS1, StateS2, StateS3);

            // Link EventListener AnyState
            Link(State21, State103, Evt2);
            Link(State103, Evt2, true);

            Link(State22, State104, Evt1);
            Link(State104, Evt4, false);

            // Link EventListener 混合冲突
            Link(State61, State105, Evt6);
            LinkWhiteList(State105, Evt6, State60, State61, State62);
            // 21, 22, 60
            LinkBlackList(State105, Evt6,
                          State11, State12,
                          State50, State51, State52, State61, State62,
                          State100, State102, State103, State104, State105, State106, State107, State108,
                          State1000, State1001, State1002, State1003, State1004, State1005, State1006,
                          State1007, State1008, State1009, State1010, State1011, State1012,
                          StateS1, StateS2, StateS3);

            // Link ExitCode 普通添加
            Link(State11, State106, 0);
            Link(State11, State107, 1);

            // Link ExitCode 默认添加及混合冲突
            Link(State21, State108, 0);
            Link(State21, State108);

            // Link ExitCode + Condition
            Link(State1005, State1013, 0, Con1013, false, State1017, State1018);
            Link(State1013, State1005);

            Link(State1005, State1014, 0, Con1014, false);
            Link(State1014, State1005);

            Link(State1005, State1015, 1, Con1015, false);
            Link(State1015, State1005);

            Link(State1005, State1016, 1, Con1016, false);
            Link(State1016, State1005);

            // Book 白名单
            BookServiceProvider(StateS1, ServiceProvider1);
            BookServiceProvider(StateS1, ServiceProvider2);
            BookServiceProviderWhiteList(ServiceProvider3, StateS1, StateS2);

            BookEventListener(StateS1, EventListener1);
            BookEventListener(StateS1, EventListener2);
            BookEventListenerWhiteList(EventListener3, StateS1, StateS2);

            // Book 黑名单（冲突）
            BookServiceProvider(StateS3, ServiceProvider2);
            BookServiceProvider(StateS3, ServiceProvider3);
            BookServiceProvider(StateS1, ServiceProvider4);
            // S1, S2
            BookServiceProviderBlackList(ServiceProvider4,
                                         State11, State12, State21, State22,
                                         State50, State51, State52, State60, State61, State62,
                                         State100, State102, State103, State104, State105, State106, State107, State108,
                                         State1000, State1001, State1002, State1003, State1004, State1005, State1006,
                                         State1007, State1008, State1009, State1010, State1011, State1012,
                                         StateS3);

            BookEventListener(StateS3, EventListener1);
            BookEventListener(StateS3, EventListener2);
            BookEventListener(StateS1, EventListener4);
            // S1, S2
            BookEventListenerBlackList(EventListener4,
                                       State11, State12, State21, State22,
                                       State50, State51, State52, State60, State61, State62,
                                       State100, State102, State103, State104, State105, State106, State107, State108,
                                       State1000, State1001, State1002, State1003, State1004, State1005, State1006,
                                       State1007, State1008, State1009, State1010, State1011, State1012,
                                       StateS3);

            // Book AnyState
            BookServiceProvider(ServiceProvider5);
            BookEventListener(EventListener5);

            //--------------------------------------------------
            // 基本流程测试
            //--------------------------------------------------

            SetEntry(State1000);
            Link(State1000, State1001, Con1001, false);
            Link(State1001, State1000);
            Link(State1000, State1002, Con1002, true); // 测试反转条件
            Link(State1002, State1001);
            Link(State1001, State1003, 1);
            Link(State1003, State1004, Evt1001);
            Link(State1004, State1003);
            Link(State1003, State1005, Evt1002);

            //--------------------------------------------------
            // 路径测试
            //--------------------------------------------------

            // 路径无视 ExitCode
            Link(State1006, State1012, 0); // 默认出口

            // 普通路径
            Link(State1005, State1008, Con1006_1, false, State1006, State1007);

            // 路径中断
            Link(State1008, State1010, Con1009, false, State1009);

            // 中断转向
            Link(State1009, State1011, Con1006_2, false, State1006);

            //--------------------------------------------------
            // 条件复用
            //--------------------------------------------------

            Link(State1011, State1010, Con1001, false);
            Link(State1010, State1005, Evt1001);
        }
    }
}