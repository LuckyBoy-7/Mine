using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Managers;
using UnityEngine;
using UnityEngine.Serialization;


namespace Lucky.Interactive 
{
    /// <summary>
    /// 注意先把相机大小放大100倍
    /// </summary>
    public class GameCursor : Singleton<GameCursor>
    {
        public static Vector2 MouseWorldPos => Camera.main.ScreenToWorldPoint(Input.mousePosition);
        public static Vector2 MouseWorldCellPos => new(Mathf.Floor(MouseWorldPos.x + 0.5f), Mathf.Floor(MouseWorldPos.y + 0.5f));
        private Vector2 PreviousMouseWorldPosition { get; set; }
        private InteractableBase PreviousInteractable { get; set; }
        private InteractableBase CurrentInteractable { get; set; }
        public InteractableBase MouseButtonDownInteractable { get; set; } // 点击时对应的第一个对象
        private float MouseButtonDownTimestamp { get; set; } = -1;
        private float RealtimeSinceMouseButtonDown => Time.realtimeSinceStartup - MouseButtonDownTimestamp;
        [Header("Click")] [SerializeField] private float clickTimeThreshold = 0.2f;
        [Header("LongPress")] [SerializeField] private float longPressTimeThreshold = 0.8f;
        [SerializeField] private float longPressOffsetTolerance = 0.1f;
        private bool IsLongPressShake { get; set; } // 鼠标位置是否有偏移（长按必须一开始就长按，不能刚开始按，然后鼠标歪了还能长按（不然有点反直觉））
        [Header("Wipe")] [SerializeField] private float wipeDistanceThreshold = 3f;

        private void Update()
        {
            UpdateCurrentInteractable();
            // 如果是刚开始的话，就把previous改成当前位置
            if (PreviousMouseWorldPosition == default)
                PreviousMouseWorldPosition = MouseWorldPos;

            if (CurrentInteractable != PreviousInteractable)
                PreviousInteractable?.CursorExit();
            if (CurrentInteractable != PreviousInteractable)
                CurrentInteractable?.CursorEnter();
            CurrentInteractable?.CursorHover();
            if (Input.GetMouseButtonDown(0))
            {
                MouseButtonDownTimestamp = Time.realtimeSinceStartup;
                MouseButtonDownInteractable = CurrentInteractable;
                CurrentInteractable?.CursorPress();
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 delta = MouseWorldPos - PreviousMouseWorldPosition;
                MouseButtonDownInteractable?.CursorDrag(delta);

                // longPress
                if (delta.magnitude > longPressOffsetTolerance)
                    IsLongPressShake = true;
                if (RealtimeSinceMouseButtonDown >= longPressTimeThreshold && !IsLongPressShake)
                {
                    IsLongPressShake = true;
                    MouseButtonDownInteractable?.CursorLongPress();
                }

                // wipe
                if (CurrentInteractable != null)
                {
                    CurrentInteractable.WipeDistanceAccumulator += delta.magnitude;
                    if (CurrentInteractable.WipeDistanceAccumulator >= wipeDistanceThreshold)
                    {
                        CurrentInteractable.WipeDistanceAccumulator -= wipeDistanceThreshold;
                        CurrentInteractable.CursorWipe();
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                IsLongPressShake = false;
                if (RealtimeSinceMouseButtonDown <= clickTimeThreshold)
                    MouseButtonDownInteractable?.CursorClick();
                MouseButtonDownInteractable?.CursorRelease();

                if (MouseButtonDownInteractable != null && MouseButtonDownInteractable.IsPositionInBounds(MouseWorldPos))
                    MouseButtonDownInteractable?.CursorReleaseInBounds();
                MouseButtonDownInteractable = null;
            }

            PreviousMouseWorldPosition = MouseWorldPos;
        }

        private void UpdateCurrentInteractable()
        {
            // 拿到当前鼠标指向的所有的Interactable
            List<InteractableBase> hitInteractables = new();
            // 正交透视相机都适用
            foreach (var hitCollider in Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition)))
            {
                var component = hitCollider.collider.GetComponent<InteractableBase>();
                if (component != null && component.canInteract)
                    hitInteractables.Add(component);
            }

            // 找个最高的
            InteractableBase topInteractable = null;
            foreach (var curInteractable in hitInteractables)
            {
                if (topInteractable == null || curInteractable.CompareSortingOrder(topInteractable) == 1)
                    topInteractable = curInteractable;
            }

            // 更新
            PreviousInteractable = CurrentInteractable;
            CurrentInteractable = topInteractable;
        }
    }
}