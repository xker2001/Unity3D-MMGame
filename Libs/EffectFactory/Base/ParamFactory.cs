using System;
using UnityEngine;

namespace MMGame.EffectFactory
{
    /// <summary>
    /// 参数工厂。
    /// </summary>
    [Serializable]
    abstract public class ParamFactory
    {
        abstract public bool IsNull();

        /// <summary>
        /// 生成一个物体。
        /// </summary>
        /// <returns>参数工厂生成的物体。</returns>
        abstract protected ParamObject Produce();

        /// <summary>
        /// 在指定位置生成一个物体。
        /// </summary>
        /// <param name="position">生成的位置。</param>
        /// <returns>参数工厂生成的物体。</returns>
        protected ParamObject Produce(Vector3 position)
        {
            if (IsNull())
            {
                throw new ApplicationException("Try to create ParamObject from a null ParamFactory.");
            }

            ParamObject obj = Produce();
            obj.transform.position = position;
            return obj;
        }

        /// <summary>
        /// 在指定位置，按指定方向生成一个物体。
        /// </summary>
        /// <param name="position">生成的位置。</param>
        /// <param name="rotation">生成的方向。</param>
        /// <returns>参数工厂生成的物体。</returns>
        protected ParamObject Produce(Vector3 position, Quaternion rotation)
        {
            ParamObject obj = Produce();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }

        /// <summary>
        /// 生成一个物体，挂接到指定的父节点并对其位置和方向。
        /// </summary>
        /// <param name="parent">父节点。</param>
        /// <returns>参数工厂生成的物体。</returns>
        protected ParamObject Produce(Transform parent)
        {
            ParamObject obj = Produce(parent.position, parent.rotation);
            obj.transform.parent = parent;
            return obj;
        }
    }
}