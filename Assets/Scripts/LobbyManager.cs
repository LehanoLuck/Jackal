using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Assets.Scripts;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField LobbyName;

    public GameObject LobbyModal;

    public Text LogText;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = "Player " + Random.Range(1000, 9999);

        Log("Player's name is set to " + PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.ConnectUsingSettings();

        RaiseEventManager.ActivateCallbacks();
        //PhotonNetwork.ConnectToRegion("ru");
    }

    private void Log(string message)
    {
        Debug.Log(message);
        LogText.text += "\n";
        LogText.text += message;
    }

    public void ShowCreateLobbyModalDialog()
    {
        LobbyModal.SetActive(true);
    }

    public void JoinRoom()
    {
        JoinRoomByName(LobbyName.text);
        LobbyModal.SetActive(false);
    }

    public void JoinRoomByName(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 3;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public override void OnCreatedRoom()
    {
        Log($"Lobby with name {LobbyName.text} has been created");
    }

    public override void OnJoinedRoom()
    {
        Log($"Joined in lobby with name {LobbyName.text}");
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log("Failed to Join Room");
    }

    public override void OnConnected()
    {
        Log("Connected to Photon!");
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected to Master!");
    }
}
