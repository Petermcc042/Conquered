using Photon.Pun;
using UnityEngine;

namespace GlassyGames.Conquered
{
    public class PlayerControllerC : MonoBehaviourPun
    {
        [SerializeField]
        private float walkSpeed = 3f;
        [SerializeField]
        private float runSpeed = 5f;
        [SerializeField]
        private float sensitivityX = 20f;
        [SerializeField]
        private float sensitivityY = 0.2f;
        [SerializeField]
        float gravity = -30f;
        [SerializeField]
        float jumpForce = 10f;

        [SerializeField]
        private PlayerMotorC motor;
        [SerializeField]
        private PlayerShootC playerShoot;
        PlayerInput input;
        Animator animator;
        CharacterController characterController;

        Vector2 horizontalInput = Vector2.zero;
        Vector2 mouseInput = Vector2.zero;

        float groundedGravity = -0.5f;

        bool isMovementPressed;
        bool isRunPressed;
        bool isJumpPressed;

        int isWalkingHash;
        int isRunningHash;

        private void Start()
        {
            motor = GetComponent<PlayerMotorC>();


            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");
        }

        private void Awake()
        {
            if (!photonView.IsMine) { return; }

            input = new PlayerInput();
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();

            // FOR ACTUAL GAME UNCOMMENT
            // Cursor.lockState = CursorLockMode.Locked;

            input.Gameplay.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
            input.Gameplay.Move.canceled += ctx => horizontalInput = ctx.ReadValue<Vector2>();
            input.Gameplay.Move.started += ctx => horizontalInput = ctx.ReadValue<Vector2>();
            input.Gameplay.Run.performed += ctx => isRunPressed = ctx.ReadValueAsButton();
            input.Gameplay.Run.canceled += ctx => isRunPressed = ctx.ReadValueAsButton();
            input.Gameplay.Jump.performed += ctx => isJumpPressed = ctx.ReadValueAsButton();
            input.Gameplay.Jump.canceled += ctx => isJumpPressed = ctx.ReadValueAsButton();
            input.Gameplay.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
            input.Gameplay.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
            input.Gameplay.Shoot.performed += _ => playerShoot.Shoot();
        }

        private void Update()
        {
            if (!animator) { return; }

            // final movement vector
            Vector3 _velocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * walkSpeed;

            if (isRunPressed)
            {
                _velocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * runSpeed;
            }

            if (characterController.isGrounded)
            {
                _velocity.y = groundedGravity;
            }
            else
            {
                _velocity.y += gravity * Time.deltaTime;
            }

            // calculate jump force based on player input
            Vector3 _jumpForce = Vector3.zero;
            if (isJumpPressed)
            {
                _jumpForce = Vector3.up * jumpForce;
            }
            motor.ApplyJump(_jumpForce);

            // apply movement
            motor.Move(_velocity);


            // Player Rotation
            // calculate rotation as a float angle
            float _rotation = mouseInput.x * sensitivityX;
            // apply player rotation
            motor.Rotate(_rotation);


            // Camera Rotation
            // calculate rotation as a float angle
            float _cameraRotationX = mouseInput.y * sensitivityY;
            // apply player rotation
            motor.RotateCamera(_cameraRotationX);


            // Player Animation
            isMovementPressed = horizontalInput.x != 0 || horizontalInput.y != 0;

            bool isRunning = animator.GetBool(isRunningHash);
            bool isWalking = animator.GetBool(isWalkingHash);

            if (isMovementPressed && !isWalking)
            {
                animator.SetBool(isWalkingHash, true);
            }

            if (!isMovementPressed && isWalking)
            {
                animator.SetBool(isWalkingHash, false);
            }

            if ((isMovementPressed && isRunPressed) && !isRunning)
            {
                animator.SetBool(isRunningHash, true);
            }

            if ((!isMovementPressed || !isRunPressed) && isRunning)
            {
                animator.SetBool(isRunningHash, false);
            }


        }

        #region Input on off

        private void OnEnable()
        {
            input.Enable();
        }

        private void OnDestroy()
        {
            input.Disable();
        }

        #endregion

    }
}
