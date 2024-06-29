using System;
using UnityEngine;

namespace Lucky.Test
{
    public class SimpleMove : MonoBehaviour
    {
        public float speed = 1;

        private void Update()
        {
            float dirX = Input.GetAxisRaw("Horizontal");
            float dirY = Input.GetAxisRaw("Vertical");
            transform.position += new Vector3(dirX, dirY).normalized * (speed * Time.deltaTime);
        }
    }
}