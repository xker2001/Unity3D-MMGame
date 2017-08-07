using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MMGame.AI.FiniteStateMachine
{
    abstract public class FsmSystem : MonoBehaviour
    {
        /// <summary>
        /// 用于储存状态连线目标状态和中间路径的数据类。
        /// </summary>
        class ToAndList
        {
            public FsmState To;
            public FsmState[] StateList;

            public ToAndList(FsmState to, FsmState[] stateList)
            {
                To = to;
                StateList = stateList;
            }
        }

        [SerializeField]
        private bool playOnEnable;

        void OnEnable()
        {
            if (playOnEnable)
            {
                Run();
            }
        }

        void OnDisable()
        {
            if (IsRunning)
            {
                Stop();
            }
        }

        //--------------------------------------------------
        // 状态机结构
        //--------------------------------------------------

        /// <summary>
        /// 状态机中所有的状态。
        /// </summary>
        private FsmState[] states;

        /// <summary>
        /// 入口状态。
        /// </summary>
        private FsmState entry;

        /// <summary>
        /// 基于 ICondition 的连接数据。
        /// </summary>
        private Dictionary<FsmState, Dictionary<ICondition, ToAndList>> graphConditionConnections;

        /// <summary>
        /// 基于 Event 的连接数据。
        /// </summary>
        private Dictionary<FsmState, Dictionary<EventListener, ToAndList>> graphEventConnections;

        /// <summary>
        /// 基于 Exit 的连接数据。
        /// </summary>
        private Dictionary<FsmState, Dictionary<int, ToAndList>> graphExitConnections;

        /// <summary>
        /// 基于 Exit+Condition 的连接数据。
        /// </summary>
        private Dictionary<FsmState, Dictionary<int, Dictionary<ICondition, ToAndList>>> graphExitConditionConnections;

        /// <summary>
        /// 状态附加的 ServiceProvider 集合。
        /// </summary>
        private Dictionary<FsmState, List<ServiceProvider>> graphServiceProviders;

        /// <summary>
        /// 状态附加的 EventListener 集合。
        /// </summary>
        private Dictionary<FsmState, List<EventListener>> graphEventListeners;

        /// <summary>
        /// 结果反转的 Condition 字典。
        /// </summary>
        private Dictionary<ICondition, ICondition> graphInverseConditions;

        //--------------------------------------------------
        // 维护运行状态的私有变量
        //--------------------------------------------------

        /// <summary>
        /// 当前迁移的最终终点状态。
        /// </summary>
        private FsmState currTarget;

        /// <summary>
        /// 当前正在执行的状态路径。
        /// 如果不经路径直接转移到目标状态，则为 null。
        /// </summary>
        private FsmState[] currPath;

        /// <summary>
        /// 当前状态在当前路径上的索引。
        /// 等于 -1 时即可认为当前不在中间状态上。
        /// </summary>
        private int indexOnPath = -1;

        /// <summary>
        /// 当前状态的独立服务列表。
        /// </summary>
        private List<ServiceProvider> currServiceProviders;

        /// <summary>
        /// 当前状态的事件服务列表。
        /// </summary>
        private List<EventListener> currEventListeners;

        /// <summary>
        /// 当前状态 ICondition 类型的连接数据。
        /// </summary>
        private Dictionary<ICondition, ToAndList> currConditionConnections;

        /// <summary>
        /// 当前状态 EventListener 类型的连接数据。
        /// </summary>
        private Dictionary<EventListener, ToAndList> currEventConnections;

        /// <summary>
        /// 当前状态 ExitCode 类型的连接数据。
        /// </summary>
        private Dictionary<int, ToAndList> currExitConnections;

        /// <summary>
        /// 当前状态 ExitCode+Condition 类型的连接数据。
        /// </summary>
        private Dictionary<int, Dictionary<ICondition, ToAndList>> currExitConditionConnections;

        /// <summary>
        /// 状态机是否已经初始化。
        /// </summary>
        private bool isInitialized;

        //--------------------------------------------------
        // 公有成员
        //--------------------------------------------------

        /// <summary>
        /// 当前状态。
        /// </summary>
        public FsmState CurrentState { get; private set; }

        /// <summary>
        /// 状态机是否在运行。
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 状态机是否处于暂停状态。
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// 初始化状态机。
        /// 包括搭建状态机内部结构，初始化各状态。
        /// </summary>
        public void Init()
        {
            if (isInitialized)
            {
                return;
            }

            InitGraphVariables();
            Design();

            isInitialized = true;
        }

        /// <summary>
        /// 启动状态机。
        /// </summary>
        /// <param name="state">初始状态，为 null 时使用默认的入口状态。</param>
        public void Run(FsmState state = null)
        {
            // 如果状态机还没有初始化，则初始化一下
            Init();

            if (IsRunning)
            {
                Debug.LogFormat("The FsmSystem is already running: {0}", name);
                return;
            }

            state = state == null ? entry : state;

            if (state == null)
            {
                Debug.LogErrorFormat("Try to run with a null state: {0}", name);
                return;
            }

            // 进入新状态
            state.Enter();
            CurrentState = state;

            // 更新当前状态数据 - 连接
            UpdateCurrentConnections();

            // 更新当前状态数据 - 附加服务
            currServiceProviders = GetBookedServices(state, graphServiceProviders);
            currEventListeners = GetBookedServices(state, graphEventListeners);

            // 启动当前状态的附加服务
            EnableCurrentServices();

            UpdateManager.RegisterUpdate(OnUpdate);
            IsRunning = true;
        }

        /// <summary>
        /// 暂停状态机。
        /// </summary>
        public void Pause()
        {
            PauseState(CurrentState);
            IsPaused = true;
        }

        /// <summary>
        /// 恢复运行状态机。
        /// </summary>
        public void Resume()
        {
            ResumeState(CurrentState);
            IsPaused = false;
        }

        /// <summary>
        /// 停止并重置状态机。
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            StopCurrentServices();
            CurrentState.Stop();
            ResetCurrentData();
            UpdateManager.UnregisterUpdate(OnUpdate);

            IsRunning = false;
            IsPaused = false;
        }

        /// <summary>
        /// 在这里设计状态机结构。
        /// </summary>
        abstract protected void Design();

        //--------------------------------------------------
        // 用于设计状态机结构的方法
        //--------------------------------------------------

        /// <summary>
        /// 设置状态机中的状态。
        /// 注意！必须在其他设计指令之前调用！
        /// </summary>
        /// <param name="states">状态机中所有的状态。</param>
        protected void SetStates(params FsmState[] states)
        {
            if (states.Length == 0)
            {
                Debug.LogError("Try to set an empty FsmSystem!");
                return;
            }

            if (this.states != null)
            {
                Debug.LogError("The states have been set already!");
                return;
            }

            this.states = states;
        }

        /// <summary>
        /// 设置默认的入口状态。
        /// </summary>
        /// <param name="state">入口状态。</param>
        protected void SetEntry(FsmState state)
        {
            entry = state;
        }

        //-----------------------------------
        // Link with condition
        //-----------------------------------

        /// <summary>
        /// 用条件连接任意状态到终点状态，当条件满足时中断当前状态并转移。
        /// 如果希望在特定起点和终点状态之前加入路径，则为它们单独创建一个带路径的 Link。
        /// </summary>
        /// <param name="to">终点状态。</param>
        /// <param name="condition">条件类的实例。</param>
        /// <param name="includeSelf">是否允许从自身转移到自身。</param>
        protected void Link(FsmState to, ICondition condition, bool inverse, bool includeSelf = false)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (!includeSelf && states[i] == to)
                {
                    continue;
                }

                Link(states[i], to, GetCondition(condition, inverse), false, graphConditionConnections);
            }
        }

        /// <summary>
        /// 用条件和路径连接两个状态，当条件满足时中断当前状态并转移。
        /// </summary>
        /// <param name="from">起点状态。</param>
        /// <param name="to">终点状态。</param>
        /// <param name="condition">条件类的实例。</param>
        /// <param name="path">起点和终点状态之间的过渡路径。</param>
        protected void Link(FsmState from, FsmState to, ICondition condition, bool inverse, params FsmState[] path)
        {
            Link(from, to, GetCondition(condition, inverse), true, graphConditionConnections, path);
        }


        [Obsolete("Miss parameter: inverse", true)]
        protected void Link(FsmState from, FsmState to, ICondition condition, FsmState inverse, params FsmState[] path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 用条件连接白名单中的状态到终点状态，当条件满足时中断当前状态并转移。
        /// 如果希望在特定起点和终点状态之前加入路径，则为它们单独创建一个带路径的 Link。
        /// </summary>
        /// <param name="to">终点状态。</param>
        /// <param name="condition">条件类的实例。</param>
        /// <param name="whiteList">状态白名单。</param>
        protected void LinkWhiteList(FsmState to, ICondition condition, bool inverse, params FsmState[] whiteList)
        {
            for (int i = 0; i < whiteList.Length; i++)
            {
                Link(whiteList[i], to, GetCondition(condition, inverse), false, graphConditionConnections);
            }
        }

        [Obsolete("Miss parameter: inverse", true)]
        protected void LinkWhiteList(FsmState to, ICondition condition, FsmState inverse, params FsmState[] whiteList)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 用条件连接非黑名单中的状态到终点状态，当条件满足时中断当前状态并转移。
        /// 如果希望在特定起点和终点状态之前加入路径，则为它们单独创建一个带路径的 Link。
        /// </summary>
        /// <param name="to">终点状态。</param>
        /// <param name="condition">条件。</param>
        /// <param name="blackList">状态黑名单。</param>
        protected void LinkBlackList(FsmState to, ICondition condition, bool inverse, params FsmState[] blackList)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (Array.Exists(blackList, x => x == states[i]))
                {
                    continue;
                }

                Link(states[i], to, GetCondition(condition, inverse), false, graphConditionConnections);
            }
        }

        [Obsolete("Miss parameter: inverse", true)]
        protected void LinkBlackList(FsmState to, ICondition condition, FsmState inverse, params FsmState[] blackList)
        {
            throw new NotImplementedException();
        }

        //-----------------------------------
        // Link with event listener
        //-----------------------------------

        /// <summary>
        /// 用事件连接任意状态到终点状态，当事件触发时中断当前状态并转移。
        /// 如果希望在特定起点和终点状态之前加入路径，则为它们单独创建一个带路径的 Link。
        /// </summary>
        /// <param name="to">终点状态。</param>
        /// <param name="eventListener">事件处理组件。</param>
        /// <param name="includeSelf">是否允许从自身转移到自身。</param>
        protected void Link(FsmState to, EventListener eventListener, bool includeSelf = false)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (!includeSelf && states[i] == to)
                {
                    continue;
                }

                Link(states[i], to, eventListener, false, graphEventConnections);
                BookEventListener(states[i], eventListener);
            }
        }

        /// <summary>
        /// 用事件和路径连接两个状态，当事件触发时中断当前状态并转移。
        /// </summary>
        /// <param name="from">起点状态。</param>
        /// <param name="to">终点状态。</param>
        /// <param name="eventListener">事件处理组件。</param>
        /// <param name="path">起点和终点状态之间的过渡路径。</param>
        protected void Link(FsmState from, FsmState to, EventListener eventListener, params FsmState[] path)
        {
            Link(from, to, eventListener, true, graphEventConnections, path);
            BookEventListener(from, eventListener);
        }

        /// <summary>
        /// 用事件连接白名单中的状态到终点状态，当事件触发时中断当前状态并转移。
        /// 如果希望在特定起点和终点状态之前加入路径，则为它们单独创建一个带路径的 Link。
        /// </summary>
        /// <param name="to">终点状态。</param>
        /// <param name="eventListener">事件处理组件。</param>
        /// <param name="whiteList">状态白名单。</param>
        protected void LinkWhiteList(FsmState to, EventListener eventListener, params FsmState[] whiteList)
        {
            for (int i = 0; i < whiteList.Length; i++)
            {
                Link(whiteList[i], to, eventListener, false, graphEventConnections);
                BookEventListener(whiteList[i], eventListener);
            }
        }

        /// <summary>
        /// 用事件连接非黑名单中的状态到终点状态，当事件触发时中断当前状态并转移。
        /// 如果希望在特定起点和终点状态之前加入路径，则为它们单独创建一个带路径的 Link。
        /// </summary>
        /// <param name="to">终点状态。</param>
        /// <param name="eventListener">事件处理组件。</param>
        /// <param name="blackList">状态黑名单。</param>
        protected void LinkBlackList(FsmState to, EventListener eventListener, params FsmState[] blackList)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (Array.Exists(blackList, x => x == states[i]))
                {
                    continue;
                }

                Link(states[i], to, eventListener, false, graphEventConnections);
                BookEventListener(states[i], eventListener);
            }
        }

        //-----------------------------------
        // Link with exit code
        //-----------------------------------

        /// <summary>
        /// 用退出码和路径连接两个状态。
        /// </summary>
        /// <param name="from">起点状态。</param>
        /// <param name="to">终点状态。</param>
        /// <param name="exitCode">出口编号。</param>
        /// <param name="path">起点和终点状态之间的过渡路径。</param>
        protected void Link(FsmState from, FsmState to, int exitCode, params FsmState[] path)
        {
            Link(from, to, exitCode, true, graphExitConnections, path);
        }

        /// <summary>
        /// 用默认退出码（0）和路径连接两个状态。
        /// </summary>
        /// <param name="from">起点状态。</param>
        /// <param name="to">终点状态。</param>
        /// <param name="path">起点和终点状态之间的过渡路径。</param>
        protected void Link(FsmState from, FsmState to, params FsmState[] path)
        {
            Link(from, to, 0, path);
        }

        //--------------------------------------------------
        // Link with exit code and condition
        //--------------------------------------------------

        /// <summary>
        /// 用退出码和条件和路径连接两个状态，当起点状态按指定退出码退出并满足条件时按路径迁移到终点状态。
        /// </summary>
        /// <param name="from">起点状态。</param>
        /// <param name="to">终点状态。</param>
        /// <param name="exitCode">退出码。</param>
        /// <param name="condition">条件类的实例。</param>
        /// <param name="path">起点和终点状态之间的过渡路径。</param>
        protected void Link(FsmState from, FsmState to, int exitCode, ICondition condition, bool inverse,
                            params FsmState[] path)
        {
            Dictionary<int, Dictionary<ICondition, ToAndList>> connections;
            Dictionary<ICondition, ToAndList> subConnections;

            condition = GetCondition(condition, inverse);

            if (!graphExitConditionConnections.TryGetValue(from, out connections))
            {
                connections = new Dictionary<int, Dictionary<ICondition, ToAndList>>();
                subConnections = new Dictionary<ICondition, ToAndList>();
                subConnections.Add(condition, new ToAndList(to, path));
                connections.Add(exitCode, subConnections);
                graphExitConditionConnections.Add(from, connections);
                return;
            }

            if (!connections.TryGetValue(exitCode, out subConnections))
            {
                subConnections = new Dictionary<ICondition, ToAndList>();
                subConnections.Add(condition, new ToAndList(to, path));
                connections.Add(exitCode, subConnections);
                return;
            }

            ToAndList toAndList;

            if (!subConnections.TryGetValue(condition, out toAndList))
            {
                toAndList = new ToAndList(to, path);
                subConnections.Add(condition, toAndList);
                return;
            }

            throw new Exception(string.Format(
                                    "Try to override exist exit+condition connection:" +
                                    "\n{0} --> {2} : {1}\n{0} --> {3} : exit({4}) + {1}\n",
                                    from.GetType().Name,
                                    condition.GetType().Name,
                                    toAndList.To.GetType().Name,
                                    to.GetType().Name,
                                    exitCode));
        }

        [Obsolete("Miss parameter: inverse", true)]
        protected void Link(FsmState from, FsmState to, int exitCode, ICondition condition, FsmState inverse,
                            params FsmState[] path)
        {
            throw new NotImplementedException();
        }

        //-----------------------------------
        // Book service provider
        //-----------------------------------

        /// <summary>
        /// 为所有状态预订一个独立服务。
        /// </summary>
        /// <param name="serviceProvider">服务组件实例。</param>
        protected void BookServiceProvider(ServiceProvider serviceProvider)
        {
            for (int i = 0; i < states.Length; i++)
            {
                BookService(states[i], serviceProvider, graphServiceProviders);
            }
        }

        /// <summary>
        /// 为指定状态预订一个独立服务。
        /// 该服务跟随状态启用和停用。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="serviceProvider">服务组件实例。</param>
        protected void BookServiceProvider(FsmState state, ServiceProvider serviceProvider)
        {
            BookService(state, serviceProvider, graphServiceProviders);
        }

        /// <summary>
        /// 为白名单中的状态预订一个独立服务。
        /// </summary>
        /// <param name="serviceProvider">服务组件实例。</param>
        /// <param name="whiteList">状态白名单。</param>
        protected void BookServiceProviderWhiteList(ServiceProvider serviceProvider, params FsmState[] whiteList)
        {
            for (int i = 0; i < whiteList.Length; i++)
            {
                BookService(whiteList[i], serviceProvider, graphServiceProviders);
            }
        }

        /// <summary>
        /// 为除了黑名单的所有状态预订一个独立服务。
        /// </summary>
        /// <param name="serviceProvider">服务组件实例。</param>
        /// <param name="blackList">状态黑名单。</param>
        protected void BookServiceProviderBlackList(ServiceProvider serviceProvider, params FsmState[] blackList)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (Array.Exists(blackList, x => x == states[i]))
                {
                    continue;
                }

                BookService(states[i], serviceProvider, graphServiceProviders);
            }
        }

        //-----------------------------------
        // Book event listener
        //-----------------------------------

        /// <summary>
        /// 为所有状态预订一个事件侦听和处理服务。
        /// 该服务跟随状态启用和停用。
        /// </summary>
        /// <param name="eventListener">事件侦听服务的实例。</param>
        protected void BookEventListener(EventListener eventListener)
        {
            for (int i = 0; i < states.Length; i++)
            {
                BookService(states[i], eventListener, graphEventListeners);
            }
        }

        /// <summary>
        /// 为指定状态预订一个事件侦听和处理服务。
        /// 该服务跟随状态启用和停用。
        /// </summary>
        /// <param name="state">预订服务的状态。</param>
        /// <param name="eventListener">事件侦听服务的实例。</param>
        protected void BookEventListener(FsmState state, EventListener eventListener)
        {
            BookService(state, eventListener, graphEventListeners);
        }

        /// <summary>
        /// 为白名单中的状态预订一个事件侦听和处理服务。
        /// </summary>
        /// <param name="eventListener">事件侦听服务的实例。</param>
        /// <param name="whiteList">状态白名单。</param>
        protected void BookEventListenerWhiteList(EventListener eventListener, params FsmState[] whiteList)
        {
            for (int i = 0; i < whiteList.Length; i++)
            {
                BookService(whiteList[i], eventListener, graphEventListeners);
            }
        }

        /// <summary>
        /// 为除了黑名单的所有状态预订一个事件侦听和处理服务。
        /// </summary>
        /// <param name="eventListener">事件侦听服务的实例。</param>
        /// <param name="blackList">状态黑名单。</param>
        protected void BookEventListenerBlackList(EventListener eventListener, params FsmState[] blackList)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (Array.Exists(blackList, x => x == states[i]))
                {
                    continue;
                }

                BookService(states[i], eventListener, graphEventListeners);
            }
        }

        //--------------------------------------------------
        // 私有方法
        //--------------------------------------------------

        /// <summary>
        /// 初始化状态机结构数据（字典）
        /// </summary>
        private void InitGraphVariables()
        {
            graphConditionConnections = new Dictionary<FsmState, Dictionary<ICondition, ToAndList>>();
            graphEventConnections = new Dictionary<FsmState, Dictionary<EventListener, ToAndList>>();
            graphExitConnections = new Dictionary<FsmState, Dictionary<int, ToAndList>>();
            graphExitConditionConnections =
                new Dictionary<FsmState, Dictionary<int, Dictionary<ICondition, ToAndList>>>();
            graphServiceProviders = new Dictionary<FsmState, List<ServiceProvider>>();
            graphEventListeners = new Dictionary<FsmState, List<EventListener>>();
            graphInverseConditions = new Dictionary<ICondition, ICondition>();
        }

        /// <summary>
        /// 重置所有用于维护状态机状态的数据。
        /// </summary>
        private void ResetCurrentData()
        {
            currTarget = null;
            currPath = null;
            indexOnPath = -1;
            currServiceProviders = null;
            currEventListeners = null;
            currConditionConnections = null;
            currEventConnections = null;
            currExitConnections = null;
            currExitConditionConnections = null;
        }

        /// <summary>
        /// 执行暂停一个状态。
        /// </summary>
        /// <param name="state">被执行的状态。</param>
        private void PauseState(FsmState state)
        {
            if (state == null)
            {
                return;
            }

            state.Pause();
            PauseCurrentServices();
        }

        /// <summary>
        /// 执行恢复一个状态的运行。
        /// </summary>
        /// <param name="state">被执行的状态。</param>
        private void ResumeState(FsmState state)
        {
            if (state == null)
            {
                return;
            }

            state.Resume();
            ResumeCurrentServices();
        }

        /// <summary>
        /// 状态更新函数，由 UpdateManager 调用。
        /// </summary>
        /// <param name="deltaTime">心跳时长。</param>
        private void OnUpdate(float deltaTime)
        {
            if (IsPaused)
            {
                return;
            }

            // 检查事件连线
            if (currEventConnections != null)
            {
                foreach (KeyValuePair<EventListener, ToAndList> kv in currEventConnections)
                {
                    if (kv.Key.IsTriggered)
                    {
                        kv.Key.ResetTrigger();
                        TransitToTarget(kv.Value);
                        CurrentState.UpdateState(deltaTime);
                        return;
                    }
                }
            }

            // 检查条件连线
            if (currConditionConnections != null)
            {
                foreach (KeyValuePair<ICondition, ToAndList> kv in currConditionConnections)
                {
                    if (!kv.Key.IsInitialized)
                    {
                        kv.Key.Init();
                    }

                    if (kv.Key.Check())
                    {
                        TransitToTarget(kv.Value);
                        CurrentState.UpdateState(deltaTime);
                        return;
                    }
                }
            }

            // 检查出口连线
            if (CurrentState.ExitCode >= 0)
            {
                if (indexOnPath != -1) // 如果当前是中间状态，无视 ExitCode，沿路径过渡
                {
                    if (indexOnPath == currPath.Length - 1)
                    {
                        TransitTo(currTarget);
                        indexOnPath = -1;
                    }
                    else
                    {
                        TransitTo(currPath[indexOnPath + 1]);
                        indexOnPath += 1;
                    }

                    CurrentState.UpdateState(deltaTime);
                }
                else // 当前不是中间状态，根据 ExitCode 选择下一个状态
                {
                    ToAndList toAndList;

                    // 存在普通出口
                    if (currExitConnections != null &&
                            currExitConnections.TryGetValue(CurrentState.ExitCode, out toAndList))
                    {
                        TransitToTarget(toAndList);
                        CurrentState.UpdateState(deltaTime);
                    }
                    // 检查 Exit + Condition 出口
                    else
                    {
                        Dictionary<ICondition, ToAndList> subConnections;

                        if (currExitConditionConnections != null &&
                                currExitConditionConnections.TryGetValue(CurrentState.ExitCode, out subConnections))
                        {
                            foreach (KeyValuePair<ICondition, ToAndList> kv in subConnections)
                            {
                                if (!kv.Key.IsInitialized)
                                {
                                    kv.Key.Init();
                                }

                                if (kv.Key.Check())
                                {
                                    TransitToTarget(kv.Value);
                                    CurrentState.UpdateState(deltaTime);
                                    return;
                                }
                            }
                        }

                        Debug.LogErrorFormat("State exits whithout an exit connection : {0}", CurrentState.name);
                        Debug.Break();
                    }
                }

                return;
            }

            CurrentState.UpdateState(deltaTime);
        }

        /// <summary>
        /// 迁移到终点状态。
        /// 如果存在中间状态，则经由中间状态到达终点状态。
        /// </summary>
        /// <param name="toAndList">目标状态机路径数据。</param>
        private void TransitToTarget(ToAndList toAndList)
        {
            currTarget = toAndList.To;
            currPath = toAndList.StateList;

            if (currPath.Length > 0)
            {
                TransitTo(currPath[0]);
                indexOnPath = 0;
            }
            else
            {
                TransitTo(currTarget);
                indexOnPath = -1;
            }
        }

        /// <summary>
        /// 执行迁移到指定状态。
        /// </summary>
        /// <param name="state">下一个状态。</param>
        private void TransitTo(FsmState state)
        {
            // 获取下一个状态的服务
            var serviceProviders = GetBookedServices(state, graphServiceProviders);
            var eventListeners = GetBookedServices(state, graphEventListeners);

            // 停止下一个状态不需要的服务
            if (currServiceProviders != null)
            {
                for (int i = 0; i < currServiceProviders.Count; i++)
                {
                    if (serviceProviders == null || serviceProviders.IndexOf(currServiceProviders[i]) == -1)
                    {
                        currServiceProviders[i].Disable();
                    }
                }
            }

            if (currEventListeners != null)
            {
                for (int i = 0; i < currEventListeners.Count; i++)
                {
                    currEventListeners[i].ResetTrigger(); // 清除所有事件的 Trigger 标记

                    if (eventListeners == null || eventListeners.IndexOf(currEventListeners[i]) == -1)
                    {
                        currEventListeners[i].Disable();
                    }
                }
            }

            // 停止当前状态
            CurrentState.Stop();

            // 进入新状态
            state.Enter();
            CurrentState = state;

            // 更新当前状态连接数据
            UpdateCurrentConnections();

            // 更新当前状态附加服务数据
            currServiceProviders = serviceProviders;
            serviceProviders = null;

            currEventListeners = eventListeners;
            eventListeners = null;

            // 启动当前状态的附加服务
            EnableCurrentServices();
        }

        /// <summary>
        /// 更新当前状态的连接数据。
        /// </summary>
        private void UpdateCurrentConnections()
        {
            currConditionConnections = GetConnections(CurrentState, graphConditionConnections);
            currEventConnections = GetConnections(CurrentState, graphEventConnections);
            currExitConnections = GetConnections(CurrentState, graphExitConnections);
            currExitConditionConnections = GetConnections(CurrentState, graphExitConditionConnections);
        }

        /// <summary>
        /// 启动当前状态的附加服务。
        /// </summary>
        private void EnableCurrentServices()
        {
            if (currServiceProviders != null)
            {
                for (int i = 0; i < currServiceProviders.Count; i++)
                {
                    currServiceProviders[i].Enable();
                }
            }

            if (currEventListeners != null)
            {
                for (int i = 0; i < currEventListeners.Count; i++)
                {
                    currEventListeners[i].ResetTrigger();
                    currEventListeners[i].Enable();
                }
            }
        }

        /// <summary>
        /// 停止当前状态的全部附加服务。
        /// </summary>
        private void StopCurrentServices()
        {
            if (currServiceProviders != null)
            {
                for (int i = 0; i < currServiceProviders.Count; i++)
                {
                    currServiceProviders[i].Disable();
                }
            }

            if (currEventListeners != null)
            {
                for (int i = 0; i < currEventListeners.Count; i++)
                {
                    currEventListeners[i].Disable();
                    currEventListeners[i].ResetTrigger();
                }
            }
        }

        /// <summary>
        /// 暂停当前状态的附加服务。
        /// </summary>
        private void PauseCurrentServices()
        {
            if (currServiceProviders != null)
            {
                for (int i = 0; i < currServiceProviders.Count; i++)
                {
                    currServiceProviders[i].Pause();
                }
            }

            if (currEventListeners != null)
            {
                for (int i = 0; i < currEventListeners.Count; i++)
                {
                    currEventListeners[i].Pause();
                }
            }
        }

        /// <summary>
        /// 恢复当前状态的附加服务。
        /// </summary>
        private void ResumeCurrentServices()
        {
            if (currServiceProviders != null)
            {
                for (int i = 0; i < currServiceProviders.Count; i++)
                {
                    currServiceProviders[i].Resume();
                }
            }

            if (currEventListeners != null)
            {
                for (int i = 0; i < currEventListeners.Count; i++)
                {
                    currEventListeners[i].Resume();
                }
            }
        }

        /// <summary>
        /// 根据是否反转返回正确的 ICondition 实例。
        /// 如果不反转，直接返回原实例；如果反转，创建一个 InverseCondition 实例并返回。
        /// </summary>
        /// <param name="state">条件实例。</param>
        /// <param name="dic">是否反转。</param>
        /// <returns>条件实例。</returns>
        private ICondition GetCondition(ICondition condition, bool inverse)
        {
            if (!inverse)
            {
                return condition;
            }

            ICondition inverseCondition;

            if (!graphInverseConditions.TryGetValue(condition, out inverseCondition))
            {
                inverseCondition = new InverseCondition(condition);
                graphInverseConditions.Add(condition, inverseCondition);
            }

            return inverseCondition;
        }

        /// <summary>
        /// 设置在指定状态下满足特定条件时（中断）并转移到终点状态。
        /// </summary>
        /// <param name="from">起点状态。</param>
        /// <param name="to">终点状态。</param>
        /// <param name="connectionType">从上一个状态迁移到下一个状态的条件（条件、事件、状态结束）。</param>
        /// <param name="warnOverride">重复设置时是否发出警告。</param>
        /// <param name="dic">储存设置的字典。</param>
        /// <param name="path">从起点到终点经过的路径。</param>
        /// <typeparam name="T">导致转移发生的条件。</typeparam>
        private void Link<T>(FsmState from,
                             FsmState to,
                             T connectionType,
                             bool warnOverride,
                             Dictionary<FsmState, Dictionary<T, ToAndList>> dic,
                             params FsmState[] path)
        {
            Dictionary<T, ToAndList> connections;

            // 没有 from 键，新建一个
            if (!dic.TryGetValue(from, out connections))
            {
                connections = new Dictionary<T, ToAndList>();
                connections.Add(connectionType, new ToAndList(to, path));
                dic.Add(from, connections);
                return;
            }

            ToAndList toAndList;

            // 没有 connectionType 键，新建一个
            if (!connections.TryGetValue(connectionType, out toAndList))
            {
                toAndList = new ToAndList(to, path);
                connections.Add(connectionType, toAndList);
                return;
            }

            if (toAndList.To != to)
            {
                throw new Exception(string.Format(
                                        "Try to override exist connection:\n{0} --> {2} : {1}\n{0} --> {3} : {1}\n",
                                        from.GetType().Name,
                                        connectionType.GetType().Name,
                                        toAndList.To.GetType().Name,
                                        to.GetType().Name));
            }

            // 如果还没有路径则添加新的路径
            if (toAndList.StateList.Length == 0)
            {
                toAndList.StateList = path;
            }
            // 如果已经存在路径则返回
            else if (warnOverride)
            {
                Debug.LogError("Try to override the path of a conditional connection.");
            }
        }

        /// <summary>
        /// 为状态预订一个服务（ServiceProvider or EventListener）。
        /// </summary>
        /// <param name="state">预订服务的状态。</param>
        /// <param name="service">服务实例。</param>
        /// <param name="dic">储存设置的字典。</param>
        /// <typeparam name="T">服务的类型。</typeparam>
        private void BookService<T>(FsmState state, T service, Dictionary<FsmState, List<T>> dic)
        where T : ServiceComponent
        {
            List<T> services;

            if (!dic.TryGetValue(state, out services))
            {
                services = new List<T>();
                dic.Add(state, services);
            }

            if (!services.Contains(service))
            {
                service.Disable();
                services.Add(service);
            }
        }

        /// <summary>
        /// 获取指定状态预订的服务。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="dic">储存服务的字典。</param>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>服务列表，没有是返回 null。</returns>
        private List<T> GetBookedServices<T>(FsmState state, Dictionary<FsmState, List<T>> dic)
        where T : ServiceComponent
        {
            List<T> list;

            if (!dic.TryGetValue(state, out list))
            {
                return null;
            }

            return list;
        }

        /// <summary>
        /// 获取指定状态指定类型连接的数据（ExitCode+Condition 类型除外）。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="dic">储存所有指定类型连接数据的字典。</param>
        /// <typeparam name="T">连接类型。</typeparam>
        /// <returns>指定类型的连接数据字典，没有时返回 null。</returns>
        private Dictionary<T, ToAndList> GetConnections<T>(FsmState state,
                Dictionary<FsmState, Dictionary<T, ToAndList>> dic)
        {
            Dictionary<T, ToAndList> connections;

            if (!dic.TryGetValue(state, out connections))
            {
                return null;
            }

            return connections;
        }

        private Dictionary<int, Dictionary<ICondition, ToAndList>>
        GetConnections(FsmState state,
                       Dictionary<FsmState, Dictionary<int, Dictionary<ICondition, ToAndList>>> dic)
        {
            Dictionary<int, Dictionary<ICondition, ToAndList>> connections;

            if (!dic.TryGetValue(state, out connections))
            {
                return null;
            }

            return connections;
        }

        //--------------------------------------------------
        // 开放状态机结构数据，供测试用
        //--------------------------------------------------

        // 获取指定状态的 Conditions，没有时为 null
        public ICondition[] GetConditions(FsmState state)
        {
            return GetConnectionTypes(state, graphConditionConnections);
        }

        // 获取指定状态的 EventListeners，没有时为 null
        public EventListener[] GetEventListeners(FsmState state)
        {
            return GetConnectionTypes(state, graphEventConnections);
        }

        // 获取指定状态的 ExitCodes，没有时为 null
        public int[] GetExitCodes(FsmState state)
        {
            return GetConnectionTypes(state, graphExitConnections);
        }

        // 获取指定状态指定 Condtion 的目标状态和路径
        public bool GetConditionTo(FsmState state, ICondition condition, out FsmState to, out FsmState[] path)
        {
            return GetTo(state, condition, graphConditionConnections, out to, out path);
        }

        // 获取指定状态指定 EventListener 的目标状态和路径
        public bool GetEventTo(FsmState state, EventListener eventListener, out FsmState to, out FsmState[] path)
        {
            return GetTo(state, eventListener, graphEventConnections, out to, out path);
        }

        // 获取指定状态指定 ExitCode 的目标状态和路径
        public bool GetExitTo(FsmState state, int exitCode, out FsmState to, out FsmState[] path)
        {
            return GetTo(state, exitCode, graphExitConnections, out to, out path);
        }

        // 获取指定状态的 ExitCode+Condition 连接
        public bool GetExitConditionTo(FsmState state, int exitCode, ICondition condition,
                                       out FsmState to, out FsmState[] path)
        {
            to = null;
            path = null;

            Dictionary<int, Dictionary<ICondition, ToAndList>> connections;

            if (!graphExitConditionConnections.TryGetValue(state, out connections))
            {
                return false;
            }

            Dictionary<ICondition, ToAndList> subConnections;

            if (!connections.TryGetValue(exitCode, out subConnections))
            {
                return false;
            }

            ToAndList toAndList;

            if (!subConnections.TryGetValue(condition, out toAndList))
            {
                return false;
            }

            to = toAndList.To;
            path = new FsmState[toAndList.StateList.Length];
            toAndList.StateList.CopyTo(path, 0);

            return true;
        }

        // 获取指定状态的独立 ServiceProvider 列表，没有时为 null
        public IList<ServiceProvider> GetBookedServiceProviders(FsmState state)
        {
            return GetBookedServices(state, graphServiceProviders).AsReadOnly();
        }

        // 获取指定状态的独立 EventListener 列表，没有时为 null
        public IList<EventListener> GetBookedEventListeners(FsmState state)
        {
            return GetBookedServices(state, graphEventListeners).AsReadOnly();
        }

        /// <summary>
        /// 获取指定状态指定类型的迁移条件（条件、事件、出口）数组。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="dic">储存指定类型连接的字典。</param>
        /// <typeparam name="T">迁移条件的类型。</typeparam>
        /// <returns>迁移条件数组。</returns>
        private T[] GetConnectionTypes<T>(FsmState state, Dictionary<FsmState, Dictionary<T, ToAndList>> dic)
        {
            Dictionary<T, ToAndList> connections;

            if (!dic.TryGetValue(state, out connections))
            {
                return null;
            }

            return connections.Keys.ToArray<T>();
        }

        /// <summary>
        /// 获取指定状态指定条件的目标状态和路径。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="connectionType">迁移条件。</param>
        /// <param name="dic">储存指定类型连接的字典。</param>
        /// <param name="to">终点状态。</param>
        /// <param name="path">状态路径。</param>
        /// <typeparam name="T">迁移条件的类型。</typeparam>
        /// <returns>连接存在返回 true，反之则返回 false。</returns>
        private bool GetTo<T>(FsmState state,
                              T connectionType,
                              Dictionary<FsmState, Dictionary<T, ToAndList>> dic,
                              out FsmState to,
                              out FsmState[] path)
        {
            to = null;
            path = null;

            Dictionary<T, ToAndList> connections;

            if (!dic.TryGetValue(state, out connections))
            {
                return false;
            }

            ToAndList toAndList;

            if (!connections.TryGetValue(connectionType, out toAndList))
            {
                return false;
            }

            to = toAndList.To;
            path = new FsmState[toAndList.StateList.Length];
            toAndList.StateList.CopyTo(path, 0);

            return true;
        }
    }
}