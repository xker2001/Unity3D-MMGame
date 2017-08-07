using UnityEngine;

namespace MMGame.AI.FiniteStateMachine
{
    public class OrConditions : Condition
    {
        [SerializeField]
        private Condition[] conditions;

        protected override void OnInit()
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                conditions[i].Init();
            }
        }

        protected override bool OnCheck()
        {
            bool result = false;

            for (int i = 0; i < conditions.Length; i++)
            {
                result = result || conditions[i].Check();

                if (result)
                {
                    return true;
                }
            }

            return false;
        }
    }
}