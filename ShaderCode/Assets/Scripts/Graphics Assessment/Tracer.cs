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

        public RaycastHit hit;

        private bool finished = false;
        private Texture decal;
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
            decal = a_tracer.decal;

            particleSys = GetComponent<ParticleSystem>();
        }

        public void ImpactFX()
        {
            if (hit.transform == null) return;

            // USING DECAL SHADER
            //MeshRenderer mesh;
            //if (hit.transform.TryGetComponent(out mesh))
            //{
            //    Material mat = mesh.sharedMaterial;

            //    List<Vector4> list = Player.bulletDecals;
            //    int size = list.Count;

            //    if (size == 50)
            //        list.RemoveAt(0);

            //    list.Add(hit.point);
            //    size++;

            //    Vector4[] points = new Vector4[50];

            //    list.CopyTo(points);

            //    mat.SetInt("_PointsSize", size);
            //    mat.SetVectorArray("_Points", points);
            //}
        }

        public void Update()
        {
            DrawTracer();

            if (!finished && Vector3.Distance(transform.position, end) <= 0.1f)
            {
                finished = true;
                particleSys.Stop();
                ImpactFX();
                Destroy(gameObject, particleSys.main.startLifetimeMultiplier);
            }
        }
    }
}