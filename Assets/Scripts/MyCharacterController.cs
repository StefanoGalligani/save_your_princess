using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;
    public float rotSpeed = 20;
    public float m_StepInterval;
    public AudioClip[] m_FootstepSounds;
    bool onGround = true;
    Camera cam;
    Rigidbody rb;
    Vector3 horizForward;
    LayerMask m;
    private float m_StepCycle = 0;
    private float m_NextStep = 0;
    
    private void Start() {
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        m = ~LayerMask.NameToLayer("Player");
    }

    void FixedUpdate()
    {
        onGround = OnGround();
        horizForward = new Vector3(transform.forward.x, 0, transform.forward.z);
        HandleInput();
        ProgressStepCycle(speed);
    }

    void LateUpdate() {
        MouseLook();
    }

    private void HandleInput() {
        Vector3 movement = CalcMovementDir() * CalcSpeedMultiplier() * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        if (Input.GetKeyDown(KeyCode.Space) && onGround) {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    private Vector3 CalcMovementDir() {
        return (horizForward * VertMovement()
            + Quaternion.Euler(0,90,0) * horizForward * HorMovement()).normalized;
    }

    private float CalcSpeedMultiplier() {
        if (!onGround) return 1;
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
        bool g = Physics.SphereCast(transform.position, 0.49f, -transform.up, out hit, .4f, ~LayerMask.NameToLayer("Floor"));
        return g;
    }

    private void MouseLook() {
        transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, 0);
        if (Mathf.Cos(Mathf.PI / 180 * (cam.transform.localRotation.eulerAngles.x -Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed)) > 0.5)
            cam.transform.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed, 0, 0);
    }
    
    private void ProgressStepCycle(float speed)
    {
        if (CalcMovementDir().sqrMagnitude > 0.001f)
        {
            m_StepCycle += speed*Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!onGround)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        GetComponent<AudioSource>().clip = m_FootstepSounds[n];
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = GetComponent<AudioSource>().clip;
        GetComponent<AudioSource>().clip = null;
    }
}
