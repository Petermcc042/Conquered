using Photon.Pun;
using UnityEngine;

namespace GlassyGames.Conquered
{
    public class PlayerC : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        readonly int maxHealth = 100;

        float currentHealth;


        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int _damage)
        {
            currentHealth -= _damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            print(name + "was destroyed");
        }


        #region IPunObservable implementation

/*        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(currentHealth);
            }
            else
            {
                // Network player, recieve data
                this.currentHealth = (float)stream.ReceiveNext();
            }
        }*/

        #endregion
    }
}
