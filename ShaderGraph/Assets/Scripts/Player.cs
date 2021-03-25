using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    CharacterController controller = null;
    Animator animator = null;

    public float speed = 80.0f;
    public float animationSpeed = 80.0f;
    public float turnSpeed = 80.0f;
    public float pushPower = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        float leftX = Input.GetAxis("Mouse X");
        float leftY = Input.GetAxis("Mouse Y");

        //controller.SimpleMove(transform.forward * vertical * speed * Time.fixedDeltaTime);
        controller.SimpleMove(((transform.right * horizontal) + (transform.forward * vertical)) * speed * Time.fixedDeltaTime);

        transform.Rotate(transform.up, leftX * turnSpeed * Time.fixedDeltaTime);

        animator.SetFloat("Xpos", horizontal * animationSpeed * Time.fixedDeltaTime);
        animator.SetFloat("Ypos", vertical * animationSpeed * Time.fixedDeltaTime);
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
