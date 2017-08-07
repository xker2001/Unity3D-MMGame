using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.Scene2D
{
    /// <summary>
    /// 基于 LayerCamera 和一维场景（卷轴）的 Spawner。
    /// 1. 从当前已填充距离连续生成场景元素到目标距离，不能仅在可视范围内生成（更优化）。
    /// 2. 生成的元素输送到 SceneCulling 中接受可见性管理。
    /// 3. 假设屏幕分辨率不会中途改变。
    /// 4. 正反向的设定与屏幕坐标系相同。
    /// </summary>
    public class OneDDistanceSpawner : MonoBehaviour
    {
        private enum AlignType
        {
            LeftOrTop,
            RightOrBottom,
        }

        /// <summary>
        /// 层摄像机，用于判断可见性。
        /// </summary>
        [SerializeField]
        private ALayerCamera layerCamera;

        /// <summary>
        /// 场景是水平卷轴还是垂直卷轴。
        /// </summary>
        [SerializeField]
        private ScrollAxis scrollAxis;

        /// <summary>
        /// 起点处第一个 Sprite 与屏幕的对齐方式。
        /// </summary>
        [SerializeField]
        private AlignType startAlignType;

        /// <summary>
        /// 透视摄像机计算可视区域大小时所用射线的长度。
        /// </summary>
        [SerializeField]
        private float cameraRayLength = 10;

        /// <summary>
        /// 安全可视区域扩展比例。
        /// 安全可视区域 = 屏幕区域 * safeRangeInPercent
        /// </summary>
        [Tooltip("安全可视区域 = 屏幕区域 * 扩展比例。")]
        [SerializeField]
        private float safeVisibleAreaPercent = 1.2f;

        /// <summary>
        /// 场景可视化管理器，场景元素的容器。
        /// </summary>
        [SerializeField]
        private ASceneCulling sceneCulling;

        /// <summary>
        /// 单步场景元素生成器。
        /// </summary>
        [SerializeField]
        private AOneDDistanceSpawnExecutor spawnExecutor;

        [SerializeField]
        private ASpawnElementModifier[] elementModifiers;

        /// <summary>
        /// 已经被元素填充的正向方向上的（距离摄像机初始位置的）距离。
        /// 等于最新场景元素的正向端边界位置。
        /// </summary>
        private float filledPositiveDistance;

        /// <summary>
        /// 已经被元素填充的反向方向上的（距离摄像机初始位置的）距离。
        /// 等于最新场景元素的反向端边界位置。
        /// </summary>
        private float filledNegativeDistance;

        void Awake()
        {
            Assert.IsNotNull(layerCamera);
            Assert.IsNotNull(layerCamera.Camera);
            Assert.IsNotNull(sceneCulling);
            Assert.IsNotNull(spawnExecutor);

            // 根据对齐方式和屏幕区域计算正反向起始填充距离
            float length = GetSafeVisibleAreaLength();

            if (startAlignType == AlignType.LeftOrTop)
            {
                filledPositiveDistance = -length * 0.5f;
                filledNegativeDistance = filledPositiveDistance;
            }
            else
            {
                filledPositiveDistance = length * 0.5f;
                filledNegativeDistance = filledPositiveDistance;
            }

            // 设置单步 Spawner 的属性
            spawnExecutor.LayerCamera = layerCamera;
            spawnExecutor.ScrollAxis = scrollAxis;
        }

        void OnEnable()
        {
            layerCamera.CameraUpdated += UpdateSpawn;
        }

        void OnDisable()
        {
            layerCamera.CameraUpdated -= UpdateSpawn;
        }

        /// <summary>
        /// 摄像机更新回调。
        /// </summary>
        private void UpdateSpawn(ALayerCamera layerCamera)
        {
            float camViewPositiveDist;
            float camViewNegativeDist;
            GetCameraViewDistance(out camViewPositiveDist, out camViewNegativeDist);

            if (filledPositiveDistance < camViewPositiveDist)
            {
                filledPositiveDistance =
                    SpawnToPositiveDistance(filledPositiveDistance, camViewPositiveDist, sceneCulling);
            }

            if (filledNegativeDistance > camViewNegativeDist)
            {
                filledNegativeDistance =
                    SpawnToNegativeDistance(filledNegativeDistance, camViewNegativeDist, sceneCulling);
            }
        }

        /// <summary>
        /// 填充场景元素到正向目标距离。
        /// </summary>
        /// <param name="filledDistance">已被填充的正向距离。</param>
        /// <param name="targetDistance">新的最大距离。</param>
        /// <param name="sceneCulling">场景元素可见性管理器。</param>
        /// <returns>新的已填充距离。</returns>
        private float SpawnToPositiveDistance(float filledDistance,
                                              float targetDistance,
                                              ASceneCulling sceneCulling)
        {
            if (filledDistance >= targetDistance)
            {
                return filledDistance;
            }

            while (filledDistance < targetDistance)
            {
                ASceneElement element;

                if (!spawnExecutor.SpawnOnePositiveElement(out element, ref filledDistance))
                {
                    break;
                }

                for (int i = 0; i < elementModifiers.Length; i++)
                {
                    if (elementModifiers[i] != null)
                    {
                        elementModifiers[i].Modify(element);
                    }
                }

                sceneCulling.AddElement(element);
            }

            return filledDistance;
        }

        /// <summary>
        /// 填充场景元素到正向目标距离。
        /// </summary>
        /// <param name="filledDistance">已被填充的反向距离。</param>
        /// <param name="targetDistance">新的最大距离。</param>
        /// <param name="sceneCulling">场景元素可见性管理器。</param>
        /// <returns>新的已填充距离。</returns>
        private float SpawnToNegativeDistance(float filledDistance,
                                              float targetDistance,
                                              ASceneCulling sceneCulling)
        {
            if (filledDistance <= targetDistance)
            {
                return filledDistance;
            }

            while (filledDistance > targetDistance)
            {
                ASceneElement element;

                if (!spawnExecutor.SpawnOneNegativeElement(out element, ref filledDistance))
                {
                    break;
                }

                for (int i = 0; i < elementModifiers.Length; i++)
                {
                    if (elementModifiers[i] != null)
                    {
                        elementModifiers[i].Modify(element);
                    }
                }

                sceneCulling.AddElement(element);
            }

            return filledDistance;
        }

        /// <summary>
        /// 获取当前摄像机位置下安全可视区域的最大距离。
        /// </summary>
        /// <param name="positiveDistance"></param>
        /// <param name="negativeDistance"></param>
        private void GetCameraViewDistance(out float positiveDistance, out float negativeDistance)
        {
            Vector3 deltaCamPos = layerCamera.Position - layerCamera.StartPosition;

            float camDistance = scrollAxis == ScrollAxis.Horizontal ? deltaCamPos.x : deltaCamPos.y;
            float viewLen = GetSafeVisibleAreaLength();
            positiveDistance = camDistance + viewLen * 0.5f;
            negativeDistance = camDistance - viewLen * 0.5f;
        }

        /// <summary>
        /// 获取当前安全可视范围在卷轴方向上世界单位长度。
        /// </summary>
        /// <returns>长度值。</returns>
        private float GetSafeVisibleAreaLength()
        {
            Vector2 screenUnitSize = layerCamera.GetScreenUnitSize(cameraRayLength);
            float len = scrollAxis == ScrollAxis.Horizontal ? screenUnitSize.x : screenUnitSize.y;
            return len * safeVisibleAreaPercent;
        }
    }
}