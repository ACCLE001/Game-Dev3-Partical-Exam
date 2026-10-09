using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ThanethHamburut
{
    [RequireComponent(typeof(CharacterController))]
    public class CapsulePlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float sprintSpeed = 12f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float gravity = 20f;
        [SerializeField] private float fallThreshold = -8f;

        private CharacterController characterController;
        private Camera mainCamera;
        private Vector3 velocity;
        private bool hasFallen = false;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            mainCamera = Camera.main;
            hasFallen = false;
        }

        private void Update()
        {
            if (Stage1Manager.Instance != null && Stage1Manager.Instance.IsGameOver)
            {
                return;
            }

            // Check Fall
            if (!hasFallen && transform.position.y < fallThreshold)
            {
                hasFallen = true;
                if (Stage1Manager.Instance != null)
                {
                    Stage1Manager.Instance.PlayerFell();
                }
                return;
            }

            HandleMovement();
        }

        private void HandleMovement()
        {
            Vector2 input = GetMovementInput();
            bool isSprinting = GetSprintInput();

            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

            // Camera-relative movement direction
            Vector3 camForward = mainCamera != null ? mainCamera.transform.forward : Vector3.forward;
            Vector3 camRight = mainCamera != null ? mainCamera.transform.right : Vector3.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = (camForward * input.y + camRight * input.x).normalized;

            if (moveDirection.magnitude > 0.1f)
            {
                // Smooth rotation towards movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
            }

            // Gravity
            if (characterController.isGrounded)
            {
                velocity.y = -1f;
            }
            else
            {
                velocity.y -= gravity * Time.deltaTime;
            }

            characterController.Move(velocity * Time.deltaTime);
        }

        private Vector2 GetMovementInput()
        {
            float horizontal = 0f;
            float vertical = 0f;

#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) vertical += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) vertical -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontal += 1f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontal -= 1f;
            }
#endif

            // Fallback for legacy input
            if (horizontal == 0f && vertical == 0f)
            {
                horizontal = Input.GetAxisRaw("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");
            }

            return new Vector2(horizontal, vertical).normalized;
        }

        private bool GetSprintInput()
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard != null && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed))
            {
                return true;
            }
#endif
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
    }
}
