using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlassyGames.Conquered
{
    public class PlayerInputManagerC : MonoBehaviourPun
    {
        PlayerInput input;

        [SerializeField] PlayerMovementC movement;
        [SerializeField] PlayerMouseLookC mouseLook;
        [SerializeField] PlayerShootC playerShoot;

        Vector2 horizontalInput;
        Vector2 mouseInput;
        Vector3 Velocity;

        public bool isRunPressed;
        public bool isShootPressed;

        #region MonoBehaviour

        private void Awake()
        {
            if (!photonView.IsMine) { return; }

            input = new PlayerInput();

            Cursor.lockState = CursorLockMode.Locked;

            input.Gameplay.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();

            input.Gameplay.Move.performed += OnMovementInput;
            input.Gameplay.Move.started += OnMovementInput;
            input.Gameplay.Move.canceled += OnMovementInput;

            input.Gameplay.Run.performed += ctx => isRunPressed = ctx.ReadValueAsButton();
            input.Gameplay.Run.canceled += ctx => isRunPressed = ctx.ReadValueAsButton();

            input.Gameplay.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
            input.Gameplay.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();

            input.Gameplay.Jump.performed += _ => movement.OnJumpPressed();

            input.Gameplay.Shoot.performed += ctx => isShootPressed = ctx.ReadValueAsButton();
            input.Gameplay.Shoot.canceled += ctx => isShootPressed = ctx.ReadValueAsButton();
        }

        private void Update()
        {
            movement.RecieveInput(horizontalInput);
            mouseLook.HandleRotation(mouseInput);
        }

        #endregion

        #region On Events

        void OnMovementInput(InputAction.CallbackContext ctx)
        {
            horizontalInput = ctx.ReadValue<Vector2>();
            Velocity.x = horizontalInput.x;
            Velocity.z = horizontalInput.y;
        }

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
