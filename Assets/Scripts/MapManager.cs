using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MapManager : MonoBehaviour
{
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

    void Start()
    {
        TilesTable = new Hashtable();
        TilesTable.Add(1, GroundTileTemplate);
        TilesTable.Add(2, CoinTileTemplate);
        TilesTable.Add(0, WaterTileTemplate);

        GenerationMapMatrix = MapMatrixManager.GenerationMapMatrix;
        GenerateMap();

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
}
