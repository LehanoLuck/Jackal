﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class RaiseEventManager: IOnEventCallback
    {
        public static MapManager EventMapManager;
        public static ShipManager EventShipManager;
        private static RaiseEventManager EventManager { get; set; }

        private const int StartGameEvent = 1;
        private const int MoveShipEvent = 2;
        private const int ReplaceShipEvent = 3;
        private const int MovePirateEvent = 4;
        private const int SetNewQueuePlayersEvent = 5;

        private const int ShipMovementCode = 1;
        private const int PirateMovementCode = 2;
        private const int MovementSettingsCode = 3;

        private static RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        private static RaiseEventOptions raiseEventOptionsForOther = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        private static SendOptions sendOptions = new SendOptions { Reliability = true };

        public static void ActivateCallbacks()
        {
            if (EventManager == null)
            {
                EventManager = new RaiseEventManager();
                PhotonNetwork.AddCallbackTarget(EventManager);

                PhotonPeer.RegisterType(typeof(ShipMovementData), ShipMovementCode, ShipMovementData.Serialize, ShipMovementData.Deserialize);
                PhotonPeer.RegisterType(typeof(MovementSettings), MovementSettingsCode, MovementSettings.Serialize, MovementSettings.Deserialize);
                PhotonPeer.RegisterType(typeof(PirateMovementData), PirateMovementCode, PirateMovementData.Serialize, PirateMovementData.Deserialize);
            }
        }

        public static void RaiseStartGameEvent()
        {
            int[][] mapMatrix = MapMatrixManager.CreateRandomGenerationMapMatrix(MapSettings.Default);

            PhotonNetwork.RaiseEvent(StartGameEvent, mapMatrix, raiseEventOptions, sendOptions);
        }

        public static void RaiseMoveShipEvent(ShipMovementData shipMovementData)
        {
            PhotonNetwork.RaiseEvent(MoveShipEvent, shipMovementData, raiseEventOptions, sendOptions);
        }

        public static void RaiseReplaceShipEvent(ShipMovementData shipMovementData)
        {
            PhotonNetwork.RaiseEvent(ReplaceShipEvent, shipMovementData, raiseEventOptionsForOther, sendOptions);
        }

        public static void RaiseMovePirateEvent(PirateMovementData pirateMovementData)
        {
            PhotonNetwork.RaiseEvent(MovePirateEvent, pirateMovementData, raiseEventOptionsForOther, sendOptions);
        }

        public static void RaiseSetNewQueuePlayersEvent(int[] actorNumbers)
        {
            PhotonNetwork.RaiseEvent(SetNewQueuePlayersEvent, actorNumbers, raiseEventOptionsForOther, sendOptions);
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case StartGameEvent:
                    MapMatrixManager.GenerationMapMatrix = (int[][])photonEvent.CustomData;
                    PhotonNetwork.LoadLevel("SampleScene");
                    break;
                case MoveShipEvent:
                    var shipMovementData = (ShipMovementData)photonEvent.CustomData;
                    var ship = EventMapManager.ShipTiles[shipMovementData.Id];
                    var tile = EventMapManager.Map[shipMovementData.XPos][shipMovementData.YPos];
                    ship.Move(tile);
                    break;
                case ReplaceShipEvent:
                    shipMovementData = (ShipMovementData)photonEvent.CustomData;
                    ship = EventMapManager.ShipTiles[shipMovementData.Id];
                    tile = EventMapManager.Map[shipMovementData.XPos][shipMovementData.YPos];
                    ship.Replace(tile);
                    break;
                case MovePirateEvent:
                    var pirateMovementData = (PirateMovementData)photonEvent.CustomData;
                    ship = EventMapManager.ShipTiles[pirateMovementData.ShipId];
                    var pirate = ship.ShipPirates[pirateMovementData.Id];
                    tile = EventMapManager.Map[pirateMovementData.XPos][pirateMovementData.YPos];
                    pirate.MoveOnTile(pirateMovementData, tile);
                    break;
                case SetNewQueuePlayersEvent:
                    var actorNumbers = (int[])photonEvent.CustomData;

                    List<Player> players = new List<Player>();
                    foreach(int actorNumber in actorNumbers)
                    {
                        var player = PhotonNetwork.PlayerList.First(p => p.ActorNumber == actorNumber);
                        players.Add(player);
                    }

                    StepByStepSystem.Players = new Queue<Player>(players);
                    break;
            }
        }
    }
}