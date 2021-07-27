using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon;

namespace GlassyGames.Conquered
{
    public class PlayerMovementC : MonoBehaviourPun
    {
        #region Private Variables

        // reference to the new input system and maps

        [SerializeField]
        PlayerInputManagerC inputManager;

        [SerializeField]
        float walkSpeed = 4f;
        [SerializeField]
        float runSpeed = 11f;
        [SerializeField]
        float gravity = -30.0f;


        CharacterController characterController;


        Animator animator;
        int isWalkingHash;
        int isRunningHash;

        Vector2 horizontalInput;
        Vector3 Velocity;

        bool isMovementPressed;
        bool isRunPressed;
        // bool isJumpPressed;

        #endregion

        #region Recieving

        public void RecieveInput(Vector2 _horizontalInput)
        {
            horizontalInput = _horizontalInput;
        }

        #endregion

        #region MonoBehaviours

        private void Awake()
        {
            if (!photonView.IsMine) { return; }


            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
   
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
        }

        private void Start()
        {
            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");
        }

        private void Update()
        {
            if (!animator) { return; }

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) { return; }

            isMovementPressed = horizontalInput.x != 0 || horizontalInput.y != 0;
            isRunPressed = inputManager.isRunPressed;

            HandleMove();
            HandleAnimation();
            HandleGravity();

            if (isRunPressed)
            {
                Velocity *= runSpeed;
            }
            Vector3 finalVelocity = new Vector3 (Velocity.x * Time.deltaTime, Velocity.y, Velocity.z * Time.deltaTime);
            characterController.Move(finalVelocity);
        }


        #endregion


        #region HandleMovements
        private void HandleMove()
        {
            Velocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * walkSpeed;
        }

        void HandleAnimation()
        {
            bool isRunning = animator.GetBool(isRunningHash);
            bool isWalking = animator.GetBool(isWalkingHash);

            // start walking if movement passed is tru and not already walking
            if (isMovementPressed && !isWalking)
            {
                animator.SetBool(isWalkingHash, true);
            }

            // stop walking if movementPressed is false and not already wlaking
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

        void HandleGravity()
        {
            if (characterController.isGrounded)
            {
                float groundedGravity = -0.5f;
                Velocity.y = groundedGravity;
            }
            else
            {
                Velocity.y += gravity * Time.deltaTime;
            }
        }

        #endregion


        #region On Events

        public void OnJumpPressed()
        {
            // isJumpPressed = true;
        }

        public void OnRunPressed()
        {
            isRunPressed = true;
        }

        #endregion
    }
}
