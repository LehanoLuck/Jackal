using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviour, IPunCallbacks
{
    public Text LogText;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.playerName = "Player " + Random.Range(1000, 9999);

        Log("Player's name is set to " + PhotonNetwork.playerName);
        PhotonNetwork.ConnectUsingSettings("1.0.");
    }

    private void Log(string message)
    {
        Debug.Log(message);
        LogText.text += "\n";
        LogText.text += message;
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("Room " + Random.Range(1000,9999));
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnConnectedToPhoton()
    {
        Log("Connected to Photon!");
    }

    public void OnLeftRoom()
    {
        throw new System.NotImplementedException();
    }

    public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        throw new System.NotImplementedException();
    }

    public void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        throw new System.NotImplementedException();
    }

    public void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        throw new System.NotImplementedException();
    }

    public void OnCreatedRoom()
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinedLobby()
    {
        throw new System.NotImplementedException();
    }

    public void OnLeftLobby()
    {
        throw new System.NotImplementedException();
    }

    public void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        throw new System.NotImplementedException();
    }

    public void OnConnectionFail(DisconnectCause cause)
    {
        throw new System.NotImplementedException();
    }

    public void OnDisconnectedFromPhoton()
    {
        throw new System.NotImplementedException();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }

    public void OnReceivedRoomListUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinedRoom()
    {
        Log("Join the Room");
        PhotonNetwork.LoadLevel("SampleScene");
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        throw new System.NotImplementedException();
    }

    public void OnConnectedToMaster()
    {
        Log("Connected to Master!");
    }

    public void OnPhotonMaxCccuReached()
    {
        throw new System.NotImplementedException();
    }

    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        throw new System.NotImplementedException();
    }

    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdatedFriendList()
    {
        throw new System.NotImplementedException();
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        throw new System.NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        throw new System.NotImplementedException();
    }

    public void OnWebRpcResponse(OperationResponse response)
    {
        throw new System.NotImplementedException();
    }

    public void OnOwnershipRequest(object[] viewAndPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnLobbyStatisticsUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnOwnershipTransfered(object[] viewAndPlayers)
    {
        throw new System.NotImplementedException();
    }
}
