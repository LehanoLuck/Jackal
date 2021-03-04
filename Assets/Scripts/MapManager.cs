using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public CameraMovement camera;
    public int[][] GenerationMapMatrix;
    public GroundTile ClosedTileTemplate;
    public GroundTile GroundTileTemplate;
    public WaterTile WaterTileTemplate;
    public CoinTile CoinTileTemplate;
    public ShipTile ShipTileTemplate;

    public Dictionary<int, ShipTile> ShipTiles = new Dictionary<int, ShipTile>();

    public BaseTile[][] Map;
    private Hashtable TilesTable;
    public List<ShipTile> ships = new List<ShipTile>();
    public Text Log;
        
    void Start()
    {
        TilesTable = new Hashtable();
        TilesTable.Add(1, GroundTileTemplate);
        TilesTable.Add(2, CoinTileTemplate);
        TilesTable.Add(0, WaterTileTemplate);

        GenerationMapMatrix = MapMatrixManager.GenerationMapMatrix;
        GenerateMap();

        Log.text = $"Coins left - {MapMatrixManager.CoinsCount}";
        RaiseEventManager.EventMapManager = this;
    }

    public void GenerateMap()
    {
        this.Map = new BaseTile[GenerationMapMatrix.Length][];
        for (byte i = 0; i < GenerationMapMatrix.Length; i++)
        {
            Map[i] = new BaseTile[GenerationMapMatrix[i].Length];
            for (byte j = 0; j < GenerationMapMatrix[i].Length; j++)
            {
                CreateTile(i, j);
            }
        }

        var length = Map.Length * 3.2f;
        var width = Map[0].Length * 3.2f;
        var zShift = 0f; //Добавочная длина, если длина больше ширины

        var xPos = length / 2 - 1.6f;

        if (length > width)
        {
            zShift = -(length - width) / 2;
            width = length;
        }

        var zDistance = (width / (1 - Mathf.Tan(Mathf.Deg2Rad * camera.longPlaneAngle) / Mathf.Tan(Mathf.Deg2Rad * camera.shortPlaneAngle)));
        var yPos = Mathf.Tan(Mathf.Deg2Rad * camera.longPlaneAngle) * zDistance;
        var zPos = (width - 2.4f) - zDistance + zShift;

        camera.maxHeight = yPos;
        camera.startPosition = new Vector3(xPos, yPos, zPos);
        camera.SetPositionToStart();
        camera.centerMapPoint = new Vector3(xPos, 0f, width / 2 - 1.6f);
    }

    private void PlaceTile(BaseTile tile, byte i, byte j)
    {
        float tileXSize = ClosedTileTemplate.transform.localScale.x * 3.2f;
        float tileYSize = ClosedTileTemplate.transform.localScale.y * 3.2f;

        tile.HorizontalIndex = i;
        tile.VerticalIndex = j;
        tile.SetTransformPosition(new Vector3(tileXSize * i, 0, tileYSize * j));
        Map[i][j] = tile;
        tile.Map = this.Map;
    }

    private void CreateTile(byte i, byte j)
    {
        var tile = TilesTable[GenerationMapMatrix[i][j]];
        if (tile is GroundTile)
            CreateGroundTile(i, j, tile as GroundTile);
        else
            CreateWaterTile(i, j);
    }

    private void CreateGroundTile(byte i, byte j, GroundTile groundTile)
    {
        GroundTile tile = Instantiate(ClosedTileTemplate, this.transform);

        PlaceTile(tile, i, j);
        tile.HiddenTile = groundTile;
        tile.isHidden = false;
    }

    private void CreateWaterTile(byte i, byte j)
    {
        WaterTile tile = Instantiate(WaterTileTemplate, this.transform);
        PlaceTile(tile, i, j);
    }

    public void AddShipTile(ShipTile ship)
    {
        byte id = (byte)ShipTiles.Count;
        ShipTiles.Add(id, ship);
        ship.Id = id;
    }

    public void EndGame(int actorNumber)
    {
        StartCoroutine(EndGameCoroutine(actorNumber));
    }

    private IEnumerator EndGameCoroutine(int actorNumber)
    {
        var winner = PhotonNetwork.CurrentRoom.Players.Values.FirstOrDefault(p => p.ActorNumber == actorNumber);
        Log.fontSize = 30;
        Log.text = $"{winner.NickName} is Win!!!";

        yield return new WaitForSeconds(3);

        var controller = FindObjectOfType<ConnectionController>();
        controller.LeaveRoom();
    }
}
