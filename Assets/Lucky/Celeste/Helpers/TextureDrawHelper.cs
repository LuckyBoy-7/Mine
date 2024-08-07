using System;
using System.Collections.Generic;
using Lucky.Managers;
using UnityEngine;
using UnityEngine.Pool;

namespace Lucky.Celeste.Helpers
{
    public class TextureDrawHelper : MonoBehaviour
    {
        private List<SpriteRenderer> srs = new();
        private int i;

        public void Clear()
        {
            for (int j = 0; j < i; j++)
            {
                srs[j].enabled = false;
            }

            i = 0;
        }

        public void Draw(Sprite sprite, Vector3 position, Color color, Vector2 scale, float rotation = 0, bool useWorldPos = true)
        {
            if (i == srs.Count)
                srs.Add(Instantiate(Resources.Load<SpriteRenderer>("Components/SpriteRenderer"), transform));

            var sr = srs[i++];
            sr.enabled = true;
            sr.sprite = sprite;
            sr.color = color;
            if (useWorldPos)
                sr.transform.position = position;
            else
                sr.transform.localPosition = position;
            sr.transform.localScale = Vector3.one * scale;
            sr.transform.eulerAngles = new Vector3(0, 0, rotation * Mathf.Rad2Deg);
        }
    }
}