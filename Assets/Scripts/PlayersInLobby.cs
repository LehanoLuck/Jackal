using Assets.Scripts;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class PlayersInLobby : MonoBehaviour
{
    public GameObject playerInfo;
    public Toggle IsUseDefaultSettings;
    public GenerationSettings Settings;
    public GameObject MasterPanel;
    public GameObject GenerationMapSettings;

    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            MasterPanel.SetActive(true);

        AddPlayerInLobby();
        //var a = PhotonPeer.RegisterType(typeof(int[,]), 242, SerealizeMatrix, DeserializeMatrix);
        
    }

    public void AddPlayerInLobby()
    {
        PhotonNetwork.Instantiate(playerInfo.name, parent.position, Quaternion.identity, 0);
    }

    public void StartGame()
    {
        //if (PhotonNetwork.PlayerList.Length < 2)
        //    return;

        var readyPlayers = PhotonNetwork.PlayerList.Where(p => (bool)p.CustomProperties["IsReady"]);
        var count = readyPlayers.Count();

        if (count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            MapSettings mapSettings;
            if (IsUseDefaultSettings.isOn)
            {
                mapSettings = MapSettings.Default;
            }
            else
            {
                byte width = Convert.ToByte(Settings.mapWidth.value);
                byte length = Convert.ToByte(Settings.mapLength.value);
                byte coins = Convert.ToByte(Settings.coinsCount.value);
                byte ground = Convert.ToByte(Settings.groundCount.value);
                mapSettings = new MapSettings(width, length, coins, ground);
            }
            StepByStepSystem.SetPlayers(readyPlayers);
            RaiseEventManager.RaiseStartGameEvent(mapSettings);
        }
    }

    public void ChangeMapSettings()
    {
        GenerationMapSettings.SetActive(!IsUseDefaultSettings.isOn);
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
