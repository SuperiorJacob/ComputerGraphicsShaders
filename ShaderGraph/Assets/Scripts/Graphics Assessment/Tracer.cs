using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
    public class Tracer : MonoBehaviour
    {
        public Vector3 start;
        public Vector3 end;
        public float speed;

        public void DrawTracer()
        {
            transform.position = Vector3.Lerp(transform.position, end, speed * Time.deltaTime);
            transform.LookAt(start, end);
        }

        public void Init(TracerFX a_tracer, Vector3 a_start, Vector3 a_end)
        {
            speed = a_tracer.speed;
            start = a_start;
            end = a_end;
        }

        public void Update()
        {
            DrawTracer();

            if (Vector3.Distance(transform.position, end) <= 1)
            {
                Destroy(gameObject);
            }
        }
    }
}