using System;
using System.Collections;
using UnityEngine;

namespace Lucky.Extensions
{
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// 动态的计时，并且会随着gameObject关闭或是StopAllCoroutines而停止，而invokeRepeating好像会一直执行，且不动态
        /// </summary>
        /// <param name="orig">调用对象</param>
        /// <param name="callback">回调函数</param>
        /// <param name="interval">调用时间间隔</param>
        /// <param name="isScaledTime">是否受时间缩放影响</param>
        /// <param name="isOneShot">是否就执行一次</param>
        public static void CreateFuncTimer(
            this MonoBehaviour orig,
            Action callback,
            Func<float> interval,
            bool isScaledTime = true,
            bool isOneShot = false,
            bool isStartImmediate = false
        )
        {
            float elapse = isStartImmediate ? interval() : 0;
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                while (true)
                {
                    elapse += isScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
                    if (elapse >= interval())
                    {
                        elapse = 0;
                        callback();
                        if (isOneShot)
                            yield break;
                    }

                    yield return null;
                }
            }
        }

        public static void WaitForOneFrameToExecution(this MonoBehaviour orig, Action callback)
        {
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                yield return null;
                callback?.Invoke();
            }
        }

        public static void DoWaitUntil(this MonoBehaviour orig, Func<bool> conditionCallback, Action callback)
        {
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                yield return new WaitUntil(conditionCallback);
                callback?.Invoke();
            }
        }

        public static void DoWaitUntilEndOfFrame(this MonoBehaviour orig, Action callback)
        {
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                yield return new WaitForEndOfFrame();
                callback?.Invoke();
            }
        }

        public static float Dist(this MonoBehaviour orig, MonoBehaviour other) => (orig.transform.position - other.transform.position).magnitude;
        public static float Dist(this MonoBehaviour orig, Vector3 pos) => (orig.transform.position - pos).magnitude;
        public static Vector3 Dir(this MonoBehaviour orig, MonoBehaviour other) => (other.transform.position - orig.transform.position).normalized;
        public static Vector3 Dir(this MonoBehaviour orig, Vector3 pos) => (pos - orig.transform.position).normalized;
    }
}