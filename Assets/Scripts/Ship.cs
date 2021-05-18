using Assets.Scripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ship : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPunInstantiateMagicCallback
{
    private Camera GameCamera;
    public BoxCollider Collider;

    private PhotonView photonView;

    public GamePlayer SelfPlayer;

    public byte Id { get; set; }

    public Pirate PirateTemplate;

    public WaterTile CurrentTile;
    public WaterTile TargetTile;

    public List<Pirate> Pirates = new List<Pirate>();
    public Dictionary<byte, Pirate> ShipPirates = new Dictionary<byte, Pirate>();

    public TrajectoryMovement trajectoryMovement;

    private bool isMyTurn => (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsMyTurn"];

    void Start()
    {
        GameCamera = Camera.main;

        photonView = GetComponent<PhotonView>();
        Collider = GetComponent<BoxCollider>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        CurrentTile.ShowAvailableForShipMoveCells();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        var groundPlane = new Plane(Vector3.up, Vector3.zero);

        Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float position))
        {
            Vector3 worldPosition = ray.GetPoint(position);

            trajectoryMovement.ShowTrajectory(worldPosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        CurrentTile.HideAvailableForShipMoveCells();
        var isCanMove = IsCanMoveOnTile(eventData);

        if (isCanMove)
        {

            var shipMovementData = new ShipMovementData
            {
                Id = this.Id,
                XPos = TargetTile.XPos,
                YPos = TargetTile.YPos
            };

            StepByStepSystem.StartNextTurn();
            RaiseEventManager.RaiseMoveShipEvent(shipMovementData);
            MoveOnTile();
        }

        this.trajectoryMovement.HideTrajectory();
    }

    private bool IsCanMoveOnTile(PointerEventData eventData)
    {
        TargetTile = GetTargetTile();

        if (TargetTile)
        {
            return CurrentTile.IsPossibleForShipMove(TargetTile);
        }
        else
        {
            return false;
        }

        WaterTile GetTargetTile()
        {
            var point = eventData.pointerEnter;
            if (point)
            {
                if (point.GetComponent<WaterTile>())
                {
                    return eventData.pointerEnter.GetComponent<WaterTile>();
                }
            }
            return null;
        }
    }

    public void MoveOnTile(Tile tile)
    {
        TargetTile = (WaterTile)tile;
        MoveOnTile();
    }

    public void MoveOnTile()
    {
        CurrentTile.LeaveShip();
        TargetTile.EnterShip(this);
    }

    public void CreateShip(Tile tile)
    {
        CurrentTile = (WaterTile)tile;
        CurrentTile.EnterShip(this);

        SelfPlayer = FindObjectOfType<GamePlayer>();

        if (photonView.IsMine)
            this.AddPirateOnShip(3);
    }

    private void AddPirateOnShip(int count)
    {
        for (int i = 0; i < count; i++)
        {
            byte pirateId = (byte)i;
            Pirate pirate = PhotonNetwork.Instantiate(PirateTemplate.name, this.transform.position, Quaternion.identity, 0, new object[] { pirateId, this.Id }).GetComponent<Pirate>();
        }
    }

    public void EnterPirate(Pirate pirate)
    {
        this.Pirates.Add(pirate);
    }

    public void LeavePirate(Pirate pirate)
    {
        this.Pirates.Remove(pirate);
    }

    public bool TryAttack(Pirate pirate)
    {
        pirate.Die();
        return false;
    }

    //public void AddCoin(Coin coin)
    //{
    //    Destroy(coin.gameObject);
    //    if (photonView.IsMine)
    //    {
    //        var value = ++this.SelfPlayer.PlayerUI.CoinsCount;
    //        this.SelfPlayer.PlayerUI.CoinsValue.text = value.ToString();
    //    }

    //    var mapManager = FindObjectOfType<MapManager>();
    //    MapMatrixManager.CoinsCount--;
    //    mapManager.Log.text = $"Coins left - {MapMatrixManager.CoinsCount}";

    //    if (EndGameManager.IsWin(VictoryCondition.CollectMinimumCoins, SelfPlayer))
    //    {
    //        RaiseEventManager.RaiseEndGameEvent(PhotonNetwork.LocalPlayer.ActorNumber);
    //    }

    //    //Создать новый ивент
    //    //if (SelfPlayer.PlayerUI.CoinsCount >= (double)MapMatrixManager.StartCoinsCount / (double)PhotonNetwork.CurrentRoom.PlayerCount)
    //    //{
    //    //    mapManager.Log.fontSize = 30;
    //    //    mapManager.Log.text = $"{PhotonNetwork.LocalPlayer.NickName} is Win!!!";
    //    //    var controller = FindObjectOfType<ConnectionController>();
    //    //    controller.LeaveRoom();
    //    //}
    //}

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var mapManger = FindObjectOfType<MapManager>();
        var data = info.photonView.InstantiationData;
        this.Id = (byte)data[0];

        mapManger.AddShip(this);
    }
}
