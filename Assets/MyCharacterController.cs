using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;
    public float rotSpeed = 20;
    Camera cam;
    
    private void Start() {
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleInput();
    }

    void LateUpdate() {
        MouseLook();
    }

    private void HandleInput() {
        Vector3 movement = CalcMovementDir() * Time.deltaTime * speed;
        GetComponent<Rigidbody>().MovePosition(transform.position + movement);
        if (Input.GetKeyDown(KeyCode.Space) && OnGround()) {
            GetComponent<Rigidbody>().AddForce(transform.up * jumpForce, ForceMode.Impulse);
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
        return Physics.Raycast(transform.position, -transform.up, 0.76f);
    }

    private void MouseLook() {
        transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, 0);
        if (Mathf.Cos(Mathf.PI / 180 * (cam.transform.localRotation.eulerAngles.x -Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed)) > 0.5)
            cam.transform.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed, 0, 0);
    }
}
