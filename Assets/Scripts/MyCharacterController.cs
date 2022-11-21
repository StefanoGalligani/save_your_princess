using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;
    public float rotSpeed = 20;
    bool onGround = true;
    Camera cam;
    Rigidbody rb;
    Vector3 horizForward;
    
    private void Start() {
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        onGround = OnGround();
        horizForward = new Vector3(transform.forward.x, 0, transform.forward.z);
        HandleInput();
    }

    void LateUpdate() {
        MouseLook();
    }

    private void HandleInput() {
        Vector3 movement = CalcMovementDir() * CalcSpeedMultiplier() * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        if (Input.GetKeyDown(KeyCode.Space) && onGround) {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private Vector3 CalcMovementDir() {
        return (horizForward * VertMovement()
            + Quaternion.Euler(0,90,0) * horizForward * HorMovement()).normalized;
    }

    private float CalcSpeedMultiplier() {
        if (!onGround) return 1;
        LayerMask m = ~LayerMask.NameToLayer("Player");
        float e = 0.1f;
        RaycastHit hit;
        float h1=0, h2=0;

        Physics.Raycast(transform.position, -transform.up, out hit, 10f, m);
        h1 = hit.distance;
        Physics.Raycast(transform.position + horizForward * e, -transform.up, out hit, 10f, m);
        h2 = hit.distance;
        float d = (h1-h2)/e;
        if (d<0) return 1;
        return 1/Mathf.Sqrt(1+d*d*4);
    }

    private float VertMovement() {
        float mov = 0;
        if (Input.GetKey(KeyCode.W)) mov++;
        if (Input.GetKey(KeyCode.S)) mov--;
        return mov;
    }
    private float HorMovement() {
        float mov = 0;
        if (Input.GetKey(KeyCode.D)) mov++;
        if (Input.GetKey(KeyCode.A)) mov--;
        return mov;
    }

    private bool OnGround() {
        RaycastHit hit;
        return Physics.SphereCast(transform.position, 0.5f, -transform.up, out hit, 0.6f);
    }

    private void MouseLook() {
        transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, 0);
        if (Mathf.Cos(Mathf.PI / 180 * (cam.transform.localRotation.eulerAngles.x -Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed)) > 0.5)
            cam.transform.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed, 0, 0);
    }
}
