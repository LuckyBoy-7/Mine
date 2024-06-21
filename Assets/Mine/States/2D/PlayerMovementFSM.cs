using System.Collections.Generic;
using UnityEngine;
using Mine.Extensions;
using static UnityEngine.Mathf;

namespace Mine.States._2D
{
    public enum PlayerMovementStateType
    {
        Idle,
        Run,
        Jump,
        Fall,
        Dash,
        WallSlide, // 由于wallJump只能由WallSlide转移过来，所以没必要为WallJump定义一个类型
        WallJump,
        DoubleJump,
    }

    public enum PlayerMovementAbility
    {
        DoubleJump,
        Dash,
        WallSlide
    }

    /// <summary>
    /// Player的基本运动
    /// </summary>
    public class PlayerMovementFSM : PhysicsFSM<PlayerMovementStateType>
    {
        public static PlayerMovementFSM instance;

        protected virtual void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }
        [Header("Debug")] public bool isAllAbilityDebug;

        [Header("Abilities")] public Dictionary<PlayerMovementAbility, bool> hasAbilityDic = new();
        public bool hasDoubleJumpAbility;
        public bool hasDashAbility;
        public bool hasWallSlideAbility;

        [Header("Input")] public Vector2Int keyDownDirection;


        [Header("Run")] public float runMaxSpeed = 10;
        public float runVelocityChangeSpeed = 100;
        [Header("Idle")] public float idleVelocityChangeSpeed = 60;
        [Header("Air")] public float runInAirMaxSpeed = 11; // 有时可以让空中的速度略大于地面上的，不仅有一种轻盈的感觉，还能鼓励玩家跳跃
        public float runInAirVelocityChangeSpeed = 90;
        [Header("Jump")] public float jumpForce = 16;
        public float jumpAfloatMaxTime = 0.2f; // jump至多会提供这么多的上升速度，继续按跳也还是下降
        public float jumpAfloatMinTime = 0.08f; // jump至少会提供这么多的上升速度
        public float jumpBufferTime = 0.1f;
        [HideInInspector] public float jumpBufferExpireTime = -1;
        public float wolfJumpBufferTime = 0.1f;
        [HideInInspector] public float wolfJumpBufferExpireTime = -1;
        [Header("DoubleJump")] public bool canDoubleJump = true;
        public float doubleJumpForce = 15;
        public float doubleJumpAfloatMaxTime = 0.2f; // 一般来说double jump就是再一次普通跳，但是有的游戏需求可能不同
        public float doubleJumpAfloatMinTime = 0.08f;
        public float doubleJumpBufferTime = 0.1f;
        [HideInInspector] public float doubleJumpBufferExpireTime = -1; // double jump不需要狼跳帧

        [Header("WallSlide")] public float wallSlideMaxSpeed = 5;
        public float wallSlideVelocityChangeSpeed = 15;
        [Header("WallJump")] public Vector2 wallJumpForce = new(18, 18);
        public float wallJumpVelocityChangeSpeed = 90;
        public float wallJumpAfloatMaxTime = 0.2f; // 一般来说wall jump就是再一次普通跳(水平方向速度有差异)，但是有的游戏需求可能不同
        public float wallJumpAfloatMinTime = 0.08f;
        public float wallWolfJumpBufferTime = 0.1f;
        [HideInInspector] public float wallWolfJumpBufferExpireTime = -1;

        [Header("Fall")] public float maxFallingSpeed = 20; // 搭配重力为8或4
        [Header("Dash")] public bool canDash;
        [HideInInspector] public float gravityScaleBackup;
        public float dashSpeed = 35;
        public float dashDuration = 0.15f;
        public float dashBufferTime = 0.1f;
        [HideInInspector] public float dashBufferExpireTime = -1;
        public float dashCoolDown = 0.2f;
        [HideInInspector] public float dashCoolDownExpireTime = -1;


        [Header("Key")] public KeyCode leftKey = KeyCode.A;
        public KeyCode rightKey = KeyCode.D;
        public KeyCode upKey = KeyCode.W;
        public KeyCode downKey = KeyCode.S;
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode dashKey = KeyCode.J;


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
            gravityScaleBackup = rigidbody.gravityScale;
            dashBufferExpireTime = -1;
            jumpBufferExpireTime = -1;
            doubleJumpBufferExpireTime = -1;
            dashCoolDownExpireTime = -1;
            wolfJumpBufferExpireTime = -1;
            wallWolfJumpBufferExpireTime = -1;

