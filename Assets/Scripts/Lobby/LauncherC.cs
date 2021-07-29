using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace GlassyGames.Conquered
{
    public class LauncherC: MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;
        [Tooltip("The UI panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI panel to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progresslabel;
        [SerializeField]
        public GameObject connectButton;


        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon
        /// we need to keep track of this to properly adjust the behaviour when we receive call back by Photon
        /// typically this is used for the OnConnectedToMaster() callback
        /// </summary>
        bool isConnecting;


        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion, which allows you to make breaking changes
        /// </summary>
        readonly string gameVersion = "1";

        #endregion

        #region MonoBehaviour Callbacks

        /// <summary>
        /// MonoBehaviour method called on Gameobject by Unity during early Initialisations
        /// </summary>
        private void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;            
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by unity during initialisation phase
        /// </summary>
        private void Start()
        {
            progresslabel.SetActive(false);
            controlPanel.SetActive(false);

            PhotonNetwork.ConnectUsingSettings(); // connects to Master photon server
            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process
        ///  - If already connected, we attempt joining a random room
        ///  - If not yet connected, connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            progresslabel.SetActive(true);
            controlPanel.SetActive(false);


            // we check if we are connected or not, we join if we are, else we initiate the connection to the server
            if (PhotonNetwork.IsConnected)
            {
                // #Critical, we need at this point to attempt joining a random room. If it fails we will get notified in OnJoinRandomFailed() and we'll create one
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first connect to the Photon Online Server
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("Launcher: Player has connected to Photon Servers");

            // we don't want to do anything if we are not attempting to join a room
            //this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything
            if (isConnecting)
            {
                // #Critical: First we try to do is to join a potential existing room. If there is, good, else we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("Launcher: OnDisconnected was called by PUN");

            progresslabel.SetActive(false);
            controlPanel.SetActive(true);

            isConnecting = false;

        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Launcher: OnJoinRandomFailed() was called by PUN. No random room available, so we create one. \nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are full. We create a new room
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Launcher: OnJoinedRoom() was called by PUN. Now this client is in a room.");

            // #Critical: We only load if we are the first player, else we rely on PhotonNetwork.AutomaticallySyncScene to sync our instance
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                // #Critical
                // Load the Room level
                PhotonNetwork.LoadLevel("Room for 1");

            }
        }

        #endregion
    }
}