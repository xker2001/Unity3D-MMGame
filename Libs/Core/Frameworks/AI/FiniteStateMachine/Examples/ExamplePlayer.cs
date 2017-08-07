using UnityEngine;

namespace MMGame.AI.FiniteStateMachine.Example
{
    // 控制状态机的启用和停用。
    // 如果状态机在 GameObject 激活时启用，则可以省略这个类，直接在状态机中设置 PlayOnEnable 为 true 即可。
    // 多个状态机可以同时运行。
    public class ExamplePlayer : MonoBehaviour
    {
        [SerializeField]
        private FsmSystem fsmPlayer;


        void Start()
        {
            // 初始化状态机，也可以由 Run() 进行初始化
            fsmPlayer.Init();

            // 运行状态机
            // 执行 Run 的时候，如果状态机还未初始化，会先执行初始化方法
            fsmPlayer.Run();
        }
    }
}