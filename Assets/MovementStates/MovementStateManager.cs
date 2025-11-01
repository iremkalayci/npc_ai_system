using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    // State machine
    public MovementBaseState currentState;
    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public RunState Run = new RunState();
    public CrouchState Crouch = new CrouchState();

    [HideInInspector] public Animator anim;
    [HideInInspector] public CharacterController controller;

    [Header("Movement Settings")]
    public float currentMoveSpeed = 3f;
    public float walkSpeed=3, walkBackSpeed=2;
    public float runSpeed=7, runBackSpeed=5;
    public float crouchSpeed=2, crouchBackSpeed=1;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    public float groundYOffset = 0.1f;
    public LayerMask groundMask;

    [HideInInspector] public float hzInput;
    [HideInInspector] public float vInput;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public bool isGrounded;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        SwitchState(Idle);
    }

    void Update()
    {
        // Input al
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        // Hareket yönü
        dir = transform.forward * vInput + transform.right * hzInput;
        Vector3 move = dir.normalized * currentMoveSpeed;

        // Gravity
        Vector3 spherePos = transform.position - new Vector3(0, groundYOffset, 0);
        isGrounded = Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;

        move += velocity;

    
        controller.Move(move * Time.deltaTime);

        // Animator parametreleri
        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);

        // State güncelle
        currentState.UpdateState(this);
    }

    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    private void OnDrawGizmos()
    {
        if (controller != null)
        {
            Gizmos.color = Color.red;
            Vector3 spherePos = transform.position - new Vector3(0, groundYOffset, 0);
            Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
        }
    }
}
