using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
    /// <summary>
    /// Tracer class to only be used by bullets or TracerFX.
    /// </summary>
    public class Tracer : MonoBehaviour
    {
        #region Fields
        public Vector3 start;
        public Vector3 end;
        public float speed;

        public RaycastHit hit;

        private bool finished = false;
        private GameObject impactFX;
        private float impactTimer;
        private ParticleSystem particleSys;
        private BulletData data;
        #endregion

        #region Functions
        /// <summary>
        /// Drawing or more proffessionally explained, moving the particle.
        /// </summary>
        public void DrawTracer()
        {
            transform.LookAt(end);
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        /// <summary>
        /// Initializing the tracer data.
        /// </summary>
        /// <param name="a_tracer">What tracerFX is it?</param>
        /// <param name="a_start">Tracer origin</param>
        /// <param name="a_end">Tracer finish</param>
        /// <param name="a_data">Data to reflect tracer</param>
        public void Init(TracerFX a_tracer, Vector3 a_start, Vector3 a_end, BulletData a_data)
        {
            speed = a_tracer._tracerSpeed * a_tracer._tracerSpeed;
            start = a_start;
            end = a_end;
            impactFX = a_tracer._impactFX;
            impactTimer = a_tracer._impactDestroyTimer;
            data = a_data;

            particleSys = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// Impact FX (if applicable) creates a particle effect on hit.
        /// </summary>
        public void ImpactFX()
        {
            if (hit.transform == null || impactFX == null) return;

            Destroy(Instantiate(impactFX, hit.point, Quaternion.identity, null), impactTimer);
        }

        public void Update()
        {
            DrawTracer();

            if (!finished && Vector3.Distance(transform.position, end) <= 0.1f)
            {
                finished = true;
                particleSys.Stop();

                if (hit.collider)
                {
                    Bullet.ApplyDamage(data.force, hit, data.damage);

                    if (data.penetration > 0)
                    {
                        data.penetration -= 1;
                        data.origin = hit.point + data.direction * 0.5f;

                        Bullet.Initiate(data);
                    }
                }

                ImpactFX();
                Destroy(gameObject, particleSys.main.startLifetimeMultiplier);
            }
        }
        #endregion
    }
}