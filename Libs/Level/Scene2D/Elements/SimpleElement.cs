using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 最简单的场景元素，只有基本属性（位置、旋转、缩放）。
    /// </summary>
    public class SimpleElement : ASceneElement
    {
        private Transform entity;
        private Vector3 originalScale;

        public override void OnBecameVisible(ALayerCamera layerCamera)
        {
            if (IsVisible)
            {
                return;
            }

            Assert.IsNull(entity);

            entity = PoolManager.Spawn(PoolName, Prefab, Position, Rotation);
            originalScale = entity.localScale;
            entity.localScale = Vector3.Scale(originalScale, RelativeScale);
            SetToCameraLayer(entity.gameObject, layerCamera);
            IsVisible = true;
        }

        public override void OnBecameInvisible(ALayerCamera layerCamera)
        {
            if (!IsVisible)
            {
                return;
            }

            Assert.IsNotNull(entity);

            entity.localScale = originalScale;
            PoolManager.Despawn(entity);
            entity = null;
            IsVisible = false;
        }

        protected override void OnReset()
        {
            if (entity != null)
            {
                entity.localScale = originalScale;
                PoolManager.Despawn(entity);
                entity = null;
            }
        }
    }
}