using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using Photon.Realtime;

public class StepByStepSystem : MonoBehaviour
{
    public static Queue<Player> Players;
    public static Player CurrentPlayer;
    private static Hashtable CustomProperties;

    public static void SetPlayers(IEnumerable<Player> players)
    {
        var list = players.ToList().Shuffle();
        Players = new Queue<Player>(list);
        CurrentPlayer = Players.Peek();
        
        foreach(var player in players)
        {
            CustomProperties = new Hashtable();

            if (player != CurrentPlayer)
            {
                CustomProperties.Add("IsMyTurn", false);
            }
            else
            {
                CustomProperties.Add("IsMyTurn", true);
            }

            player.SetCustomProperties(CustomProperties);
        }
        SendNewQueue();
    }

    public static string StartNextTurn()
    {
        var previousPlayer = Players.Dequeue();
        Players.Enqueue(previousPlayer);
        CurrentPlayer = Players.Peek();

        CustomProperties = previousPlayer.CustomProperties;
        CustomProperties["IsMyTurn"] = false;
        previousPlayer.SetCustomProperties(CustomProperties);

        CustomProperties = CurrentPlayer.CustomProperties;
        CustomProperties["IsMyTurn"] = true;
        CurrentPlayer.SetCustomProperties(CustomProperties);

        SendNewQueue();
        return CurrentPlayer.NickName;
    }

    public static void SendNewQueue()
    {
        var actorNumbers = Players.Select(a => a.ActorNumber).ToArray();
        RaiseEventManager.RaiseSetNewQueuePlayersEvent(actorNumbers);
    }
}
