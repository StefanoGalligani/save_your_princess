using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;
    public float rotSpeed = 20;
    Camera cam;
    Rigidbody rb;
    
    private void Start() {
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleInput();
    }

    void LateUpdate() {
        MouseLook();
    }

    private void HandleInput() {
        Vector3 movement = CalcMovementDir() * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        if (Input.GetKeyDown(KeyCode.Space) && OnGround()) {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private Vector3 CalcMovementDir() {
        return (new Vector3(transform.forward.x, 0, transform.forward.z) * VertMovement()
            + new Vector3(transform.right.x, 0, transform.right.z) * HorMovement()).normalized;
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
        return Physics.SphereCast(transform.position, 0.25f, -transform.up, out hit, 0.6f);
    }

    private void MouseLook() {
        transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, 0);
        if (Mathf.Cos(Mathf.PI / 180 * (cam.transform.localRotation.eulerAngles.x -Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed)) > 0.5)
            cam.transform.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed, 0, 0);
    }
}
