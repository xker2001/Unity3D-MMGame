using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MMGame
{
    /// <summary>
    /// Sprite 拖拽控制类。
    /// 必须与 SpriteRenderer + Collider2D 配合使用。
    /// </summary>
    public class DragAndDropSprite : PoolBehaviour, IInitializePotentialDragHandler,
                                     IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // 允许其他组件从这里注册拖拽相关事件

        public event Action<PointerEventData> InitializePotentialDrag;
        public event Action<PointerEventData> BeginDrag;
        public event Action<PointerEventData> Dragging;
        public event Action<PointerEventData> EndDrag;

        private Vector3 pointerOffset; // 鼠标指针到物体中心的偏差
        private float zDistToCamera; // 摄像机到物体的 Z 距离

        /// <summary>
        /// 用于在 inspector 中显示 checkbox，方便调试。
        /// </summary>
        void Start()
        {
        }

        public override void OnSpawn()
        {
        }

        public override void OnDespawn()
        {
            InitializePotentialDrag = null;
            BeginDrag = null;
            Dragging = null;
            EndDrag = null;
        }

        /// <summary>
        /// 接口实现，初始化可能的拖拽。
        /// </summary>
        /// <param name="eventData">事件数据。</param>
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (InitializePotentialDrag != null)
            {
                InitializePotentialDrag(eventData);
            }
        }

        /// <summary>
        /// 接口实现，开始拖拽。
        /// </summary>
        /// <param name="eventData">事件数据。</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // sprite 起始位置
            Vector3 startPosition = transform.position;

            // 物体到摄像机的 z 距离
            zDistToCamera = Mathf.Abs(startPosition.z - Camera.main.transform.position.z);

            // 指针到物体的距离偏差
            Vector3 pointerScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistToCamera);
            pointerOffset = startPosition - Camera.main.ScreenToWorldPoint(pointerScreenPos);

            // 禁用 collider 以支持 OnDrop（遮挡了接收物体）
            GetComponent<Collider2D>().enabled = false;

            if (BeginDrag != null)
            {
                BeginDrag(eventData);
            }
        }

        /// <summary>
        /// 接口实现，拖拽过程更新。
        /// </summary>
        /// <param name="eventData">事件数据。</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1)
            {
                return;
            }

            Vector3 pointerScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistToCamera);
            transform.position = Camera.main.ScreenToWorldPoint(pointerScreenPos) + pointerOffset;

            if (Dragging != null)
            {
                Dragging(eventData);
            }
        }

        /// <summary>
        /// 接口实现，结束拖拽。
        /// </summary>
        /// <param name="eventData">事件数据。</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            GetComponent<Collider2D>().enabled = true;

            if (EndDrag != null)
            {
                EndDrag(eventData);
            }
        }
    }
}