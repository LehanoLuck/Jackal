using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerInfo : MonoBehaviour, IPunObservable,IPunCallbacks
{
    public PhotonView photonView;

    public Toggle toggleReady;

    public Text PlayerName;

    public bool isReady;

    private Hashtable customProperties;

    // Start is called before the first frame update
    void Start()
    {
        customProperties = new Hashtable();
        customProperties.Add("IsReady", isReady);
        PhotonNetwork.SetPlayerCustomProperties(customProperties);
        if (!photonView.isMine)
        {
            toggleReady.enabled = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(isReady);
        }
        else
        {
            isReady = (bool)stream.ReceiveNext();
            this.toggleReady.SetIsOnWithoutNotify(isReady);
        }
        PhotonNetwork.player.CustomProperties["IsReady"] = isReady;
    }

    public void SwitchIsReady()
    {
        if(photonView.isMine)
        {
            isReady = !isReady;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var parent = FindObjectOfType<PlayersInLobby>().transform;
        this.transform.SetParent(parent);
        this.transform.localScale = new Vector3(1, 1, 1);
        this.PlayerName.text = photonView.owner.NickName;
    }

    public void OnConnectedToPhoton()
    {
        throw new System.NotImplementedException();
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

    public void OnReceivedRoomListUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinedRoom()
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
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
