using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
    public enum DamageType
    {
        Bullet,
        Slash,
        Fall,
        Burn,
        Blast
    }

    /// <summary>
    /// Static bullet class for shooting bullets etc; Applying it to objects sounds useless.
    /// </summary>
    public class Bullet
    {
        /// <summary>
        /// Dealing damage to a specific raycast and applying force.
        /// </summary>
        /// <param name="a_force">Force applied to the rigidbody on hit.</param>
        /// <param name="a_hit">The raycast of the hit.</param>
        /// <param name="a_damage">How much damage will we do to the damageable class if applicable.</param>
        public static void ApplyDamage(float a_force, RaycastHit a_hit, float a_damage)
        {
            Rigidbody rb = a_hit.rigidbody;

            if (rb)
                rb.velocity = a_hit.normal * a_force;

            Enemy enemy;
            if (a_hit.transform.TryGetComponent(out enemy))
            {
                enemy.TakeDamage(a_damage);
            }
        }

        /// <summary>
        /// Initiating a bullet and shooting it with respected effects.
        /// </summary>
        /// <param name="a_shootPos">Where does the bullet start from?</param>
        /// <param name="a_shootDir">In which direction will the bullet shoot from the shoot pos?</param>
        /// <param name="a_spread">The spread which will be randomly calculated between the negative to the positive of this value.</param>
        /// <param name="a_force">How much force will the bullet apply to the hit object?</param>
        /// <param name="a_damage">How much damage will it deal if the object is damageable?</param>
        /// <param name="a_penetration">How many objects will the bullet pass through before destroying itself?</param>
        /// <param name="a_tracer">What tracer will it use?</param>
        /// <param name="a_filter">What layers will the bullet ignore?</param>
        /// <returns>Returns the bullet hit as its shot using raycast.</returns>
        public static RaycastHit Initiate(Vector3 a_shootPos, Vector3 a_shootDir, Vector3 a_spread, float a_force, float a_damage, float a_penetration = 0, TracerFX a_tracer = null, LayerMask a_filter = default)
        {
            a_shootDir += new Vector3(Random.Range(-a_spread.x, a_spread.x), Random.Range(-a_spread.y, a_spread.y), Random.Range(-a_spread.z, a_spread.z));

            RaycastHit hit;
            if (Physics.Raycast(a_shootPos, a_shootDir, out hit, 1000, a_filter))
            {
                Bullet.ApplyDamage(a_force, hit, a_damage);
            }
            else
            {
                hit.point = a_shootPos + a_shootDir * 100;
            }

            Tracer tracer = a_tracer.CreateTracer(a_shootPos, new Quaternion(), a_shootPos, hit.point);
            tracer.hit = hit;

            return hit;
        }

        /// <summary>
        /// Initiating a bullet and shooting it with respected effects.
        /// </summary>
        /// <param name="a_shootPos">Where does the bullet start from?</param>
        /// <param name="a_shootDir">In which direction will the bullet shoot from the shoot pos?</param>
        /// <param name="a_bulletNum">How many bullets do we shoot?</param>
        /// <param name="a_spread">The spread which will be randomly calculated between the negative to the positive of this value.</param>
        /// <param name="a_force">How much force will the bullet apply to the hit object?</param>
        /// <param name="a_damage">How much damage will it deal if the object is damageable?</param>
        /// <param name="a_penetration">How many objects will the bullet pass through before destroying itself?</param>
        /// <param name="a_tracer">What tracer will it use?</param>
        /// <param name="a_filter">What layers will the bullet ignore?</param>
        public static List<RaycastHit> ShootBullets(Vector3 a_shootPos, Vector3 a_shootDir, int a_bulletNum, Vector3 a_spread, float a_force, float a_damage, float a_penetration = 0, TracerFX a_tracer = null, LayerMask a_filter = default)
        {
            List<RaycastHit> bulletHits = new List<RaycastHit>();
            for (int i = 0; i < a_bulletNum; i++)
            {
                bulletHits.Add(Bullet.Initiate(a_shootPos, a_shootDir, a_spread, a_force, a_damage, a_penetration, a_tracer, a_filter));
            }

            return bulletHits;
        }
    }

}