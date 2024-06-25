using System.Collections.Generic;
using UnityEngine;

namespace Lucky.States
{
    public class FSM<T> : MonoBehaviour
    {
        [Header("Debug")] public bool isDebug;
        protected Dictionary<T, IState> states = new();
        protected IState curState;
        [SerializeField] private T curStateType; // 方便debug
        private T preStateType;

        protected virtual void Update()
        {
            UpdateDebug();
            if (curState == null)
            {
                Debug.LogError("The start state hasn't been set up!!!");
                return;
            }

            curState.OnUpdate();
        }


        protected virtual void FixedUpdate()
        {
            if (curState == null)
            {
                Debug.LogError("The start state hasn't been set up!!!");
                return;
            }

            curState.OnFixedUpdate();
        }


        public void TransitionState(T stateType)
        {
            if (curState != null && stateType.Equals(curStateType))
            {
                // 与上一个状态相等，一般是代码写错了
                Debug.LogError("State can't transition to itself!!!");
                return;
            }

            curState?.OnExit();
            preStateType = curStateType;
            curStateType = stateType;
            curState = states[stateType];
            curState.OnEnter();
        }

        protected virtual void UpdateDebug()
        {
        }
    }
}