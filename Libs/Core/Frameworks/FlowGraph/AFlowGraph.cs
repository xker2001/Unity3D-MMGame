using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMGame.FlowGraph
{
    abstract public class AFlowGraph : AFlowAction
    {
        private class ActionBinding
        {
            public readonly AFlowAction Action;
            public readonly Func<bool> Condition;
            public readonly float Delay;
            public int Times; // -1 为无限制

            public ActionBinding(AFlowAction action, Func<bool> condition, float delay, int times)
            {
                Action = action;
                Condition = condition;
                Delay = delay;
                Times = times;
            }
        }

        private readonly HashSet<ActionBinding> playBindings = new HashSet<ActionBinding>();
        private readonly HashSet<ActionBinding> playPendings = new HashSet<ActionBinding>();
        private readonly HashSet<ActionBinding> stopBindings = new HashSet<ActionBinding>();
        private readonly HashSet<ActionBinding> stopPendings = new HashSet<ActionBinding>();

        virtual protected void OnDisable()
        {
            if (IsPlaying)
            {
                Stop();
            }
        }

        abstract protected void Design();

        protected void BindPlay(AFlowAction action, Func<bool> condition, float delay, int times)
        {
            // 只重置被播放的 action，不负责重置被停止的 action。
            // 只被停止的 action 应该由外部负责播放的 Graph 重置。
            action.Reset();
            playBindings.Add(new ActionBinding(action, condition, delay, times));
        }

        protected void BindStop(AFlowAction action, Func<bool> condition, float delay, int times)
        {
            stopBindings.Add(new ActionBinding(action, condition, delay, times));
        }

        protected override void ExecutePlay()
        {
            Design();
            UpdateManager.RegisterUpdate(FastUpdate);
        }

        protected override void ExecuteStop()
        {
            StopAllCoroutines();
            playPendings.Clear();
            stopPendings.Clear();

            foreach (ActionBinding binding in playBindings)
            {
                binding.Action.Stop();
            }

            foreach (ActionBinding binding in stopBindings)
            {
                binding.Action.Stop();
            }

            playBindings.Clear();
            stopBindings.Clear();
            UpdateManager.UnregisterUpdate(FastUpdate);
        }

        private void FastUpdate(float deltatime)
        {
            foreach (ActionBinding binding in playBindings)
            {
                if (binding.Times == 0 || binding.Action.IsPlaying ||
                    !binding.Condition() || playPendings.Contains(binding))
                {
                    continue;
                }

                if (Mathf.Approximately(binding.Delay, 0))
                {
                    binding.Action.Play();

                    if (binding.Times > 0)
                    {
                        binding.Times -= 1;
                    }
                }
                else
                {
                    StartCoroutine(OperatePlayAction(binding, binding.Delay));
                    playPendings.Add(binding);
                }
            }

            foreach (ActionBinding binding in stopBindings)
            {
                if (binding.Times == 0 || !binding.Action.IsPlaying ||
                    !binding.Condition() || stopPendings.Contains(binding))
                {
                    continue;
                }

                if (Mathf.Approximately(binding.Delay, 0))
                {
                    binding.Action.Stop();

                    if (binding.Times > 0)
                    {
                        binding.Times -= 1;
                    }
                }
                else
                {
                    StartCoroutine(OperateStopAction(binding, binding.Delay));
                    stopPendings.Add(binding);
                }
            }
        }

        private IEnumerator OperatePlayAction(ActionBinding binding, float delay)
        {
            yield return new WaitForSeconds(delay);

            binding.Action.Play();

            if (binding.Times > 0)
            {
                binding.Times -= 1;
            }

            playPendings.Remove(binding);
        }

        private IEnumerator OperateStopAction(ActionBinding binding, float delay)
        {
            yield return new WaitForSeconds(delay);

            binding.Action.Stop();

            if (binding.Times > 0)
            {
                binding.Times -= 1;
            }

            stopPendings.Remove(binding);
        }
    }
}