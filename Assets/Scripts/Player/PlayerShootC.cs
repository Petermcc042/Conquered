using Photon.Pun;
using System;
using UnityEngine;

namespace GlassyGames.Conquered
{
    public class PlayerShootC : MonoBehaviourPun
    {
        #region variables

        public PlayerWeaponC weapon;

        [SerializeField]
        private Camera cam;

        #endregion

        #region MonoBehaviours

        private void Start()
        {
            if (cam == null)
            {
                Debug.LogError("Player Shoot: No Camera Referenced");
                this.enabled = false;
            }

        }


        #endregion

        #region public methods



        public void Shoot()
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit _hit, weapon.range))
            {
                if (_hit.collider.tag == "Player")
                {
                    // _hit.collider.GetComponent<PlayerHealthC>().TakeDamage(weapon.damage);
                    this.photonView.RPC("PlayerShot", RpcTarget.All, _hit.collider.name, weapon.damage);
                }
            }
        }


        [PunRPC]
        void PlayerShot(string _playerID, int _damage)
        {
            Debug.Log(_playerID + " has been shot.");

            PlayerC _player = GameManagerC.GetPlayer(_playerID);
            _player.TakeDamage(_damage);
        }

        #endregion
    }
}
