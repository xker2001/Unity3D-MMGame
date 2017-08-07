using UnityEngine;

namespace MMGame.AI.FiniteStateMachine.Example
{
    public class FsmSystemPlayer : FsmSystem
    {
        // 行走状态
        [SerializeField]
        private FsmState stateWalk;

        // 跳跃状态
        [SerializeField]
        private FsmState stateJump;

        // 发呆状态
        [SerializeField]
        private FsmState stateIdle;

        // 死亡状态
        [SerializeField]
        private FsmState stateDie;

        // 从发呆到死亡的过渡状态
        [SerializeField]
        private FsmState stateFallDownFromIdle;

        // 从行走到死亡的过渡状态
        [SerializeField]
        private FsmState stateFallDownFromWalk;

        // 从跳跃到死亡的过渡状态
        [SerializeField]
        private FsmState stateFallDownFromJump;

        // 条件检查：K 键是否处于按下状态
        [SerializeField]
        private Condition conditionKIsDown;

        // 事件侦听服务：角色死亡事件
        [SerializeField]
        private EventListener eventListenerDie;

        // 事件侦听服务：J 键被按下
        [SerializeField]
        private EventListener eventListenerJDown;

        [SerializeField]
        private ServiceProvider componentShowHud;

        // 构建状态机的结构和流程
        protected override void Design()
        {
            // 将所有要用到的状态添加到状态机，必须最先执行
            SetStates(stateWalk, stateJump, stateIdle, stateDie,
                      stateFallDownFromIdle, stateFallDownFromWalk, stateFallDownFromJump);

            // 设置状态机的默认入口
            SetEntry(stateIdle);

            // 当满足条件 K 键处于按下状态时，Idle 状态转为 Walk 状态
            Link(stateIdle, stateWalk, conditionKIsDown, false);

            // 当满足条件 K 键处于松开状态时，Walk 状态转为 Idle 状态
            // 参数 true 指明对 conditionKIsDown 的结果进行反转
            Link(stateWalk, stateIdle, conditionKIsDown, true);

            // 当满足条件 J 键被按下，Walk 状态转为 Jump 状态
            Link(stateWalk, stateJump, eventListenerJDown);

            // 当 Jump 状态结束(Exit)后，如果 K 键是按下的则转为 Walk 状态，反之转为 Idle 状态
            Link(stateJump, stateWalk, 0, conditionKIsDown, false);
            Link(stateJump, stateIdle, 0, conditionKIsDown, true);

            // 当发生 Die 事件时，Idle 状态转为 Die 状态，中间通过对应的 FallDown 状态过渡
            Link(stateIdle, stateDie, eventListenerDie, stateFallDownFromIdle);

            // 当发生 Die 事件时，Walk 状态转为 Die 状态，中间通过对应的 FallDown 状态过渡
            Link(stateWalk, stateDie, eventListenerDie, stateFallDownFromWalk);

            // 当发生 Die 事件时，Jump 状态转为 Die 状态，中间通过对应的 FallDown 状态过渡
            Link(stateJump, stateDie, eventListenerDie, stateFallDownFromJump);

            // 为除了 Die 状态之外的所有状态预订死亡事件侦听服务
            BookEventListenerBlackList(eventListenerDie, stateDie);

            // 为所有状态预订显示 hud 的独立服务
            BookServiceProvider(componentShowHud);
        }
    }
}