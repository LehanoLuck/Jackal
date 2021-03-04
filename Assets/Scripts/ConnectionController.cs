using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;

public class ConnectionController : MonoBehaviourPunCallbacks
{
    public Text Log;

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //Когда текущий игрок (мы) покидает комнату
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            LogData(string.Format("Master {0} is here!!!", newPlayer));
        }
        else
        {
            LogData(string.Format("Player {0} entered the room", newPlayer));
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left the room", otherPlayer);
    }

    public void LogData(string message)
    {
        Log.text += message;
        Debug.LogFormat(message);
    }
}
