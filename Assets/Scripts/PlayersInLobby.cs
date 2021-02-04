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

public class PlayersInLobby : MonoBehaviour
{

    public GameObject playerInfo;
    public GameObject startGameButton;

    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            startGameButton.SetActive(true);

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
            StepByStepSystem.SetPlayers(readyPlayers);
            RaiseEventManager.RaiseStartGameEvent();
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

    //public static object DeserializeMatrix(byte[] data)
    //{
    //    byte width = data[data.Length - 2];
    //    byte length = data[data.Length - 1];

    //    int[,] matrix = new int[width, length];

    //    for (int i = 0; i < matrix.GetLength(0); i++)
    //    {
    //        for (int j = 0; j < matrix.GetLength(1); j++)
    //        {
    //            matrix[i, j] = BitConverter.ToInt32(data, (i * matrix.GetLength(1) + j) * 4);
    //        }
    //    }

    //    return matrix;
    //}

    //public static byte[] SerealizeMatrix(object obj)
    //{
    //    var matrix = obj as int[,];

    //    byte[] array = new byte[matrix.Length * 4 + 2];

    //    int i = 0;
    //    foreach (var value in matrix)
    //    {
    //        BitConverter.GetBytes(value).CopyTo(array, i);
    //        i += 4;
    //    }
    //    array[i++] = Convert.ToByte(matrix.GetLength(0));
    //    array[i++] = Convert.ToByte(matrix.GetLength(1));
    //    return array;
    //}
}
