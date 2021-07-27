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

        private PlayerMotorC motor;
        private PlayerShootC playerShoot;
        PlayerInput input;
        Animator animator;
        CharacterController characterController;

        Vector2 horizontalInput = Vector2.zero;
        Vector2 mouseInput = Vector2.zero;

        float xClamp = 85f;
        float groundedGravity = -0.5f;


        bool isShootPressed;
        bool isMovementPressed;
        bool isRunPressed;

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

            Cursor.lockState = CursorLockMode.Locked;

            input.Gameplay.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
            input.Gameplay.Move.canceled += ctx => horizontalInput = ctx.ReadValue<Vector2>();
            input.Gameplay.Move.started += ctx => horizontalInput = ctx.ReadValue<Vector2>();
            input.Gameplay.Run.performed += ctx => isRunPressed = ctx.ReadValueAsButton();
            input.Gameplay.Run.canceled += ctx => isRunPressed = ctx.ReadValueAsButton();
            input.Gameplay.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
            input.Gameplay.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
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

            // apply movement
            motor.Move(_velocity);


            // Player Rotation
            // calculate rotation as a float angle
            float _rotation = mouseInput.x * sensitivityX;
            // apply player rotation
            motor.Rotate(_rotation);


            // Camera Rotation
            // clamp rotation
            float mouseInputY = Mathf.Clamp(-mouseInput.y, -xClamp, xClamp);
            // calculate rotation as a float angle
            Vector3 _cameraRotation = new Vector3(mouseInputY, 0f, 0f) * sensitivityY;
            // apply player rotation
            motor.RotateCamera(_cameraRotation);


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
