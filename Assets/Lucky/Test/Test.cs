using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using UnityEngine;

public class Test : MonoBehaviour
{
    private SpriteRenderer sr;
    private SineWave sine;
    private int width;
    private int height;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sine = gameObject.AddComponent<SineWave>();
        sine.OnUpdate += f =>
        {
            width = (int)(160 + 159 * f);
            height = (int)(90 + 89 * f);
        };
    }

    private void OnRenderObject()
    {
        // IEnumerable<Vector3> T()
        // {
        //     for (int i = 0; i < width; i++)
        //     {
        //         for (int j = 0; j < height; j++)
        //         {
        //             yield return new Vector3(i, j);
        //         }
        //     }
        // }

        // this.DrawDots(T(), Color.green);
        this.DrawRect(Vector3.zero, width, height, Color.green);
    }


    private void Update()
    {

    }
    
}