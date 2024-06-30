using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Utilities;
using UnityEngine;

public class BetweenIntervalTest : MonoBehaviour
{
    public SpriteRenderer sr;
    public float timer = 5f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        Color color = Color.white;
        if (timer > 0 && Timer.BetweenInterval(0.25f))
        {
            color = Color.black;
        }

        sr.color = color;
    }
}