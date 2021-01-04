using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GroundTile ClosedTileTemplate;
    public GroundTile GroundTileTemplate;
    public WaterTile WaterTileTemplate;
    public ShipTile ShipTileTemplate;
    public CoinTile CoinTileTemplate;
    public int Width;
    public int Length;
    public int waterWidth = 1;
    public int waterLength = 1;
    public int coinsCount = 5;

    private int groundTilesCount;

    public BaseTile[][] Map;

    private List<GroundTile> TilesList = new List<GroundTile>();
    // Start is called before the first frame update
    void Start()
    {
        groundTilesCount = (Width - waterWidth * 2) * (Length - waterLength * 2);
        GenerateMatrix();
    }

    private void GenerateMatrix()
    {
        Map = new BaseTile[Width][];

        if (coinsCount > groundTilesCount)
            coinsCount = groundTilesCount;

        for(int i = 0; i < coinsCount; i++)
        {
            TilesList.Add(CoinTileTemplate);
        }
        for (int i = 0; i < groundTilesCount - coinsCount; i++)
        {
            TilesList.Add(GroundTileTemplate);
        }

        TilesList = TilesList.Shuffle();

        for (int i = 0; i < this.Width; i++)
        {
            Map[i] = new BaseTile[Length];
            for (int j = 0; j < this.Length; j++)
            {
                if (i < waterWidth || i >= Width - waterWidth || j < waterLength || j >= Length - waterLength)
                {
                    CreateWaterTile(i, j);
                }
                else
                {
                    CreateGroundTile(i, j, TilesList[0]);
                    TilesList.Remove(TilesList[0]);
                }
            }
        }
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

    //public void CreateShipTile(int i, int j)
    //{
    //    ShipTile tile = Instantiate(ShipTileTemplate, this.transform);
    //    PlaceTile(tile, i, j);

    //    for(int k = 0; k < shipCount; k++)
    //    {
    //        AddPirateOnTile(i, j);
    //    }
    //}

    //private void AddPirateOnTile(int i, int j)
    //{
    //    Pirate pirate = Instantiate(PirateTemplate, this.transform);

    //    Map[i][j].EnterPirate(pirate);
    //}

    //private void AddPirateOnMap()
    //{
    //    int i = Random.Range(waterWidth, Width - waterWidth);
    //    int j = Random.Range(waterLength, Length - waterLength);

    //    this.AddPirateOnTile(i, j);
    //}
}
