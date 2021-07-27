using UnityEngine;

namespace GlassyGames.Conquered
{
    public class PlayerMotorC : MonoBehaviour
    {
        [SerializeField]
        private Camera cam;

        private Vector3 velocity = Vector3.zero;
        private float rotation = 0f;
        private float cameraRotationX = 0f;
        private float currentCameraRotationX = 0f;
        private float cameraClamp = 85f;
        private Vector3 jumpForce = Vector3.zero;

        private CharacterController characterController;


        private void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        public void Move(Vector3 _velocity)
        {
            velocity = _velocity;
        }        
        
        public void Rotate(float _rotation)
        {
            rotation = _rotation;
        }

        public void RotateCamera(float _cameraRotationX)
        {
            cameraRotationX = _cameraRotationX;
        }

        public void ApplyJump(Vector3 _jumpForce)
        {
            jumpForce = _jumpForce;
        }

        


        private void FixedUpdate()
        {
            PerformMovement();
            PerformRotation();
        }

        private void PerformMovement()
        {
            if (velocity != Vector3.zero)
            {
                Vector3 finalVelocity = new Vector3(velocity.x * Time.deltaTime, (velocity.y +jumpForce.y) * Time.deltaTime, velocity.z * Time.deltaTime);
                characterController.Move(finalVelocity);
            }
        }

        private void PerformRotation()
        {
            transform.Rotate(Vector3.up, rotation * Time.deltaTime);
            if (cam != null)
            {
                currentCameraRotationX -= cameraRotationX * Time.deltaTime;
                currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraClamp, cameraClamp);
                cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
            }
        }
    }
}
