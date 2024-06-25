namespace Lucky.States._25D
{
    using System.Collections.Generic;
    using UnityEngine;
    using Lucky.Extensions;
    using static UnityEngine.Mathf;

    public enum PlayerMovementStateType
    {
        Idle,
        Run,
        Dash,
    }

    public enum PlayerMovementAbility
    {
        Dash,
    }

    /// <summary>
    /// Player的基本运动
    /// </summary>
    public class PlayerMovementFSM : SingletonFSM<PlayerMovementStateType>
    {
        public new Rigidbody2D rigidbody;
        [Header("Debug")] public bool isAllAbilityDebug;
        public bool hasDashAbility;

        [Header("Abilities")] public Dictionary<PlayerMovementAbility, bool> hasAbilityDic = new();

        [Header("Input")] public Vector2 keyDownDirection;
        public Vector2 facingDirection = Vector2.right;

        [Header("Run")] public float runMaxSpeed = 10;
        public float runVelocityChangeSpeed = 100;
        [Header("Idle")] public float idleVelocityChangeSpeed = 60;

        [Header("Dash")] public float dashSpeed = 35;
        public float dashDuration = 0.15f;
        public float dashBufferTime = 0.1f;
        [HideInInspector] public float dashBufferExpireTime = -1;
        public float dashCoolDown = 0.2f;
        [HideInInspector] public float dashCoolDownExpireTime = -1;


        [Header("Key")] public KeyCode leftKey = KeyCode.A;
        public KeyCode rightKey = KeyCode.D;
        public KeyCode upKey = KeyCode.W;
        public KeyCode downKey = KeyCode.S;
        public KeyCode dashKey = KeyCode.J;

        public bool runTrigger => Input.GetKey(leftKey) || Input.GetKey(rightKey) || Input.GetKey(upKey) ||
                                  Input.GetKey(downKey);

        public bool dashTrigger => HasAbility(PlayerMovementAbility.Dash) &&
                                   Time.time <= dashBufferExpireTime && Time.time >= dashCoolDownExpireTime;

        private bool HasAbility(PlayerMovementAbility type)
        {
            return hasAbilityDic.GetValueOrDefault(type, false);
        }

        private void UnlockAbility(PlayerMovementAbility type)
        {
            hasAbilityDic[type] = true;
        }

        void Start()
        {
            dashBufferExpireTime = -1;
            dashCoolDownExpireTime = -1;

            states[PlayerMovementStateType.Idle] = new PlayerIdle(this);
            states[PlayerMovementStateType.Run] = new PlayerRun(this);
            states[PlayerMovementStateType.Dash] = new PlayerDash(this);
            TransitionState(PlayerMovementStateType.Idle);

            UnlockAbility(PlayerMovementAbility.Dash);
        }

        #region Update

        protected override void Update()
        {
            // todo: 根据游戏状态决定要不要更新

            UpdateKeyDownDirection();
            UpdateInputBuffer();
            TryUpdateDebug();

            base.Update();
        }

        private void TryUpdateDebug()
        {
            if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.D))
                isDebug = true;

            if (!isDebug)
                return;


            if (isAllAbilityDebug)
            {
                hasAbilityDic[PlayerMovementAbility.Dash] = hasDashAbility;
            }
        }

        private void UpdateInputBuffer()
        {
            if (Input.GetKeyDown(dashKey))
                dashBufferExpireTime = Time.time + dashBufferTime;
        }

        #endregion

        private void UpdateKeyDownDirection()
        {
            if (Input.GetKeyDown(leftKey) || (!Input.GetKey(rightKey) && Input.GetKey(leftKey)))
                keyDownDirection.x = -1;
            else if (Input.GetKeyDown(rightKey) || (!Input.GetKey(leftKey) && Input.GetKey(rightKey)))
                keyDownDirection.x = 1;
            else if (!Input.GetKey(leftKey) && !Input.GetKey(rightKey))
                keyDownDirection.x = 0;

            if (Input.GetKeyDown(downKey) || (!Input.GetKey(upKey) && Input.GetKey(downKey)))
                keyDownDirection.y = -1;
            else if (Input.GetKeyDown(upKey) || (!Input.GetKey(downKey) && Input.GetKey(upKey)))
                keyDownDirection.y = 1;
            else if (!Input.GetKey(downKey) && !Input.GetKey(upKey))
                keyDownDirection.y = 0;
        }

        public void TryUpdateFacingDirection() // 看起来像是没什么用，但是如果我放黑波的时候改变了方向，下次就算没动键盘放出来的波会向相反方向移动
        {
            var (x, y) = keyDownDirection;
            if (x != 0)
                facingDirection.x = x;
            facingDirection.y = y;
            transform.localScale = transform.localScale.WithX(Abs(transform.localScale.x) * facingDirection.x);
        }


        private void LerpVelocityX(float to, float k)
        {
            var newX = MoveTowards(rigidbody.velocity.x, to, k * Time.fixedDeltaTime);
            rigidbody.velocity = new Vector2(newX, rigidbody.velocity.y);
        }

        private void LerpVelocityY(float to, float k)
        {
            var newY = MoveTowards(rigidbody.velocity.y, to, k * Time.fixedDeltaTime);
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, newY);
        }

        public void LerpVelocity(Vector2 velocity, float k1, float k2)
        {
            LerpVelocityX(velocity.x, k1);
            LerpVelocityY(velocity.y, k2);
        }

        public void LerpVelocity(float x, float y, float k1, float k2)
        {
            LerpVelocityX(x, k1);
            LerpVelocityY(y, k2);
        }
    }

    public class PlayerIdle : IState
    {
        private PlayerMovementFSM m;

        public PlayerIdle(PlayerMovementFSM m) => this.m = m;

        public void OnEnter()
        {
        }

        public void OnUpdate()
        {
            m.TryUpdateFacingDirection();
            if (m.runTrigger)
                m.TransitionState(PlayerMovementStateType.Run);
            else if (m.dashTrigger)
                m.TransitionState(PlayerMovementStateType.Dash);
        }

        public void OnFixedUpdate()
        {
            m.LerpVelocity(Vector2.zero, m.idleVelocityChangeSpeed, m.idleVelocityChangeSpeed);
        }

        public void OnExit()
        {
        }
    }

    public class PlayerRun : IState
    {
        private PlayerMovementFSM m;

        public PlayerRun(PlayerMovementFSM m) => this.m = m;

        public void OnEnter()
        {
        }

        public void OnUpdate()
        {
            m.TryUpdateFacingDirection();
            if (!m.runTrigger)
                m.TransitionState(PlayerMovementStateType.Idle);
            else if (m.dashTrigger)
                m.TransitionState(PlayerMovementStateType.Dash);
        }

        public void OnFixedUpdate()
        {
            m.LerpVelocity(m.runMaxSpeed * m.keyDownDirection.normalized, m.runVelocityChangeSpeed,
                m.runVelocityChangeSpeed);
        }

        public void OnExit()
        {
        }
    }


    public class PlayerDash : IState
    {
        private PlayerMovementFSM m;
        private float elapse;

        public PlayerDash(PlayerMovementFSM m) => this.m = m;

        public void OnEnter()
        {
            m.dashBufferExpireTime = -1;
            m.rigidbody.velocity = m.facingDirection.normalized * m.dashSpeed;
            elapse = 0;
        }

        public void OnUpdate()
        {
            elapse += Time.deltaTime;
            if (elapse <= m.dashDuration)
                return;

            m.TransitionState(m.runTrigger ? PlayerMovementStateType.Run : PlayerMovementStateType.Idle);
        }

        public void OnFixedUpdate()
        {
        }

        public void OnExit()
        {
            m.dashCoolDownExpireTime = Time.time + m.dashCoolDown;
            m.rigidbody.velocity = Vector2.zero;
        }
    }
}