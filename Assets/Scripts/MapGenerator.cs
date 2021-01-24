using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int[][] GenerationMapMatrix;
    public GroundTile ClosedTileTemplate;
    public GroundTile GroundTileTemplate;
    public WaterTile WaterTileTemplate;
    public ShipTile ShipTileTemplate;
    public CoinTile CoinTileTemplate;

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
    }

    public void GenerateMap()
    {
        this.Map = new BaseTile[GenerationMapMatrix.Length][];
        for (int i = 0; i < GenerationMapMatrix.Length; i++)
        {
            Map[i] = new BaseTile[GenerationMapMatrix[i].Length];
            for (int j = 0; j < GenerationMapMatrix[i].Length; j++)
            {
                CreateTile(i, j);
            }
        }
    }

    private void GenerateMatrix()
    {
        //Map = new BaseTile[Width][];

        //if (coinsCount > groundTilesCount)
        //    coinsCount = groundTilesCount;

        //for (int i = 0; i < coinsCount; i++)
        //{
        //    TilesList.Add(CoinTileTemplate);
        //}

        //for (int i = 0; i < groundTilesCount - coinsCount; i++)
        //{
        //    TilesList.Add(GroundTileTemplate);
        //}

        //TilesList = TilesList.Shuffle();

        //for (int i = 0; i < this.Width; i++)
        //{
        //    Map[i] = new BaseTile[Length];
        //    for (int j = 0; j < this.Length; j++)
        //    {
        //        if (i < waterWidth || i >= Width - waterWidth || j < waterLength || j >= Length - waterLength)
        //        {
        //            CreateWaterTile(i, j);
        //        }
        //        else
        //        {
        //            CreateGroundTile(i, j, TilesList[0]);
        //            TilesList.Remove(TilesList[0]);
        //        }
        //    }
        //}
    }

    private void PlaceTile(BaseTile tile, int i, int j)
    {
        float tileXSize = ClosedTileTemplate.transform.localScale.x * 3.2f;
        float tileYSize = ClosedTileTemplate.transform.localScale.y * 3.2f;

        tile.HorizontalIndex = i;
        tile.VerticalIndex = j;
        tile.SetTransformPosition(new Vector3(tileXSize * i, 0, tileYSize * j));
        Map[i][j] = tile;
        tile.Map = this.Map;
    }

    private void CreateTile(int i, int j)
    {
        var tile = TilesTable[GenerationMapMatrix[i][j]];
        if (tile is GroundTile)
            CreateGroundTile(i, j, tile as GroundTile);
        else
            CreateWaterTile(i, j);
    }

    private void CreateGroundTile(int i, int j, GroundTile groundTile)
    {
        GroundTile tile = Instantiate(ClosedTileTemplate, this.transform);

        PlaceTile(tile, i, j);
        tile.HiddenTile = groundTile;
        tile.isHidden = false;
    }

    private void CreateWaterTile(int i, int j)
    {
        WaterTile tile = Instantiate(WaterTileTemplate, this.transform);
        PlaceTile(tile, i, j);
    }
}
