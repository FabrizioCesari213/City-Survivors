using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(CharacterController))]

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private PlayerMovement movement;
    private CharacterController controller;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (animator == null) return;

        Vector3 velocity = controller.velocity;
        velocity.y = 0f;

        Vector3 localVelocity =
            transform.InverseTransformDirection(velocity);

        float referenceSpeed =
            Mathf.Max(0.01f, movement.AnimationReferenceSpeed);

        float moveX = Mathf.Clamp(
            localVelocity.x / referenceSpeed, -1f, 1f);

        float moveY = Mathf.Clamp(
            localVelocity.z / referenceSpeed, -1f, 1f);

        animator.SetFloat(
            "MoveX", moveX, 0.1f, Time.deltaTime);

        animator.SetFloat(
            "MoveY", moveY, 0.1f, Time.deltaTime);

        animator.SetBool(
            "IsSprinting", movement.IsSprinting);

        animator.SetBool(
            "IsCrouching", movement.IsCrouching);

        animator.SetBool(
            "IsGrounded", controller.isGrounded);

        animator.SetFloat(
            "VerticalSpeed", movement.VerticalSpeed);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            animator.SetBool(
                "IsAiming", context.ReadValueAsButton());
        }
    }
}