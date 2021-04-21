using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
    /// <summary>
    /// Scriptable object used for creating Tracers.
    /// </summary>
    [CreateAssetMenu(fileName = "Bullet Tracer", menuName = "Bullets/Bullet Tracer", order = 1)]
    public class TracerFX : ScriptableObject
    {
        #region Fields
        public GameObject _particleTracer;
        public GameObject _impactFX;
        public float _impactDestroyTimer = 1;
        public float _tracerSpeed;
        #endregion

        /// <summary>
        /// Creating a particle tracer which also reflects damage depending on how its setup.
        /// </summary>
        /// <param name="a_position">Origin of the tracer</param>
        /// <param name="a_rotation">Rotational origin of the tracer</param>
        /// <param name="a_start">Where did it start? (usually the origin)</param>
        /// <param name="a_end">Where will it end?</param>
        /// <param name="a_data">Bullet data to reflect tracer</param>
        /// <returns></returns>
        public Tracer CreateTracer(Vector3 a_position, Quaternion a_rotation, Vector3 a_start, Vector3 a_end, BulletData a_data)
        {
            GameObject obj = Instantiate(_particleTracer, a_position, a_rotation, null);

            Tracer instance = obj.GetComponent<Tracer>();
            instance.Init(this, a_start, a_end, a_data);

            return instance;
        }
    }

}
