using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace GlassyGames.Conquered
{
    /// <summary>
    /// Player name input field. let the user input their name, will appear above the player in the game
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputFieldC : MonoBehaviour
    {
        #region Private Constants

        // store the PlayerPref key to avoid typos
        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region Monobehaviour Callbacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity dureing initialisation phase
        /// </summary>
		void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();

            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}