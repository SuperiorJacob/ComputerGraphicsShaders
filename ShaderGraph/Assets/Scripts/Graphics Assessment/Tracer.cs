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

        private ParticleSystem particleSys;

        public void DrawTracer()
        {
            //transform.position = Vector3.Lerp(transform.position, end, speed * Time.deltaTime);
            transform.LookAt(end);
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        public void Init(TracerFX a_tracer, Vector3 a_start, Vector3 a_end)
        {
            speed = a_tracer.speed * a_tracer.speed;
            start = a_start;
            end = a_end;

            particleSys = GetComponent<ParticleSystem>();
        }

        public void Update()
        {
            DrawTracer();

            if (!particleSys.isStopped && Vector3.Distance(transform.position, end) <= 0.1f)
            {
                particleSys.Stop();
                Destroy(gameObject, particleSys.main.startLifetimeMultiplier);
            }
        }
    }
}