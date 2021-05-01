using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
    /// <summary>
    /// Enemy class that attacks the player / player fights back.
    /// </summary>
    [RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
        #region Fields
        public Transform _target;
        public UnityEngine.Animations.Rigging.Rig _armRig;

        public float _baseHealth = 100;
        public float _defaultSpeed = 5.0f;
        public float _turnSpeed = 1.0f;
        public float _pushPower = 2.0f;
        public float _gravity = -9.81f;
        public float _attackTime = 2.0f;
        public float _attackHitTime = 1.0f;

        public Material _dissolveMaterial;

        private bool isGrounded = false;
        private bool isDead = false;

        private Vector3 enemyVelocity = Vector3.zero;

        private CharacterController characterController = null;
        private Animator characterAnimator = null;

        private bool attacking = false;
        private bool attacked = false;

        private float attackTimer = 0;

        private float health;
        private float maxHealth;

        private float deathAmount = 0;
        #endregion

        #region Functions
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            characterAnimator = GetComponent<Animator>();

            deathAmount = 0;

            health = _baseHealth;
            maxHealth = health;

            _target = Player.main._enemyTarget;
        }

        /// <summary>
        /// Deal damage to the enemy.
        /// </summary>
        /// <param name="a_amount">How much damage to deal?</param>
        /// <param name="a_dmgType">Damage type (effects damage style later)</param>
        public void TakeDamage(float a_amount, DamageType a_dmgType = DamageType.Blast)
        {
            if (isDead) return;

            health -= a_amount;

            if (health < 0)
            {
                isDead = true;
                _armRig.weight = 0;

                Player.enemyKills++;

                characterAnimator.SetBool("isDead", true);

                foreach (SkinnedMeshRenderer a_smr in GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    a_smr.material = _dissolveMaterial;
                }

                characterController.enabled = false;

                Spawner.spawnedEnemies--;

                Destroy(gameObject, 5);
            }
        }

        /// <summary>
        /// Moving the enemy towards the target.
        /// </summary>
        private void Move()
        {
            Vector3 go = _target.position - transform.position;
            go.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(go), Time.deltaTime * _turnSpeed);
            characterController.Move(transform.forward * _defaultSpeed * Time.deltaTime);

            enemyVelocity.y += _gravity * Time.deltaTime;
            characterController.Move(enemyVelocity * Time.deltaTime);

            isGrounded = characterController.isGrounded;
            if (isGrounded && enemyVelocity.y < 0)
            {
                enemyVelocity.y = 0f;
            }
        }

        private void FixedUpdate()
        {
            if (isDead)
            {
                foreach (SkinnedMeshRenderer a_smr in GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    deathAmount += (0.1f * Time.deltaTime);

                    a_smr.material.SetFloat("D_Amount", deathAmount);
                }

                return;
            }

            if (Cursor.lockState == CursorLockMode.Confined) return;

            if (attacking)
            {
                if (!attacked && (attackTimer - _attackHitTime) < Time.realtimeSinceStartup)
                {
                    attacked = true;

                    if (Vector3.Distance(Player.main.transform.position, transform.position) < 3)
                        Player.main.TakeDamage(10);
                }
                else if (attackTimer < Time.realtimeSinceStartup)
                {
                    attacking = false;
                    attacked = false;
                    attackTimer = 0;

                    characterAnimator.SetBool("attacking", false);
                }
            }
            else Move();
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // Killing
            if (!attacking && hit.gameObject.layer == 9) // We know this is the player.
            {
                attacking = true;
                attackTimer = Time.realtimeSinceStartup + _attackTime;
                characterAnimator.SetBool("attacking", true);
            }

            // Pushing
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
                return;

            if (hit.moveDirection.y < -0.3f)
                return;

            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDirection * _pushPower;
        }
        #endregion
    }
}