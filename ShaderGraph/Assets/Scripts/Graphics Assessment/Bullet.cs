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
        public Vector3 shootPos;
        public Vector3 shootDir;
        public Vector3 spread;
        public float force;
        public float damage;
        public float penetration;
        public TracerFX tracerFX;
        public LayerMask filter;

        public RaycastHit hit;

        private Tracer tracer;

        public void ApplyDamage()
        {
            Rigidbody rb = hit.rigidbody;

            if (rb)
                rb.velocity = shootDir * force;
        }

        public void Initiate(Vector3 a_shootPos, Vector3 a_shootDir, int a_bulletNum, Vector3 a_spread, float a_force, float a_damage, float a_penetration, TracerFX a_tracer, LayerMask a_filter)
        {
            shootPos = a_shootPos; shootDir = a_shootDir; spread = a_spread; force = a_force; damage = a_damage; penetration = a_penetration; tracerFX = a_tracer; filter = a_filter;

            shootDir = shootDir + new Vector3(Random.Range(-spread.x, spread.x), Random.Range(-spread.y, spread.y), Random.Range(-spread.z, spread.z));

            if (Physics.Raycast(shootPos, shootDir, out hit, Mathf.Infinity, filter))
            {
                tracer = tracerFX.CreateTracer(shootPos, new Quaternion(), shootPos, hit.point);

                ApplyDamage();
            }

            Destroy(gameObject);
        }

        public static void ShootBullets(GameObject bulletObject, Vector3 a_shootPos, Vector3 a_shootDir, int a_bulletNum, Vector3 a_spread, float a_force, float a_damage, float a_penetration, TracerFX a_tracer, LayerMask a_filter)
        {
            for (int i = 0; i < a_bulletNum; i++)
            {
                GameObject inst = Instantiate(bulletObject);

                Bullet bullet = inst.GetComponent<Bullet>();
                bullet.Initiate(a_shootPos, a_shootDir, a_bulletNum, a_spread, a_force, a_damage, a_penetration, a_tracer, a_filter);
            }
        }
    }

}