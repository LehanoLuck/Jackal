using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersInLobby : MonoBehaviour, IOnEventCallback
{
    private const byte StartGameEvent = 1;

    public GameObject playerInfo;
    public GameObject startGameButton;

    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
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
        if (PhotonNetwork.PlayerList.Length < 2)
            return;

        var readyPlayers = PhotonNetwork.PlayerList.Where(p => (bool)p.CustomProperties["IsReady"]);
        var count = readyPlayers.Count();

        if (count == PhotonNetwork.CountOfPlayers)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            var raiseCode = PhotonNetwork.RaiseEvent(StartGameEvent, true, raiseEventOptions, sendOptions);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        switch(photonEvent.Code)
        {
            case StartGameEvent:
                PhotonNetwork.LoadLevel("SampleScene");
                break;
        }    
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
