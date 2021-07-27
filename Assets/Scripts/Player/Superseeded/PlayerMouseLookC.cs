using UnityEngine;
using Photon.Pun;

namespace GlassyGames.Conquered
{
    public class PlayerMouseLookC : MonoBehaviourPun
    {
        [SerializeField]
        float sensitivityX = 20f;
        [SerializeField]
        float sensitivityY = 0.1f;
        [SerializeField]
        private Transform playerCameraTransform = null;

        readonly float xClamp = 85f;
        Vector2 mouseInput;
        float mouseX, mouseY;
        float xRotation = 0f;


        void Awake()
        {
            if (photonView.IsMine)
            {
                playerCameraTransform.gameObject.SetActive(true);
            }

        }

        public void HandleRotation(Vector2 _mouseInput)
        {

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) { return; }

            mouseInput = _mouseInput;
            mouseX = mouseInput.x * sensitivityX;
            mouseY = mouseInput.y * sensitivityY;

            this.transform.Rotate(Vector3.up, mouseX * Time.deltaTime);

            // gives the correct rotation based on mouse movement up and down
            xRotation -= mouseY;
            // clamps the look rotation
            xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
            // gets the current rotation of the player
            Vector3 targetRotation = transform.eulerAngles;
            // sets the rotation of the player to equal our mouse movement
            targetRotation.x = xRotation;
            // sets the rotation of the camera to that of the mouse movement
            playerCameraTransform.eulerAngles = targetRotation;
        }
    }
}
