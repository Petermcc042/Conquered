using UnityEngine;
using Photon.Pun;

namespace GlassyGames.Conquered
{
    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManagerC : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Variables

        bool IsFiring;

        public float maxHealth = 1f;
        public float currentHealth = 1f;

        [Tooltip("The local player instance. Uses this to know if the local player is represented in the scene")]
        public static GameObject LocalPlayerInstance;


        #endregion


        #region MonoBehaviour CallBacks

        void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronised
            if (photonView.IsMine)
            {
                LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronisation, thus giving a seamless experience when levels load
            DontDestroyOnLoad(this.gameObject);
        }


        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {
            if (photonView.IsMine)
            {
                if (currentHealth <= 0f)
                {
                    GameManagerC.Instance.LeaveRoom();
                }
            }
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }


        void CalledOnLevelWasLoaded(int level)
        {
            //check if we are outside the Arena and if it's the case, spawn around the centre of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 10f, 0f);
            }
        }

        public override void OnDisable()
        {
            // always call the base to remove callbacks
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion


        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(IsFiring);
                stream.SendNext(currentHealth);
            }
            else
            {
                // Network player, recieve data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.currentHealth = (float)stream.ReceiveNext();
            }
        }

        #endregion


    }
}
