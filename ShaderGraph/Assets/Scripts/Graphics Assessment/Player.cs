using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    CharacterController controller = null;
    Animator animator = null;

    public float defaultSpeed = 5.0f;
    public float turnSpeed = 1.0f;
    public float lookSpeed = 1.0f;
    public float pushPower = 2.0f;
    public float jumpPower = 10.0f;
    public float gravity = -9.81f;

    public float aimLowerClamp;
    public float aimUpperClamp;

    private float speed = 5.0f;
    private bool jumping = false; // FOr holding jump
    private bool groundedPlayer = false;
    private Vector3 playerVelocity = Vector3.zero;

    public Transform target;

    private RaycastHit aim;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        float leftX = Input.GetAxis("Mouse X");
        float leftY = Input.GetAxis("Mouse Y");

        speed = Input.GetKey(KeyCode.LeftShift) ? defaultSpeed * 1.5f : defaultSpeed;

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            jumping = false;
            playerVelocity.y = 0f;
        }

        transform.Rotate(transform.up, leftX * turnSpeed * Time.deltaTime);

        cam.transform.Rotate(-Vector3.right, leftY * lookSpeed * Time.deltaTime);

        //Physics.Raycast(cam.transform.position, cam.transform.forward, out aim, 10, ~(1 << 8));

        target.position = cam.transform.position + cam.transform.forward * 10;

        animator.SetFloat("Xpos", Mathf.Clamp(horizontal, -1, 1), 1.0f, Time.deltaTime * 10.0f);
        animator.SetFloat("Ypos", Mathf.Clamp(vertical, -1, 1), 1.0f, Time.deltaTime * 10.0f);

        animator.SetBool("IsAiming", Input.GetMouseButton(1));

        controller.Move((((transform.right * horizontal) + (transform.forward * vertical)) * speed) * Time.fixedDeltaTime);

        animator.SetFloat("SpeedMod", speed/defaultSpeed);

        bool jump = Input.GetButton("Jump") && groundedPlayer;
        if (jump)
        {
            jumping = true;
            playerVelocity.y += Mathf.Sqrt(jumpPower * -3.0f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (jumping != animator.GetBool("IsJumping"))
            animator.SetBool("IsJumping", jumping);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, 1);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDirection * pushPower;
    }
}
