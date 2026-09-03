using UnityEngine;
using UnityEngine.InputSystem;

namespace SillySeal.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class SealController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Animator animator;

        [Header("Input Actions")]
        [Tooltip("The InputSystem_Actions asset. Actions are resolved by name from its 'Player' action map.")]
        [SerializeField] private InputActionAsset inputActions;

        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction diveAction;

        [Header("Land Movement")]
        [SerializeField] private float landMoveSpeed = 4f;
        [SerializeField] private float landTurnSpeed = 720f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -20f;

        [Header("Swim Movement")]
        [SerializeField] private float swimMoveSpeed = 6f;
        [SerializeField] private float swimVerticalSpeed = 3f;
        [SerializeField] private float swimTurnSpeed = 360f;

        [Header("Animation")]
        [Tooltip("Animator state names from Seal_Anim.controller. The controller has no parameters, so states are played directly by name.")]
        [SerializeField] private string landIdleState = "Seal|Idle_on_land_1";
        [SerializeField] private string landWalkState = "Seal|Walk_F_IP";
        [SerializeField] private string swimIdleState = "Seal|Swim_idle_horisontal";
        [SerializeField] private string swimMoveState = "Seal|Swim_F_IP";
        [SerializeField] private float animCrossfadeTime = 0.15f;
        [SerializeField] private float animMoveThreshold = 0.1f;

        private CharacterController controller;
        private Vector3 verticalVelocity;
        private bool isInWater;
        private bool jumpQueued;
        private string currentAnimState;

        public bool IsInWater => isInWater;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;

            if (animator != null)
                animator.applyRootMotion = false;

            if (inputActions != null)
            {
                InputActionMap playerMap = inputActions.FindActionMap("Player");
                moveAction = playerMap?.FindAction("Move");
                jumpAction = playerMap?.FindAction("Jump");
                diveAction = playerMap?.FindAction("Crouch");
            }
        }

        private void OnEnable()
        {
            moveAction?.Enable();
            jumpAction?.Enable();
            diveAction?.Enable();

            if (jumpAction != null)
                jumpAction.performed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            if (jumpAction != null)
                jumpAction.performed -= OnJumpPerformed;

            moveAction?.Disable();
            jumpAction?.Disable();
            diveAction?.Disable();
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            jumpQueued = true;
        }

        private void Update()
        {
            if (isInWater)
                UpdateSwim();
            else
                UpdateLand();

            jumpQueued = false;

            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (animator == null) return;

            Vector3 horizontalVelocity = controller.velocity;
            horizontalVelocity.y = 0f;
            bool isMoving = horizontalVelocity.sqrMagnitude > animMoveThreshold * animMoveThreshold;

            string desiredState = isInWater
                ? (isMoving ? swimMoveState : swimIdleState)
                : (isMoving ? landWalkState : landIdleState);

            if (desiredState == currentAnimState) return;

            currentAnimState = desiredState;
            animator.CrossFadeInFixedTime(desiredState, animCrossfadeTime);
        }

        public void EnterWater()
        {
            if (isInWater) return;
            isInWater = true;
            verticalVelocity = Vector3.zero;
        }

        public void ExitWater()
        {
            if (!isInWater) return;
            isInWater = false;
            verticalVelocity = Vector3.zero;
        }

        private void UpdateLand()
        {
            Vector3 moveDirection = ReadMoveDirection();

            if (moveDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, landTurnSpeed * Time.deltaTime);
            }

            if (controller.isGrounded)
            {
                verticalVelocity.y = -0.5f;
                if (jumpQueued)
                    verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else
            {
                verticalVelocity.y += gravity * Time.deltaTime;
            }

            Vector3 motion = moveDirection * landMoveSpeed + Vector3.up * verticalVelocity.y;
            controller.Move(motion * Time.deltaTime);
        }

        private void UpdateSwim()
        {
            Vector3 moveDirection = ReadMoveDirection();

            float vertical = 0f;
            if (jumpAction != null && jumpAction.IsPressed()) vertical += 1f;
            if (diveAction != null && diveAction.IsPressed()) vertical -= 1f;
            moveDirection += Vector3.up * vertical;

            if (moveDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, swimTurnSpeed * Time.deltaTime);
            }

            Vector3 planar = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized * swimMoveSpeed;
            Vector3 verticalMotion = Vector3.up * vertical * swimVerticalSpeed;

            controller.Move((planar + verticalMotion) * Time.deltaTime);
        }

        private Vector3 ReadMoveDirection()
        {
            Vector2 input = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
            if (input.sqrMagnitude < 0.0001f) return Vector3.zero;

            if (cameraTransform != null)
            {
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();
                return (forward * input.y + right * input.x).normalized;
            }

            return new Vector3(input.x, 0f, input.y).normalized;
        }
    }
}
