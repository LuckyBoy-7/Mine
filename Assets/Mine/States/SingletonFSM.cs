using System;
using System.Collections.Generic;
using Mine.Managers;
using UnityEngine;

namespace Mine.States
{
    public class SingletonFSM<T> : FSM<T> where T : Enum
    {
        public static SingletonFSM<T> instance;

        protected virtual void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }
    }
}