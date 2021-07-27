using UnityEngine;

namespace GlassyGames.Conquered
{
    public class PlayerMotorC : MonoBehaviour
    {
        [SerializeField]
        private Camera cam;

        private Vector3 velocity = Vector3.zero;
        private float rotation = 0f;
        private Vector3 cameraRotation = Vector3.zero;

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

        public void RotateCamera(Vector3 _cameraRotation)
        {
            cameraRotation = _cameraRotation;
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
                Vector3 finalVelocity = new Vector3(velocity.x * Time.deltaTime, velocity.y, velocity.z * Time.deltaTime);
                characterController.Move(finalVelocity);
            }
        }

        private void PerformRotation()
        {
            transform.Rotate(Vector3.up, rotation * Time.deltaTime);
            if (cam != null)
            {
                cam.transform.Rotate(cameraRotation * Time.deltaTime);
            }
        }
    }
}
