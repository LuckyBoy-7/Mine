using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class ScreenWipe : MonoBehaviour
    {
        protected virtual void Awake()
        {
            float cameraHeight = Camera.main.orthographicSize * 2;
            transform.localScale = Vector3.one * cameraHeight / 1080;
        }
    }
}