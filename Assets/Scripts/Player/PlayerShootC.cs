using Photon.Pun;
using UnityEngine;

namespace GlassyGames.Conquered
{
    public class PlayerShootC : MonoBehaviourPun
    {
        #region variables

        public PlayerWeaponC weapon;

        public Camera cam;

        bool isShootPressed;

        #endregion

        #region MonoBehaviours
        public void GetIsShootPressed(bool _isShootPressed)
        {
            isShootPressed = _isShootPressed;
        }

        private void Awake()
        {
            if (!photonView.IsMine) { return; }
        }

        private void Start()
        {
            cam = GetComponentInChildren<Camera>();

            if (cam == null)
            {
                Debug.LogError("Player Shoot: No Camera Referenced");
                this.enabled = false;
            }

        }

        private void Update()
        {
            if (isShootPressed)
            {
                Debug.Log($"yay");
                Shoot();
            }
        }

        #endregion

        #region public methods



        public void Shoot()
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit _hit, weapon.range))
            {
                if (_hit.collider.GetComponent<PlayerHealthC>() != null)
                {
                    _hit.collider.GetComponent<PlayerHealthC>().TakeDamage(weapon.damage);

                }
            }
        }

        #endregion
    }
}
