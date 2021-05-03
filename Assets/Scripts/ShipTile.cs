using Assets.Scripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipTile : BasicTile, IDragHandler, IBeginDragHandler, IEndDragHandler, IPunInstantiateMagicCallback
{
    private Camera GameCamera;

    private PhotonView photonView;

    public byte Id { get; set; }
    public Pirate PirateTemplate;

    public GamePlayer SelfPlayer;
    private BasicTile tempTile;
    public BoxCollider Collider;
    public bool isInFreeSpace = false;

    public List<Pirate> ShipPirates = new List<Pirate>();

    private bool isMyTurn => (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsMyTurn"];

    void Start()
    {
        GameCamera = Camera.main;
        Collider = GetComponent<BoxCollider>();

        photonView = GetComponent<PhotonView>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        Collider.enabled = false;

        //Отключаем колайдеры чтобы рейкаст не попадал на модельки, из-за чего ломается перемещение
        foreach (Pirate pirate in Pirates)
        {
            pirate.Collider.enabled = false;
        }
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
            worldPosition.y += 1.5f;

            transform.position = worldPosition;

            //Заменить каждый фрейм вычисляется положение для каждого  пирата по гипер стрёмной формуле
            PlacePirateOnTile();
        }

        //Получение временной ячейки, костыль чтобы запомнить ячейку при отпускании пирата
        if (eventData.pointerEnter && eventData.pointerEnter.GetComponent<BasicTile>())
        {
            tempTile = eventData.pointerEnter.GetComponent<BasicTile>();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        Collider.enabled = true;
        if (eventData.pointerEnter && eventData.pointerEnter.GetComponent<WaterTile>())
        {
            if (TryReplace(tempTile))
            {
                StepByStepSystem.StartNextTurn();
                RaiseEventManager.RaiseReplaceShipEvent(
                new ShipMovementData
                {
                    Id = Id,
                    XPos = tempTile.XPos,
                    YPos = tempTile.YPos
                });
                this.Replace(tempTile);
            }
            else
            {
                this.SetTransformPosition(this.fixedPosition);
            }
        }
        else
        {
            this.SetTransformPosition(this.fixedPosition);
        }

        PlacePirateOnTile();

        foreach (Pirate pirate in Pirates)
        {
            pirate.Collider.enabled = true;
        }
    }

    private bool TryReplace(BasicTile tile)
    {
        return (Mathf.Abs(XPos - tile.XPos) < 2 &&
            Mathf.Abs(YPos - tile.YPos) < 2);
    }

    public void Replace(Tile tile)
    {
        byte i = tile.XPos;
        byte j = tile.YPos;

        tile.XPos = this.XPos;
        tile.YPos = this.YPos;
        Map[this.XPos][this.YPos] = tile;
        Map[i][j] = this;

        this.XPos = i;
        this.YPos = j;

        var tempPos = this.fixedPosition;
        this.SetTransformPosition(tile.fixedPosition);
        tile.SetTransformPosition(tempPos);
    }

    public void Move(Tile tile)
    {
        this.XPos = tile.XPos;
        this.YPos = tile.YPos;
        this.Map = tile.Map;
        Map[tile.XPos][tile.YPos] = this;

        this.SetTransformPosition(tile.fixedPosition);
        Destroy(tile.gameObject);
        isInFreeSpace = false;
        Collider.enabled = true;

        SelfPlayer = FindObjectOfType<GamePlayer>();

        if(photonView.IsMine)
            this.AddPirateOnTile(3);
    }

    private void AddPirateOnTile(int count)
    {
        for(int i = 0; i < count; i++)
        {
            byte pirateId = (byte)this.ShipPirates.Count;
            Pirate pirate = PhotonNetwork.Instantiate(PirateTemplate.name, this.transform.position, Quaternion.identity, 0, new object[] { pirateId, this.Id }).GetComponent<Pirate>();
            pirate.SelfPlayer = this.SelfPlayer;
        }
    }

    public override void EnterPirate(Pirate pirate)
    {
        base.EnterPirate(pirate);
    }

    public bool TryAttack(Pirate pirate)
    {
        pirate.Die();
        return false;
    }

    public void AddCoin(Coin coin)
    {
        Destroy(coin.gameObject);
        if (photonView.IsMine)
        {
            var value = ++this.SelfPlayer.PlayerUI.CoinsCount;
            this.SelfPlayer.PlayerUI.CoinsValue.text = value.ToString();
        }

        var mapManager = FindObjectOfType<MapManager>();
        MapMatrixManager.CoinsCount--;
        mapManager.Log.text = $"Coins left - {MapMatrixManager.CoinsCount}";

        if (EndGameManager.IsWin(VictoryCondition.CollectMinimumCoins, SelfPlayer))
        {
            RaiseEventManager.RaiseEndGameEvent(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        //Создать новый ивент
        //if (SelfPlayer.PlayerUI.CoinsCount >= (double)MapMatrixManager.StartCoinsCount / (double)PhotonNetwork.CurrentRoom.PlayerCount)
        //{
        //    mapManager.Log.fontSize = 30;
        //    mapManager.Log.text = $"{PhotonNetwork.LocalPlayer.NickName} is Win!!!";
        //    var controller = FindObjectOfType<ConnectionController>();
        //    controller.LeaveRoom();
        //}
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var mapManger = FindObjectOfType<MapManager>();
        var data = info.photonView.InstantiationData;
        this.Id = (byte)data[0];

        mapManger.ShipTiles.Add(Id, this);
    }
}
