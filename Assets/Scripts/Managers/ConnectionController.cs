using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;

public class ConnectionController : MonoBehaviourPunCallbacks
{
    public Transform EnemyPanel;
    public GameObject Template;
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

    void Start()
    {
        foreach (var player in PhotonNetwork.PlayerListOthers)
        {
            var enemy = Instantiate(Template, EnemyPanel);
            enemy.GetComponent<GamePlayer>().SetPlayerName(player.ActorNumber);
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
