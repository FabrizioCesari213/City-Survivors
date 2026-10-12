using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStamina))]
public class PlayerMovement : MonoBehaviour
{      
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float groundAcceleration = 20f;
    [SerializeField] private float groundDeceleration = 25f;
    [SerializeField] private float airAcceleration = 5f;

   // [SerializeField] private float maxMomentumSpeed = 14f;
    [SerializeField] private float momentumDecay = 1.5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchingHeight = 1.2f;
    [SerializeField] private float crouchTransitionSpeed = 6f;
    [SerializeField] private LayerMask obstructionMask = ~0;


    private bool crouchHeld;
    private bool isCrouching;
    private float capsuleBottomOffset;
 

    private Vector2 moveInput;
    private CharacterController controller;
    private PlayerStamina stamina;
    private bool sprintHeld;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;



    public void OnMove(InputAction.CallbackContext context){
         moveInput = context.ReadValue<Vector2>();

        
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            sprintHeld = true;
        }
        else if (context.canceled)
        {
            sprintHeld = false;
        }
    }
   public void OnCrouch(InputAction.CallbackContext context)
{
    if (context.performed || context.canceled)
    {
        crouchHeld = context.ReadValueAsButton();

        Debug.Log(
            $"Crouch | Fase: {context.phase} | " +
            $"Presionado: {crouchHeld}"
        );
    }
}
   public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded && !isCrouching)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

   
    private bool CanStandUp()
    {
        float radius = controller.radius * 0.95f;

        float bottomY = capsuleBottomOffset + controller.radius;
        float topY = capsuleBottomOffset + standHeight - controller.radius;

        Vector3 bottom = transform.TransformPoint(
            new Vector3(controller.center.x, bottomY, controller.center.z)
        );

        Vector3 top = transform.TransformPoint(
            new Vector3(controller.center.x, topY, controller.center.z)
        );

        bool isObstructed = Physics.CheckCapsule(
            bottom,
            top,
            radius,
            obstructionMask,
            QueryTriggerInteraction.Ignore
        );

        return !isObstructed;
    }


    private void HandleCrouch()
    {
        bool shouldCrouch = crouchHeld;
        if (!shouldCrouch && controller.height < standHeight - 0.01f)
        {
            if (!CanStandUp())
        {
            shouldCrouch = true;
            }
        }
        
        isCrouching = shouldCrouch;
        
        float targetHeight = isCrouching
        ? crouchingHeight
        : standHeight;

    controller.height = Mathf.MoveTowards(
        controller.height,
        targetHeight,
        crouchTransitionSpeed * Time.deltaTime
    );

    Vector3 center = controller.center;
    center.y = capsuleBottomOffset + controller.height * 0.5f;
    controller.center = center;
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        stamina = GetComponent<PlayerStamina>();
        capsuleBottomOffset = controller.center.y - controller.height * 0.5f;
    }
    void Update()
    {   
        HandleCrouch();
       

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(forward);

        transform.rotation = Quaternion.RotateTowards(
         transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        Vector3 moveDirection =
        forward * moveInput.y +
        right * moveInput.x;

        moveDirection = Vector3.ClampMagnitude(
            moveDirection,
            1f
            );

   
    bool hasMovementInput = moveInput.sqrMagnitude > 0.01f;

    bool isSprinting = 
    sprintHeld && 
    hasMovementInput && 
    controller.isGrounded && 
    !isCrouching &&
    stamina.CanSprint;


    if (isSprinting)
    {
        stamina.ConsumeSprint(Time.deltaTime);
    } 

    float targetSpeed;
    if (isCrouching)
    {
        targetSpeed = crouchSpeed;
    }
    else if (isSprinting)
    {
        targetSpeed = sprintSpeed;
    }
    else
    {
        targetSpeed = moveSpeed;
    }
    
    Vector3 desiredVelocity = moveDirection * targetSpeed;
    

    if(controller.isGrounded){
        if (hasMovementInput)
            {
                float currentSpeed = horizontalVelocity.magnitude;
                if (currentSpeed <= targetSpeed)
                {
                    horizontalVelocity = Vector3.MoveTowards(
                        horizontalVelocity,
                        desiredVelocity,
                        groundAcceleration * Time.deltaTime
                    );
                }
                else
                {
                    Vector3 momentumTarget = moveDirection * currentSpeed; 
                    
                    horizontalVelocity = Vector3.MoveTowards(
                        horizontalVelocity,
                        momentumTarget,
                        groundAcceleration * Time.deltaTime
                    );
                
                if(horizontalVelocity.magnitude > targetSpeed)
                {
                    float decayedSpeed = Mathf.MoveTowards(
                        horizontalVelocity.magnitude,
                        targetSpeed,
                        momentumDecay * Time.deltaTime
                    );
                    horizontalVelocity = horizontalVelocity.normalized * decayedSpeed;
                }
                }
            }
            else
            {
                horizontalVelocity = Vector3.MoveTowards(
                    horizontalVelocity,
                    Vector3.zero,
                    groundDeceleration * Time.deltaTime
                );
            }  
       }
       else if (hasMovementInput)
        {
            float currentSpeed = horizontalVelocity.magnitude;

            Vector3 targetVelocity =
                moveDirection * Mathf.Max(currentSpeed, moveSpeed);

            horizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity,
                targetVelocity,
                airAcceleration * Time.deltaTime
            );
        }


         if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 velocity = horizontalVelocity;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
        
    }

    
}
