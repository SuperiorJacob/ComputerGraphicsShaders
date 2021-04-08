using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
    [CreateAssetMenu(fileName = "Bullet Tracer", menuName = "Bullets/Bullet Tracer", order = 1)]
    public class TracerFX : ScriptableObject
    {
        public GameObject _particleTracer;
        public GameObject _impactFX;
        public float _impactDestroyTimer = 1;
        public float _tracerSpeed;

        public Tracer CreateTracer(Vector3 a_position, Quaternion a_rotation, Vector3 a_start, Vector3 a_end)
        {
            GameObject obj = Instantiate(_particleTracer, a_position, a_rotation, null);

            Tracer instance = obj.GetComponent<Tracer>();
            instance.Init(this, a_start, a_end);

            return instance;
        }
    }

}
