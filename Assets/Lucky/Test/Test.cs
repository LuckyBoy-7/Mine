using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using Lucky.Interactive;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{
    private void Update()
    {
        transform.position = GameCursor.MouseWorldPos;
        // EventSystem.current.IsPointerOverGameObject()
    }
}