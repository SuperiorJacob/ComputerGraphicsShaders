using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
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

        private bool isGrounded = false;
        private bool isDead = false;

        private Vector3 enemyVelocity = Vector3.zero;

        private CharacterController characterController = null;
        private Animator characterAnimator = null;

        private float health;
        private float maxHealth;
        #endregion

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            characterAnimator = GetComponent<Animator>();

            health = _baseHealth;
            maxHealth = health;
        }

        public void TakeDamage(float a_amount, DamageType a_dmgType = DamageType.Blast)
        {
            if (isDead) return;

            health -= a_amount;

            if (health < 0)
            {
                isDead = true;
                _armRig.weight = 0;

                characterAnimator.SetBool("isDead", true);

                Destroy(gameObject, 5);
            }
        }

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
            if (isDead) return;

            Move();
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // Pushing
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
                return;

            if (hit.moveDirection.y < -0.3f)
                return;

            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDirection * _pushPower;
        }
    }
}