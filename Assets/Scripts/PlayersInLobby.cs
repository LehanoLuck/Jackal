using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;

public class PlayersInLobby : MonoBehaviour
{
    public GameObject playerInfo;
    public GameObject startGameButton;

    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.isMasterClient)
            startGameButton.SetActive(true);

        AddPlayerInLobby();
    }

    public void AddPlayerInLobby()
    {
        PhotonNetwork.Instantiate(playerInfo.name, parent.position, Quaternion.identity, 0);
    }

    [System.Obsolete]
    public void StartGame()
    {
        if (PhotonNetwork.playerList.Length < 2)
            return;

        var readyPlayers = PhotonNetwork.playerList.Where(p => (bool)p.customProperties["IsReady"]);
        var count = readyPlayers.Count();
        var photonCount = PhotonNetwork.playerList.Count();
        if (count == PhotonNetwork.countOfPlayers)
        {
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }
}