            states[PlayerMovementStateType.Idle] = new PlayerIdle(this);
            states[PlayerMovementStateType.Run] = new PlayerRun(this);
            states[PlayerMovementStateType.Jump] = new PlayerJump(this);
            states[PlayerMovementStateType.WallJump] = new PlayerWallJump(this);
            states[PlayerMovementStateType.WallSlide] = new PlayerWallSlide(this);
            states[PlayerMovementStateType.Fall] = new PlayerFall(this);
            states[PlayerMovementStateType.Dash] = new PlayerDash(this);
            states[PlayerMovementStateType.DoubleJump] = new PlayerDoubleJump(this);
            TransitionState(PlayerMovementStateType.Idle);
            
            UnlockAbility(PlayerMovementAbility.Dash);
            UnlockAbility(PlayerMovementAbility.DoubleJump);
            UnlockAbility(PlayerMovementAbility.WallSlide);
        }

        #region Update

        protected override void Update()
        {
            // todo: 根据游戏状态决定要不要更新

            UpdateKeyDownDirection();
            UpdateInputBuffer();
            UpdateStates();
            TryUpdateDebug();

            base.Update();
        }

        private void TryUpdateDebug()
        {
            if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.D))
                isDebug = true;

            if (!isDebug)
                return;


            canDoubleJump = true;
            canDash = true;
            if (isAllAbilityDebug)
            {
                hasAbilityDic[PlayerMovementAbility.Dash] = hasDashAbility;
                hasAbilityDic[PlayerMovementAbility.DoubleJump] = hasDoubleJumpAbility;
                hasAbilityDic[PlayerMovementAbility.WallSlide] = hasWallSlideAbility;
            }
        }

        private void UpdateStates()
        {
            if (isTouchingGround)
            {
                canDoubleJump = true;
                canDash = true; // 虽然你冷却好了，预输入也有，但是不能冲就是不能冲
            }
        }

        private void UpdateInputBuffer()
        {
            if (Input.GetKeyDown(jumpKey))
            {
                jumpBufferExpireTime = Time.time + jumpBufferTime;
                doubleJumpBufferExpireTime = Time.time + doubleJumpBufferTime;
            }

            if (Input.GetKeyDown(dashKey))
                dashBufferExpireTime = Time.time + dashBufferTime;
            if (isTouchingGround)
            {
                wolfJumpBufferExpireTime = Time.time + wolfJumpBufferTime;
                wallWolfJumpBufferExpireTime = -1; // 如果在墙上滑倒底但是不跳。其实应该可以不加，因为毕竟0.1s很短
            }

            if (curState == states[PlayerMovementStateType.WallSlide])
                wallWolfJumpBufferExpireTime = Time.time + wallWolfJumpBufferTime;
        }

        #endregion

        private void UpdateKeyDownDirection()
        {
            var x = Input.GetKey(leftKey) ? -1 : Input.GetKey(rightKey) ? 1 : 0;
            var y = Input.GetKey(downKey) ? -1 : Input.GetKey(upKey) ? 1 : 0;
            keyDownDirection = new Vector2Int(x, y);
        }

        public void TryUpdateFacingDirection() // 看起来像是没什么用，但是如果我放黑波的时候改变了方向，下次就算没动键盘放出来的波会向相反方向移动
        {
            var (x, y) = keyDownDirection;
            if (x != 0)
                facingDirection.x = x;
            if (y != 0)
                facingDirection.y = y;
            transform.localScale = transform.localScale.WithX(Abs(transform.localScale.x) * facingDirection.x);
        }

        public void ReverseFacingDirection()
        {
            facingDirection.x *= -1;
            transform.localScale = transform.localScale.WithX(Abs(transform.localScale.x) * facingDirection.x);
        }


        public void LoseGravity() => rigidbody.gravityScale = 0;
        public void ResetGravity() => rigidbody.gravityScale = gravityScaleBackup;
        public bool isMovingUp => rigidbody.velocity.y > 1e-3;
        public bool isMovingDown => rigidbody.velocity.y < -1e-3;


        public void LerpVelocityX(float to, float k)
        {
            var newX = MoveTowards(rigidbody.velocity.x, to, k * Time.fixedDeltaTime);
            rigidbody.velocity = new Vector2(newX, rigidbody.velocity.y);
        }

        public void LerpVelocityY(float to, float k)
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

        public bool runTrigger => Input.GetKey(leftKey) || Input.GetKey(rightKey);

        public bool jumpTrigger => // 就是尝试跳跃后，如果在地上或不在地上但是狼跳还在
            Time.time <= jumpBufferExpireTime && (isTouchingGround || Time.time <= wolfJumpBufferExpireTime);

        public bool wallJumpTrigger => // 就是尝试跳跃后，如果在墙上或不在墙上但是狼跳还在
            Time.time <= jumpBufferExpireTime && Time.time <= wallWolfJumpBufferExpireTime;

        public bool doubleJumpTrigger =>
            HasAbility(PlayerMovementAbility.DoubleJump) && Time.time <= doubleJumpBufferExpireTime && canDoubleJump;

        // 还要限定速度，不然朝同侧墙蹬墙跳一出去就就又变成wallSlide状态了，且下落状态才能trigger，由于代码限定了只有从fall才能到wallSlide，所以不用写下落条件
        public bool wallSlideTrigger => HasAbility(PlayerMovementAbility.WallSlide)
                                        && (isTouchingLeftWall && Input.GetKey(leftKey) && rigidbody.velocity.x <= 1e-3
                                            || isTouchingRightWall && Input.GetKey(rightKey) &&
                                            rigidbody.velocity.x >= -1e-3);

        public bool dashTrigger => HasAbility(PlayerMovementAbility.Dash) &&
                                   Time.time <= dashBufferExpireTime && Time.time >= dashCoolDownExpireTime && canDash;

        public bool fallTrigger => isMovingDown;

        public bool offWallTrigger =>
            isTouchingLeftWall && Input.GetKey(rightKey) || isTouchingRightWall && Input.GetKey(leftKey);
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
            else if (m.jumpTrigger)
                m.TransitionState(PlayerMovementStateType.Jump);
            else if (m.dashTrigger)
                m.TransitionState(PlayerMovementStateType.Dash);
            else if (m.fallTrigger)
                m.TransitionState(PlayerMovementStateType.Fall);
        }

        public void OnFixedUpdate()
        {
            m.LerpVelocityX(0, m.idleVelocityChangeSpeed);
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
            else if (m.jumpTrigger)
                m.TransitionState(PlayerMovementStateType.Jump);
            else if (m.fallTrigger)
                m.TransitionState(PlayerMovementStateType.Fall);
            else if (m.dashTrigger)
                m.TransitionState(PlayerMovementStateType.Dash);
        }

        public void OnFixedUpdate()
        {
            m.LerpVelocityX(m.runMaxSpeed * m.keyDownDirection.x, m.runVelocityChangeSpeed);
        }

        public void OnExit()
        {
        }
    }

    public class PlayerJump : IState
    {
        private PlayerMovementFSM m;
        private bool isKeyReleased;
        private float jumpAFloatElapse;

        public PlayerJump(PlayerMovementFSM m) => this.m = m;

        public void OnEnter()
        {
            m.LoseGravity();
            m.rigidbody.velocity = new Vector2(m.rigidbody.velocity.x, m.jumpForce);
            m.jumpBufferExpireTime = -1; // 重置，否则如果从跳跃到落地的时间<bufferTime，则跳跃会被触发两次 
            m.wolfJumpBufferExpireTime = -1;
            m.doubleJumpBufferExpireTime = -1; // 清一下doubleJump的buffer，不然刚进来又出去了
            jumpAFloatElapse = 0;
            isKeyReleased = false;
        }

        public void OnUpdate()
        {
            m.TryUpdateFacingDirection();
            isKeyReleased = isKeyReleased || Input.GetKeyUp(m.jumpKey);
            jumpAFloatElapse += Time.deltaTime;
            if (jumpAFloatElapse > m.jumpAfloatMaxTime)
                m.ResetGravity();
            // 跳跃至少持续0.08s，但撞到头例外（但作图基本不会让这发生）
            if ((jumpAFloatElapse > m.jumpAfloatMinTime && isKeyReleased) || m.isHittingCeiling)
            {
                m.rigidbody.velocity = m.rigidbody.velocity.WithY(0);
                m.ResetGravity();
                m.TransitionState(PlayerMovementStateType.Fall);
                return;
            }


            if (m.fallTrigger)
                m.TransitionState(PlayerMovementStateType.Fall);
            else if (m.doubleJumpTrigger)
                m.TransitionState(PlayerMovementStateType.DoubleJump);
            else if (m.dashTrigger)
                m.TransitionState(PlayerMovementStateType.Dash);
        }

        public void OnFixedUpdate()
        {
            m.LerpVelocityX(m.runInAirMaxSpeed * m.keyDownDirection.x, m.runInAirVelocityChangeSpeed);
        }

        public void OnExit()
        {
            m.ResetGravity(); // 有时可能不能及时reset（比如受伤后update直接就不执行了），所以exit的时候要reset
        }
    }

    public class PlayerWallJump : IState
    {
        private PlayerMovementFSM m;
        private bool isKeyReleased;
        private float jumpAFloatElapse;

        public PlayerWallJump(PlayerMovementFSM m) => this.m = m;

        public void OnEnter()
        {
            m.LoseGravity();
            m.rigidbody.velocity = new Vector2(m.facingDirection.x * m.wallJumpForce.x, m.wallJumpForce.y);
            m.jumpBufferExpireTime = -1; // 重置，否则如果从跳跃到落地的时间<bufferTime，则跳跃会被触发两次 
            m.wallWolfJumpBufferExpireTime = -1;
            isKeyReleased = false;
            jumpAFloatElapse = 0;
            m.doubleJumpBufferExpireTime = -1; // 把doubleJump的buffer清一下，不然刚进来又进到doubleJump了
        }

        public void OnUpdate()
        {
            m.TryUpdateFacingDirection();
            isKeyReleased = isKeyReleased || Input.GetKeyUp(m.jumpKey);
            jumpAFloatElapse += Time.deltaTime;
            // 跳跃至少持续0.08s
            if (jumpAFloatElapse > m.wallJumpAfloatMaxTime)
                m.ResetGravity();
            // 跳跃至少持续0.08s，但撞到头例外（但作图基本不会让这发生）
            if ((jumpAFloatElapse > m.wallJumpAfloatMinTime && isKeyReleased) || m.isHittingCeiling)
            {
                m.rigidbody.velocity = new Vector2(m.rigidbody.velocity.x, 0);
                m.ResetGravity();
                m.TransitionState(PlayerMovementStateType.Fall);
                return;
            }

            if (m.fallTrigger)
                m.TransitionState(PlayerMovementStateType.Fall);
            else if (m.dashTrigger)
                m.TransitionState(PlayerMovementStateType.Dash);
            else if (m.doubleJumpTrigger)
                m.TransitionState(PlayerMovementStateType.DoubleJump);
        }

        public void OnFixedUpdate()
        {
            m.LerpVelocityX(m.runInAirMaxSpeed * m.keyDownDirection.x, m.wallJumpVelocityChangeSpeed);
        }

        public void OnExit()
        {
            m.ResetGravity();
        }
    }

    public class PlayerWallSlide : IState
    {
        private PlayerMovementFSM m;

        public PlayerWallSlide(PlayerMovementFSM m) => this.m = m;

        public void OnEnter()
        {
            m.rigidbody.velocity = Vector2.zero;  // 看需求
            m.LoseGravity();
            m.canDash = true;
            m.canDoubleJump = true;
            m.ReverseFacingDirection();
        }

        public void OnUpdate()
        {
            if (m.isTouchingGround)
                m.TransitionState(m.runTrigger ? PlayerMovementStateType.Run : PlayerMovementStateType.Idle);
            else if (m.wallJumpTrigger)
                m.TransitionState(PlayerMovementStateType.WallJump);
            else if (m.offWallTrigger || !m.isTouchingWall) // 手动出去或者被动出去
                m.TransitionState(PlayerMovementStateType.Fall);
            else if (m.dashTrigger)
                m.TransitionState(PlayerMovementStateType.Dash);
        }

        public void OnFixedUpdate()
        {
            m.LerpVelocityY(-m.wallSlideMaxSpeed, m.wallSlideVelocityChangeSpeed);
        }

        public void OnExit()
        {
            m.ResetGravity();
        }
    }

    public class PlayerFall : IState
    {
        private PlayerMovementFSM m;

        public PlayerFall(PlayerMovementFSM m)
            => this.m = m;

        public void OnEnter()
        {
        }

        public void OnUpdate()
        {
            m.TryUpdateFacingDirection();

            if (m.isTouchingGround)
                m.TransitionState(m.runTrigger ? PlayerMovementStateType.Run : PlayerMovementStateType.Idle);
            else if (m.wallJumpTrigger) // 墙上的狼跳
                m.TransitionState(PlayerMovementStateType.WallJump);
            else if (m.jumpTrigger) // 狼跳
                m.TransitionState(PlayerMovementStateType.Jump);
            else if (m.doubleJumpTrigger)
                m.TransitionState(PlayerMovementStateType.DoubleJump);
            else if (m.dashTrigger)
                m.TransitionState(PlayerMovementStateType.Dash);
            else if (m.wallSlideTrigger)
                m.TransitionState(PlayerMovementStateType.WallSlide);
        }

        public void OnFixedUpdate()
        {
            // 这里没办法，除非重力也由代码控制
            var newY = Max(m.rigidbody.velocity.y, -m.maxFallingSpeed);
            m.rigidbody.velocity = m.rigidbody.velocity.WithY(newY);
            m.LerpVelocityX(m.runInAirMaxSpeed * m.keyDownDirection.x, m.runInAirVelocityChangeSpeed);
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
            m.canDash = false;
            m.rigidbody.velocity = new Vector2(m.facingDirection.x * m.dashSpeed, 0);
            m.LoseGravity();
            elapse = 0;
        }

        public void OnUpdate()
        {
            elapse += Time.deltaTime;
            if (elapse <= m.dashDuration)
                return;

            if (m.jumpTrigger)
                m.TransitionState(
                    m.isTouchingGround ? PlayerMovementStateType.Jump : PlayerMovementStateType.DoubleJump);
            else if (m.isTouchingGround)
                m.TransitionState(m.runTrigger ? PlayerMovementStateType.Run : PlayerMovementStateType.Idle);
            else if (!m.isTouchingGround) // 这里的处理要特殊一点，因为player冲刺状态y轴速度恒为0，不这么判断状态就出不去了
                m.TransitionState(PlayerMovementStateType.Fall);
        }

        public void OnFixedUpdate()
        {
        }

        public void OnExit()
        {
            m.ResetGravity();
            m.dashCoolDownExpireTime = Time.time + m.dashCoolDown;
            m.rigidbody.velocity = Vector2.zero;
        }
    }

    public class PlayerDoubleJump : IState
    {
        private PlayerMovementFSM m;
        private bool isKeyReleased;
        private float jumpAFloatElapse;
        private bool hasSetSpeedYZero;

        public PlayerDoubleJump(PlayerMovementFSM m)
            => this.m = m;

        public void OnEnter()
        {
            m.LoseGravity();
            m.rigidbody.velocity = new Vector2(m.rigidbody.velocity.x, m.doubleJumpForce);
            // m.rigidbody.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
            m.doubleJumpBufferExpireTime = -1; // 重置，否则如果从跳跃到落地的时间<bufferTime，则跳跃会被触发两次 
            m.canDoubleJump = false;
            isKeyReleased = false;
            jumpAFloatElapse = 0;
        }

        public void OnUpdate()
        {
            m.TryUpdateFacingDirection();
            isKeyReleased = isKeyReleased || Input.GetKeyUp(m.jumpKey);
            jumpAFloatElapse += Time.deltaTime;
            // 跳跃至少持续0.08s
            if (jumpAFloatElapse > m.doubleJumpAfloatMaxTime)
                m.ResetGravity();
            // 跳跃至少持续0.08s，但撞到头例外（但作图基本不会让这发生）
            if ((jumpAFloatElapse > m.doubleJumpAfloatMinTime && isKeyReleased) || m.isHittingCeiling)
            {
                m.rigidbody.velocity = new Vector2(m.rigidbody.velocity.x, 0);
                m.ResetGravity();
                m.TransitionState(PlayerMovementStateType.Fall);
                return;
            }

            if (m.fallTrigger)
                m.TransitionState(PlayerMovementStateType.Fall);
            else if (m.dashTrigger)
                m.TransitionState(PlayerMovementStateType.Dash);
        }

        public void OnFixedUpdate()
        {
            m.LerpVelocityX(m.runInAirMaxSpeed * m.keyDownDirection.x, m.runInAirVelocityChangeSpeed);
        }

        public void OnExit()
        {
            m.ResetGravity();
        }
    }
}