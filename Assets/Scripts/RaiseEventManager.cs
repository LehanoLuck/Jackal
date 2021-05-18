using ExitGames.Client.Photon;
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
        private const int CreateShipEvent = 3;
        private const int MovePirateEvent = 4;
        private const int SetNewQueuePlayersEvent = 5;
        private const int EndGameEvent = 6;

        private const int ShipMovementCode = 1;
        private const int PirateMovementCode = 2;

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
                PhotonPeer.RegisterType(typeof(PirateMovementData), PirateMovementCode, PirateMovementData.Serialize, PirateMovementData.Deserialize);
            }
        }

        public static void RaiseStartGameEvent(MapSettings settings)
        {
            int[][] mapMatrix = MapMatrixManager.CreateRandomGenerationMapMatrix(settings);

            PhotonNetwork.RaiseEvent(StartGameEvent, mapMatrix, raiseEventOptions, sendOptions);
        }

        public static void RaiseMoveShipEvent(ShipMovementData shipMovementData)
        {
            PhotonNetwork.RaiseEvent(MoveShipEvent, shipMovementData, raiseEventOptionsForOther, sendOptions);
        }

        public static void RaiseCreateShipEvent(ShipMovementData shipMovementData)
        {
            PhotonNetwork.RaiseEvent(CreateShipEvent, shipMovementData, raiseEventOptions, sendOptions);
        }

        public static void RaiseMovePirateEvent(PirateMovementData pirateMovementData)
        {
            PhotonNetwork.RaiseEvent(MovePirateEvent, pirateMovementData, raiseEventOptionsForOther, sendOptions);
        }

        public static void RaiseSetNewQueuePlayersEvent(int[] actorNumbers)
        {
            PhotonNetwork.RaiseEvent(SetNewQueuePlayersEvent, actorNumbers, raiseEventOptionsForOther, sendOptions);
        }

        public static void RaiseEndGameEvent(int actorNumber)
        {
            PhotonNetwork.RaiseEvent(EndGameEvent, actorNumber, raiseEventOptions, sendOptions);
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case StartGameEvent:
                    MapMatrixManager.SetGenerationMapMatrix((int[][])photonEvent.CustomData);
                    PhotonNetwork.LoadLevel("SampleScene");
                    break;
                case MoveShipEvent:
                    var shipMovementData = (ShipMovementData)photonEvent.CustomData;
                    var ship = EventMapManager.ShipsDictionary[shipMovementData.Id];
                    var tile = EventMapManager.Map[shipMovementData.XPos][shipMovementData.YPos];
                    ship.MoveOnTile(tile);
                    break;
                case CreateShipEvent:
                    shipMovementData = (ShipMovementData)photonEvent.CustomData;
                    ship = EventMapManager.ShipsDictionary[shipMovementData.Id];
                    tile = EventMapManager.Map[shipMovementData.XPos][shipMovementData.YPos];
                    ship.CreateShip(tile);
                    break;
                case MovePirateEvent:
                    var pirateMovementData = (PirateMovementData)photonEvent.CustomData;
                    ship = EventMapManager.ShipsDictionary[pirateMovementData.ShipId];
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
                case EndGameEvent:
                    var number = (int)photonEvent.CustomData;
                    EventMapManager.EndGame(number);
                    break;
            }
        }
    }
}
