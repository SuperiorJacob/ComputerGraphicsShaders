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

    public class Bullet : MonoBehaviour
    {
        public static void ApplyDamage(float a_force, RaycastHit a_hit, float a_damage)
        {
            Rigidbody rb = a_hit.rigidbody;

            if (rb)
                rb.velocity = a_hit.normal * a_force;
        }

        public static void Initiate(Vector3 a_shootPos, Vector3 a_shootDir, Vector3 a_spread, float a_force, float a_damage, float a_penetration, TracerFX a_tracer, LayerMask a_filter)
        {
            a_shootDir = a_shootDir + new Vector3(Random.Range(-a_spread.x, a_spread.x), Random.Range(-a_spread.y, a_spread.y), Random.Range(-a_spread.z, a_spread.z));

            RaycastHit hit;
            if (Physics.Raycast(a_shootPos, a_shootDir, out hit, 1000, a_filter))
            {
                a_tracer.CreateTracer(a_shootPos, new Quaternion(), a_shootPos, hit.point);

                Bullet.ApplyDamage(a_force, hit, a_damage);
            }
            else
            {
                a_tracer.CreateTracer(a_shootPos, new Quaternion(), a_shootPos, a_shootPos + a_shootDir * 100);
            }
        }

        public static void ShootBullets(Vector3 a_shootPos, Vector3 a_shootDir, int a_bulletNum, Vector3 a_spread, float a_force, float a_damage, float a_penetration, TracerFX a_tracer, LayerMask a_filter)
        {
            for (int i = 0; i < a_bulletNum; i++)
            {
                Bullet.Initiate(a_shootPos, a_shootDir, a_spread, a_force, a_damage, a_penetration, a_tracer, a_filter);
            }
        }
    }

}