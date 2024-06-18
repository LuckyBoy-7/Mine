using System;
using UnityEngine;

namespace Mine.States._2D
{
    public class PhysicsFSM<T> : FSM<T> where T : Enum
    {
        [Header("RaycastCheck")] public new Rigidbody2D rigidbody;
        public Collider2D physicalBoxCollider;
        public float boxLeftRightCastDist = 0.1f;
        public float wallCheckBoxHeight = 0.95f;
        public float wallCheckBoxPosYRatio = 0.5f; // 表示这个检测的box的中心位置 
        public float boxDownCastDist = 0.1f;
        public LayerMask groundLayer;

        public Vector2Int facingDirection = Vector2Int.right;

        private Vector3 boxSize => physicalBoxCollider.bounds.size;
        private Vector3 center => physicalBoxCollider.bounds.center;
        private Vector3 downPos => center + (boxSize.y + boxDownCastDist) / 2 * Vector3.down;
        private Vector3 upPos => center + (boxSize.y + boxDownCastDist) / 2 * Vector3.up;

        // 但是这样写会导致对象在缩放的时候相对碰撞位置发生改变（相对厚度甚至也会改变），但还是要根据需求改，所以问题不大
        private Vector3 leftPos => center + (boxSize.x + boxLeftRightCastDist) / 2 * Vector3.left +
                                   (wallCheckBoxPosYRatio - 0.5f) * Vector3.up;

        private Vector3 rightPos => center + (boxSize.x + boxLeftRightCastDist) / 2 * Vector3.right +
                                    (wallCheckBoxPosYRatio - 0.5f) * Vector3.up;

        private Vector3 upDownSize => new(boxSize.x * 0.95f, boxDownCastDist);
        private Vector3 leftRightSize => new(boxLeftRightCastDist, wallCheckBoxHeight);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            // Ground Box
            var position = transform.position;
            Gizmos.DrawWireCube(downPos, upDownSize);
            // Wall Box
            Gizmos.DrawWireCube(leftPos, leftRightSize);
            Gizmos.DrawWireCube(rightPos, leftRightSize);
        }


        public bool isHittingCeiling
        {
            get
            {
                var collider = Physics2D.OverlapBox(upPos, upDownSize, 0, groundLayer);
                // 如果是碰到除了单向通过的天花板，要加 !collider.CompareTag("OneWaySlab")
                return collider != null && rigidbody.velocity.y > 1e-3;
            }
        }


        public bool isTouchingGround => Physics2D.OverlapBox(downPos, upDownSize, 0, groundLayer);

        protected bool isTouchingRightWall => Physics2D.OverlapBox(rightPos, leftRightSize, 0, groundLayer);

        protected bool isTouchingLeftWall => Physics2D.OverlapBox(leftPos, leftRightSize, 0, groundLayer);

        public bool isTouchingWall => isTouchingLeftWall || isTouchingRightWall;
    }
}