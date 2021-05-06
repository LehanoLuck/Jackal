using Assets.Scripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ship : MonoBehaviour
{
    private Camera GameCamera;

    private PhotonView photonView;

    public byte Id { get; set; }

    public Pirate PirateTemplate;

    public WaterTile CurrentTile;
    public WaterTile TargetTile;
    public BoxCollider Collider;

    private Vector3 StartPosition;

    public List<Pirate> Pirates = new List<Pirate>();

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

        //ќтключаем колайдеры чтобы рейкаст не попадал на модельки, из-за чего ломаетс€ перемещение
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
        }

        //ѕолучение временной €чейки, костыль чтобы запомнить €чейку при отпускании корабл€
        if (eventData.pointerEnter && eventData.pointerEnter.GetComponent<WaterTile>())
        {
            TargetTile = eventData.pointerEnter.GetComponent<WaterTile>();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!photonView.IsMine || !isMyTurn)
            return;

        Collider.enabled = true;
        if (eventData.pointerEnter && eventData.pointerEnter.GetComponent<WaterTile>())
        {
            if (TryReplace(TargetTile))
            {
                StepByStepSystem.StartNextTurn();
                RaiseEventManager.RaiseReplaceShipEvent(
                new ShipMovementData
                {
                    Id = Id,
                    XPos = TargetTile.XPos,
                    YPos = TargetTile.YPos
                });
                this.Replace(TargetTile);
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

    private bool TryReplace(WaterTile tile)
    {
        return (Mathf.Abs(CurrentTile.XPos - tile.XPos) < 2 &&
            Mathf.Abs(TargetTile.XPos - tile.YPos) < 2);
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

        if (photonView.IsMine)
            this.AddPirateOnTile(3);
    }

    private void AddPirateOnTile(int count)
    {
        for (int i = 0; i < count; i++)
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

        //—оздать новый ивент
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
