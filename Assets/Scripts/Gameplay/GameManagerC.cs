using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections.Generic;
using System;

namespace GlassyGames.Conquered
{
    public class GameManagerC: MonoBehaviourPunCallbacks
    {
        #region Variables

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public static GameManagerC Instance;

        private static Dictionary<string, PlayerC> players = new Dictionary<string, PlayerC>();

        private const string PLAYER_PREFIX = "Player ";

        #endregion

        #region Public Methods

        private void Start()
        {
            Instance = this;

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager' ", this);
            }
            else
            {
                if (PlayerManagerC.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are instantiating LocalPlayer from {0}", SceneManager.GetActiveScene().name);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion


        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }


        /// <summary>
        /// Called when the local player has left the room. We need to load the launcher scene 
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("GameManager: Trying to load a level but we are not the master client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        // when a player joins they are assigned in photon an id 
        public static void RegisterPlayer(string _netID, PlayerC _player)
        {
            string _playerID = PLAYER_PREFIX + _netID;
            players.Add(_playerID, _player);
            _player.transform.name = _playerID;
        }

        public static void UnregisterPlayer(string _playerID)
        {
            players.Remove(_playerID);
        }

        public static PlayerC GetPlayer(string _playerID)
        {
            return players[_playerID];
        }

/*        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(200, 200, 200, 500));
            GUILayout.BeginVertical();

            foreach(string _playerID in players.Keys)
            {
                GUILayout.Label(_playerID + "  =  " + players[_playerID].transform.name);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }*/

        #endregion


    }
}

