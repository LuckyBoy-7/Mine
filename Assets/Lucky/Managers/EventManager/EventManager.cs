using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Managers
{
    public partial class EventManager : Singleton<EventManager>
    {
        public delegate void EventDelegate(params object[] param);

        // 因为有static的属性，意味着用这个做了事件就要小心开启reloadScene模式
        // 但我们可以每次运行重新生成个实例，这样方便调试
        // 注意不能用addComponent，不然就递归了(但我不明白为什么会内存泄漏，因为爆栈？)，反正这个显示在inspector里也没用，new一个就行了
        protected override void Awake()
        {
            base.Awake();
            instance = this;
            Dispatcher<string>.Clear();
        }

        public void Broadcast<T>(T eventName, params object[] param)
        {
            Dispatcher<T>.Dispatch(eventName, param);
        }

        public void Register<T>(T eventName, EventDelegate listener)
        {
            Dispatcher<T>.AddListener(eventName, listener);
        }

        public void Unregister<T>(T eventName, EventDelegate listener)
        {
            Dispatcher<T>.RemoveListener(eventName, listener);
        }

        public void UnregisterAll<T>()
        {
            Dispatcher<T>.RemoveAllListener();
        }

        public void RegisterOneShot<T>(T eventName, EventDelegate listener) // 监听到一次事件就注销
        {
            EventDelegate foo = null;
            foo = param =>
            {
                listener(param);
                Dispatcher<T>.RemoveListener(eventName, foo);
            };
            Dispatcher<T>.AddListener(eventName, foo);
        }


        private static class Dispatcher<T>
        {
            private static Dictionary<T, EventDelegate> _eventDict = new();

            public static void Clear()
            {
                _eventDict.Clear();
            }
            public static void Dispatch(T eventName, params object[] param)
            {
                if (_eventDict.TryGetValue(eventName, out var eventDelegate)) // 找到并调用
                {
                    eventDelegate?.Invoke(param);
                }
            }

            public static void AddListener(T eventName, EventDelegate listener)
            {
                if (_eventDict.TryGetValue(eventName, out var eventDelegate))
                {
                    foreach (Delegate @delegate in eventDelegate.GetInvocationList()) // @符号用来转义保留关键字，使其作为标识符使用
                    {
                        // method对应方法，target对应类的实例
                        if (@delegate.Method == listener.Method && @delegate.Target == listener.Target)
                        {
                            Debug.LogWarning("不允许添加相同的监听者");
                            return;
                        }
                    }

                    _eventDict[eventName] = Delegate.Combine(eventDelegate, listener) as EventDelegate;
                    return;
                }

                _eventDict[eventName] = listener;
            }

            public static void RemoveListener(T eventName, EventDelegate listener)
            {
                if (_eventDict.TryGetValue(eventName, out var eventDelegate))
                {
                    EventDelegate eventDelegate2 = Delegate.Remove(eventDelegate, listener) as EventDelegate;
                    if (eventDelegate2 == null) // 空了，删除字典的key
                    {
                        _eventDict.Remove(eventName);
                        return;
                    }

                    _eventDict[eventName] = eventDelegate2;
                }
            }

            public static void RemoveAllListener()
            {
                _eventDict = new Dictionary<T, EventDelegate>();
            }
        }
    }
}