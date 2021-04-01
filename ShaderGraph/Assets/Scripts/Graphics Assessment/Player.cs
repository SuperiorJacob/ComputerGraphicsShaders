using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
    [RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
    public class Player : MonoBehaviour
    {
        [System.Serializable]
        public struct GunRigging
        {
            public UnityEngine.Animations.Rigging.Rig scopingGunWeight;
            public UnityEngine.Animations.Rigging.TwoBoneIKConstraint scopingLeftWeight;
            public UnityEngine.Animations.Rigging.Rig unScopingLeftWeight;
        }

        [System.Serializable]
        public struct GunFiring
        {
            public GameObject bulletObject;
            public TracerFX bulletTracer;
            public Vector3 bulletSpread;
            public int bulletCount;
            public float bulletForce;
            public float bulletDamage;
            public float bulletPenetration;
            public LayerMask bulletFilter;
        }

        public float defaultSpeed = 5.0f;
        public float turnSpeed = 1.0f;
        public float lookSpeed = 1.0f;
        public float pushPower = 2.0f;
        public float jumpPower = 10.0f;
        public float gravity = -9.81f;

        [Space()]

        public GunRigging gunControl;

        public float aimLowerClamp;
        public float aimUpperClamp;
        public Transform target;

        [Space()]

        public GunFiring bulletData;

        [Space()]

        private float speed = 5.0f;
        private bool jumping = false; // FOr holding jump
        private bool groundedPlayer = false;

        private Vector3 playerVelocity = Vector3.zero;
        private RaycastHit aim;
        private Camera cam;
        private CharacterController controller = null;
        private Animator animator = null;

        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            cam = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Aim()
        {
            // Mouse Axis declarations
            float leftX = Input.GetAxis("Mouse X");
            float leftY = Input.GetAxis("Mouse Y");

            transform.Rotate(transform.up, leftX * turnSpeed * Time.deltaTime);

            cam.transform.Rotate(-Vector3.right, leftY * lookSpeed * Time.deltaTime);

            // Aim Target
            target.position = cam.transform.position + cam.transform.forward * 10;

            // Aim Animation
            if (Input.GetMouseButton(1))
            {
                animator.SetBool("IsAiming", true);
                gunControl.scopingGunWeight.weight = 0;
                gunControl.scopingLeftWeight.weight = 1;
                gunControl.unScopingLeftWeight.weight = 0;
            }
            else
            {
                animator.SetBool("IsAiming", false);
                gunControl.scopingGunWeight.weight = 1;
                gunControl.scopingLeftWeight.weight = 0;
                gunControl.unScopingLeftWeight.weight = 1;
            }
        }

        private void Move()
        {
            // Keyboard Axis declarations
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            // Jump
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                jumping = false;
                playerVelocity.y = 0f;
            }

            // Move
            speed = Input.GetKey(KeyCode.LeftShift) ? defaultSpeed * 1.5f : defaultSpeed;

            controller.Move((((transform.right * horizontal) + (transform.forward * vertical)) * speed) * Time.fixedDeltaTime);

            bool jump = Input.GetButton("Jump") && groundedPlayer;
            if (jump)
            {
                jumping = true;
                playerVelocity.y += Mathf.Sqrt(jumpPower * -3.0f * gravity);
            }

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            // Animation
            animator.SetFloat("SpeedMod", speed / defaultSpeed);
            animator.SetFloat("Xpos", Mathf.Clamp(horizontal, -1, 1), 1.0f, Time.deltaTime * 10.0f);
            animator.SetFloat("Ypos", Mathf.Clamp(vertical, -1, 1), 1.0f, Time.deltaTime * 10.0f);

            if (jumping != animator.GetBool("IsJumping"))
                animator.SetBool("IsJumping", jumping);
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            Aim();
            
            if (Input.GetMouseButtonDown(0))
            {
                Bullet.ShootBullets(bulletData.bulletObject, cam.transform.position, cam.transform.forward, bulletData.bulletCount, bulletData.bulletSpread, bulletData.bulletForce, bulletData.bulletDamage, bulletData.bulletPenetration, bulletData.bulletTracer, bulletData.bulletFilter);
            }
        }

        void OnDrawGizmosSelected()
        {
            // Aim
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, 1);
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
            body.velocity = pushDirection * pushPower;
        }
    }
}
