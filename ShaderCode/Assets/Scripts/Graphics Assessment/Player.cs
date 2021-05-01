using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonPlayerShooter
{
    /// <summary>
    /// Player class is the main class used by the user which allows for lots of data manipulation and gameplay mechanics.
    /// </summary>
    [RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
    public class Player : MonoBehaviour
    {
        public static Player main;
        public static int enemyKills = 0;

        #region EditorDataOrganisers (structs)
        [System.Serializable]
        public struct GunRigging
        {
            public UnityEngine.Animations.Rigging.Rig scopingGunWeight;
            public UnityEngine.Animations.Rigging.Rig scopingLeftWeight;
            public UnityEngine.Animations.Rigging.Rig unScopingLeftWeight;
        }

        [System.Serializable]
        public struct GunFiring
        {
            public TracerFX bulletTracer;
            public Vector3 bulletSpread;
            public int bulletCount;
            public float bulletForce;
            public float bulletDamage;
            public float bulletPenetration;
            public Transform firePos;
            public bool automatic;
            public float fireRate;
            public LayerMask bulletFilter;
        }

        [System.Serializable]
        public struct Ghosting
        {
            public Material ghostBody;
            public Material worldGhost;
            public Material ghostGun;
        }
        #endregion

        #region Fields
        [Header("Player Controller")]

        [Tooltip("How fast is the player without sprinting?")] public float _defaultSpeed = 5.0f;
        public float _turnSpeed = 1.0f;
        public float _lookSpeed = 1.0f;
        public Vector2 _upwardsLock = Vector2.zero;
        public float _pushPower = 2.0f;
        public float _jumpPower = 10.0f;
        public float _gravity = -9.81f;
        public Transform _objectFollowing;
        public Transform _enemyTarget;

        [Space()]
        [Header("UI")]
        public UnityEngine.UI.Slider _healthBar;
        public UnityEngine.UI.Text _killText;
        public GameObject _deathScreen;
        public UnityEngine.UI.Text _deathScreenText;

        [Space()]
        [Header("Ghost Setup")]

        public Ghosting _ghostData;

        [Space()]
        [Header("Gun Control")]

        public GunRigging _gunControl;

        public float _aimLowerClamp;
        public float _aimUpperClamp;
        public float _aimSpeed;

        public Transform _target;

        [Space()]

        public GunFiring _bulletData;

        // Private

        private bool isJumping = false; // For holding jump
        private bool isGrounded = false;

        private Vector3 playerVelocity = Vector3.zero;
        private Vector2 cameraMovement = Vector2.zero;

        private Camera sceneCamera;
        private CharacterController characterController = null;
        private Animator characterAnimator = null;

        private float movementSpeed = 5.0f;
        private float lastGunFire;
        private float defaultFieldOfView = 75;

        private List<RaycastHit> bulletHits; // List of previous bullet hits.

        // Input declarations to be used in FixedUpdate physics.
        private float mouseAxisX = 0;
        private float mouseAxisY = 0;

        private float keyboardAxisVertical = 0;
        private float keyboardAxisHorizontal = 0;

        private bool isUsingJumpInput = false;
        private bool isUsingRunningInput = false;

        private bool isShooting = false;

        private float health = 100;

        private float maxHealth = 100;

        private bool isDead = false;
        #endregion

        #region Functions
        // Start is called before the first frame update
        void Start()
        {
            main = this;
            enemyKills = 0;

            characterController = GetComponent<CharacterController>();
            characterAnimator = GetComponent<Animator>();
            sceneCamera = Camera.main;
            defaultFieldOfView = sceneCamera.fieldOfView;
            Cursor.lockState = CursorLockMode.Locked;

            _ghostData.ghostBody.SetFloat("RL_Transparency", 1);
            _ghostData.ghostBody.SetFloat("RL_RimPower", 8f);
            _ghostData.ghostGun.SetFloat("RL_RimPower", 8f);
            _ghostData.ghostGun.SetFloat("RL_Transparency", 1);
            _ghostData.worldGhost.SetFloat("RL_Transparency", 1);

            _healthBar.maxValue = maxHealth;
            _healthBar.value = health;
        }

        /// <summary>
        /// A simple way to control all gun animation weights in one line.
        /// </summary>
        /// <param name="a_scopingGunWeight">Scoped gun weight (zoom in)</param>
        /// <param name="a_scopingLeftWeight">Scoping arm weight (zoom in)</param>
        /// <param name="a_unScopingLeftWeight">Unscoping gun weight (normal)</param>
        private void GunControlWeights(float a_scopingGunWeight, float a_scopingLeftWeight, float a_unScopingLeftWeight)
        {
            _gunControl.scopingGunWeight.weight = a_scopingGunWeight;
            _gunControl.scopingLeftWeight.weight = a_scopingLeftWeight;
            _gunControl.unScopingLeftWeight.weight = a_unScopingLeftWeight;
        }

        /// <summary>
        /// Custom function used to clamp angles to prevent buggy aim.
        /// </summary>
        /// <param name="a_angle">Angle to clamp.</param>
        /// <param name="a_from">Minimum clamp</param>
        /// <param name="a_to">Maximum clamp</param>
        /// <returns></returns>
        private float ClampAngle(float a_angle, float a_from, float a_to)
        {
            if (a_angle > 180) a_angle = 360 - a_angle;
            a_angle = Mathf.Clamp(a_angle, a_from, a_to);

            return a_angle;
        }

        /// <summary>
        /// Player aim control.
        /// </summary>
        /// <param name="a_isPhysics">FixedUpdate = true, Update = false. Very important.</param>
        private void Aim(bool a_isPhysics)
        {
            if (a_isPhysics)
            {
                // Camera and body movement

                //sceneCamera.transform.Rotate(-Vector3.right, mouseAxisY * _lookSpeed * Time.deltaTime);

                // Aim Target
                _target.position = sceneCamera.transform.position + sceneCamera.transform.forward * 10;
            }
            else
            {
                // Mouse Axis declarations
                mouseAxisX = Input.GetAxis("Mouse X");
                mouseAxisY = Input.GetAxis("Mouse Y");

                transform.Rotate(transform.up, mouseAxisX * _turnSpeed * Time.deltaTime);

                // Sensitivity is gross when in fixed update so we put it here to give the ultimate user experience.
                cameraMovement.x += mouseAxisX * _lookSpeed;

                cameraMovement.y -= mouseAxisY * _lookSpeed;
                cameraMovement.y = ClampAngle(cameraMovement.y, _upwardsLock.x, _upwardsLock.y);

                sceneCamera.transform.eulerAngles = new Vector3(cameraMovement.y, sceneCamera.transform.eulerAngles.y, sceneCamera.transform.eulerAngles.z);

                // Aim Animation
                if (Input.GetMouseButton(1))
                {
                    characterAnimator.SetBool("IsAiming", true);
                    GunControlWeights(0, 1, 0);

                    sceneCamera.fieldOfView = Mathf.Lerp(sceneCamera.fieldOfView, defaultFieldOfView / 1.5f, Time.fixedDeltaTime * _aimSpeed);
                }
                else
                {
                    characterAnimator.SetBool("IsAiming", false);
                    GunControlWeights(1, 0, 1);

                    sceneCamera.fieldOfView = Mathf.Lerp(sceneCamera.fieldOfView, defaultFieldOfView, Time.fixedDeltaTime * _aimSpeed);
                }
            }
        }

        /// <summary>
        /// Player movement control.
        /// </summary>
        /// <param name="a_isPhysics">FixedUpdate = true, Update = false. Very important.</param>
        private void Move(bool a_isPhysics)
        {
            if (a_isPhysics)
            {
                // Jump
                if (isUsingJumpInput)
                {
                    isJumping = true;
                    playerVelocity.y += Mathf.Sqrt(_jumpPower * -3.0f * _gravity);
                }

                // Move
                movementSpeed = isUsingRunningInput ? _defaultSpeed * 1.5f : _defaultSpeed;

                characterController.Move((((transform.right * keyboardAxisHorizontal) + (transform.forward * keyboardAxisVertical)) * movementSpeed) * Time.deltaTime);

                playerVelocity.y += _gravity * Time.deltaTime;
                characterController.Move(playerVelocity * Time.deltaTime);

                // Jump
                isGrounded = characterController.isGrounded;
                if (isGrounded && playerVelocity.y < 0)
                {
                    isJumping = false;
                    playerVelocity.y = 0f;
                }
            }
            else
            {
                // Keyboard Axis declarations
                keyboardAxisVertical = Input.GetAxis("Vertical");
                keyboardAxisHorizontal = Input.GetAxis("Horizontal");

                isUsingJumpInput = Input.GetButton("Jump") && isGrounded;
                isUsingRunningInput = Input.GetKey(KeyCode.LeftShift);

                // Animation
                characterAnimator.SetFloat("SpeedMod", movementSpeed / _defaultSpeed);
                characterAnimator.SetFloat("Xpos", Mathf.Clamp(keyboardAxisHorizontal, -1, 1), 1.0f, Time.deltaTime * 10.0f);
                characterAnimator.SetFloat("Ypos", Mathf.Clamp(keyboardAxisVertical, -1, 1), 1.0f, Time.deltaTime * 10.0f);

                if (isJumping != characterAnimator.GetBool("IsJumping"))
                    characterAnimator.SetBool("IsJumping", isJumping);
            }
        }

        /// <summary>
        /// Used to shoot from the gun.
        /// </summary>
        private void Shoot()
        {
            isShooting = (!_bulletData.automatic && Input.GetMouseButtonDown(0)) || (_bulletData.automatic && Input.GetMouseButton(0));

            if (lastGunFire < Time.realtimeSinceStartup && isShooting)
            {
                lastGunFire = Time.realtimeSinceStartup + _bulletData.fireRate;
                bulletHits = Bullet.ShootBullets(_bulletData.firePos.position, sceneCamera.transform.forward, _bulletData.bulletCount, _bulletData.bulletSpread, _bulletData.bulletForce, _bulletData.bulletDamage, _bulletData.bulletPenetration, _bulletData.bulletTracer, _bulletData.bulletFilter);
            }
        }

        // Stuff used to UI interaction.
        public void GhostMode(float a_transparency)
        {
            _ghostData.ghostBody.SetFloat("RL_Transparency", a_transparency);
            _ghostData.ghostBody.SetFloat("RL_RimPower", (7.5f * a_transparency) + 0.5f);

            _ghostData.ghostGun.SetFloat("RL_RimPower", (7.5f * a_transparency) + 0.5f);
            _ghostData.ghostGun.SetFloat("RL_Transparency", a_transparency);

            _ghostData.worldGhost.SetFloat("RL_Transparency", a_transparency);

            LayerMask layer;
            if (a_transparency > 0.5)
            {
                layer = LayerMask.NameToLayer("Player");
            }
            else
            {
                layer = LayerMask.NameToLayer("Ghost");
            }

            gameObject.layer = layer;
        }

        public void BulletPenetration(float a_penetration)
        {
            _bulletData.bulletPenetration = a_penetration;
        }

        public void ResetWorld()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Animation");
        }
        //

        // Enemy/Player could be inheriting a "Damageable" baseclass but I didn't think about it when I coded this..
        /// <summary>
        /// Take damage for the player using damage and damage type.
        /// </summary>
        /// <param name="a_amount">Damage amount</param>
        /// <param name="a_dmgType">Damage type used for effects and multiplyers.</param>
        public void TakeDamage(float a_amount, DamageType a_dmgType = DamageType.Slash)
        {
            if (isDead) return;

            health -= a_amount;

            if (health < 0)
            {
                isDead = true;

                Cursor.lockState = CursorLockMode.Confined;

                _deathScreen.SetActive(true);
                _deathScreenText.text = string.Format(_deathScreenText.text, enemyKills);
                characterAnimator.SetBool("isDead", true);
            }
        }

        // Used for physics movement to keep it exact regardless of computer specs.
        private void FixedUpdate()
        {
            if (isDead) return;

            if (Cursor.lockState != CursorLockMode.Confined)
            {
                Move(true);
                Aim(true);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (isDead) return;

            _healthBar.value = Mathf.Lerp(_healthBar.value, health, Time.deltaTime * 2f); // Should be the only way to lerp healthbars.
            _killText.text = $"Enemies Killed: {enemyKills}";

            if (Cursor.lockState != CursorLockMode.Confined)
            {
                Move(false);
                Aim(false);

                if (_ghostData.ghostBody.GetFloat("RL_Transparency") > 0.5)
                {
                    Shoot();
                }
            }
            else isShooting = false;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Confined ? CursorLockMode.Locked : CursorLockMode.Confined;
            }
        }

        // Draw target in the editor.
        void OnDrawGizmosSelected()
        {
            // Aim
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_target.position, 1);
        }

        // Moving rigidbody objects around in the scene.
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
        #endregion
    }
}