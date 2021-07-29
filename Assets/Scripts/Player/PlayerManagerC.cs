using UnityEngine;
using Photon.Pun;

namespace GlassyGames.Conquered
{
    /// <summary>
    /// Handles scene management.
    /// </summary>
    [RequireComponent(typeof(PlayerC))]
    public class PlayerManagerC : MonoBehaviourPunCallbacks
    {
        #region Variables

        [Tooltip("The local player instance. Uses this to know if the local player is represented in the scene")]
        public static GameObject LocalPlayerInstance;

        [SerializeField]
        Behaviour[] componentsToDisable;
        [SerializeField]
        string remoteLayerName = "RemotePlayer";

        public float maxHealth = 1f;
        public float currentHealth = 1f;


        Camera sceneCamera;

        #endregion


        #region MonoBehaviour CallBacks

        void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronised
            if (this.photonView.IsMine)
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

            if (!photonView.IsMine)
            {
                DisableComponents();
                AssignRemoteLayer();
            }
            else
            {
                // if we find a scene camera while in the game disable it
                sceneCamera = Camera.main;
                if (sceneCamera != null)
                {
                    sceneCamera.gameObject.SetActive(false);
                }
            }

            if (PhotonNetwork.IsConnected == true)
            {
                string _netID = photonView.InstantiationId.ToString();
                PlayerC _player = GetComponent<PlayerC>();

                GameManagerC.RegisterPlayer(_netID, _player);
            }
        }


        public override void OnDisable()
        {
            // always call the base to remove callbacks
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(true);
            }

            GameManagerC.UnregisterPlayer(transform.name);
        }

        #endregion

        void DisableComponents()
        {
            //disabling other players components so we do not access them
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }

        void AssignRemoteLayer()
        {
            gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
        }


        #region scene management

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

        #endregion




    }
}
