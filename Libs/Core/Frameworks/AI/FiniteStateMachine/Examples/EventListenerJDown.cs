using UnityEngine;

namespace MMGame.AI.FiniteStateMachine.Example
{
    public class EventListenerJDown : EventListener
    {
        // 检测 J 键的按下事件
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                IsTriggered = true;
            }
        }

        public override void OnSpawn()
        {
        }

        public override void OnDespawn()
        {
        }
    }
}