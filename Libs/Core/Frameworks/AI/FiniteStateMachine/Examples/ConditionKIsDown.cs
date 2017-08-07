using UnityEngine;

namespace MMGame.AI.FiniteStateMachine.Example
{
    public class ConditionKIsDown : Condition
    {
        // 在第一次检查条件结果时使用。Condition 继承了 MonoBehaviour，因此
        // 也可以使用 Awake() 和 Start() 进行初始化。
        protected override void OnInit()
        {
            // do something
        }

        // 状态机每帧都会状态检查可用的条件
        protected override bool OnCheck()
        {
            return Input.GetKey(KeyCode.K);
        }
    }
}